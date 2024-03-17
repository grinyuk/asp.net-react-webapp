using Core.Entities;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Services
{
    public class UserPhotosService : IUserPhotosService
    {
        private readonly IUserPhotosRepository _userPhotosRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly AppConfig _appConfig;

        public UserPhotosService(IUserPhotosRepository userPhotosRepository, IMemoryCache memoryCache, AppConfig appConfig)
        {
            _userPhotosRepository = userPhotosRepository;
            _memoryCache = memoryCache;
            _appConfig = appConfig;
        }

        public async Task<string?> GetPhotoAsync(Guid userId)
        {
            if (_memoryCache.TryGetValue(userId, out string? photo))
            {
                return photo!;
            }

            photo = await _userPhotosRepository.GetPhotoAsync(userId);
            var ceo = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_appConfig.CacheLifeTime);
            _memoryCache.Set(userId, photo, ceo);
            return photo;
        }

        public async Task<bool> UploadPhotoAsync(Guid userId, string photo)
        {
            var ceo = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_appConfig.CacheLifeTime);
            _memoryCache.Set(userId, photo, ceo);
            return await _userPhotosRepository.UploadPhotoAsync(userId, photo);
        }

        public async Task<bool> DeletePhotoAsync(Guid userId)
        {
            _memoryCache.Remove(userId);
            return await _userPhotosRepository.DeletePhotoAsync(userId);
        }
    }
}