using System;
using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Equipments {
    public class Missile : Entity {
        public static ParticleType RedTeam = new ParticleType {
            Size = 1f,
            Color = Calc.HexToColor("EE0000"),
            Color2 = Calc.HexToColor("EE0000"),
            ColorMode = ParticleType.ColorModes.Blink,
            FadeMode = ParticleType.FadeModes.Late,
            SpeedMin = 10f,
            SpeedMax = 30f,
            DirectionRange = 0.6981317f,
            LifeMin = 0.3f,
            LifeMax = 0.6f
        };

        public static ParticleType BlueTeam = new ParticleType {
            Size = 1f,
            Color = Calc.HexToColor("66CCFF"),
            Color2 = Calc.HexToColor("66CCFF"),
            ColorMode = ParticleType.ColorModes.Blink,
            FadeMode = ParticleType.FadeModes.Late,
            SpeedMin = 10f,
            SpeedMax = 30f,
            DirectionRange = 0.6981317f,
            LifeMin = 0.3f,
            LifeMax = 0.6f
        };

        private float moveSpeed;
        private float damage;
        private float maxTime;

        private float shootTime;

        public String shooterName, shooterTeam;
        public int id;

        private Vector2 target;
        private Level level;
        private Sprite sprite;
        public Missile()
            : base(Vector2.Zero) {
            Tag |= Tags.PauseUpdate;
            Add(sprite = GFX.SpriteBank.Create("badeline_projectile"));
            base.Depth = -1000000;
            base.Collider = new Hitbox(8f, 8f, -4f, -4f);
            Add(new PlayerCollider(OnPlayer));
        }

        public Missile Init(String shooterName, String shooterTeam, Vector2 target, Vector2 position, float moveSpeed, float damage, float maxTime, int id) {
            this.shooterName = shooterName;
            this.shooterTeam = shooterTeam;
            this.target = target;
            this.Position = position;
            this.moveSpeed = moveSpeed;
            this.damage = damage;
            this.maxTime = maxTime;
            this.shootTime = 0f;
            this.id = id;
            this.sprite.Play("charge", true, false);
            return this;
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            this.level = base.SceneAs<Level>();
            GameController.onCreateMissile(this);
        }

        private void OnPlayer(Player player) {
            if (shooterTeam == PlayerInfo.PlayerTeam && shooterName != PlayerInfo.getPlayerName()) {
                player.PointBounce(Position);
                GameController.removeMissile(this.shooterName, this.id);
                this.RemoveSelf();
            }
            else if (shooterTeam != PlayerInfo.PlayerTeam) {
                // PlayerInfo.PlayerHealth -= this.damage;
                GameController.damage(damage, new DataPlayerDie() {
                    Sender = Controller.module.Context.Client.PlayerInfo,
                    area = PlayerInfo.Area,
                    playerName = PlayerInfo.getPlayerName(),
                    playerTeam = PlayerInfo.PlayerTeam,
                    killerName = shooterName,
                    killerTeam = shooterTeam,
                    money = PlayerInfo.PlayerMoney / 2
                }, Position, true);
                GameController.removeMissile(this.shooterName, this.id);
                this.RemoveSelf();
            }
        }
        public override void Update() {
            base.Update();
            Vector2 dest = this.Position + target * moveSpeed * Engine.DeltaTime;
            shootTime += Engine.DeltaTime;
            if (shootTime > 0.2f && base.CollideCheck<Ghost>(dest)) {
                Audio.Play("event:/char/madeline/landing", Controller.player.Position);
                this.RemoveSelf();
            }
            if (shootTime <= 0.1f || (!base.CollideCheck<Solid>(dest) && shootTime <= maxTime)) {
                this.Position = dest;
            } else {
                GameController.removeMissile(this.shooterName, this.id);
                this.RemoveSelf();
            }
            if (base.Scene.OnInterval(0.01f)) {
                if (this.shooterTeam == "RED") {
                    this.level.ParticlesFG.Emit(RedTeam, 1, base.Center, Vector2.One * 2f, target.Angle());
                }
                else  {
                    this.level.ParticlesFG.Emit(BlueTeam, 1, base.Center, Vector2.One * 2f, target.Angle());
                }
            }
        }
    }
}
