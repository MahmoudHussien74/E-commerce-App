using Microsoft.AspNetCore.Http;

namespace E_commerce.Application.Abstractions.Services;

public interface IImageManagementService
{
    Task<IReadOnlyList<string>> AddImageAsync(IFormFileCollection files, string folderName);
    Task DeleteImageAsync(string path);
}
