using Celeste.Mod.HeartWars.Controllers;

namespace Celeste.Mod.HeartWars.Inventory {
    public class GoldenStrawberryItem : Item  {
        public GoldenStrawberryItem() : base() {
            // this.count = 999;
            this.count = 0;
            this.cost = 30;
            this.dialogName = "OPTIMIZE2_HEARTWARS_GOLDENSTRAWBERRY";
            this.texture = HeartWarsModule.GoldenStrawberryTexture;
        }

        public override void select() {
            this.count -= 1;
            PlayerInfo.DamageAbsorbEffect = 60f;
            PlayerInfo.RegenerationEffect = 15f;
            PlayerInfo.PlayerExtraHealth = 20f;
        }

        public override bool tryBuy() {
            InventoryController.inventory.ForEach(e => {
                if (e is GoldenStrawberryItem) {
                    e.count += 1;
                }
            });
            return true;
        }
    }
}