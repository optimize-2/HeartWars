namespace Celeste.Mod.HeartWars.Inventory {
    public class GunItem : Item  {
        public GunItem() {
            this.count = 1;
            this.cost = 0;
            this.dialogName = "OPTIMIZE2_HEARTWARS_NODIALOG";
            this.texture = HeartWarsModule.GunTexture;
        }

        public override void select() {
            PlayerInfo.Hand = ItemType.GUN;
        }

        public override bool tryBuy() {
            return false;
        }
    }
}