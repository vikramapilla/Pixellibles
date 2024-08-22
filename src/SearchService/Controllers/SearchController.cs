using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Auction>>> GetAllAuctions(string? searchTerm, int pageNumber = 1, int pageSize = 4)
        {
            var query = DB.PagedSearch<Auction>();
            query.Sort(x => x.Ascending(a => a.Make));

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query.Match(Search.Full, searchTerm).SortByTextScore();
            }
            query.PageNumber(pageNumber);
            query.PageSize(pageSize);

            var result = await query.ExecuteAsync();
            return Ok(new
            {
                results = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            });
        }
    }
}
