namespace HobeyGridApi.Services
{
    using Microsoft.EntityFrameworkCore;
    using HobeyGridApi.Data;
    using HobeyGridApi.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json; // For JSON serialization/deserialization
    using System.Text.RegularExpressions; // For parsing category strings
    using System.Threading.Tasks;

    public class GridGenerationService
    {
        private readonly ApplicationDbContext _context;
        private readonly Random _random = new Random();

        public GridGenerationService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of player contexts (PlayerId, SeasonYear, TeamId) that satisfy a given category.
        /// This method queries the database based on the structured GridCategory object.
        /// </summary>
        /// <param name="category">The structured GridCategory object.</param>
        /// <returns>A list of tuples containing PlayerId, SeasonYear, and TeamId for matching players.</returns>
        private async Task<List<Guid>> GetPlayerSeasonContextsForCategory(GridCategory category)
        {
            // TODO: Implement the database querying logic based on category.Type and its properties.
            // This will involve LINQ queries against _context.Players, _context.PlayerSeasonStats, _context.Awards, _context.PlayerAwards.
            // Ensure AsNoTracking() for read-only queries.
            // Handle nullable fields (e.g., category.MinValue.HasValue, p.DraftRoundNhl.HasValue).
            // For Award/Draft categories, you might return dummy SeasonYear and TeamId (e.g., 0) as they are not context-specific.
            var playerIds = new List<Guid>();
            switch (category.Type)
            {
                case "Team":
                    var team = await _context.CollegeTeams
                                             .AsNoTracking()
                                             .FirstOrDefaultAsync(t => t.TeamName == category.Value);
                    if (team != null)
                    {
                        playerIds = await _context.PlayerSeasonStats
                                                  .AsNoTracking()
                                                  .Where(pcs => pcs.TeamId == team.TeamId)
                                                  .Select(pcs => pcs.PlayerId)
                                                  .Distinct()
                                                  .ToListAsync();
                    }
                    break;
                case "Award":
                    var award = await _context.Awards
                                              .AsNoTracking()
                                              .FirstOrDefaultAsync(a => a.AwardName == category.Value);
                    if (award != null)
                    {
                        playerIds = await _context.PlayerAwards
                                                  .AsNoTracking()
                                                  .Where(pa => pa.AwardId == award.AwardId)
                                                  .Select(pa => pa.PlayerId)
                                                  .Distinct()
                                                  .ToListAsync();
                    }
                    break;
                case "Stat":
                    IQueryable<PlayerCollegeSeason> statQuery = _context.PlayerSeasonStats.AsNoTracking();

                    if (category.StatField == "G" && category.MinValue.HasValue)
                    {
                        statQuery = statQuery.Where(pcs => pcs.G >= category.MinValue.Value);
                    }
                    else if (category.StatField == "A" && category.MinValue.HasValue)
                    {
                        statQuery = statQuery.Where(pcs => pcs.A >= category.MinValue.Value);
                    }
                    else if (category.StatField == "TP" && category.MinValue.HasValue)
                    {
                        statQuery = statQuery.Where(pcs => pcs.Tp >= category.MinValue.Value);
                    }
                    else if (category.StatField == "PIM" && category.MinValue.HasValue)
                    {
                        statQuery = statQuery.Where(pcs => pcs.Pim >= category.MinValue.Value);
                    }

                    playerIds = await statQuery.Select(pcs => pcs.PlayerId).Distinct().ToListAsync();

                    break;
            }
            return playerIds;
        }

