using System;

using Celeste.Mod.CelesteNet;
using Celeste.Mod.CelesteNet.DataTypes;

namespace Celeste.Mod.HeartWars {
    public class DataPlayerDie : DataType<DataPlayerDie> {
        static DataPlayerDie() => DataID = $"{HeartWarsModule.Name}_DataPlayerDie_{HeartWarsModule.ProtocolVersion}";
        public DataPlayerInfo Sender;
        public override MetaType[] GenerateMeta(DataContext ctx) => new MetaType[] { new MetaPlayerUpdate(Sender) };
        public override void FixupMeta(DataContext ctx) => Sender = Get<MetaPlayerUpdate>(ctx).Player;

        public String area;
        public String playerName, playerTeam;
        public String killerName, killerTeam;
        public int money;

        protected override void Read(CelesteNetBinaryReader reader) {
            area = reader.ReadString();
            playerName= reader.ReadString();
            playerTeam = reader.ReadString();
            killerName = reader.ReadString();
            killerTeam = reader.ReadString();
            money = int.Parse(reader.ReadString());
        }

        protected override void Write(CelesteNetBinaryWriter writer) {
            writer.Write(area);
            writer.Write(playerName);
            writer.Write(playerTeam);
            writer.Write(killerName);
            writer.Write(killerTeam);
            writer.Write(money.ToString());
        }
    }
}