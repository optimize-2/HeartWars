using System;
using Celeste.Mod.HeartWars.Entities.Renderers;
using Celeste.Mod.HeartWars.Controllers;
using Monocle;

namespace Celeste.Mod.HeartWars {
    public class HeartWarsModule : EverestModule {
        public static HeartWarsModule Instance { get; private set; }

        public static readonly String Name = "HeartWars";
        public static readonly String ProtocolVersion = "Ver_1_0_3";
        public static readonly String IdentifyCode = "heartwars123456";
        public static int IdentifyStamp = 0;

        public static readonly int maxPing = 10;
        public static readonly int maxDistance = 500;
        
        public override Type SettingsType => typeof(HeartWarsModuleSettings);
        public static HeartWarsModuleSettings Settings => (HeartWarsModuleSettings) Instance._Settings;

        public override Type SessionType => typeof(HeartWarsModuleSession);
        public static HeartWarsModuleSession Session => (HeartWarsModuleSession) Instance._Session;

        public static Controller controller;
        public static GameController gameController;

        public static MTexture GunTexture;
        public static MTexture GoldenStrawberryTexture;
        public static MTexture FireBallTexture;
        public static MTexture BadPearlTexture;

        public HeartWarsModule() {
            Instance = this;
        }

        public override void LoadContent(bool firstLoad) {
            GunTexture = GFX.Game["HeartWars/Gun"];
            GoldenStrawberryTexture = GFX.Game["collectables/goldberry/idle00"];
            FireBallTexture = GFX.Game["HeartWars/HeartWarsFireBall/idle0"];
            BadPearlTexture = GFX.Game["HeartWars/HeartWarsBadPearl/idle00"];
            base.LoadContent(firstLoad);
        }

        public override void Load() {
            Logger.SetLogLevel(Name, LogLevel.Info);
            controller = new Controller(Celeste.Instance);
            gameController = new GameController(Celeste.Instance);
            PlayerInfo.initialize();
            Celeste.Instance.Components.Add(controller);
            Celeste.Instance.Components.Add(gameController);
        }

        public override void Initialize() {
            HeartWarsRenderer.Initialize();
            base.Initialize();
        }

        public override void Unload() {
            controller.Unload();
            gameController.Unload();
            Celeste.Instance.Components.Remove(controller);
            Celeste.Instance.Components.Remove(gameController);
        }

        public static void info(string s) {
            Logger.Log(LogLevel.Info, Name, s);
        }
    }
}