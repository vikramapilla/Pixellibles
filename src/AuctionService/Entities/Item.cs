﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionService.Entities;

[Table("Items")]
public class Item
{
    public Guid Id { get; set; }
    public required string Make { get; set; }
    public required string Model { get; set; }
    public required int Year { get; set; }
    public required string Color { get; set; }
    public required int Mileage { get; set; }
    public required string ImageUrl { get; set; }

    // nav properties
    public Auction? Auction { get; set; }
    public Guid AuctionId { get; set; }
}
