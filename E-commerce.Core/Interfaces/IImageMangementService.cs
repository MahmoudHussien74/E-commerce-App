using Microsoft.AspNetCore.Http;

namespace E_commerce.Core.Interfaces;

public interface IImageMangementService
{
    Task<List<string>> AddImageAsync(IFormFileCollection files, string src);
    void DeleteImageAsync(string src);
}
