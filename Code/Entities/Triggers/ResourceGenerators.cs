using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Triggers {
    [Tracked]
    [TrackedAs(typeof(ResourceGenerator))]
    [CustomEntity("HeartWars/ResourceGeneratorI")]
    public class ResourceGeneratorI : ResourceGenerator {
        public ResourceGeneratorI(EntityData data, Vector2 offset) : base(data, offset) {
            this.isPublic = true;
            this.speed = 3;
            this.money = 1;
        }
    }

    [Tracked]
    [TrackedAs(typeof(ResourceGenerator))]
    [CustomEntity("HeartWars/ResourceGeneratorII")]
    public class ResourceGeneratorII : ResourceGenerator {
        public ResourceGeneratorII(EntityData data, Vector2 offset) : base(data, offset) {
            this.isPublic = false;
            this.speed = 30;
            this.money = 50;
        }
    }

    [Tracked]
    [TrackedAs(typeof(ResourceGenerator))]
    [CustomEntity("HeartWars/ResourceGeneratorIII")]
    public class ResourceGeneratorIII : ResourceGenerator {
        public ResourceGeneratorIII(EntityData data, Vector2 offset) : base(data, offset) {
            this.isPublic = false;
            this.speed = 60;
            this.money = 200;
        }
    }
}
