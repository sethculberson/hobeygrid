namespace HobeyGridApi.Models // Corrected namespace
{
    // Models/PlayerCollegeSeason.cs
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class PlayerCollegeSeason
    {
        [Key]
        public Guid StatId { get; set; }

        public Guid PlayerId { get; set; }
        public int TeamId { get; set; }
        public short SeasonYear { get; set; }
        public short Gp { get; set; }
        public short G { get; set; }
        public short A { get; set; }
        public short Tp { get; set; }

        [Column(TypeName = "numeric(4,2)")]
        public decimal Ppg { get; set; }

        public short Pim { get; set; }
        public short Pm { get; set; }
        public bool IsCaptain { get; set; } = false;

        public Player? Player { get; set; }
        public CollegeTeam? Team { get; set; }
    }
}