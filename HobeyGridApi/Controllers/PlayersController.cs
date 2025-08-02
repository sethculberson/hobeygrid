// Controllers/PlayersController.cs
namespace HobeyGridApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HobeyGridApi.Data;
    using HobeyGridApi.Models;

    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlayersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            return await _context.Players.ToListAsync();
        }

        // REVISED ENDPOINT: GetPlayerSeasonStats with Pagination
        [HttpGet("seasonstats")]
        public async Task<ActionResult<PagedResponse<PlayerCollegeSeason>>> GetPlayerSeasonStats(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100) // Default page size of 100
        {
            // Ensure page numbers and sizes are reasonable
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 500) pageSize = 500; // Cap max page size to prevent overly large requests

            var totalRecords = await _context.PlayerSeasonStats.CountAsync();

            var query = _context.PlayerSeasonStats
                                 .AsNoTracking() // Added for read-only queries performance
                                 .Include(pcs => pcs.Player)
                                 .Include(pcs => pcs.Team)
                                 // OrderBy is crucial for consistent pagination results
                                 .OrderBy(pcs => pcs.SeasonYear) // Example ordering
                                 .ThenBy(pcs => pcs.Player!.FullName) // Then by player name
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize);

            var data = await query.ToListAsync();

            return Ok(new PagedResponse<PlayerCollegeSeason>(data, pageNumber, pageSize, totalRecords));
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Player>>> SearchPlayers(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search name cannot be empty.");
            }
            return await _context.Players
                                 .AsNoTracking() // Added for read-only queries performance
                                 .Where(p => p.FullName.ToLower().Contains(name.ToLower()))
                                 .ToListAsync();
        }

        [HttpGet("byteam/{teamName}")]
        public async Task<ActionResult<IEnumerable<PlayerCollegeSeason>>> GetPlayersByTeamAndSeason(string teamName, [FromQuery] short? season)
        {
            // Prioritize exact team name match, then abbreviation
            var team = await _context.CollegeTeams
                                     .AsNoTracking() // Added for read-only queries performance
                                     .FirstOrDefaultAsync(t => t.TeamName.ToLower() == teamName.ToLower());

            if (team == null)
            {
                // If no exact team name match, try by abbreviation
                team = await _context.CollegeTeams
                                     .AsNoTracking() // Added for read-only queries performance
                                     .FirstOrDefaultAsync(t => t.Abbreviation.ToLower() == teamName.ToLower());
            }

            if (team == null)
            {
                return NotFound($"Team '{teamName}' not found.");
            }

            var query = _context.PlayerSeasonStats
                                .AsNoTracking() // Added for read-only queries performance
                                .Include(pcs => pcs.Player)
                                .Include(pcs => pcs.Team)
                                .Where(pcs => pcs.TeamId == team.TeamId);

            if (season.HasValue)
            {
                query = query.Where(pcs => pcs.SeasonYear == season.Value);
            }

            return await query.ToListAsync();
        }
    }
}
