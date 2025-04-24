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
        
        CreateMap<Domain.Entities.Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        CreateMap<Domain.Entities.Product, ProductDetailDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        CreateMap<CreateProductRequest, Domain.Entities.Product>();
        CreateMap<UpdateProductRequest, Domain.Entities.Product>();
        
        CreateMap<ProductImage, ProductImageDto>();
        CreateMap<CreateProductImageRequest, ProductImage>();
        
        CreateMap<ProductVariant, ProductVariantDto>();
        CreateMap<CreateProductVariantRequest, ProductVariant>();
        
        CreateMap<ProductVariantAttribute, VariantAttributeDto>();
        CreateMap<CreateVariantAttributeRequest, ProductVariantAttribute>();
    }
    
}