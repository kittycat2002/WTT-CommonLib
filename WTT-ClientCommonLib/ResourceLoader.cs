using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BepInEx.Logging;
using EFT.UI.DragAndDrop;
using UnityEngine;
using WTTClientCommonLib.Helpers;
using WTTClientCommonLib.Models;
using WTTClientCommonLib.Services;
using Object = UnityEngine.Object;

namespace WTTClientCommonLib;

public class ResourceLoader(ManualLogSource logger, AssetLoader assetLoader)
{
    public static AudioManifest Manifest { get; private set; } = new();
    public static AudioClipCache ClipCache { get; private set; }

    static ResourceLoader()
    {
        LogHelper.LogDebug("ResourceLoader static constructor: Creating AudioClipCache");
        var go = new GameObject("WTT_AudioClipCache");
        Object.DontDestroyOnLoad(go);
        ClipCache = go.AddComponent<AudioClipCache>();
        ClipCache.hideFlags = HideFlags.DontUnloadUnusedAsset;
        LogHelper.LogDebug("AudioClipCache created and set to DontDestroyOnLoad");
    }

    public async void LoadAllResourcesFromServer()
    {
        try
        {
            LogHelper.LogDebug("Loading resources from server...");
            LoadVoicesFromServer();
            LoadSlotImagesFromServer();
            LoadRigLayoutsFromServer();
            LoadAudioManifestFromServer();
            LoadAudioBundlesFromServer();
            assetLoader.InitializeBundles("/wttcommonlib/spawnsystem/bundles/get");
            assetLoader.SpawnConfigs = assetLoader.FetchSpawnConfigs("/wttcommonlib/spawnsystem/configs/get");
            LogHelper.LogDebug($"Loaded {assetLoader.SpawnConfigs.Count} spawn configurations");
            LogHelper.LogDebug("All resources loaded successfully from server");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error loading resources from server: {ex}");
        }
    }
    public async Task LoadAudioManifestFromServer()
    {
        try
        {
            LogHelper.LogDebug("Loading audio manifest from server...");
        
            var manifestResponse = Utils.Get<AudioManifest>("/wttcommonlib/audio/manifest/get");
            if (manifestResponse == null)
            {
                LogHelper.LogWarn("No audio manifest received from server");
                return;
            }

            Manifest = manifestResponse;
            LogHelper.LogDebug($"Loaded manifest with {Manifest.AudioBundles.Count} audio bundles");
            LogHelper.LogDebug($"FaceCard mappings: {Manifest.FaceCardMappings.Count} entries");
            LogHelper.LogDebug($"Radio audio: {Manifest.RadioAudio.Count} entries");
        }
        catch (Exception ex)
        {
            LogHelper.LogError($"Error loading audio manifest: {ex}");
        }
    }
    public void LoadAudioBundlesFromServer()
    {
        try
        {
            var bundleMap = Utils.Get<Dictionary<string, string>>("/wttcommonlib/audio/bundles/get");
            if (bundleMap == null)
            {
                LogHelper.LogWarn("No audio bundles received from server");
                return;
            }

            LogHelper.LogDebug($"Received {bundleMap.Count} audio bundles");

            foreach (var kvp in bundleMap)
            {
                var bundleName = kvp.Key;
                var base64Data = kvp.Value;
        
                if (string.IsNullOrEmpty(base64Data))
                    continue;

                try
                {
                    byte[] bundleData = Convert.FromBase64String(base64Data);
                    var bundle = AssetBundle.LoadFromMemory(bundleData);
            
                    if (bundle == null)
                    {
                        LogHelper.LogWarn($"Failed to load audio bundle: {bundleName}");
                        continue;
                    }

                    var clips = bundle.LoadAllAssets<AudioClip>();
                    foreach (var clip in clips)
                    {
                        clip.LoadAudioData();
                        ClipCache.CacheAudioClip(clip.name, clip);
                        LogHelper.LogDebug($"Cached audio from bundle: {clip.name}");
                    }

                    LogHelper.LogDebug($"Loaded {clips.Length} audio clips from {bundleName}");
                }
                catch (Exception ex)
                {
                    LogHelper.LogError($"Failed to load audio bundle {bundleName}: {ex.Message}");
                }
            }

            LogHelper.LogDebug("Audio bundles loaded successfully");
        }
        catch (Exception ex)
        {
            LogHelper.LogError($"Error loading audio bundles: {ex}");
        }
    }

    
public static string GetAudioForFace(string faceName)
{
    LogHelper.LogDebug($"GetAudioForFace called with: {faceName}");
    LogHelper.LogDebug($"Manifest has {Manifest.FaceCardMappings.Count} face entries");
    
    if (Manifest.FaceCardMappings.TryGetValue(faceName, out var audioKeys))
    {
        if (audioKeys.Count > 0)
        {
            var audioKey = audioKeys[0];
            LogHelper.LogDebug($"Returning audio key: {audioKey}");
            
            if (ResourceLoader.ClipCache.TryGetAudioClip(audioKey, out var clip))
            {
                LogHelper.LogDebug($"Clip in cache: {clip.name}, duration: {clip.length}s");
            }
            else
            {
                LogHelper.LogDebug($"Clip NOT in cache: {audioKey}");
            }
            
            return audioKey;
        }
    }
    
    return null;
}


public static List<string> GetRadioAudio()
{
    LogHelper.LogDebug($"GetRadioAudio called. Returning {Manifest.RadioAudio.Count} radio tracks");
    LogHelper.LogDebug($"Radio audio: {string.Join(", ", Manifest.RadioAudio)}");
    return Manifest.RadioAudio;
}
    private void LoadVoicesFromServer()
    {
        try
        {
            var voiceResponse = Utils.Get<Dictionary<string, string>>("/wttcommonlib/voices/get");
            if (voiceResponse == null)
            {
                logger.LogWarning("No voice data received from server");
                return;
            }

            foreach (var kvp in voiceResponse)
                if (!ResourceKeyManagerAbstractClass.Dictionary_0.ContainsKey(kvp.Key))
                {
                    ResourceKeyManagerAbstractClass.Dictionary_0[kvp.Key] = kvp.Value;
                    LogHelper.LogDebug($"Added voice key: {kvp.Key}");
                }

            LogHelper.LogDebug($"Loaded {voiceResponse.Count} voice mappings from server");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error loading voices: {ex}");
        }
    }

