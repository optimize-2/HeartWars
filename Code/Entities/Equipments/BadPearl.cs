using System;
using System.Threading;
using System.Threading.Tasks;
using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Equipments {
    public class BadPearl : Entity {

        private float moveSpeed;
        private float damage;
        private float maxTime;

        private float shootTime;
        private bool dead;

        public String shooterName, shooterTeam;

        private Vector2 target;
        private Level level;
        private Sprite sprite;
        public BadPearl()
            : base(Vector2.Zero) {
            Tag |= Tags.PauseUpdate;
            Add(sprite = GFX.SpriteBank.Create("HeartWarsBadPearl"));
            base.Depth = -1000000;
            base.Collider = new Hitbox(20f, 20f, -10f, -10f);
            Add(new PlayerCollider(OnPlayer));
        }

        public BadPearl Init(String shooterName, String shooterTeam, Vector2 target, Vector2 position) {
            this.shooterName = shooterName;
            this.shooterTeam = shooterTeam;
            this.target = target;
            this.Position = position;
            this.moveSpeed = 400f;
            this.damage = 20f;
            this.maxTime = 20f;
            this.shootTime = 0f;
            this.dead = false;
            this.sprite.Play("idle", true, false);
            return this;
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            this.level = base.SceneAs<Level>();
        }

        private void teleport() {
            if (Controller.player == null) return;
            if (this.shooterName != PlayerInfo.getPlayerName()) return;
            if ((this.Position - Controller.player.Position).Length() <= 40f) {
                Audio.Play("event:/char/badeline/booster_final", this.Position);
                return;
            }
            Audio.Play("event:/char/badeline/booster_begin", this.Position);
            Controller.player.Position = this.Position;
            this.RemoveSelf();
        }

        private void OnPlayer(Player player) {
            if (PlayerInfo.getPlayerName() == shooterName) return;
            this.RemoveSelf();
        }

        public override void Update() {
            base.Update();
            if (dead) return;
            Vector2 dest = this.Position + target * moveSpeed * Engine.DeltaTime;
            shootTime += Engine.DeltaTime;
            if (shootTime > maxTime) this.RemoveSelf();
            if (shootTime > 0.05f && base.CollideCheck<Ghost>(dest)) {
                teleport();
            }
            if (shootTime <= 0.1f || !base.CollideCheck<Solid>(dest)) {
                this.Position = dest;
            } else {
                teleport();
            }
        }
    }
}
