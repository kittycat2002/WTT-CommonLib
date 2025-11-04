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
    private readonly Dictionary<string, List<string>> _faceCardAudio = new(); 
    private readonly List<string> _radioAudio = new();

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

    public void AddFaceCardAudio(string faceName, string audioKey)
    {
        if (!_faceCardAudio.ContainsKey(faceName))
            _faceCardAudio[faceName] = new List<string>();

        _faceCardAudio[faceName].Add(audioKey);
        LogHelper.Debug(logger, $"Added FaceCard audio for {faceName}: {audioKey}");
    }

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
    public Dictionary<string, List<string>> FaceCardMappings { get; set; } = new();
    public List<string> RadioAudio { get; set; } = new();
}
