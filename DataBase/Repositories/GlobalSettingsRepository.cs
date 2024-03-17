using Core.Enums;
using Core.Interfaces.Repository;

namespace DataBase.Repositories;

public class GlobalSettingsRepository : IGlobalSettingsRepository
{
    private readonly PnkDbContext _context;
    
    public GlobalSettingsRepository(PnkDbContext context)
    {
        _context = context;
    }

    public string GetValueByKey(GlobalSettingsType key)
    {
        var setting = _context.GlobalSettings.FirstOrDefault(x => key == x.Key);
        if (setting == null)
        {
            throw new Exception($"Setting with key \"{key}\" not found");
        }
        
        return setting.Value!;
    }
}