using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace E_commerce.Infrastructure.Service;

public class ImageMangementService(IFileProvider fileProvider) : IImageManagementService
{
    private readonly IFileProvider _fileProvider = fileProvider;

    public async Task<IReadOnlyList<string>> AddImageAsync(IFormFileCollection files, string folderName)
    {
        var savedImages = new List<string>();
        var imageDirectory = Path.Combine("wwwroot", "Images", folderName);

        if (!Directory.Exists(imageDirectory))
        {
            Directory.CreateDirectory(imageDirectory);
        }

        foreach (var file in files)
        {
            if (file.Length <= 0)
            {
                continue;
            }

            var imageName = Path.GetFileName(file.FileName);
            var physicalPath = Path.Combine(imageDirectory, imageName);
            var relativePath = $"Images/{folderName}/{imageName}";

            await using var stream = new FileStream(physicalPath, FileMode.Create);
            await file.CopyToAsync(stream);
            savedImages.Add(relativePath);
        }

        return savedImages;
    }

    public Task DeleteImageAsync(string path)
    {
        var fileInfo = _fileProvider.GetFileInfo(path);
        if (File.Exists(fileInfo.PhysicalPath))
        {
            File.Delete(fileInfo.PhysicalPath!);
        }

        return Task.CompletedTask;
    }
}
