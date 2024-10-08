﻿namespace NATSInternal.Services.Dtos;

public class ProductDetailResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Unit { get; set; }
    public long Price { get; set; }
    public decimal VatFactor { get; set; }
    public int StockingQuantity { get; set; }
    public bool IsForRetail { get; set; }
    public bool IsDiscontinued { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public string ThumbnailUrl { get; set; }
    public ProductCategoryResponseDto Category { get; set; }
    public BrandBasicResponseDto Brand { get; set; }
    public ProductAuthorizationResponseDto Authorization { get; set; }

    public ProductDetailResponseDto(
            Product product,
            ProductAuthorizationResponseDto authorization)
    {
        Id = product.Id;
        Name = product.Name;
        Description = product.Description;
        Unit = product.Unit;
        Price = product.Price;
        VatFactor = product.VatFactor;
        StockingQuantity = product.StockingQuantity;
        IsForRetail = product.IsForRetail;
        IsDiscontinued = product.IsDiscontinued;
        CreatedDateTime = product.CreatedDateTime;
        UpdatedDateTime = product.UpdatedDateTime;
        ThumbnailUrl = product.ThumbnailUrl;

        if (product.Category != null)
        {
            Category = new ProductCategoryResponseDto(product.Category);
        }

        if (product.Brand != null)
        {
            Brand = new BrandBasicResponseDto(product.Brand);
        }

        Authorization = authorization;
    }
}
