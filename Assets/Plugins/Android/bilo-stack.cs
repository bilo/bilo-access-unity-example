/*
 * Copyright 2017 Urs Fässler
 * SPDX-License-Identifier: Apache-2.0
 */

using UnityEngine;
using System.Collections.Generic;

namespace Bilo.Stack
{
    using Bilo.Base;


    public class Stack : AndroidAdapter
    {
        private Block baseBlock;
        private ObservableCollection<Block> blocks;

        public Stack(AndroidJavaObject ajo) : base(ajo)
        {
            baseBlock = CallJavaObject("getBase", x => new Block(x));
            blocks = CallJavaObject("getBlocks", x => new ObservableCollection<Block>(x, y => new Block(y)));
        }

        public Block GetBase()
        {
            return baseBlock;
        }

        public ObservableCollection<Block> GetBlocks()
        {
            return blocks;
        }
    }

    public class Block : AndroidAdapter
    {
        private BlockId id;
        private List<RgbLed> leds = new List<RgbLed>();

        public Block(AndroidJavaObject ajo) : base(ajo)
        {
            var items = CallJavaCollection("getLeds", x => new RgbLed(x));
            leds.AddRange(items);

            AndroidJavaObject javaId = ajo.Call<AndroidJavaObject>("getId");

            BlockType type = ConvertBlockType(javaId.Get<AndroidJavaObject>("type"));
            Vector position = ConvertVector(javaId.Get<AndroidJavaObject>("position"));
            Rotation rotation = ConvertRotation(javaId.Get<AndroidJavaObject>("rotation"));

            id = new BlockId(type, position, rotation);
        }

        private static BlockType ConvertBlockType(AndroidJavaObject ajo)
        {
            int intType = ajo.Call<int>("ordinal");
            BlockType type = (BlockType)intType;
            return type;
        }

        private static Rotation ConvertRotation(AndroidJavaObject ajo)
        {
            int intRotation = ajo.Call<int>("ordinal");
            Rotation rotation = (Rotation)intRotation;
            return rotation;
        }

        private static Vector ConvertVector(AndroidJavaObject ajo)
        {
            int px = ajo.Get<int>("x");
            int py = ajo.Get<int>("y");
            int pz = ajo.Get<int>("z");
            Vector vector = new Vector(px, py, pz);
            return vector;
        }

        public BlockId getId()
        {
            return id;
        }

        public List<RgbLed> GetLeds()
        {
            return leds;
        }
    }

    public class RgbLed : AndroidAdapter
    {
        public RgbLed(AndroidJavaObject ajo) : base(ajo)
        {
        }

        public Color Color
        {
            get
            {
                return ColorConverter.FromJavaColor(ajo.Call<AndroidJavaObject>("getColor"));
            }

            set
            {
                ajo.Call("setColor", ColorConverter.ToJavaColor(value));
            }
        }
    }

    public class BlockId
    {
        public readonly BlockType type;
        public readonly Vector position;
        public readonly Rotation rotation;

        public BlockId(BlockType type, Vector position, Rotation rotation)
        {
            this.type = type;
            this.position = position;
            this.rotation = rotation;
        }
    }

    public enum BlockType
    {
        Block4x2, Block2x2, Base10x10
    }

    public class Vector
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        public Vector(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        override
        public string ToString()
        {
            return "(" + x + "/" + y + "/" + z + ")";
        }
    }

    public enum Rotation
    {
        Deg0, Deg90, Deg180, Deg270
    }

}
