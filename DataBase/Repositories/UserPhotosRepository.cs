using Core.Interfaces.Repository;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Repositories
{
    public class UserPhotosRepository : IUserPhotosRepository
    {
        protected readonly PnkDbContext _context;
        public UserPhotosRepository(PnkDbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetPhotoAsync(Guid userId)
        {
            var userPhotos = await _context.UserPhotos.FirstOrDefaultAsync(u => u.UserId == userId);

            return userPhotos?.Photo;
        }

        public async Task<bool> UploadPhotoAsync(Guid userId, string photo)
        {
            // Check if this an actual photo
            if (photo != null)
            {
                var userPhotos = await _context.UserPhotos.FirstOrDefaultAsync(u => u.UserId == userId);

                if (userPhotos != null)
                {
                    userPhotos.Photo = photo;
                    userPhotos.UploadDate = DateTime.UtcNow;
                }   
                else
                {
                    var dbUserPhotos = new UserPhotos()
                    {
                        UserId = userId,
                        Photo = photo,
                        UploadDate = DateTime.UtcNow
                    };

                    await _context.UserPhotos.AddAsync(dbUserPhotos);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            
            return false;
        }

        public async Task<bool> DeletePhotoAsync(Guid userId)
        {
            var userPhotos = await _context.UserPhotos.FirstOrDefaultAsync(u => u.UserId == userId);

            if (userPhotos != null)
            {
                userPhotos.Photo = null;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
