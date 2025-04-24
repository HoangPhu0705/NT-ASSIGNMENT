using AutoMapper;
using Domain.Entities;
using SharedViewModels.Category;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<Category, CategoryDetailDto>();
    }
    
}