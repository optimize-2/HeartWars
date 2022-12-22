using Celeste.Mod.Entities;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Triggers {
    [Tracked]
    [CustomEntity("HeartWars/DashEnableTrigger")]
    public class DashEnableTrigger : Trigger {
        private bool disabledBeforeEnter;
        public DashEnableTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            disabledBeforeEnter = GameController.disableDash;
            GameController.disableDash = false;
        }

        public override void OnStay(Player player) {
            base.OnStay(player);
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);
            GameController.disableDash = disabledBeforeEnter;
        }
    }
}