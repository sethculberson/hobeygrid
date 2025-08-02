namespace HobeyGridApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using HobeyGridApi.Models;
    using System.Linq;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<CollegeTeam> CollegeTeams { get; set; }
        public DbSet<PlayerCollegeSeason> PlayerSeasonStats { get; set; }
        public DbSet<Award> Awards { get; set; } // Added DbSet for Awards
        public DbSet<PlayerAward> PlayerAwards { get; set; } // Added DbSet for PlayerAwards
        public DbSet<GridInstance> GridInstances { get; set; } // Added DbSet for GridInstances

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>().ToTable("players");
            modelBuilder.Entity<CollegeTeam>().ToTable("college_teams");
            modelBuilder.Entity<PlayerCollegeSeason>().ToTable("player_season_stats");
            modelBuilder.Entity<Award>().ToTable("awards"); // Mapping for Awards table
            modelBuilder.Entity<PlayerAward>().ToTable("player_awards"); // Mapping for PlayerAwards table
            modelBuilder.Entity<GridInstance>().ToTable("grids"); // Mapping for GridInstances table

            // Explicitly map column names if they differ from C# property names by more than just casing
            modelBuilder.Entity<Player>()
                .Property(p => p.PlayerId)
                .HasColumnName("player_id");

            modelBuilder.Entity<Player>()
                .Property(p => p.FirstName)
                .HasColumnName("first_name");

            modelBuilder.Entity<Player>()
                .Property(p => p.LastName)
                .HasColumnName("last_name");

            modelBuilder.Entity<Player>()
                .Property(p => p.FullName)
                .HasColumnName("full_name");

            modelBuilder.Entity<Player>()
                .Property(p => p.Position)
                .HasColumnName("pos");

            modelBuilder.Entity<CollegeTeam>()
                .Property(ct => ct.TeamId)
                .HasColumnName("team_id");

            modelBuilder.Entity<CollegeTeam>()
                .Property(ct => ct.TeamName)
                .HasColumnName("team_name");

            modelBuilder.Entity<CollegeTeam>()
                .Property(ct => ct.Abbreviation)
                .HasColumnName("abbreviation");

            modelBuilder.Entity<PlayerCollegeSeason>()
                .Property(pcs => pcs.StatId)
                .HasColumnName("stat_id");

            modelBuilder.Entity<PlayerCollegeSeason>()
                .Property(pcs => pcs.PlayerId)
                .HasColumnName("player_id");

            modelBuilder.Entity<PlayerCollegeSeason>()
                .Property(pcs => pcs.TeamId)
                .HasColumnName("team_id");

            modelBuilder.Entity<PlayerCollegeSeason>()
                .Property(pcs => pcs.SeasonYear)
                .HasColumnName("season_year");

            modelBuilder.Entity<PlayerCollegeSeason>()
                .Property(pcs => pcs.Gp)
                .HasColumnName("gp");

            modelBuilder.Entity<PlayerCollegeSeason>()
                .Property(pcs => pcs.G)
                .HasColumnName("g");

            modelBuilder.Entity<PlayerCollegeSeason>()
                .Property(pcs => pcs.A)
                .HasColumnName("a");

            modelBuilder.Entity<PlayerCollegeSeason>()
                .Property(pcs => pcs.Tp)
                .HasColumnName("tp");

            modelBuilder.Entity<PlayerCollegeSeason>()
                .Property(pcs => pcs.Ppg)
                .HasColumnName("ppg");

            modelBuilder.Entity<PlayerCollegeSeason>()
                .Property(pcs => pcs.Pim)
                .HasColumnName("pim");

            modelBuilder.Entity<PlayerCollegeSeason>()
                .Property(pcs => pcs.Pm)
                .HasColumnName("pm");

            modelBuilder.Entity<PlayerCollegeSeason>()
                .Property(pcs => pcs.IsCaptain)
                .HasColumnName("is_captain");

            // Award mappings
            modelBuilder.Entity<Award>()
                .Property(a => a.AwardId)
                .HasColumnName("award_id");
            modelBuilder.Entity<Award>()
                .Property(a => a.AwardName)
                .HasColumnName("award_name");

            // PlayerAward mappings
            modelBuilder.Entity<PlayerAward>()
                .Property(pa => pa.PlayerAwardId)
                .HasColumnName("player_award_id");
            modelBuilder.Entity<PlayerAward>()
                .Property(pa => pa.PlayerId)
                .HasColumnName("player_id");
            modelBuilder.Entity<PlayerAward>()
                .Property(pa => pa.AwardId)
                .HasColumnName("award_id");
            modelBuilder.Entity<PlayerAward>()
                .Property(pa => pa.SeasonYear)
                .HasColumnName("season_year");
            modelBuilder.Entity<PlayerAward>()
                .Property(pa => pa.TeamId)
                .HasColumnName("team_id");

            // GridInstance mappings
            modelBuilder.Entity<GridInstance>()
                .Property(gi => gi.GridId)
                .HasColumnName("grid_id");
            modelBuilder.Entity<GridInstance>()
                .Property(gi => gi.GridDate)
                .HasColumnName("grid_date");
            modelBuilder.Entity<GridInstance>()
                .Property(gi => gi.RowCategory1)
                .HasColumnName("row_category_1");
            modelBuilder.Entity<GridInstance>()
                .Property(gi => gi.RowCategory2)
                .HasColumnName("row_category_2");
            modelBuilder.Entity<GridInstance>()
                .Property(gi => gi.RowCategory3)
                .HasColumnName("row_category_3");
            modelBuilder.Entity<GridInstance>()
                .Property(gi => gi.ColCategory1)
                .HasColumnName("col_category_1");
            modelBuilder.Entity<GridInstance>()
                .Property(gi => gi.ColCategory2)
                .HasColumnName("col_category_2");
            modelBuilder.Entity<GridInstance>()
                .Property(gi => gi.ColCategory3)
                .HasColumnName("col_category_3");
            modelBuilder.Entity<GridInstance>()
                .Property(gi => gi.CorrectAnswersJson)
                .HasColumnName("correct_answers_json");


            // Configure relationships
            modelBuilder.Entity<PlayerCollegeSeason>()
                .HasOne(pcs => pcs.Player)
                .WithMany(p => p.PlayerCollegeSeasons)
                .HasForeignKey(pcs => pcs.PlayerId);

            modelBuilder.Entity<PlayerCollegeSeason>()
                .HasOne(pcs => pcs.Team)
                .WithMany(t => t.PlayerCollegeSeasons)
                .HasForeignKey(pcs => pcs.TeamId);

            modelBuilder.Entity<PlayerAward>()
                .HasOne(pa => pa.Player)
                .WithMany(p => p.PlayerAwards) // This now correctly references the new property
                .HasForeignKey(pa => pa.PlayerId);

            modelBuilder.Entity<PlayerAward>()
                .HasOne(pa => pa.Award)
                .WithMany(a => a.PlayerAwards)
                .HasForeignKey(pa => pa.AwardId);

            modelBuilder.Entity<PlayerAward>()
                .HasOne(pa => pa.Team)
                .WithMany() // No navigation property back from CollegeTeam to PlayerAward
                .HasForeignKey(pa => pa.TeamId);


            // Configure unique indexes
            modelBuilder.Entity<CollegeTeam>()
                .HasIndex(ct => ct.TeamName)
                .IsUnique();

            modelBuilder.Entity<PlayerCollegeSeason>()
                .HasIndex(pcs => new { pcs.PlayerId, pcs.TeamId, pcs.SeasonYear })
                .IsUnique();

            modelBuilder.Entity<Award>()
                .HasIndex(a => a.AwardName)
                .IsUnique();

            modelBuilder.Entity<GridInstance>()
                .HasIndex(gi => gi.GridDate)
                .IsUnique();
        }
    }
}