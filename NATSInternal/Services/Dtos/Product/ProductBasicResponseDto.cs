namespace NATSInternal.Services.Dtos;

public class ProductBasicResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Unit { get; set; }
    public long Price { get; set; }
    public int StockingQuantity { get; set; }
    public string ThumbnailUrl { get; set; }
}
