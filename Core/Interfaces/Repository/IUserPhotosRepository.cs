using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repository
{
    public interface IUserPhotosRepository
    {
        Task<string?> GetPhotoAsync(Guid userId);
        Task<bool> UploadPhotoAsync(Guid userId, string photo);
        Task<bool> DeletePhotoAsync(Guid userId);
    }
}
