using Celeste.Mod.Entities;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Triggers {
    [Tracked]
    [CustomEntity("HeartWars/DashDisableTrigger")]
    public class DashDisableTrigger : Trigger {
        public DashDisableTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            GameController.disableDash = true;
        }

        public override void OnStay(Player player) {
            base.OnStay(player);
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);
        }
    }
}