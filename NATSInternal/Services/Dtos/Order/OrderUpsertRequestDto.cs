namespace NATSInternal.Services.Dtos
{
    public class OrderUpsertRequestDto : IRequestDto<OrderUpsertRequestDto>
    {
        public DateTime? OrderedDateTime { get; set; }
        public string Note { get; set; }
        public int CustomerId { get; set; }
        public List<OrderItemRequestDto> Items { get; set; }
        public OrderPaymentRequestDto Payment { get; set; }
        public List<OrderPhotoRequestDto> Photos { get; set; }
        
        public OrderUpsertRequestDto TransformValues()
        {
            return this;
        }
    }
}