using System;
using Celeste.Mod.Entities;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Triggers {
    [Tracked]
    [CustomEntity("HeartWars/RedTeamTrigger")]
    public class RedTeamTrigger : Trigger {
        public RedTeamTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            if ((DateTime.UtcNow - GameController.lastGameStartResponse).TotalSeconds <= HeartWarsModule.maxPing) return;
            PlayerInfo.PlayerTeam = "RED";
            PlayerInfo.PlayerHairColor = player.Hair.Color = Calc.HexToColor("EE0000");
            Controller.player = player;
        }

        public override void OnStay(Player player) {
            base.OnStay(player);
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);
        }
    }
}
