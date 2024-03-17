using Core.Enums;

namespace Core.Interfaces.Repository;

public interface IGlobalSettingsRepository
{
    string GetValueByKey(GlobalSettingsType key);
}