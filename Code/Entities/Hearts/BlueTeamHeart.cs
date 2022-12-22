using System;
using System.Reflection;
using Celeste.Mod.Entities;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.HeartWars.Entities.Hearts {
    [Tracked]
    [CustomEntity("HeartWars/BlueTeamHeart")]
    public class BlueTeamHeart : Entity {
        private static readonly FieldInfo DASH_ATTACK_TIMER_FIELD = typeof(Player).GetField("dashAttackTimer", BindingFlags.NonPublic | BindingFlags.Instance);
        private Sprite sprite;
        private DynData<FakeHeart> baseData;
        private string spriteId;

        private float bounceSfxDelay;
        private float timer;
        private Wiggler moveWiggler;
        private Vector2 moveWiggleDir;

        private HoldableCollider holdableCollider;

        public BlueTeamHeart(EntityData data, Vector2 offset) : base(data.Position + offset) {
            Collider = new Hitbox(16f, 16f, -8f, -8f);
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);
            sprite = GFX.SpriteBank.Create("heartgem0");
            Add(sprite);
            sprite.Play("spin");
            moveWiggler = Wiggler.Create(0.8f, 2f);
            moveWiggler.StartZero = true;
            Add(moveWiggler);
            moveWiggleDir = (base.Center - Scene.Tracker.GetEntity<Player>().Center).SafeNormalize(Vector2.UnitY);
            sprite.OnLoop = delegate (string anim) {
                if (Visible && anim == "spin") {
                    Audio.Play("event:/new_content/game/10_farewell/fakeheart_pulse", Position);
                    (base.Scene as Level).Displacement.AddBurst(Position, 0.35f, 8f, 48f, 0.25f);
                }
            };
            Add(new PlayerCollider(onPlayer));
        }

        public void onPlayer(Player player) {
            if (player.DashAttacking && PlayerInfo.PlayerTeam == "RED") {
                player.PointBounce(base.Center);
                GameController.BlueTeamHeartBroke = true;
                RemoveSelf();
        GameController.broadcast(PlayerInfo.getPlayerName() + " destoryed BLUE team's heart!");
                GameController.sendHeartBreak();
                return;
            }
            if (bounceSfxDelay <= 0f) {
                Audio.Play("event:/game/general/crystalheart_bounce", Position);
                bounceSfxDelay = 0.1f;
            }
            player.PointBounce(base.Center);
            moveWiggler.Start();
            moveWiggleDir = (base.Center - player.Center).SafeNormalize(Vector2.UnitY);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        }

        public override void Update() {
            bounceSfxDelay -= Engine.DeltaTime;
            timer += Engine.DeltaTime;
            sprite.Position = Vector2.UnitY * (float)Math.Sin(timer * 2f) * 2f + moveWiggleDir * moveWiggler.Value * -8f;
            base.Update();
        }

        public override void Removed(Scene scene) {
            base.Removed(scene);
        }
    }
}