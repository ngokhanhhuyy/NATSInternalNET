namespace NATSInternal.Services.Dtos
{
    public class DebtPaymentUpsertRequestDto : IRequestDto<DebtPaymentUpsertRequestDto>
    {
        public long Amount { get; set; }
        public string Note { get; set; }
        public DateTime? PaidDateTime { get; set; }
        public int CustomerId { get; set; }
        public string UpdatingReason { get; set; }
        
        public DebtPaymentUpsertRequestDto TransformValues()
        {
            Note = Note?.ToNullIfEmpty();
            UpdatingReason?.ToNullIfEmpty();
            return this;
        }
    }
}