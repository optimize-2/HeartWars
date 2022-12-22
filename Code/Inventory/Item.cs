using Celeste.Mod.HeartWars.Controllers;
using Monocle;

namespace Celeste.Mod.HeartWars.Inventory {
    public enum ItemType {
        GUN, FIREBALL, BADPEARL
    }

    public abstract class Item  {
        public int count;
        public int cost;
        public MTexture texture;
        public string dialogName;

        public Item() {
        }

        public abstract void select();

        public abstract bool tryBuy();

        public void buy() {
            if (PlayerInfo.PlayerMoney >= cost) {
                PlayerInfo.PlayerMoney -= cost;
                if (tryBuy()) {
                    GameController.broadcast(Dialog.Get("OPTIMIZE2_HEARTWARS_BUY_SUCCESS") + " " + Dialog.Get(dialogName));
                } else {
                    GameController.broadcast(Dialog.Get("OPTIMIZE2_HEARTWARS_BUY_FAILED_HAVE"));
                }
            } else {
                GameController.broadcast(Dialog.Get("OPTIMIZE2_HEARTWARS_BUY_FAILED_MONEY"));
            }
        }
    }
}