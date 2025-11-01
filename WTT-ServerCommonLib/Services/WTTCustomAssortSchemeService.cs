using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using WTTServerCommonLib.Helpers;
using WTTServerCommonLib.Models;
using Path = System.IO.Path;

namespace WTTServerCommonLib.Services;

[Injectable(InjectionType.Singleton)]
public class WTTCustomAssortSchemeService(
    DatabaseServer databaseServer,
    ISptLogger<WTTCustomAssortSchemeService> logger,
    ModHelper modHelper,
    ConfigHelper configHelper
)
{
    private readonly List<Dictionary<string, TraderAssort>> _customAssortSchemes = new();

    /// <summary>
    /// Loads custom trader assortment schemes from JSON/JSONC files and merges them into trader inventories.
    /// 
    /// Assort schemes are loaded from the mod's "db/CustomAssortSchemes" directory (or a custom path if specified).
    /// </summary>
    /// <param name="assembly">The calling assembly, used to determine the mod folder location</param>
    /// <param name="relativePath">(OPTIONAL) Custom path relative to the mod folder</param>
    public async Task CreateCustomAssortSchemes(Assembly assembly, string? relativePath = null)

    {
        
        var assemblyLocation = modHelper.GetAbsolutePathToModFolder(assembly);
        var defaultDir = Path.Combine("db", "CustomAssortSchemes");
        var finalDir = Path.Combine(assemblyLocation, relativePath ?? defaultDir);

        if (!Directory.Exists(finalDir))
            throw new DirectoryNotFoundException($"Config directory not found at {finalDir}");

        var jsonFiles = Directory.GetFiles(finalDir, "*.json")
            .Concat(Directory.GetFiles(finalDir, "*.jsonc"))
            .ToArray();
        if (jsonFiles.Length == 0)
        {
            logger.Warning($"No assort scheme files found in {finalDir}");
            return;
        }

        var assortList = await configHelper.LoadAllJsonFiles<Dictionary<string, TraderAssort>>(finalDir);

        if (assortList.Count == 0)
        {
            logger.Warning($"No assort data could be loaded from {finalDir}");
            return;
        }

        foreach (var assortData in assortList)
        {
            _customAssortSchemes.Add(assortData);
            LogHelper.Debug(logger, $"Loaded {assortData.Count} trader assort(s)");
        }

        ApplyAssorts();
    }

    private void ApplyAssorts()
    {
        var tables = databaseServer.GetTables();

        foreach (var schemeDict in _customAssortSchemes)
        foreach (var kvp in schemeDict)
        {
            var traderKey = kvp.Key;
            var newAssort = kvp.Value;

            MongoId actualTraderId;

            if (TraderIds.TraderMap.TryGetValue(traderKey.ToLower(), out var traderId))
            {
                actualTraderId = traderId;
            }
            else if (traderKey.IsValidMongoId())
            {
                actualTraderId = traderKey;
            }
            else
            {
                logger.Error($"Invalid trader key: {traderKey}");
                continue;
            }

            if (!tables.Traders.TryGetValue(actualTraderId, out var trader))
            {
                logger.Warning($"Trader not found in DB: ({actualTraderId})");
                continue;
            }

            trader.Assort.Items.AddRange(newAssort.Items);

            foreach (var scheme in newAssort.BarterScheme) trader.Assort.BarterScheme[scheme.Key] = scheme.Value;

            foreach (var levelItem in newAssort.LoyalLevelItems)
                trader.Assort.LoyalLevelItems[levelItem.Key] = levelItem.Value;

            LogHelper.Debug(logger, $"Merged {newAssort.Items.Count} items into trader");
        }
    }
}