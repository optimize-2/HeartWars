using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Entities.SpawnPoints {
    [Tracked]
    [CustomEntity("HeartWars/DefaultSpawnPoint")]
    public class DefaultSpawnPoint : Entity {
        public DefaultSpawnPoint(EntityData data, Vector2 off) : base(data.Position + off) {

        }
    }
}