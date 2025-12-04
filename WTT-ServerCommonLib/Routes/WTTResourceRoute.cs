using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using WTTServerCommonLib.Helpers;
using WTTServerCommonLib.Models;
using WTTServerCommonLib.Services;

namespace WTTServerCommonLib.Routes;

[Injectable]
public class WTTResourcesRouter(
    JsonUtil jsonUtil,
    WTTCustomQuestZoneService zoneService,
    WTTCustomRigLayoutService rigService,
    WTTCustomSlotImageService slotService,
    WTTCustomStaticSpawnService staticSpawnService,
    WTTCustomVoiceService voiceService,
    WTTCustomAudioService audioService,
    WTTCustomCustomizationService customizationService,
    ISptLogger<WTTResourcesRouter> logger) : StaticRouter(jsonUtil, [
    
    // Zones
    new RouteAction<EmptyRequestData>(
        "/wttcommonlib/zones/get", (_, _, _, _) =>
        {
            var zones = zoneService.GetZones();
            return ValueTask.FromResult(jsonUtil.Serialize(zones) ??
                                        throw new NullReferenceException("Could not serialize voice mappings!"));
        }
    ),

    new RouteAction<EmptyRequestData>(
        "/wttcommonlib/riglayouts/get", async (_, _, _, _) =>
        {
            var allBundles = rigService.GetLayoutManifest();
            var payload = new Dictionary<string, string>();
            foreach (var bundleName in allBundles)
            {
                var bundleData = await rigService.GetBundleData(bundleName);
                if (bundleData?.Length > 0)
                    payload.Add(bundleName, Convert.ToBase64String(bundleData));
            }

            return jsonUtil.Serialize(payload) ?? throw new NullReferenceException("Could not serialize payload!");
        }
    ),

    // Configs route
    new RouteAction<EmptyRequestData>(
        "/wttcommonlib/spawnsystem/configs/get", (_, _, _, _) =>
        {
            var configs = staticSpawnService.GetAllSpawnConfigs();
            return ValueTask.FromResult(jsonUtil.Serialize(configs) ?? string.Empty);
        }
    ),

    new RouteAction<EmptyRequestData>(
        "/wttcommonlib/slotimages/get", async (_, _, _, _) =>
        {
            var result = new Dictionary<string, string>();

            foreach (var name in slotService.GetImageManifest())
            {
                var data = await slotService.GetImageData(name);
                if (data?.Length > 0) result.Add(name, Convert.ToBase64String(data));
            }

            return jsonUtil.Serialize(result) ?? throw new NullReferenceException("Could not serialize payload!");
        }
    ),
    new RouteAction<EmptyRequestData>(
        "/wttcommonlib/hideout/icons/get", async (_, _, _, _) =>
        {
            var result = new Dictionary<string, string>();

            foreach (var iconName in customizationService.GetHideoutIconManifest())
            {
                var data = await customizationService.GetHideoutIconData(iconName);
                if (data?.Length > 0) 
                    result.Add(iconName, Convert.ToBase64String(data));
            }

            return jsonUtil.Serialize(result) ?? throw new NullReferenceException("Could not serialize hideout icons!");
        }
    ),

    new RouteAction<EmptyRequestData>(
        "/wttcommonlib/hideout/marktextures/get", async (_, _, _, _) =>
        {
            var result = new Dictionary<string, string>();

            foreach (var textureName in customizationService.GetMarkTextureManifest())
            {
                var data = await customizationService.GetMarkTextureData(textureName);
                if (data?.Length > 0) 
                    result.Add(textureName, Convert.ToBase64String(data));
            }

            return jsonUtil.Serialize(result) ?? throw new NullReferenceException("Could not serialize mark textures!");
        }
    ),

    // Voices
    new RouteAction<EmptyRequestData>(
        "/wttcommonlib/voices/get", (_, _, _, _) =>
        {
            var voiceMappings = voiceService.GetVoiceBundleMappings();
            return ValueTask.FromResult(jsonUtil.Serialize(voiceMappings) ??
                                        throw new NullReferenceException("Could not serialize voice mappings!"));
        }
    ),
    
    // CustomAudio
    new RouteAction<EmptyRequestData>(
    "/wttcommonlib/audio/manifest/get", (_, _, _, _) =>
    {
        var manifest = audioService.GetAudioManifest();
        return ValueTask.FromResult(jsonUtil.Serialize(manifest) ??
                                    throw new NullReferenceException("Could not serialize audio manifest!"));
    }
    ),
]);