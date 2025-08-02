namespace HobeyGridApi.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public class GridCategory
    {
        public string Name { get; set; } = string.Empty; // Display name (e.g., "Hobey Baker Winner")
        public string Type { get; set; } = string.Empty; // Type of category (e.g., "Award", "Team", "Stat", "Draft")
        public string? Value { get; set; } // Specific value (e.g., "Hobey Baker Award", "Michigan", "20")
        public int? MinValue { get; set; } // For stat-based categories (e.g., min goals)
        public int? MaxValue { get; set; } // For stat-based categories (e.g., max goals)
        public string? StatField { get; set; } // For stat-based categories (e.g., "G", "TP")
        public int? DraftRound { get; set; } // For draft-based categories
    }
}
