using Celeste.Mod.Entities;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Triggers {
    [Tracked]
    [CustomEntity("HeartWars/RedTeamTrapTrigger")]
    public class RedTeamTrapTrigger : Trigger {
        public static int windLevel;
        public static bool blindTrap;
        public RedTeamTrapTrigger(EntityData data, Vector2 offset) : base(data, offset) {
        }

        public void initialize() {
            windLevel = 1;
            blindTrap = false;
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
        }

        public override void OnStay(Player player) {
            base.OnStay(player);
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);
        }
    }
}