using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Categories;
using Application.Services.Categories;
using AutoMapper;
using Domain.Entities;
using Moq;
using SharedViewModels.Category;
using SharedViewModels.Shared;
using Xunit;

namespace API.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockRepository = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _categoryService = new CategoryService(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllCategories_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Electronics" },
                new Category { Id = Guid.NewGuid(), Name = "Books" }
            };

            var categoryDtos = new List<CategoryDto>
            {
                new CategoryDto { Id = categories[0].Id, Name = categories[0].Name },
                new CategoryDto { Id = categories[1].Id, Name = categories[1].Name }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(categories);
                
            _mockMapper.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories))
                .Returns(categoryDtos);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetById_ExistingCategory_ReturnsCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category { Id = categoryId, Name = "Electronics" };
            var categoryDto = new CategoryDetailDto { Id = categoryId, Name = "Electronics" };

            _mockRepository.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync(category);
                
            _mockMapper.Setup(m => m.Map<CategoryDetailDto>(category))
                .Returns(categoryDto);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(categoryId, result.Data.Id);
            Assert.Equal("Electronics", result.Data.Name);
        }

        [Fact]
        public async Task CreateCategory_ValidCategory_ReturnsNewCategory()
        {
            // Arrange
            var createRequest = new CreateCategoryRequest { Name = "New Category" };
            
            var createdCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = createRequest.Name
            };
            
            var categoryDetailDto = new CategoryDetailDto
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name
            };

            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Category>()))
                .ReturnsAsync(createdCategory);
                
            _mockRepository.Setup(repo => repo.GetByIdAsync(createdCategory.Id))
                .ReturnsAsync(createdCategory);
                
            _mockMapper.Setup(m => m.Map<CategoryDetailDto>(createdCategory))
                .Returns(categoryDetailDto);

            // Act
            var result = await _categoryService.CreateCategoryAsync(createRequest);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(createRequest.Name, result.Data.Name);
        }
    }
}