using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Airport.UI.Models.Interface;

public interface IFileOperation
{
    Task<string> UploadFile(IFormFile file);
    string GetFile(string key);
}
