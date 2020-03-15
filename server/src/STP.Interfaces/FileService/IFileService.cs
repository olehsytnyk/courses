//using STP.Domain.FileService.DTO;
using System.IO;
using System.Threading.Tasks;

namespace STP.Interfaces
{
    public interface IFileService
    {
        Task<Stream> DownloadFileAsync(string filepath);
        Task<string> UploadFileAsync(Stream file, string path, string extension, string fileName = null);
    }
}
