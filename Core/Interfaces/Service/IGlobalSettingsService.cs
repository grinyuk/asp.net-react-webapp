using Core.Enums;
using Core.Models.Assignment;

namespace Core.Interfaces.Service;

public interface IGlobalSettingsService
{
    string GetValueByKey(GlobalSettingsType key);
    AssignmentOption GetAssignmentOption();
}