namespace NATSInternal.Services.Dtos;

public class PatchListRequestDto<TRequestDto> : IRequestDto<PatchListRequestDto<TRequestDto>>
        where TRequestDto : IRequestDto<TRequestDto> {
    public List<PatchRequestDto<TRequestDto>> Items { get; set; }

    public PatchListRequestDto<TRequestDto> TransformValues() {
        Items = Items
            .Select(item => {
                item.Data = item.Data.TransformValues();
                return item;
            }).ToList();
        return this;
    }
}
