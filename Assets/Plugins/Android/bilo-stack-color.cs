/*
 * Copyright 2017 Urs Fässler
 * SPDX-License-Identifier: Apache-2.0
 */

using UnityEngine;
using System.Collections.Generic;
using System;

namespace Bilo.Stack
{


    public enum Color
    {
        Black, Red, Green, Yellow, Blue, Magenta, Cyan, White
    }

    public static class ColorConverter
    {
        private static Dictionary<Color, AndroidJavaObject> map = createMap();

        private static Dictionary<Color, AndroidJavaObject> createMap()
        {
            Dictionary<Color, AndroidJavaObject> map = new Dictionary<Color, AndroidJavaObject>();

            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                map.Add(color, Convert(color));
            }

            return map;
        }

        private static AndroidJavaObject Convert(Color value)
        {
            int bifield = (int)value;
            bool red = Bit(bifield, 0);
            bool green = Bit(bifield, 1);
            bool blue = Bit(bifield, 2);

            AndroidJavaClass jo = new AndroidJavaClass("world.bilo.stack.Color");
            AndroidJavaObject color = jo.CallStatic<AndroidJavaObject>("produce", red, green, blue);

            return color;
        }

        private static bool Bit(int value, int position)
        {
            return ((value >> position) & 0x1) == 0x01;
        }

        public static AndroidJavaObject ToJavaColor(Color value)
        {
            return map[value];
        }

        public static Color FromJavaColor(AndroidJavaObject ajo)
        {
            int intColor = ajo.Call<int>("ordinal");
            Color color = (Color)intColor;
            return color;
        }
    }


}