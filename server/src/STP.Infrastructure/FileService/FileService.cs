using Microsoft.Extensions.Options;
using Microsoft.ServiceFabric.Services.Communication;
using STP.Common.Options;
using STP.Interfaces;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace STP.Infrastructure.FileService
{
    public class FileService : IFileService
    {         
        public async Task<Stream> DownloadFileAsync(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException();
            }
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }
            return File.OpenRead(filePath);
        }

        public async Task<string> UploadFileAsync(Stream file, string path, string extension, string fileName = null)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = GenerateFileName();
            }
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fullPath = Path.Combine(path, Path.ChangeExtension(fileName, extension));
            using (var fileStream = File.Create(fullPath))
            {
                file.Seek(0, SeekOrigin.Begin);
                await file.CopyToAsync(fileStream);
            }
            return fullPath;
        }

        private string GenerateFileName()
        {   
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssfffffff", CultureInfo.InvariantCulture);
        }
    }
}
