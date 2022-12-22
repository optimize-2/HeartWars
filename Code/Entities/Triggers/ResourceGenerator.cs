using System;
using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Triggers {
    [Tracked]
    public class ResourceGenerator : Trigger {
        private class CountText : Entity {
            private ResourceGenerator generator;

            public CountText(ResourceGenerator generator) {
                Tag |= TagsExt.SubHUD;
                Tag |= Tags.PauseUpdate;
                Tag |= Tags.FrozenUpdate;
                this.generator = generator;
            }

            public override void Render() {
                Player player = Scene.Tracker.GetEntity<Player>();
                if (player != null) {
                    Camera cam = SceneAs<Level>().Camera;
                    ActiveFont.Draw(
                        generator.getRemainTime().ToString() + " x " + generator.uncollected.ToString(),
                        (generator.TopCenter - Vector2.UnitY * 8f - cam.Position.Floor()) * 6f,
                        new Vector2(0.5f, 1f),
                        Vector2.One,
                        Color.GreenYellow
                    );
                }
            }
        }

        public DateTime gameStartTime;
        public int money, speed, counts, uncollected;
        public bool isPublic = false;

        private bool playerEntered;

        private CountText countText;

        public ResourceGenerator(EntityData data, Vector2 offset) : base(data, offset) {
            this.initialize();
        }

        public int getRemainTime() {
            return (int)(speed - (DateTime.UtcNow - GameController.gameStartTime).TotalSeconds % speed);
        }

        public void initialize() {
            countText = new CountText(this);
            counts = 0;
            playerEntered = false;
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            if (countText != null) Scene.Add(countText);
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            if (!base.CollideCheck<Ghost>(Position) && !isPublic) {
                playerEntered = true;
            }
        }

        public override void OnStay(Player player) {
            base.OnStay(player);
            if (uncollected != 0 && Controller.featuresEnabled) {
                PlayerInfo.PlayerMoney += uncollected * money;
                uncollected = 0;
                // TODO: Play sounds
            }
        }

        public override void OnLeave(Player player) {
            playerEntered = false;
            base.OnLeave(player);
        }

        public override void Update() {
            if (!Controller.featuresEnabled) return;
            while ((DateTime.UtcNow - GameController.gameStartTime).TotalSeconds >= (counts + 1) * speed){
                counts += 1;
                uncollected += 1;
            }
            if (base.CollideCheck<Ghost>(Position) && !isPublic) {
                if (!playerEntered) {
                    uncollected = 0;
                }
            }
            base.Update();
        }
    }
}
