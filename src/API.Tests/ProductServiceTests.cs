﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.Products;
using Application.Services.Product;
using AutoMapper;
using Domain.Entities;
using Moq;
using SharedViewModels.Product;
using SharedViewModels.Shared;
using Xunit;

namespace API.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _productService = new ProductService(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetById_ExistingProduct_ReturnsSuccessResponse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
            };

            var productDetailDto = new ProductDetailDto
            {
                Id = productId,
                Name = "Test Product"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(product);
                
            _mockMapper.Setup(m => m.Map<ProductDetailDto>(product))
                .Returns(productDetailDto);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(productId, result.Data.Id);
            Assert.Equal("Test Product", result.Data.Name);
        }

        [Fact]
        public async Task GetById_NonExistingProduct_ReturnsErrorResponse()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(nonExistingId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _productService.GetProductByIdAsync(nonExistingId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.Contains("not found", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        
    }   
}