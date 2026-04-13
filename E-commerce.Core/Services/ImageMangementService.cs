using E_commerce.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace E_commerce.Core.Services;

public class ImageMangementService(IFileProvider fileProvider) : IImageMangementService
{
    private readonly IFileProvider _fileProvider = fileProvider;

    public async Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
    {
        var saveImage = new List<string>();

        var imageDirecory = Path.Combine("wwwroot", "Images", src);

        if (Directory.Exists(imageDirecory) is not true)
            Directory.CreateDirectory(imageDirecory);

        foreach (var item in files)
        {
            if (item.Length > 0)
            {
                var imageName = item.FileName;
                var root = Path.Combine("wwwroot", "Images", src, imageName);
                var imageSource = $"Images/{src}/{imageName}";

                using (FileStream stream = new FileStream(root, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }
                saveImage.Add(imageSource);
            }
        }
             return saveImage;
    }
    public Task DeleteImageAsync(string src)
    {
        var info = _fileProvider.GetFileInfo(src);
        var root = info.PhysicalPath;
        if (File.Exists(root))
            File.Delete(root);

        return Task.CompletedTask;
    }
}
