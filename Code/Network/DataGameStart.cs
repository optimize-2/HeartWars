using System;

using Celeste.Mod.CelesteNet;
using Celeste.Mod.CelesteNet.DataTypes;

namespace Celeste.Mod.HeartWars {
    
    public class DataGameStart : DataType<DataGameStart> {
        static DataGameStart() => DataID = $"{HeartWarsModule.Name}_DataGameStart_{HeartWarsModule.ProtocolVersion}";
        public DataPlayerInfo Sender;
        public override MetaType[] GenerateMeta(DataContext ctx) => new MetaType[] { new MetaPlayerUpdate(Sender) };
        public override void FixupMeta(DataContext ctx) => Sender = Get<MetaPlayerUpdate>(ctx).Player;

        public String area;

        protected override void Read(CelesteNetBinaryReader reader) {
            area = reader.ReadString();
        }

        protected override void Write(CelesteNetBinaryWriter writer) {
            writer.Write(area);
        }
    }
}