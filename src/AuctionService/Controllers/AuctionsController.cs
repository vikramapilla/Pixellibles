using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController(AuctionDbContext dbContext, IMapper mapper, IPublishEndpoint publishEndpoint) : ControllerBase
{
    private readonly AuctionDbContext _context = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions(string? date)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();
        if (!string.IsNullOrEmpty(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }
        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auctions = await _context.Auctions.ToListAsync();
        var auction = auctions.FirstOrDefault(x => x.Id == id);

        if (auction == null) return NotFound();
        return _mapper.Map<AuctionDto>(auction);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto dto)
    {
        var auction = _mapper.Map<Auction>(dto);
        auction.Seller = User.Identity?.Name ?? "seller";
        _context.Auctions.Add(auction);

        var newAuction = _mapper.Map<AuctionDto>(auction);
        await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return BadRequest("Couldn't save the data!");
        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, newAuction);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto dto)
    {
        var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null) return NotFound();
        if (auction.Seller != User.Identity?.Name) return Forbid();

        auction.Item.Make = dto.Make ?? auction.Item.Make;
        auction.Item.Model = dto.Model ?? auction.Item.Model;
        auction.Item.Mileage = dto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = dto.Year ?? auction.Item.Year;
        auction.Item.Color = dto.Color ?? auction.Item.Color;

        await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));
        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return BadRequest("Couldn't update the data!");

        return Ok("Updated successfully!");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null) return NotFound();
        if (auction.Seller != User.Identity?.Name) return Forbid();

        _context.Auctions.Remove(auction);

        await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });
        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return BadRequest("Couldn't delete the data!");

        return Ok("Deleted successfully!");
    }
}
