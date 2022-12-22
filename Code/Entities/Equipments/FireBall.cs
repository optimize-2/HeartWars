using System;
using System.Threading;
using System.Threading.Tasks;
using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Equipments {
    public class FireBall : Entity {
        public static ParticleType particleType = ParticleTypes.Dust;

        private float moveSpeed;
        private float damage;
        private float maxTime;

        private float shootTime;
        private bool dead;

        public String shooterName, shooterTeam;

        private Vector2 target;
        private Level level;
        private Sprite sprite;
        public FireBall()
            : base(Vector2.Zero) {
            Tag |= Tags.PauseUpdate;
            Add(sprite = GFX.SpriteBank.Create("HeartWarsFireBall"));
			base.Depth = -1000000;
            base.Collider = new Hitbox(8f, 8f, -4f, -4f);
            Add(new PlayerCollider(OnPlayer));
        }

        public FireBall Init(String shooterName, String shooterTeam, Vector2 target, Vector2 position) {
            this.shooterName = shooterName;
            this.shooterTeam = shooterTeam;
            this.target = target;
            this.Position = position;
            this.moveSpeed = 200f;
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

        private void explode() {
            Audio.Play("event:/new_content/game/10_farewell/puffer_splode", this.Position);
            if ((Controller.player.Position - Position).Length() <= 50f) {
                GameController.damage(damage * (1f - (Controller.player.Position - Position).Length() / 50f), new DataPlayerDie() {
                        Sender = Controller.module.Context.Client.PlayerInfo,
                        area = PlayerInfo.Area,
                        playerName = PlayerInfo.getPlayerName(),
                        playerTeam = PlayerInfo.PlayerTeam,
                        killerName = shooterName,
                        killerTeam = shooterTeam,
                        money = PlayerInfo.PlayerMoney / 2
                }, Position, false);
                Controller.player.ExplodeLaunch(this.Position, false, false);
            }
            sprite.Play("explode", false, false);
            Task.Run(() => {
                this.dead = true;
                Thread.Sleep(300);
                this.RemoveSelf();
            });
        }

        private void OnPlayer(Player player) {
            if (PlayerInfo.getPlayerName() == shooterName) return;
            explode();
        }

		public override void Update() {
			base.Update();
            if (dead) return;
            Vector2 dest = this.Position + target * moveSpeed * Engine.DeltaTime;
            shootTime += Engine.DeltaTime;
            if (shootTime > maxTime) this.RemoveSelf();
            if (shootTime > 0.2f && base.CollideCheck<Ghost>(dest)) {
                explode();
            }
            if (shootTime <= 0.1f || !base.CollideCheck<Solid>(dest)) {
                this.Position = dest;
            } else {
                explode();
            }
            if (base.Scene.OnInterval(0.05f)) {
                this.level.ParticlesFG.Emit(particleType, 1, base.Center, Vector2.One * 2f, target.Angle());
            }
		}
    }
}
