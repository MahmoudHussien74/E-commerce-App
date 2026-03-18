using AutoMapper;
using E_commerce.Core.Common;
using E_commerce.Core.Contracts.Product;
using E_commerce.Core.Entities.Product;
using E_commerce.Core.Errors;
using E_commerce.Core.Interfaces;

namespace E_commerce.Core.Services;

public class ProductService(IUnitOfWork unitOfWork,IMapper mapper,IImageMangementService imageMangementService) : IProductService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IImageMangementService _imageMangementService = imageMangementService;

    public async Task<Result<List<ProductResponse>>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.ProductRepository.GetAllAsync(x=>x.Category,x=>x.Photos);

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

    public async Task<Result> AddAsync(ProductRequest request,CancellationToken cancellationToken)
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
            // Logging can be added here if a logger is available
            return Result.Failure(ProductErrors.AdditionFailed);
        }
    }
}
