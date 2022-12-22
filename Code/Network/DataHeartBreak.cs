using System;

using Celeste.Mod.CelesteNet;
using Celeste.Mod.CelesteNet.DataTypes;

namespace Celeste.Mod.HeartWars {
    
    public class DataHeartBreak : DataType<DataHeartBreak> {
        static DataHeartBreak() => DataID = $"{HeartWarsModule.Name}_DataHeartBreak_{HeartWarsModule.ProtocolVersion}";
        public DataPlayerInfo Sender;
        public override MetaType[] GenerateMeta(DataContext ctx) => new MetaType[] { new MetaPlayerUpdate(Sender) };
        public override void FixupMeta(DataContext ctx) => Sender = Get<MetaPlayerUpdate>(ctx).Player;

        public String area;
        public String player;
        public String team;

        protected override void Read(CelesteNetBinaryReader reader) {
            area = reader.ReadString();
            player = reader.ReadString();
            team = reader.ReadString();
        }

        protected override void Write(CelesteNetBinaryWriter writer) {
            writer.Write(area);
            writer.Write(player);
            writer.Write(team);
        }
    }
}