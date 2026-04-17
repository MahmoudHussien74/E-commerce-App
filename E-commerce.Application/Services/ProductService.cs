using E_commerce.Application.Abstractions.Persistence;
using E_commerce.Application.Abstractions.Services;
using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Category;
using E_commerce.Application.Contracts.Common;
using E_commerce.Application.Contracts.Product;
using E_commerce.Application.Errors;
using E_commerce.Core.Entities.Product;

namespace E_commerce.Application.Services;

internal sealed class ProductService(
    IUnitOfWork unitOfWork,
    IImageManagementService imageManagementService) : IProductService
{
    public async Task<Result<PaginatedList<ProductResponse>>> GetAllProductsAsync(RequestFilter? filter, CancellationToken cancellationToken = default)
    {
        var products = await unitOfWork.ProductRepository.GetAllProductsAsync(filter ?? new RequestFilter(), cancellationToken);
        var mapped = products.Items.Select(Map).ToList();
        return Result.Success(new PaginatedList<ProductResponse>(mapped, products.PageNumber, products.TotalCount, products.PageSize));
    }

    public async Task<Result<ProductResponse>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await unitOfWork.ProductRepository.GetProductByIdAsync(id, cancellationToken);
        return product is null
            ? Result.Failure<ProductResponse>(ProductErrors.NotFound)
            : Result.Success(Map(product));
    }

    public async Task<Result<ProductResponse>> AddAsync(ProductRequest request, CancellationToken cancellationToken = default)
    {
        var imagePaths = new List<string>();
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                NewPrice = request.NewPrice,
                OldPrice = request.OldPrice,
                CategoryId = request.CategoryId
            };

            unitOfWork.ProductRepository.Add(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            imagePaths = (await imageManagementService.AddImageAsync(request.Photo, request.Name)).ToList();
            unitOfWork.PhotoRepository.AddRange(imagePaths.Select(path => new Photo { ProductId = product.Id, ImageName = path }));

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            var created = await unitOfWork.ProductRepository.GetProductByIdAsync(product.Id, cancellationToken);
            return Result.Success(Map(created!));
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            foreach (var path in imagePaths)
            {
                await imageManagementService.DeleteImageAsync(path);
            }

            return Result.Failure<ProductResponse>(ProductErrors.AdditionFailed);
        }
    }

    public async Task<Result<ProductResponse>> UpdateAsync(int id, ProductRequest request, CancellationToken cancellationToken = default)
    {
        var newImagePaths = new List<string>();
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var product = await unitOfWork.ProductRepository.GetProductWithPhotosAsync(id, cancellationToken);
            if (product is null)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<ProductResponse>(ProductErrors.NotFound);
            }

            var existingPhotos = product.Photos.ToList();
            product.Name = request.Name;
            product.Description = request.Description;
            product.NewPrice = request.NewPrice;
            product.OldPrice = request.OldPrice;
            product.CategoryId = request.CategoryId;

            unitOfWork.ProductRepository.Update(product);

            newImagePaths = (await imageManagementService.AddImageAsync(request.Photo, request.Name)).ToList();
            unitOfWork.PhotoRepository.AddRange(newImagePaths.Select(path => new Photo { ProductId = product.Id, ImageName = path }));

            foreach (var photo in existingPhotos)
            {
                await unitOfWork.PhotoRepository.DeleteAsync(photo, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            foreach (var photo in existingPhotos)
            {
                await imageManagementService.DeleteImageAsync(photo.ImageName);
            }

            var updated = await unitOfWork.ProductRepository.GetProductByIdAsync(product.Id, cancellationToken);
            return Result.Success(Map(updated!));
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            foreach (var path in newImagePaths)
            {
                await imageManagementService.DeleteImageAsync(path);
            }

            return Result.Failure<ProductResponse>(ProductErrors.UpdateFailed);
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var product = await unitOfWork.ProductRepository.GetProductWithPhotosAsync(id, cancellationToken);
            if (product is null)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(ProductErrors.NotFound);
            }

            var photoPaths = product.Photos.Select(x => x.ImageName).ToList();
            await unitOfWork.ProductRepository.DeleteAsync(product, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            foreach (var path in photoPaths)
            {
                await imageManagementService.DeleteImageAsync(path);
            }

            return Result.Success();
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ProductErrors.UpdateFailed);
        }
    }

    private static ProductResponse Map(Product product)
        => new(
            product.Id,
            product.Name,
            product.Description,
            product.NewPrice,
            product.OldPrice,
            new CategoryResponse(product.Category.Id, product.Category.Name, product.Category.Description),
            product.Photos.Select(x => new ProductImageResponse(x.Id, x.ImageName)).ToList());
}
