using AutoMapper;
using Domain.Entities;
using SharedViewModels.Category;
using SharedViewModels.Product;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<Category, CategoryDetailDto>();
        
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        CreateMap<Product, ProductDetailDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        CreateMap<CreateProductRequest, Product>();
        CreateMap<UpdateProductRequest, Product>();
        
        CreateMap<ProductImage, ProductImageDto>();
        CreateMap<CreateProductImageRequest, ProductImage>();
        
        CreateMap<ProductVariant, ProductVariantDto>();
        CreateMap<CreateProductVariantRequest, ProductVariant>();
        
        CreateMap<ProductVariantAttribute, VariantAttributeDto>();
        CreateMap<CreateVariantAttributeRequest, ProductVariantAttribute>();
    }
    
}