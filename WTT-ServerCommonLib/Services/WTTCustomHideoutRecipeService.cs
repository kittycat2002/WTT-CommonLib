using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Hideout;
using SPTarkov.Server.Core.Models.Spt.Server;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using WTTServerCommonLib.Helpers;
using LogLevel = SPTarkov.Server.Core.Models.Spt.Logging.LogLevel;

namespace WTTServerCommonLib.Services;

[Injectable(InjectionType.Singleton)]
public class WTTCustomHideoutRecipeService(
    ISptLogger<WTTCustomHideoutRecipeService> logger,
    DatabaseServer databaseServer,
    ModHelper modHelper,
    ConfigHelper configHelper
)
{
    private DatabaseTables? _database;

    /// <summary>
    /// Loads custom hideout crafting recipes from JSON/JSONC files and registers them to the hideout production system.
    /// 
    /// Recipes are loaded from the mod's "db/CustomHideoutRecipes" directory (or a custom path if specified).
    ///
    /// </summary>
    /// <param name="assembly">The calling assembly, used to determine the mod folder location</param>
    /// <param name="relativePath">(OPTIONAL) Custom path relative to the mod folder</param>
    public async Task CreateHideoutRecipes(Assembly assembly, string? relativePath = null)

    {
        try
        {
            
            var assemblyLocation = modHelper.GetAbsolutePathToModFolder(assembly);
            var defaultDir = Path.Combine("db", "CustomHideoutRecipes");
            var finalDir = Path.Combine(assemblyLocation, relativePath ?? defaultDir);

            if (_database == null) _database = databaseServer.GetTables();

            if (!Directory.Exists(finalDir))
            {
                logger.Error($"Directory not found at {finalDir}");
                return;
            }

            var recipes = await configHelper.LoadAllJsonFiles<HideoutProduction>(finalDir);

            if (recipes.Count == 0)
            {
                logger.Warning($"No valid hideout recipes found in {finalDir}");
                return;
            }

            foreach (var recipe in recipes)
            {
                if (!MongoId.IsValidMongoId(recipe.Id))
                {
                    logger.Error($"Missing or invalid Id in recipe for end product {recipe.EndProduct}");
                    continue;
                }

                var recipeExists = _database.Hideout.Production.Recipes != null &&
                                   _database.Hideout.Production.Recipes.Any(r => r.Id == recipe.Id);
                if (recipeExists)
                {
                    if (logger.IsLogEnabled(LogLevel.Debug))
                        LogHelper.Debug(logger, $"Recipe {recipe.Id} already exists, skipping");

                    continue;
                }

                _database.Hideout.Production.Recipes?.Add(recipe);
                LogHelper.Debug(logger, $"Added hideout recipe {recipe.Id} for item {recipe.EndProduct}");
            }
        }
        catch (Exception ex)
        {
            logger.Error($"Error loading hideout recipes: {ex.Message}");
        }
    }
}