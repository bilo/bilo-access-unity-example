/*
 * Copyright 2017 Urs Fässler
 * SPDX-License-Identifier: Apache-2.0
 */

using UnityEngine;

namespace Bilo.Access.Android
{
    using Bilo.Base;


    public class BiloAccessAndroid : AndroidAdapter
    {
        private BiloAccess access;

        public BiloAccessAndroid() : base(new AndroidJavaObject("world.bilo.access.android.BiloAccessAndroid"))
        {
        }

        public void Create()
        {
            ajo.Call("create");
            access = CallJavaObject("getAccess", x => new BiloAccess(x));
        }

        public void Destroy()
        {
            ajo.Call("destroy");
            access = null;
        }

        public BiloAccess GetAccess()
        {
            return access;
        }

    }


}