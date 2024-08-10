namespace AuctionService.Entities;


public class Auction
{
    public Guid Id { get; set; }

    public int ReservePrice { get; set; } = 0;
    public required string Seller { get; set; }
    public string? Winner { get; set; }
    public int? SoldAmount { get; set; }
    public int? CurrentBid { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime EndedAt { get; set; }
    public Status Status { get; set; }
    public required Item Item { get; set; }
}
