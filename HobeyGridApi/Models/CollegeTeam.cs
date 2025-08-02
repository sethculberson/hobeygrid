namespace HobeyGridApi.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization; 

    public class CollegeTeam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<PlayerCollegeSeason>? PlayerCollegeSeasons { get; set; }
    }
}