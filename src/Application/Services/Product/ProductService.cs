using Application.Interfaces.Products;
using AutoMapper;
using Domain.Entities;
using SharedViewModels.Product;
using SharedViewModels.Shared;

namespace Application.Services.Product;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    
    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;

    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
        return ApiResponse<IEnumerable<ProductDto>>.Success(productsDto);
    }

    public async Task<ApiResponse<ProductDetailDto>> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return ApiResponse<ProductDetailDto>.Error("Product not found");

        var productDto = _mapper.Map<ProductDetailDto>(product);
        return ApiResponse<ProductDetailDto>.Success(productDto);
    }

    public async Task<ApiResponse<ProductDetailDto>> CreateProductAsync(CreateProductRequest request)
    {
        var product = _mapper.Map<Domain.Entities.Product>(request);
        product.Id = Guid.NewGuid();
            
        var createdProduct = await _productRepository.AddAsync(product);
        var productWithDetails = await _productRepository.GetByIdAsync(createdProduct.Id);
        var productDto = _mapper.Map<ProductDetailDto>(productWithDetails);

        return ApiResponse<ProductDetailDto>.Created(productDto);
    }

    public async Task<ApiResponse<ProductDetailDto>> UpdateProductAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return ApiResponse<ProductDetailDto>.Error($"Product with ID {id} not found");

        if (request.Name != null) product.Name = request.Name;
        if (request.Description != null) product.Description = request.Description;
        if (request.CategoryId.HasValue) product.CategoryId = request.CategoryId.Value;

        if (request.Images != null && request.Images.Any())
        {
            // Find which image should be primary
            var primaryImageRequest = request.Images.FirstOrDefault(i => i.IsMain && !i.IsDeleted);
    
            foreach (var existingImage in product.Images.ToList())
            {
                var updatedImage = request.Images.FirstOrDefault(i => i.Id == existingImage.Id);

                if (updatedImage == null || updatedImage.IsDeleted)
                {
                    product.Images.Remove(existingImage);
                }
                else
                {
                    existingImage.ImageUrl = updatedImage.ImageUrl;
                    // Set IsPrimary based on which image is designated as primary
                    existingImage.IsPrimary = (primaryImageRequest != null && updatedImage.Id == primaryImageRequest.Id);
                }
            }

            // Add new images
            foreach (var newImage in request.Images.Where(i => i.Id == null && !i.IsDeleted))
            {
                product.Images.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = newImage.ImageUrl,
                    IsPrimary = (primaryImageRequest != null && primaryImageRequest.Id == null && 
                                 primaryImageRequest.ImageUrl == newImage.ImageUrl)
                });
            }

            // Ensure there's a primary image if we have any images
            if (product.Images.Any() && !product.Images.Any(i => i.IsPrimary))
            {
                product.Images.First().IsPrimary = true;
            }
        }
        
        await _productRepository.UpdateAsync(product);
        
        var updatedProduct = await _productRepository.GetByIdAsync(id);
        var productDto = _mapper.Map<ProductDetailDto>(updatedProduct);
        return ApiResponse<ProductDetailDto>.Success(productDto);
    }

    public async Task<ApiResponse<bool>> DeleteProductAsync(Guid id)
    {
        var exists = await _productRepository.ExistsAsync(id);
        if (!exists)
            return ApiResponse<bool>.Error("Product not found");

        var result = await _productRepository.DeleteAsync(id);
        return ApiResponse<bool>.Success(result);
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> GetProductsByCategoryAsync(Guid categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
        return ApiResponse<IEnumerable<ProductDto>>.Success(productsDto);
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> SearchProductsAsync(string searchTerm)
    {
        var products = await _productRepository.SearchProductsAsync(searchTerm);
        var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
        return ApiResponse<IEnumerable<ProductDto>>.Success(productsDto);
    }
}