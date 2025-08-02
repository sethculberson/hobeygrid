namespace HobeyGridApi.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Award
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AwardId { get; set; }
        public string AwardName { get; set; } = string.Empty;

        public ICollection<PlayerAward>? PlayerAwards { get; set; }
    }
}