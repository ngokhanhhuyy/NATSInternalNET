namespace NATSInternal.Services.Dtos
{
    public class OrderItemRequestDto : IRequestDto<OrderItemRequestDto>
    {
        public int? Id { get; set; }
        public long Amount { get; set; }
        public decimal VatFactor { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public bool HasBeenChanged { get; set; }
        public bool HasBeenDeleted { get; set; }
        
        public OrderItemRequestDto TransformValues()
        {
            if (Id.HasValue && Id.Value == 0)
            {
                Id = null;
            }
            
            return this;
        }
    }
}