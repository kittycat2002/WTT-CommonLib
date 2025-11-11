using System;
using BepInEx;
using Comfort.Common;
using EFT;
using UnityEngine;
using WTTClientCommonLib.CommandProcessor;
using WTTClientCommonLib.Components;
using WTTClientCommonLib.Configuration;
using WTTClientCommonLib.Helpers;
using WTTClientCommonLib.Patches;
using WTTClientCommonLib.Services;

namespace WTTClientCommonLib;

[BepInPlugin("com.wtt.commonlib", "WTT-ClientCommonLib", "2.0.2")]
public class WTTClientCommonLib : BaseUnityPlugin
{
    private static CommandProcessor.CommandProcessor _commandProcessor;
    private static GameWorld _gameWorld;
    public static Player Player;
    private GameObject _updaterObject;
    public AssetLoader AssetLoader;
    private PlayerWorldStats _playerWorldStats;
    public SpawnCommands SpawnCommands;
    public static WTTClientCommonLib Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
     
        LogHelper.SetLogger(Logger);
        try
        {
            AssetLoader = new AssetLoader(Logger);
            SpawnCommands = new SpawnCommands(Logger, AssetLoader);
            _playerWorldStats = new PlayerWorldStats(Logger);
            
            UniversalConfigManager.Initialize(Config);
            ZoneConfigManager.Initialize(Config);
            StaticSpawnSystemConfigManager.Initialize(Config);
            
            RadioSettings.Init(Config);
            
            new FaceCardViewInitPatch().Enable();
            new FaceCardViewTogglePatch().Enable();
            new BoomboxAudioPatch().Enable();
            new OnGameStarted().Enable();
            new ClothingBundleRendererPatch().Enable();

            var resourceLoader = new ResourceLoader(Logger, AssetLoader);
            resourceLoader.LoadAllResourcesFromServer();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to initialize WTT-ClientCommonLib: {ex}");
        }
    }

    internal void Start()
    {
        Init();
    }

    private void Update()
    {
        if (Singleton<GameWorld>.Instantiated && (_gameWorld == null || Player == null))
        {
            _gameWorld = Singleton<GameWorld>.Instance;
            Player = _gameWorld.MainPlayer;
        }
    }

    private void Init()
    {
        if (_commandProcessor == null)
        {
            _commandProcessor = new CommandProcessor.CommandProcessor(_playerWorldStats, SpawnCommands);
            _commandProcessor.RegisterCommandProcessor();
        }

        if (_updaterObject == null)
        {
            _updaterObject = new GameObject("SpawnSystemUpdater");
            _updaterObject.AddComponent<SpawnSystemUpdater>();
            DontDestroyOnLoad(_updaterObject);
        }
    }
}
