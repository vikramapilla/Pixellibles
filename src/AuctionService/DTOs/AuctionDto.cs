namespace AuctionService.DTOs;

public class AuctionDto
{
    public Guid Id { get; set; }

    public int ReservePrice { get; set; }
    public required string Seller { get; set; }
    public string? Winner { get; set; }
    public int? SoldAmount { get; set; }
    public int? CurrentBid { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime EndedAt { get; set; }
    public string? Status { get; set; }
    public required string Make { get; set; }
    public required string Model { get; set; }
    public required int Year { get; set; }
    public required string Color { get; set; }
    public required int Mileage { get; set; }
    public required string ImageUrl { get; set; }

}
