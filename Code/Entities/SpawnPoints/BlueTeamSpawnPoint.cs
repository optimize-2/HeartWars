using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Entities.SpawnPoints {
    [Tracked]
    [CustomEntity("HeartWars/BlueTeamSpawnPoint")]
    public class BlueTeamSpawnPoint : Entity {
        public BlueTeamSpawnPoint(EntityData data, Vector2 off) : base(data.Position + off) {
            
        }
    }
}