namespace NATSInternal.Services.Dtos
{
    public class OrderPhotoRequestDto : IRequestDto<OrderPhotoRequestDto>
    {
        public int? Id { get; set; }
        public byte[] File { get; set; }
        public bool HasBeenChanged { get; set; }
        
        public OrderPhotoRequestDto TransformValues()
        {
            if (Id.HasValue && Id.Value == 0)
            { 
                Id = null;
            }
            
            return this;
        }
    }
}