using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Utils.Logger;
using WTTServerCommonLib.Helpers;

namespace WTTServerCommonLib.Services;

[Injectable(InjectionType.Singleton)]
public class WTTCustomAudioService(ModHelper modHelper, SptLogger<WTTCustomAudioService> logger)
{
    private readonly Dictionary<string, string> _audioBundles = new();
    private readonly Dictionary<string, FaceCardAudioEntry> _faceCardAudio = new(); 
    private readonly List<string> _radioAudio = new();

    
    /// <summary>
    /// Registers all Unity audio bundles with extension '.bundle' found in the specified assembly's mod folder,
    /// </summary>
    /// <param name="assembly">The calling assembly, used to determine the mod folder location</param>
    /// <param name="relativePath">(OPTIONAL) Custom path relative to the mod folder</param>
    public void RegisterAudioBundles(Assembly assembly, string? relativePath = null)
    {
        var assemblyLocation = modHelper.GetAbsolutePathToModFolder(assembly);
        var defaultDir = Path.Combine("db", "CustomAudioBundles");
        var finalDir = Path.Combine(assemblyLocation, relativePath ?? defaultDir);

        if (!Directory.Exists(finalDir))
        {
            LogHelper.Debug(logger, $"No AudioBundles directory at {finalDir}");
            return;
        }

        foreach (var bundlePath in Directory.GetFiles(finalDir, "*.bundle"))
        {
            var bundleName = Path.GetFileNameWithoutExtension(bundlePath);
            _audioBundles[bundleName] = bundlePath;
            LogHelper.Debug(logger, $"Registered audio bundle: {bundleName} from {bundlePath}");
        }
    }

    
    /// <summary>
    /// Adds a custom audio key associated with a specific face name. Optionally marks the audio to play on radio only when the face is selected.
    /// </summary>
    /// <param name="faceName">The unique identifier of the face card.</param>
    /// <param name="audioKey">The name (key) of the audio clip.</param>
    /// <param name="playOnRadioIfFaceIsSelected">
    /// If true, the audio will be included in the radio pool only when the face is actively selected. Defaults to false.
    /// </param>
    public void AddFaceCardAudio(string faceName, string audioKey, bool playOnRadioIfFaceIsSelected = false)
    {
        if (!_faceCardAudio.TryGetValue(faceName, out var entry))
        {
            entry = new FaceCardAudioEntry();
            _faceCardAudio[faceName] = entry;
        }
        entry.Audio.Add(audioKey);
        entry.PlayOnRadio = playOnRadioIfFaceIsSelected;
    }

    
    /// <summary>
    /// Adds a audio key to the global radio audio pool, which plays independent of face selection.
    /// </summary>
    /// <param name="audioKey">The name of the radio audio clip.</param>
    public void AddRadioAudio(string audioKey)
    {
        _radioAudio.Add(audioKey);
        LogHelper.Debug(logger, $"Added Radio audio: {audioKey}");
    }

    public AudioManifest GetAudioManifest()
    {
        return new AudioManifest
        {
            AudioBundles = _audioBundles.Keys.ToList(),
            FaceCardMappings = _faceCardAudio,
            RadioAudio = _radioAudio
        };
    }

    public List<string> GetAudioBundleManifest() => _audioBundles.Keys.ToList();

    public async Task<byte[]?> GetAudioBundleData(string bundleName)
    {
        LogHelper.Debug(logger, $"GetAudioBundleData called for: {bundleName}");
    
        if (_audioBundles.TryGetValue(bundleName, out var bundlePath))
        {
            LogHelper.Debug(logger, $"Found bundle path: {bundlePath}");
        
            if (File.Exists(bundlePath))
            {
                LogHelper.Debug(logger, $"Bundle exists, reading...");
                var data = await File.ReadAllBytesAsync(bundlePath);
                LogHelper.Debug(logger, $"Read {data.Length} bytes from bundle {bundleName}");
                return data;
            }
            else
            {
                LogHelper.Debug(logger, $"Bundle file does not exist: {bundlePath}");
            }
        }
        else
        {
            LogHelper.Debug(logger, $"Bundle not found: {bundleName}. Available: {string.Join(", ", _audioBundles.Keys)}");
        }
    
        return null;
    }
}

public class AudioManifest
{
    public List<string> AudioBundles { get; set; } = new();
    public Dictionary<string, FaceCardAudioEntry> FaceCardMappings { get; set; } = new();
    public List<string> RadioAudio { get; set; } = new();
}

public class FaceCardAudioEntry
{
    public List<string> Audio { get; set; } = new();
    public bool PlayOnRadio { get; set; } = false;
}

