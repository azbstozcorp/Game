﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule.Shapes;
using CoreModule.Saving;

namespace CoreModule.Entities {
    public class LevelTrigger : Entity, ISerializable<LevelTrigger> {
        public enum TriggerType : byte {
            Player, Enemy,
            DamageBlunt,
            DamagePierce,
            DamageCut,
            DamageBurn,
            DamageElectric,
        }

        public string Name { get; set; } = "unnamed";
        public List<TriggerType> Flags { get; set; } = new List<TriggerType>();

        public LevelTrigger() { }
        public LevelTrigger(PointF topLeft, PointF bottomRight) {
            Bounds = new RectF(topLeft, bottomRight);
        }

        public override void Draw() {
            base.Draw();

            CoreGame.Instance.DrawRect(Bounds.TopLeft + (PointF)Scenes.Level.Instance.CameraLocation,
                                       Bounds.BottomRight + (PointF)Scenes.Level.Instance.CameraLocation,
                                       PixelEngine.Pixel.Presets.Blue);

            if (Scenes.Level.Instance.Editing) CoreGame.Instance.DrawText(Bounds.TopLeft + (PointF)Scenes.Level.Instance.CameraLocation + (1, 2), Name, PixelEngine.Pixel.Presets.Blue);
        }

        public byte[] GetSaveData() {
            List<byte> toReturn = new List<byte>();

            byte[] nameBytes = Encoding.ASCII.GetBytes(Name);
            byte nameByteLength = (byte)nameBytes.Length;
            toReturn.Add(nameByteLength);
            toReturn.AddRange(nameBytes);

            float topLeftX = Bounds.TopLeft.X;
            float topLeftY = Bounds.TopLeft.Y;
            byte[] topLeftXBytes = BitConverter.GetBytes(topLeftX);
            byte[] topLeftYBytes = BitConverter.GetBytes(topLeftY);

            float bottomRightX = Bounds.BottomRight.X;
            float bottomRightY = Bounds.BottomRight.Y;
            byte[] bottomRightXBytes = BitConverter.GetBytes(bottomRightX);
            byte[] bottomRightYBytes = BitConverter.GetBytes(bottomRightY);

            toReturn.AddRange(topLeftXBytes);
            toReturn.AddRange(topLeftYBytes);
            toReturn.AddRange(bottomRightXBytes);
            toReturn.AddRange(bottomRightYBytes);

            byte flagsByteLength = (byte)Flags.Count;
            toReturn.Add(flagsByteLength);
            foreach (TriggerType t in Flags) toReturn.Add((byte)t);

            return toReturn.ToArray();
        }
        public void LoadSaveData(byte[] data) {
            int location = 0;
            byte nameByteLength = data[location]; location++;
            Name = Encoding.ASCII.GetString(data, location, nameByteLength); location += nameByteLength;

            float topLeftX = BitConverter.ToSingle(data, location); location += sizeof(float);
            float topLeftY = BitConverter.ToSingle(data, location); location += sizeof(float);
            float bottomRightX = BitConverter.ToSingle(data, location); location += sizeof(float);
            float bottomRightY = BitConverter.ToSingle(data, location); location += sizeof(float);
            Bounds = new RectF(topLeftX, topLeftY, bottomRightX, bottomRightY);

            byte flagsByteLength = data[location]; location++;
            for (byte i = 0; i < flagsByteLength; i++) {
                Flags.Add((TriggerType)data[location]); location++;
            }
        }
    }
}
