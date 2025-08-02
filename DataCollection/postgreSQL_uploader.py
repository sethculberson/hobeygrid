import csv
import psycopg2
from psycopg2 import Error
import uuid
import traceback
import sys

# --- Database Connection Details ---
# IMPORTANT: Replace these with your actual PostgreSQL credentials and database name
DB_HOST = "localhost"
DB_NAME = "hobey_grid_db"
DB_USER = "sethculberson"  # The user you created (e.g., 'hobey_user')
DB_PASSWORD = "#S3thIsCoo105302005" # The password for your_username

# --- CSV File Path ---
# IMPORTANT: Replace this with the actual path to your CSV file
CSV_FILE_PATH = "DataCollection/ncaa_stats_2019_2020.csv"

# --- CSV Column Mapping ---
# Ensure these match the exact column headers in your CSV file
CSV_COLUMNS = {
    'id': 'ID',       # This is now treated as a row identifier, not a unique player ID
    'player': 'Player',
    'team': 'Team',
    'gp': 'GP',
    'g': 'G',
    'a': 'A',
    'tp': 'TP',
    'ppg': 'PPG',
    'pim': 'PIM',
    'pm': 'PM',
    'season': 'Season'
}

def get_db_connection():
    """Establishes and returns a PostgreSQL database connection."""
    try:
        conn = psycopg2.connect(
            host=DB_HOST,
            database=DB_NAME,
            user=DB_USER,
            password=DB_PASSWORD
        )
        print("Database connection established successfully.")
        return conn
    except Error as e:
        print(f"Error connecting to PostgreSQL database: {e}")
        sys.exit(1) # Exit if connection fails

