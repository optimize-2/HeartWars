using System;

using Celeste.Mod.CelesteNet;
using Celeste.Mod.CelesteNet.DataTypes;

namespace Celeste.Mod.HeartWars {
    
    public class DataGameStartResponse : DataType<DataGameStartResponse> {
        static DataGameStartResponse() => DataID = $"{HeartWarsModule.Name}_DataGameStartResponse_{HeartWarsModule.ProtocolVersion}";
        public DataPlayerInfo Sender;
        public override MetaType[] GenerateMeta(DataContext ctx) => new MetaType[] { new MetaPlayerUpdate(Sender) };
        public override void FixupMeta(DataContext ctx) => Sender = Get<MetaPlayerUpdate>(ctx).Player;

        public String area;

        public String team;
        public String name;

        protected override void Read(CelesteNetBinaryReader reader) {
            area = reader.ReadString();
            team = reader.ReadString();
            name = reader.ReadString();
        }

        protected override void Write(CelesteNetBinaryWriter writer) {
            writer.Write(area);
            writer.Write(team);
            writer.Write(name);
        }
    }
}