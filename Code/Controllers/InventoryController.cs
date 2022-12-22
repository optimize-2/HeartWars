using System.Collections.Generic;
using Celeste.Mod.HeartWars.Inventory;

namespace Celeste.Mod.HeartWars.Controllers {
    public class InventoryController  {
        public static List<Item> inventory;
        public static void initialize() {
            inventory = new List<Item>();
            inventory.Add(new GunItem());
            inventory.Add(new GoldenStrawberryItem());
            inventory.Add(new FireBallItem());
            inventory.Add(new BadPearlItem());
        }
    }
}