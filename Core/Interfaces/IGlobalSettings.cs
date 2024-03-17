using Core.Enums;

namespace Core.Interfaces;

public interface IGlobalSettings
{
    GlobalSettingsType Key { get; set; }
    string? Value { get; set; }
}