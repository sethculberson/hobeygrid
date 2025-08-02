// --- HobeyGridApi/Controllers/GridController.cs ---
namespace HobeyGridApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using HobeyGridApi.Models;
    using HobeyGridApi.Services;
    using HobeyGridApi.Auth; // Added for ApiKeyAuthAttribute
    using System;
    using System.Collections.Generic;
    using System.Linq; // Added for .Select
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class GridController : ControllerBase
    {
        private readonly GridGenerationService _gridService;

        public GridController(GridGenerationService gridService)
        {
            _gridService = gridService;
        }

        // GET: api/Grid/daily
        // Endpoint to get today's grid (generates if not exists)
        [HttpGet("daily")]
        public async Task<ActionResult<GridInstance>> GetDailyGrid()
        {
            try
            {
                var grid = await _gridService.GenerateDailyGrid();
                return Ok(grid);
            }
            catch (InvalidOperationException ex)
            {
                // This might happen if a grid already exists for today and GenerateDailyGrid throws
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
        // POST: api/Grid/validate
        // Endpoint to submit player guesses
        [HttpPost("validate")]
        public async Task<ActionResult<Dictionary<string, bool>>> ValidateGrid([FromBody] SubmittedGridDto submittedGrid)
        {
            try
            {
                var validationResults = await _gridService.ValidateSubmittedGrid(submittedGrid.GridId, submittedGrid.PlayerGuesses);

                return Ok(validationResults);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred during validation: {ex.Message}");
            }
        }
    }
}
