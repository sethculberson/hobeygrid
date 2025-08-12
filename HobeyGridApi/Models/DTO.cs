namespace HobeyGridApi.Models
{
    using System;
    using System.Collections.Generic;

    public class PagedResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; } = new List<T>();

        public PagedResponse(List<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            Data = data;
            TotalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
        }
    }

    // DTO for submitted grid data (used in GridController)
    public class SubmittedGridDto
    {
        public Guid GridId { get; set; }
        public Dictionary<string, Guid?> PlayerGuesses { get; set; } = new Dictionary<string, Guid?>();
    }

    // DTO for admin grid generation request (used in GridController)
    public class AdminGridGenerationDto
    {
        public List<GridCategory> RowCategories { get; set; } = new List<GridCategory>();
        public List<GridCategory> ColCategories { get; set; } = new List<GridCategory>();
    }

    public class GridCreationDto
    {
        public List<GridCategory> RowCategories { get; set; } = new List<GridCategory>();
        public List<GridCategory> ColCategories { get; set; } = new List<GridCategory>();
    }
}