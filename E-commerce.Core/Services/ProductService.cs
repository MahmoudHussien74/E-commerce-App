using E_commerce.Core.Common;
using E_commerce.Core.Contracts.Product;
using E_commerce.Core.Entities;
using E_commerce.Core.Entities.Product;
using E_commerce.Core.Errors;
using E_commerce.Core.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace E_commerce.Core.Services;

public class ProductService(IUnitOfWork unitOfWork, IMapper mapper, IImageMangementService imageMangementService) : IProductService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IImageMangementService _imageMangementService = imageMangementService;

    public async Task<Result<PaginatedList<ProductResponse>>> GetAllProductsAsync(RequestFilter? filters, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.ProductRepository.GetAllProductsAsync(filters ?? new RequestFilter(), cancellationToken);
        var mappedItems = _mapper.Map<List<ProductResponse>>(products.Items);
        var response = new PaginatedList<ProductResponse>(mappedItems, products.PageNumber, products.TotalCount, products.PageSize);
        return Result.Success(response);
    }

    public async Task<Result<ProductResponse>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.ProductRepository.GetProductById(id, cancellationToken);
        if (product is null) return Result.Failure<ProductResponse>(ProductErrors.NotFound);
        return Result.Success(_mapper.Map<ProductResponse>(product));
    }

    public async Task<Result> AddAsync(ProductRequest request, CancellationToken cancellationToken)
    {
        var imagePaths = new List<string>();
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var product = _mapper.Map<Product>(request);
            _unitOfWork.Repository<Product>().Add(product);

            await _unitOfWork.CompleteAsync();

            imagePaths = await _imageMangementService.AddImageAsync(request.Photo, request.Name);

            var photo = imagePaths.Select(src => new Photo
            {
                ProductId = product.Id,
                ImageName = src
            }).ToList();

            _unitOfWork.Repository<Photo>().AddRange(photo);

            await _unitOfWork.CommitAsync();
            return Result.Success();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            foreach (var path in imagePaths)
            {
                await _imageMangementService.DeleteImageAsync(path);
            }
            return Result.Failure(ProductErrors.AdditionFailed);
        }
    }

    public async Task<Result> UpdateAsync(int id, ProductRequest request, CancellationToken cancellationToken)
    {
        var newImagePaths = new List<string>();
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var product = await _unitOfWork.ProductRepository.GetProductById(id, cancellationToken);
            if (product is null) return Result.Failure(ProductErrors.NotFound);

            var oldPhotos = await _unitOfWork.PhotoRepository.GetPhotoByProductId(id);

            _mapper.Map(request, product);
            _unitOfWork.Repository<Product>().Update(product);

            newImagePaths = await _imageMangementService.AddImageAsync(request.Photo, request.Name);
            var newPhotos = newImagePaths.Select(x => new Photo
            {
                ProductId = id,
                ImageName = x
            }).ToList();

            _unitOfWork.Repository<Photo>().AddRange(newPhotos);

            await _unitOfWork.CommitAsync();

            foreach (var item in oldPhotos)
            {
                await _imageMangementService.DeleteImageAsync(item.ImageName);
            }


            return Result.Success();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            foreach (var path in newImagePaths)
            {
                await _imageMangementService.DeleteImageAsync(path);
            }
            return Result.Failure(ProductErrors.UpdateFailed);
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var product = await _unitOfWork.ProductRepository.GetProductIncludeCategoryAndPhotoAsync(id, cancellationToken);
            if (product is null) return Result.Failure(ProductErrors.NotFound);

            var photoPaths = product.Photos.Select(p => p.ImageName).ToList();

            await _unitOfWork.ProductRepository.DeleteAsync(product, cancellationToken);
            await _unitOfWork.CommitAsync();

            foreach (var path in photoPaths)
            {
                await _imageMangementService.DeleteImageAsync(path);
            }

            return Result.Success();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            return Result.Failure(ProductErrors.UpdateFailed); 
        }
    }
}
