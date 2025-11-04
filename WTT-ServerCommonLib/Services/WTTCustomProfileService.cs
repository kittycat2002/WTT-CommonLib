using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using WTTServerCommonLib.Helpers;
using Path = System.IO.Path;

namespace WTTServerCommonLib.Services;

[Injectable(InjectionType.Singleton)]
public class WTTCustomProfileService(
    ModHelper modHelper, 
    ISptLogger<WTTCustomProfileService> logger, 
    ConfigHelper configHelper, 
    DatabaseService databaseService)
{
    /// <summary>
    /// Loads custom player profile editions from JSON/JSONC files and registers them to the game database.
    /// 
    /// Profiles are loaded from the mod's "config/CustomProfiles" directory (or a custom path if specified).
    /// </summary>
    /// <param name="assembly">The calling assembly, used to determine the mod folder location</param>
    /// <param name="relativePath">(OPTIONAL) Custom path relative to the mod folder</param>
    public async Task AddCustomProfiles(Assembly assembly, string? relativePath = null)

    {
        
        var assemblyLocation = modHelper.GetAbsolutePathToModFolder(assembly);
        var defaultDir = Path.Combine("db", "CustomProfiles");
        var finalDir = Path.Combine(assemblyLocation, relativePath ?? defaultDir);

        if (!Directory.Exists(finalDir))
        {
            logger.Warning($"Custom profiles directory not found at {finalDir}");
            return;
        }

        var jsonFiles = Directory.GetFiles(finalDir, "*.json")
            .Concat(Directory.GetFiles(finalDir, "*.jsonc"))
            .ToArray();
            
        if (jsonFiles.Length == 0)
        {
            logger.Warning($"No custom profile files found in {finalDir}");
            return;
        }

        var profiles = databaseService.GetTemplates().Profiles;
        foreach (var file in jsonFiles)
        {
            try
            {
                var profileName = Path.GetFileNameWithoutExtension(file);
                var profileData = await configHelper.LoadJsonFile<ProfileSides>(file);

                if (profileData == null)
                {
                    logger.Error($"Failed to load profile data from {file}");
                    continue;
                }

                profiles[profileName] = profileData;
                LogHelper.Debug(logger, $"Successfully added custom profile: {profileName}");
            }
            catch (Exception ex)
            {
                logger.Error($"Error loading profile file {file}: {ex.Message}");
            }
        }
    }
}
