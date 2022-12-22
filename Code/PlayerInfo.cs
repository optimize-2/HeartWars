using System;
using Celeste.Mod.HeartWars.Controllers;
using Celeste.Mod.HeartWars.Inventory;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.HeartWars {
    internal class PlayerInfo {
        public static ItemType Hand;

        public static String Area;
        public static String PlayerTeam;

        public static Color PlayerHairColor;
        
        public static float MissileSpeed;
        public static float MissileDamage;
        public static float MissileMaxTime;

        public static float PlayerHealth;

        public static float PlayerExtraHealth;

        public static int PlayerMoney;

        public static float WeaponInfoCooldown;

        public static int MissileIndex;

        public static float RegenerationEffect;
        public static float DamageAbsorbEffect;
        public static float BlindEffect;

        public static void initialize() {
            Hand = ItemType.GUN;
            PlayerTeam = "UNSELECTED";
            MissileSpeed = 400f;
            MissileDamage = 20f;
            MissileMaxTime = 5f;
            PlayerHealth = 100f;
            PlayerExtraHealth = 0f;
            PlayerMoney = 0;
            WeaponInfoCooldown = 0.5f;
            MissileIndex = 0;
            RegenerationEffect = 0f;
            DamageAbsorbEffect = 0f;
            BlindEffect = 0f;
        }
        public static String getPlayerName() {
            return Controller.module?.Context?.Client?.PlayerInfo?.DisplayName ?? "NULL";
        }
    }
}
