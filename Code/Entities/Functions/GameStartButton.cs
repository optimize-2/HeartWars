using System;
using System.Linq;
using System.Reflection;
using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.Entities;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Functions {
    [Tracked]
    [CustomEntity("HeartWars/GameStartButton")]
    public class GameStartButton : DashSwitch {
        private class ButtonText : Entity {
            private GameStartButton button;
            public string content;

            public ButtonText(GameStartButton button) {
                Tag |= TagsExt.SubHUD;
                Tag |= Tags.PauseUpdate;
                Tag |= Tags.FrozenUpdate;
                this.content = Dialog.Get("OPTIMIZE2_HEARTWARS_START_GAME");
                this.button = button;
            }

            public override void Render() {
                Player player = Scene.Tracker.GetEntity<Player>();
                if (player != null) {
                    Camera cam = SceneAs<Level>().Camera;
                    float alpha = Calc.Clamp(2f - Vector2.Distance(
                        button.Position,
                        Scene.Tracker.GetEntity<Player>().Position
                    ) / 64f, 0f, 1f);
                    ActiveFont.Draw(
                        content + (button.matching ? ($" { GameController.RedTeamPlayers?.Count ?? 0 } : { GameController.BlueTeamPlayers?.Count ?? 0 }") : ""),
                        (button.TopCenter - Vector2.UnitY * 8f - cam.Position.Floor()) * 6f,
                        new Vector2(0.5f, 1f),
                        Vector2.One,
                        Color.White * alpha
                    );
                }
            }
        }
        private static readonly FieldInfo PRESSED_FIELD = typeof(DashSwitch).GetField("pressed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static On.Celeste.DashSwitch.hook_OnDashed dashHook;
        private static int dashHookCounter = 0;
        private ButtonText buttonText;
        private bool pressed = false;
        public bool unHandledMatchFailure = false;
        public bool matching = false;
        Scene scene;

        public GameStartButton(EntityData data, Vector2 offset) : base(data.Position + offset, Sides.Down, false, false, new EntityID(data.Level.Name, data.ID), "mirror") {
            buttonText = new ButtonText(this);
            if (dashHookCounter++ <= 0) On.Celeste.DashSwitch.OnDashed += dashHook = (orig, dSwitch, player, dir) => {
                if (dSwitch is GameStartButton button) {
                    HeartWarsModule.info("1");
                    if (button.pressed || (bool) PRESSED_FIELD.GetValue(button)) return orig(dSwitch, player, dir);
                    HeartWarsModule.info("2");
                    if (
                        GameController.isWaitingForGameStartResponse ||
                        (DateTime.UtcNow - GameController.lastGameStartResponse).TotalSeconds <= HeartWarsModule.maxPing ||
                        Controller.module.Context?.Client == null
                    ) return DashCollisionResults.NormalCollision;
                    HeartWarsModule.info("3");
                    if (scene.Tracker.GetEntities<Ghost>().Find(
                        e => (e.Position - this.Position).Length() >= HeartWarsModule.maxDistance
                    ) != null) {
                        buttonText.content = Dialog.Get("OPTIMIZE2_HEARTWARS_START_GAME_EXIST");
                        return DashCollisionResults.NormalCollision;
                    }
                    HeartWarsModule.info("4");
                    if (PlayerInfo.PlayerTeam == "RED" || PlayerInfo.PlayerTeam == "BLUE") {
                        HeartWarsModule.info("5");
                        button.pressed = true;
                        HeartWarsModule.info("6");
                        button.matching = true;
                        HeartWarsModule.info("7");
                        buttonText.content = Dialog.Get("OPTIMIZE2_HEARTWARS_START_GAME_WAITING");
                        HeartWarsModule.info("8");
                        return orig(dSwitch, player, dir);
                    } else {
                        HeartWarsModule.info("9");
                        buttonText.content = Dialog.Get("OPTIMIZE2_HEARTWARS_START_GAME_NO_TEAM");
                        return DashCollisionResults.NormalCollision;
                    }
                } else {
                    return orig(dSwitch, player, dir);
                }
            };
        }

        public override void Awake(Scene scene) {
            this.scene = scene;
            base.Awake(scene);
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            if (buttonText != null) Scene.Add(buttonText);
        }

        public override void Update() {
            if (pressed) {
                if (!GameController.isWaitingForGameStartResponse &&
                    (DateTime.UtcNow - GameController.lastGameStartResponse).TotalSeconds >= HeartWarsModule.maxPing) {
                    HeartWarsModule.info("Send DataGameStartRequest");
                    HeartWarsModule.info("Name: " + PlayerInfo.getPlayerName());
                    GameController.sendGameStartRequest();
                }
                pressed = false;
            }
            if (unHandledMatchFailure) {
                reset();
            }
            base.Update();
        }

        public void reset() {
            Collidable = true;
            unHandledMatchFailure = false;
            matching = false;
            PRESSED_FIELD.SetValue(this, false);
            base.Awake(scene);
        }
    
        public void setContent(string s) {
            buttonText.content = s;
        }

        public void onGameMatchFailed() {
            unHandledMatchFailure = true;
        }
    }
}
