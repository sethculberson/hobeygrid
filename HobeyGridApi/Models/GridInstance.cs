namespace HobeyGridApi.Models
{
    // Models/GridInstance.cs
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class GridInstance
    {
        [Key]
        public Guid GridId { get; set; }

        [Column(TypeName = "date")] 
        public DateOnly GridDate { get; set; }

        [Column(TypeName = "jsonb")]
        public required string RowCategory1 { get; set; }

        [Column(TypeName = "jsonb")]
        public required string RowCategory2 { get; set; }

        [Column(TypeName = "jsonb")]
        public required string RowCategory3 { get; set; }

        [Column(TypeName = "jsonb")]
        public required string ColCategory1 { get; set; }

        [Column(TypeName = "jsonb")]
        public required string ColCategory2 { get; set; }

        [Column(TypeName = "jsonb")]
        public required string ColCategory3 { get; set; }

        [Column(TypeName = "jsonb")] // Maps to PostgreSQL JSONB type
        public string? CorrectAnswersJson { get; set; } // Stores serialized Dictionary<string, List<Guid>>
    }
}