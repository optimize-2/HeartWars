using Celeste.Mod.HeartWars.Controllers;

namespace Celeste.Mod.HeartWars.Inventory {
    public class BadPearlItem : Item  {
        public BadPearlItem() : base() {
            // this.count = 999;
            this.count = 0;
            this.cost = 200;
            this.dialogName = "OPTIMIZE2_HEARTWARS_BADPEARL";
            this.texture = HeartWarsModule.BadPearlTexture;
        }

        public override void select() {
            PlayerInfo.Hand = ItemType.BADPEARL;
        }

        public override bool tryBuy() {
            InventoryController.inventory.ForEach(e => {
                if (e is BadPearlItem) {
                    e.count += 1;
                }
            });
            return true;
        }
    }
}