def import_data():
    """
    Imports data from the CSV file into the PostgreSQL database.
    It handles inserting new players and teams, and then player season stats.
    """
    conn = None
    try:
        conn = get_db_connection()
        
        # Use a dictionary to store already processed players and teams
        # to avoid redundant database lookups and inserts
        # {full_name: player_id_uuid}
        existing_players_cache = {}
        # {team_name: team_id_int}
        existing_teams_cache = {}

        # Fetch existing players and teams from DB to populate caches
        # These SELECTs should ideally run outside the main transaction loop
        # or be part of a read-only transaction.
        # For simplicity, we'll let them run with autocommit=True for these initial reads
        # or ensure they are committed if they start a transaction.
        # The default for psycopg2 is autocommit=False, meaning a transaction starts with the first query.
        # We need to ensure these pre-fetches are committed before the main import transaction.
        
        # Ensure autocommit is true for these initial reads so they don't start a transaction
        # that conflicts with later session commands or the main transaction.
        conn.autocommit = True 
        with conn.cursor() as cur:
            print("Populating existing players cache...")
            cur.execute("SELECT player_id, full_name FROM players;")
            for p_id, p_name in cur.fetchall():
                existing_players_cache[p_name] = p_id
            print(f"Loaded {len(existing_players_cache)} existing players.")

            print("Populating existing teams cache...")
            cur.execute("SELECT team_id, team_name FROM college_teams;")
            for t_id, t_name in cur.fetchall():
                existing_teams_cache[t_name] = t_id
            print(f"Loaded {len(existing_teams_cache)} existing teams.")
        
        # Set autocommit back to False for the main import transaction
        conn.autocommit = False

        # Now, for the main import, use a 'with conn:' block for automatic transaction management
        # This will implicitly start a transaction and commit/rollback based on success/failure
        with conn: # This context manager will handle commit/rollback
            with conn.cursor() as cur:
                with open(CSV_FILE_PATH, mode='r', encoding='utf-8') as file:
                    reader = csv.DictReader(file)
                    print(f"Starting data import from {CSV_FILE_PATH}...")
                    
                    # Counter for successful inserts
                    players_inserted = 0
                    teams_inserted = 0
                    stats_inserted = 0

                    for i, row in enumerate(reader):
                        if (i + 1) % 100 == 0:
                            print(f"Processing row {i + 1}...")

                        try:
                            # --- Extract and Clean Data from CSV Row ---
                            # scraped_id is now just a row identifier, not used for player_id generation
                            # scraped_id = row[CSV_COLUMNS['id']].strip() 
                            full_name = row[CSV_COLUMNS['player']]
                            first_name = full_name.split(' ', 1)[0] if ' ' in full_name else full_name
                            pos = full_name.split('(')[-1].strip(')') if '(' in full_name else ""
                            full_name = full_name.split('(')[0].strip()                     
                            first_name = full_name.split(' ', 1)[0] if ' ' in full_name else full_name
                            last_name = full_name.split(' ', 1)[1] if ' ' in full_name else ""
                            full_name = f"{first_name} {last_name}".strip()
                            season_year = 2019
                            
                            if full_name == "Ryan Sullivan":
                                full_name = "Ryan Sullivan (Miami)"

                            team_name = row[CSV_COLUMNS['team']].strip()
                            # Convert stats to appropriate types, handle potential errors
                            gp = int(row[CSV_COLUMNS['gp']])
                            g = int(row[CSV_COLUMNS['g']])
                            a = int(row[CSV_COLUMNS['a']])
                            tp = int(row[CSV_COLUMNS['tp']])
                            ppg = float(row[CSV_COLUMNS['ppg']]) # Read as float, DB will store as NUMERIC
                            pim = int(row[CSV_COLUMNS['pim']])
                            pm = int(row[CSV_COLUMNS['pm']])

                            # --- Player Handling ---
                            player_uuid = None
                            if full_name in existing_players_cache:
                                player_uuid = existing_players_cache[full_name]
                            else:
                                # Generate a new, random UUID for this player
                                player_uuid = uuid.uuid4() 
                                
                                # Insert new player
                                insert_player_sql = """
                                    INSERT INTO players (player_id, first_name, last_name, full_name, pos)
                                    VALUES (%s, %s, %s, %s, %s);
                                """
                                cur.execute(insert_player_sql, (str(player_uuid), first_name, last_name, full_name, pos))
                                existing_players_cache[full_name] = player_uuid
                                players_inserted += 1

                            # --- Team Handling ---
                            team_id = None
                            if team_name in existing_teams_cache:
                                team_id = existing_teams_cache[team_name]
                            else:
                                abbreviation = "".join([word[0] for word in team_name.split() if word[0].isalpha()]).upper()
                                if not abbreviation: # Fallback for single-word names or names without alpha chars
                                    abbreviation = team_name[:3].upper() if len(team_name) >= 3 else team_name.upper()

                                # Try to find existing team by team name first
                                cur.execute("SELECT team_id, team_name FROM college_teams WHERE team_name = %s;", (team_name,))
                                existing_team_by_team_name = cur.fetchone()

                                if existing_team_by_team_name:
                                    team_id = existing_team_by_team_name[0]
                                    # Cache the current team_name to the found team_id
                                    existing_teams_cache[team_name] = team_id
                                    # Also update the cache for the existing team_name if it's different
                                    existing_teams_cache[existing_team_by_team_name[1]] = team_id
                                    print(f"Warning: Team '{team_name}' generated team name '{abbreviation}' which already exists for '{existing_team_by_team_name[1]}'. Reusing existing team_id {team_id}.")
                                else:
                                    # If no team found by abbreviation, attempt to insert
                                    insert_team_sql = """
                                        INSERT INTO college_teams (team_name, abbreviation)
                                        VALUES (%s, %s) RETURNING team_id;
                                    """
                                    cur.execute(insert_team_sql, (team_name, abbreviation))
                                    team_id = cur.fetchone()[0] # Get the auto-generated team_id
                                    existing_teams_cache[team_name] = team_id
                                    teams_inserted += 1

                            # --- Player Season Stats Handling ---
                            stat_uuid = uuid.uuid4() # Generate a new UUID for each unique stat line

                            insert_stats_sql = """
                                INSERT INTO player_season_stats (
                                    stat_id, player_id, team_id, season_year, gp, g, a, tp, ppg, pim, pm
                                ) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
                                ON CONFLICT (player_id, team_id, season_year) DO UPDATE SET
                                    gp = EXCLUDED.gp,
                                    g = EXCLUDED.g,
                                    a = EXCLUDED.a,
                                    tp = EXCLUDED.tp,
                                    ppg = EXCLUDED.ppg,
                                    pim = EXCLUDED.pim,
                                    pm = EXCLUDED.pm;
                            """
                            # Use ON CONFLICT to handle cases where a player-team-season combo might already exist
                            # This is useful if you run the script multiple times or have duplicate rows in CSV
                            cur.execute(insert_stats_sql, (
                                str(stat_uuid), str(player_uuid), team_id, season_year,
                                gp, g, a, tp, ppg, pim, pm
                            ))
                            stats_inserted += 1

                        except KeyError as ke:
                            print(f"Error: Missing expected CSV column '{ke}' in row {i+1}. Skipping row.")
                            print(f"Row data: {row}")
                            traceback.print_exc() # Print full traceback
                            raise # Re-raise to stop processing and ensure rollback
                        except ValueError as ve:
                            print(f"Error: Data type conversion failed in row {i+1} for value: {ve}. Skipping row.")
                            print(f"Row data: {row}")
                            traceback.print_exc() # Print full traceback
                            raise # Re-raise to stop processing and ensure rollback
                        except Exception as ex:
                            print(f"An unexpected error occurred processing row {i+1}: {ex}. Skipping row.")
                            print(f"Row data: {row}")
                            traceback.print_exc() # Print full traceback
                            raise # Re-raise to stop processing and ensure rollback
                
                # The 'with conn:' block will handle the commit here if no exceptions occurred
                print("\n--- Data Import Summary ---")
                print(f"Successfully imported data from {CSV_FILE_PATH}")
                print(f"New Players inserted: {players_inserted}")
                print(f"New Teams inserted: {teams_inserted}")
                print(f"Player Season Stats processed (inserted/updated): {stats_inserted}")

    except Error as e:
        print(f"Database error during import: {e}")
        # The 'with conn:' block will handle the rollback here if an exception occurred
    finally:
        if conn:
            conn.close()
        print("Database connection closed.")

if __name__ == "__main__":
    import_data()
