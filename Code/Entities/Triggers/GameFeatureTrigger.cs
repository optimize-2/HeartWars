using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.Entities.SpawnPoints;
using Celeste.Mod.HeartWars.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Triggers {
    [Tracked]
    [CustomEntity("HeartWars/GameFeatureTrigger")]
    public class GameFeatureTrigger : Trigger {
        public GameFeatureTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            
        }

        public override void OnEnter(Player player) {
            Controller.player = player;
            switch (PlayerInfo.PlayerTeam) {
            case "UNSELECTED":
                Controller.level.Session.RespawnPoint = player.Position = Controller.level.Tracker.GetEntities<DefaultSpawnPoint>().First().Position;
                return;
            case "RED":
                Controller.level.Session.RespawnPoint = Controller.level.Tracker.GetEntities<RedTeamSpawnPoint>().First().Position;
                break;
            case "BLUE":
                Controller.level.Session.RespawnPoint = Controller.level.Tracker.GetEntities<BlueTeamSpawnPoint>().First().Position;
                break;
            }
            Controller.enableFeatures();
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
