using System;

using Microsoft.Xna.Framework;
using Celeste.Mod.CelesteNet;
using Celeste.Mod.CelesteNet.DataTypes;

namespace Celeste.Mod.HeartWars {
    
    public class DataMissileShoot : DataType<DataMissileShoot> {
        static DataMissileShoot() => DataID = $"{HeartWarsModule.Name}_DataMissileShoot_{HeartWarsModule.ProtocolVersion}";
        public DataPlayerInfo Sender;
        public override MetaType[] GenerateMeta(DataContext ctx) => new MetaType[] { new MetaPlayerUpdate(Sender) };
        public override void FixupMeta(DataContext ctx) => Sender = Get<MetaPlayerUpdate>(ctx).Player;

        public String area;
        public String shooterName, shooterTeam;
        public Vector2 dir, pos;
        public float speed, damage, maxTime;
        public int id;

        protected override void Read(CelesteNetBinaryReader reader) {
            area = reader.ReadString();
            shooterName = reader.ReadString();
            shooterTeam = reader.ReadString();
            float dirx = float.Parse(reader.ReadString());
            float diry = float.Parse(reader.ReadString());
            dir = new Vector2(dirx, diry);
            float posx = float.Parse(reader.ReadString());
            float posy = float.Parse(reader.ReadString());
            pos = new Vector2(posx, posy);
            speed = float.Parse(reader.ReadString());
            damage = float.Parse(reader.ReadString());
            maxTime = float.Parse(reader.ReadString());
            id = int.Parse(reader.ReadString());
        }

        protected override void Write(CelesteNetBinaryWriter writer) {
            writer.Write(area);
            writer.Write(shooterName);
            writer.Write(shooterTeam);
            writer.Write(dir.X.ToString());
            writer.Write(dir.Y.ToString());
            writer.Write(pos.X.ToString());
            writer.Write(pos.Y.ToString());
            writer.Write(speed.ToString());
            writer.Write(damage.ToString());
            writer.Write(maxTime.ToString());
            writer.Write(id.ToString());
        }
    }
}