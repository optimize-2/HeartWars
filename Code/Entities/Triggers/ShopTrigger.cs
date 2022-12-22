using System;
using Celeste.Mod.Entities;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Triggers {
    [Tracked]
    [CustomEntity("HeartWars/ShopTrigger")]
    public class ShopTrigger : Trigger {
        public static bool active = false;
        public ShopTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            active = false;
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            active = true;
        }

        public override void OnStay(Player player) {
            base.OnStay(player);
            active = true;
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);
            active = false;
        }
    }
}
