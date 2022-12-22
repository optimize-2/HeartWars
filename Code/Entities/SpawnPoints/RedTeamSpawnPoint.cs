using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Entities.SpawnPoints {
    [Tracked]
    [CustomEntity("HeartWars/RedTeamSpawnPoint")]
    public class RedTeamSpawnPoint : Entity {
        public RedTeamSpawnPoint(EntityData data, Vector2 off) : base(data.Position + off) {
            
        }
    }
}