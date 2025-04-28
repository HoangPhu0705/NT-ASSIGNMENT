using AutoMapper;
using Domain.Entities;
using SharedViewModels.Category;
using SharedViewModels.Product;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category
        CreateMap<Category, CategoryDto>();
        CreateMap<Category, CategoryDetailDto>();
        CreateMap<CategoryAttribute, CategoryAttributeDto>()
            .ForMember(dest => dest.Values, opt => opt.MapFrom(src =>
                src.ProductVariantAttributes != null && src.ProductVariantAttributes.Any()
                    ? src.ProductVariantAttributes.SelectMany(pva => pva.Values.Select(v => v.Value)).Distinct()
                    : new List<string>()));
        
        
        // Product
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
        
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => 
                src.Variants != null && src.Variants.Any() 
                    ? src.Variants.Min(v => v.Price) 
                    : 0))
            .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants));
        
        CreateMap<ProductVariant, ProductVariantDto>()
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.AttributeValues.Select(av => 
                new VariantAttributeDto()
                {
                    Name = av.ProductVariantAttribute.CategoryAttribute.Name,
                    Value = av.Value
                })));
        
        CreateMap<UpdateProductImageRequest, ProductImage>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
    
}