namespace NATSInternal.Services.Dtos;

public class PatchRequestDto<TRequestDto> : IRequestDto<PatchRequestDto<TRequestDto>>
        where TRequestDto : IRequestDto<TRequestDto> {

    public PatchOperation Operation { get; set; }
    public int ResourceId { get; set; }
    public string[] PropertyNames { get; set; }
    public TRequestDto Data { get; set; }

    [JsonIgnore]
    public static string[] AllowedPropertyNames => typeof(TRequestDto)
        .GetProperties()
        .Select(p => p.Name)
        .ToArray();
    
    public PatchRequestDto<TRequestDto> TransformValues() {
        return this;
    }
}
