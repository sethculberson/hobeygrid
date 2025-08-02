namespace HobeyGridApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    public class Player
    {
        [Key]
        public Guid PlayerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        
        [JsonIgnore]
        public ICollection<PlayerCollegeSeason>? PlayerCollegeSeasons { get; set; }
        [JsonIgnore]
        public ICollection<PlayerAward>? PlayerAwards { get; set; }
    }
}