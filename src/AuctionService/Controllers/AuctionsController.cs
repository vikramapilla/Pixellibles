using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController(AuctionDbContext dbContext, IMapper mapper) : ControllerBase
{
    private readonly AuctionDbContext _context = dbContext;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions()
    {
        var result = await _context.Auctions.ToListAsync();
        return _mapper.Map<List<AuctionDto>>(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auctions = await _context.Auctions.ToListAsync();
        var auction = auctions.FirstOrDefault(x => x.Id == id);

        if (auction == null) return NotFound();
        return _mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto dto)
    {
        var auction = _mapper.Map<Auction>(dto);
        auction.Seller = "seller";
        _context.Auctions.Add(auction);

        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return BadRequest("Couldn't save the data!");

        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, _mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto dto)
    {
        var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null) return NotFound();

        auction.Item.Make = dto.Make ?? auction.Item.Make;
        auction.Item.Model = dto.Model ?? auction.Item.Model;
        auction.Item.Mileage = dto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = dto.Year ?? auction.Item.Year;
        auction.Item.Color = dto.Color ?? auction.Item.Color;

        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return BadRequest("Couldn't update the data!");

        return Ok("Updated successfully!");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null) return NotFound();

        _context.Auctions.Remove(auction);
        
        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return BadRequest("Couldn't delete the data!");

        return Ok("Deleted successfully!");
    }
}
