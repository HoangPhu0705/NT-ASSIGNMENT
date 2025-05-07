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

        foreach (var variant in product.Variants)
        {
            variant.Id = Guid.NewGuid();
            variant.ProductId = product.Id;
        }

        var createdProduct = await _productRepository.AddAsync(product);

        if (request.Variants != null && request.Variants.Any())
        {
            foreach (var requestVariant in request.Variants)
            {
                if (requestVariant.Attributes != null && requestVariant.Attributes.Any())
                {
                    var savedVariant = createdProduct.Variants.FirstOrDefault(v => v.Name == requestVariant.Name);
                    if (savedVariant != null)
                    {
                        await _productRepository.AddVariantAttributesAsync(savedVariant, requestVariant.Attributes);
                    }
                }
            }
        }

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

        if (request.Images != null)
        {
            var imageIdsToDelete = request.Images
                .Where(i => i.Id != null && i.IsDeleted)
                .Select(i => i.Id.Value)
                .ToList();

            if (imageIdsToDelete.Any())
            {
                await _productRepository.DeleteProductImagesAsync(imageIdsToDelete);
                product.Images = product.Images.Where(img => !imageIdsToDelete.Contains(img.Id)).ToList();
            }

            // Update existing images
            foreach (var imageToUpdate in request.Images.Where(i => i.Id != null && !i.IsDeleted))
            {
                var existingImage = product.Images.FirstOrDefault(i => i.Id == imageToUpdate.Id);
                if (existingImage != null)
                {
                    existingImage.ImageUrl = imageToUpdate.ImageUrl;
                    existingImage.IsPrimary = imageToUpdate.IsMain;
                }
            }

            // Add new images
            foreach (var newImage in request.Images.Where(i => i.Id == null && !i.IsDeleted))
            {
                product.Images.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = newImage.ImageUrl,
                    IsPrimary = newImage.IsMain
                });
            }

            // Ensure at least one image is primary
            if (product.Images.Any() && !product.Images.Any(i => i.IsPrimary))
            {
                product.Images.First().IsPrimary = true;
            }
        }

        // Handle variants
        if (request.Variants != null)
        {
            // Get IDs of variants marked for deletion
            var variantIdsToDelete = request.Variants
                .Where(v => v.Id != null && v.IsDeleted)
                .Select(v => v.Id.Value)
                .ToList();

            if (variantIdsToDelete.Any())
            {
                await _productRepository.DeleteProductVariantsAsync(variantIdsToDelete);
                product.Variants = product.Variants.Where(v => !variantIdsToDelete.Contains(v.Id)).ToList();
            }

            // Update existing variants
            foreach (var variantToUpdate in request.Variants.Where(v => v.Id != null && !v.IsDeleted))
            {
                var existingVariant = product.Variants.FirstOrDefault(v => v.Id == variantToUpdate.Id);
                if (existingVariant != null)
                {
                    existingVariant.Name = variantToUpdate.Name;
                    existingVariant.SKU = variantToUpdate.Sku;
                    existingVariant.Price = variantToUpdate.Price;
                    existingVariant.Stock = variantToUpdate.Stock;

                    // Update attributes if provided
                    if (variantToUpdate.Attributes != null && variantToUpdate.Attributes.Any())
                    {
                        await _productRepository.UpdateVariantAttributesAsync(existingVariant, variantToUpdate.Attributes);
                    }
                }
            }

            // Add new variants
            foreach (var newVariant in request.Variants.Where(v => v.Id == null && !v.IsDeleted))
            {
                var variant = new ProductVariant
                {
                    ProductId = product.Id,
                    Name = newVariant.Name,
                    SKU = newVariant.Sku,
                    Price = newVariant.Price,
                    Stock = newVariant.Stock
                };
                
                // Add attributes if provided
                if (newVariant.Attributes != null)
                {
                    variant.AttributeValues = newVariant.Attributes.Select(attr => new VariantAttributeValue
                    {
                        // Map attribute properties
                        Name = attr.Name,
                        Value = attr.Value
                    }).ToList();
                }
                
                product.Variants.Add(variant);
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
    
    public async Task<ApiResponse<bool>> DeleteProductVariantAsync(Guid productId, Guid variantId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return ApiResponse<bool>.Error($"Product with ID {productId} not found");

        var variant = product.Variants?.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            return ApiResponse<bool>.Error($"Variant with ID {variantId} not found in product {productId}");

        await _productRepository.DeleteProductVariantsAsync(new List<Guid> { variantId });
    
        return ApiResponse<bool>.Success(true);
    }
}