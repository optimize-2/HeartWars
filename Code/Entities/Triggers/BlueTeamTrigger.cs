using System;
using Celeste.Mod.Entities;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Triggers {
    [Tracked]
    [CustomEntity("HeartWars/BlueTeamTrigger")]
    public class BlueTeamTrigger : Trigger {
        public BlueTeamTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            if ((DateTime.UtcNow - GameController.lastGameStartResponse).TotalSeconds <= HeartWarsModule.maxPing) return;
            PlayerInfo.PlayerTeam = "BLUE";
            PlayerInfo.PlayerHairColor = player.Hair.Color = Calc.HexToColor("66CCFF");
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
