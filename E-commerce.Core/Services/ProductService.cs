using AutoMapper;
using E_commerce.Core.Common;
using E_commerce.Core.Contracts.Product;
using E_commerce.Core.Entities.Product;
using E_commerce.Core.Errors;
using E_commerce.Core.Interfaces;

namespace E_commerce.Core.Services;

public class ProductService(IUnitOfWork unitOfWork, IMapper mapper, IImageMangementService imageMangementService) : IProductService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IImageMangementService _imageMangementService = imageMangementService;

    public async Task<Result<List<ProductResponse>>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.ProductRepository.GetAllAsync(x => x.Category, x => x.Photos);

        var response = _mapper.Map<List<ProductResponse>>(products);

        return Result.Success(response);
    }

    public async Task<Result<ProductResponse>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.ProductRepository.GetProductById(id, cancellationToken);
        
        if (product is null)
            return Result.Failure<ProductResponse>(ProductErrors.NotFound);

        var response = _mapper.Map<ProductResponse>(product);

        return Result.Success(response);
    }

    public async Task<Result> AddAsync(ProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var product = _mapper.Map<Product>(request);

            await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken);

            var imagePath = await _imageMangementService.AddImageAsync(request.Photo, request.Name);

            var photo = imagePath.Select(src => new Photo
            {
                ProductId = product.Id,
                ImageName = src
            }).ToList();

            await _unitOfWork.PhotoRepository.AddRangeAsync(photo, cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(ProductErrors.AdditionFailed);
        }
    }

    public async Task<Result> UpdateAsync(int id, ProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.ProductRepository.GetProductById(id, cancellationToken);

            if (product is null)
                return Result.Failure(ProductErrors.NotFound);

            var oldPhotos = await _unitOfWork.PhotoRepository.GetPhotoByProductId(id);

            _mapper.Map(request, product);

            // 1. Add new photos to storage
            var imagePaths = await _imageMangementService.AddImageAsync(request.Photo, request.Name);
            var newPhotos = imagePaths.Select(x => new Photo
            {
                ProductId = id,
                ImageName = x
            }).ToList();

            await _unitOfWork.PhotoRepository.AddRangeAsync(newPhotos, cancellationToken);

            // 2. Delete old photos from storage and DB
            foreach (var item in oldPhotos)
            {
                await _imageMangementService.DeleteImageAsync(item.ImageName);
            }
            await _unitOfWork.PhotoRepository.DeleteRangeAsync(oldPhotos, cancellationToken);

            // 3. Update the product
            await _unitOfWork.ProductRepository.UpdateAsync(product, cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(ProductErrors.UpdateFailed);
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.ProductRepository.GetProductIncludeCategoryAndPhotoAsync(id, cancellationToken);

        if (product is null)
            return Result.Failure(ProductErrors.NotFound);

        // 1. Delete associated physical images first
        foreach (var item in product.Photos)
        {
            await _imageMangementService.DeleteImageAsync(item.ImageName);
        }

        // 2. Delete from DB using the entity overload for better performance
        await _unitOfWork.ProductRepository.DeleteAsync(product, cancellationToken);

        return Result.Success();
    }
}
