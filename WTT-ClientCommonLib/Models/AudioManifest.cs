using System.Collections.Generic;

namespace WTTClientCommonLib.Models;

public class AudioManifest
{
    public List<string> AudioBundles { get; set; } = new();
    public Dictionary<string, List<string>> FaceCardMappings { get; set; } = new();
    public List<string> RadioAudio { get; set; } = new();
}