        /// <summary>
        /// Generates today's daily grid by randomly selecting categories.
        /// If a grid for today already exists, it will overwrite it if allowed.
        /// </summary>
        /// <returns>A GridInstance object for today's grid.</returns>
        public async Task<GridInstance> GenerateDailyGrid()
        {
            var rows = new List<GridCategory>
            {
                new GridCategory { Name = "Boston University", Type = "Team", Value = "Boston University" },
                new GridCategory { Name = "Boston College", Type = "Team" , Value = "Boston College" },
                new GridCategory { Name = "University of Denver", Type = "Team", Value = "University of Denver" },
            };
            var cols = new List<GridCategory>
            {
                new GridCategory { Name = ">=20G", Type = "Stat", StatField = "G", MinValue = 20 },
                new GridCategory { Name = ">=30A", Type = "Stat", StatField = "A", MinValue = 30 },
                new GridCategory { Name = ">=10GP", Type = "Stat", StatField = "GP" , MinValue = 10 }
            };

            var gridDate = DateOnly.FromDateTime(DateTime.UtcNow);

            var existingGrid = await _context.GridInstances
                                             .FirstOrDefaultAsync(g => g.GridDate == gridDate);
            if (existingGrid != null)
            {
                Console.WriteLine($"Grid for {gridDate} already exists. Returning existing grid.");
                return existingGrid;
            }

            var correctAnswers = new Dictionary<string, List<Guid>>();
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    var row = rows[r];
                    var rowPlayers = await GetPlayerSeasonContextsForCategory(row);
                    var col = cols[c];
                    var colPlayers = await GetPlayerSeasonContextsForCategory(col);

                    var intersection = colPlayers.Intersect(rowPlayers).ToList();

                    correctAnswers[$"{r}_{c}"] = intersection;
                }
            }
            var newGridInstance = new GridInstance
            {
                GridId = Guid.NewGuid(),
                GridDate = gridDate,
                RowCategory1 = JsonSerializer.Serialize(rows[0]),
                RowCategory2 = JsonSerializer.Serialize(rows[1]),
                RowCategory3 = JsonSerializer.Serialize(rows[2]),
                ColCategory1 = JsonSerializer.Serialize(cols[0]),
                ColCategory2 = JsonSerializer.Serialize(cols[1]),
                ColCategory3 = JsonSerializer.Serialize(cols[2]),
                CorrectAnswersJson = JsonSerializer.Serialize(correctAnswers)
            };
            
            _context.GridInstances.Add(newGridInstance);
            await _context.SaveChangesAsync();

            return newGridInstance;
        }


        /// <summary>
        /// Validates a user-submitted grid against the correct answers stored in the database.
        /// </summary>
        /// <param name="gridId">The ID of the grid instance being validated.</param>
        /// <param name="submittedPlayerIds">A dictionary where keys are "row_col" (e.g., "0_0") and values are submitted PlayerIds (or null).</param>
        /// <returns>A dictionary where keys are "row_col" and values are booleans indicating correctness.</returns>
        /// <exception cref="ArgumentException">Thrown if the gridId is invalid.</exception>
        public async Task<Dictionary<string, bool>> ValidateSubmittedGrid(Guid gridId, Dictionary<string, Guid?> submittedPlayerIds)
        {   
            var result = new Dictionary<string, bool>();
            var gridInstance = await _context.GridInstances.FirstOrDefaultAsync(g => g.GridId == gridId) ?? throw new ArgumentException("Invalid Grid ID provided for validation.");
            if (gridInstance.CorrectAnswersJson == null)
            {
                throw new InvalidOperationException("No correct answers found for the provided grid ID.");
            }
            // Deserialize the stored correct answers
            var correctAnswers = JsonSerializer.Deserialize<Dictionary<string, List<Guid>>>(gridInstance.CorrectAnswersJson) ?? throw new InvalidOperationException("Failed to deserialize correct answers for the provided grid ID."); 
        
            foreach (var entry in submittedPlayerIds)
            {
                var cellKey = entry.Key; // e.g., "0_0"
                var submittedPlayerId = entry.Value;
                Console.WriteLine($"Validating cell {cellKey} with submitted player ID: {submittedPlayerId}");

                if (submittedPlayerId.HasValue && correctAnswers.ContainsKey(cellKey))
                {
                    // Check if the submitted player ID is in the list of correct answers for that cell
                    result[cellKey] = correctAnswers[cellKey].Contains(submittedPlayerId.Value);
                }
                else
                {
                    // If no player submitted or no correct answers for this cell, it's incorrect
                    result[cellKey] = false;
                }
                Console.WriteLine();
            }
            return result;
        }
    }

    /// <summary>
    /// Helper for comparing (PlayerId, SeasonYear, TeamId) tuples, used for precise intersections.
    /// </summary>
    public class PlayerContextComparer : IEqualityComparer<(Guid PlayerId, short SeasonYear, int TeamId)>
    {
        public bool Equals((Guid PlayerId, short SeasonYear, int TeamId) x, (Guid PlayerId, short SeasonYear, int TeamId) y)
        {
            return x.PlayerId == y.PlayerId && x.SeasonYear == y.SeasonYear && x.TeamId == y.TeamId;
        }

        public int GetHashCode((Guid PlayerId, short SeasonYear, int TeamId) obj)
        {
            return HashCode.Combine(obj.PlayerId, obj.SeasonYear, obj.TeamId);
        }
    }
}