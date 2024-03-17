using Core.Enums;
using Core.Helpers;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Models.Assignment;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Win32;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;

namespace Core.Services;

public class GlobalSettingsService : IGlobalSettingsService
{
    private readonly IGlobalSettingsRepository _globalSettingsRepository;
    private readonly IMemoryCache _memoryCache;

    public GlobalSettingsService(IGlobalSettingsRepository globalSettingsRepository, IMemoryCache memoryCache)
    {
        _globalSettingsRepository = globalSettingsRepository;
        _memoryCache = memoryCache;
    }

    public string GetValueByKey(GlobalSettingsType key)
    {
        if (_memoryCache.TryGetValue(key, out string? cachedData))
        {
            return cachedData!;
        }

        var result = _globalSettingsRepository.GetValueByKey(key);
        var ceo = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));
        _memoryCache.Set(key, result, ceo);
        return result;
    }

    public AssignmentOption GetAssignmentOption()
    {
        var options = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30));
        try
        {
            if (_memoryCache.TryGetValue(nameof(AssignmentOption), out AssignmentOption? assignmentOption))
            {
                return assignmentOption!;
            }

            assignmentOption = new AssignmentOption()
            {
                AssignmentPeriodsOption = JsonSerializer.Deserialize<List<int>>(GetValueByKey(GlobalSettingsType.AssignmentPeriodsOption)) ?? throw new NullReferenceException(nameof(GlobalSettingsType.AssignmentPeriodsOption)),
                AssignmentPeriodsScoreOption = JsonSerializer.Deserialize<List<int>>(GetValueByKey(GlobalSettingsType.AssignmentPeriodsScoreOption)) ?? throw new NullReferenceException(nameof(GlobalSettingsType.AssignmentPeriodsScoreOption)),
                AssignmentArchivePeriodScoreOption = int.Parse(GetValueByKey(GlobalSettingsType.AssignmentArchivePeriodScoreOption)),
                AssignmentAnswerAccuracyOption = int.Parse(GetValueByKey(GlobalSettingsType.AssignmentAnswerAccuracyOption))
            };

            _memoryCache.Set(nameof(AssignmentOption), assignmentOption, options);

            return assignmentOption;
        }
        catch (Exception ex)
        {
            _ = Logger.Instance.LogErrorAsync(nameof(GlobalSettingsService) + Constants.Arrow + nameof(GetAssignmentOption), ex);

            var assignmentOption = new AssignmentOption()
            {
                AssignmentPeriodsOption = new List<int> { 3, 2, 2 },
                AssignmentPeriodsScoreOption = new List<int> { 100, 50, 25 },
                AssignmentArchivePeriodScoreOption = 5,
                AssignmentAnswerAccuracyOption = 3
            };

            _memoryCache.Set(nameof(AssignmentOption), assignmentOption, options);

            return assignmentOption;
        }
    }
}