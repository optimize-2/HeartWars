using Celeste.Mod.HeartWars.Controllers;

namespace Celeste.Mod.HeartWars.Inventory {
    public class FireBallItem : Item  {
        public FireBallItem() : base() {
            // this.count = 999;
            this.count = 0;
            this.cost = 150;
            this.dialogName = "OPTIMIZE2_HEARTWARS_FIREBALL";
            this.texture = HeartWarsModule.FireBallTexture;
        }

        public override void select() {
            PlayerInfo.Hand = ItemType.FIREBALL;
        }

        public override bool tryBuy() {
            InventoryController.inventory.ForEach(e => {
                if (e is FireBallItem) {
                    e.count += 1;
                }
            });
            return true;
        }
    }
}