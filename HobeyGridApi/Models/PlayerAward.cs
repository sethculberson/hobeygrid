namespace HobeyGridApi.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    public class PlayerAward
    {
        [Key]
        public Guid PlayerAwardId { get; set; }
        public Guid PlayerId { get; set; }
        public int AwardId { get; set; }
        public short? SeasonYear { get; set; }
        public int? TeamId { get; set; } 
        public Player? Player { get; set; }
        public Award? Award { get; set; }
        public CollegeTeam? Team { get; set; }
    }
}