    private void LoadSlotImagesFromServer()
    {
        try
        {
            var images = Utils.Get<Dictionary<string, string>>("/wttcommonlib/slotimages/get");
            if (images == null)
            {
                logger.LogWarning("No slot images");
                return;
            }

            foreach (var kvp in images)
            {
                byte[] imageData;
                try
                {
                    imageData = Convert.FromBase64String(kvp.Value);
                }
                catch
                {
                    logger.LogWarning($"Invalid data for {kvp.Key}");
                    continue;
                }

                CreateAndRegisterSlotImage(imageData, kvp.Key);
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Error loading slot images: {ex}");
        }
    }

    private void LoadRigLayoutsFromServer()
    {
        try
        {
            var bundleMap = Utils.Get<Dictionary<string, string>>("/wttcommonlib/riglayouts/get");
            if (bundleMap == null)
            {
                logger.LogWarning("No rig layouts received from server");
                return;
            }

            LogHelper.LogDebug($"Received {bundleMap.Count} rig layouts from server");

            foreach (var kvp in bundleMap)
            {
                var bundleName = kvp.Key;
                var base64Data = kvp.Value;
                if (string.IsNullOrEmpty(base64Data))
                {
                    logger.LogWarning($"No data for rig layout: {bundleName}");
                    continue;
                }

                byte[] bundleData;
                try
                {
                    bundleData = Convert.FromBase64String(base64Data);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Base64 decode failed for rig layout {bundleName}: {ex}");
                    continue;
                }

                if (bundleData.Length == 0)
                {
                    logger.LogWarning($"Bundle data is empty for rig layout: {bundleName}");
                    continue;
                }

                LoadBundleFromMemory(bundleData, bundleName);
            }

            LogHelper.LogDebug($"Loaded {bundleMap.Count} rig layouts from server");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error loading rig layouts: {ex}");
        }
    }

    private void CreateAndRegisterSlotImage(byte[] data, string slotID)
    {
        try
        {
            if (data == null || data.Length == 0)
            {
                logger.LogWarning($"Empty data for slot image: {slotID}");
                return;
            }

            var texture = new Texture2D(2, 2);
            if (!texture.LoadImage(data))
            {
                logger.LogWarning($"Failed to create texture for {slotID}");
                return;
            }

            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f
            );

            ResourceHelper.AddEntry($"Slots/{slotID}", sprite);
            LogHelper.LogDebug($"Added slot sprite: {slotID}");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error creating slot sprite {slotID}: {ex}");
        }
    }

    private void LoadBundleFromMemory(byte[] data, string bundleName)
    {
        try
        {
            if (data == null || data.Length == 0)
            {
                logger.LogWarning($"Bundle data is null or empty for: {bundleName}");
                return;
            }

            var bundle = AssetBundle.LoadFromMemory(data);
            if (bundle == null)
            {
                logger.LogWarning($"Failed to load rig layout bundle: {bundleName}");
                return;
            }

            var loadedCount = 0;
            var gameObjects = bundle.LoadAllAssets<GameObject>();
            if (gameObjects == null || gameObjects.Length == 0)
                logger.LogWarning($"No GameObjects loaded from bundle: {bundleName}");

            if (gameObjects != null)
                foreach (var prefab in gameObjects)
                {
                    if (prefab == null)
                    {
                        logger.LogWarning("Encountered null prefab in bundle.");
                        continue;
                    }

                    var gridView = prefab.GetComponent<ContainedGridsView>();
                    if (gridView == null)
                    {
                        logger.LogWarning($"Prefab {prefab.name} missing ContainedGridsView.");
                        continue;
                    }

                    ResourceHelper.AddEntry($"UI/Rig Layouts/{prefab.name}", gridView);
                    loadedCount++;
                    LogHelper.LogDebug($"Added rig layout: {prefab.name}");
                }

            bundle.Unload(false);
            LogHelper.LogDebug($"Loaded {loadedCount} prefabs from bundle: {bundleName}");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error loading bundle {bundleName}: {ex}");
        }
    }
}