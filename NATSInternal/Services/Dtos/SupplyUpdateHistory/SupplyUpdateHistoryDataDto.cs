namespace NATSInternal.Services.Dtos;

public class SupplyUpdateHistoryDataDto
{
    public DateTime SuppliedDateTime { get; set; }
    public long ShipmentFee { get; set; }
    public string Note { get; set; }
    public List<SupplyItemUpdateHistoryDataDto> Items { get; set; }
    
    public SupplyUpdateHistoryDataDto(Supply supply)
    {
        SuppliedDateTime = supply.SuppliedDateTime;
        ShipmentFee = supply.ShipmentFee;
        Note = supply.Note;
        Items = supply.Items?
            .Select(si => new SupplyItemUpdateHistoryDataDto(si))
            .ToList();
    }
}