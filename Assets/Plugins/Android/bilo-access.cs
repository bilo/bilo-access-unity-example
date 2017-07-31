/*
 * Copyright 2017 Urs Fässler
 * SPDX-License-Identifier: Apache-2.0
 */

using UnityEngine;
using System.Collections.Generic;

namespace Bilo.Access
{
    using Bilo.Base;

    
    public class Device : AndroidAdapter
    {
        public Device(AndroidJavaObject ajo) : base(ajo)
        {
        }

        public string GetName()
        {
            return ajo.Call<string>("getName");
        }
    }

    public class Devices : AndroidAdapter
    {
        private List<Device> devices = new List<Device>();

        public Devices(AndroidJavaObject ajo) : base(ajo)
        {
            var items = CallJavaCollection("getDevices", x => new Device(x));
            devices.AddRange(items);
        }

        public void Connect(Device device)
        {
            ajo.Call("connect", device.GetAndroidJavaObject());
        }

        public void Disconnect()
        {
            ajo.Call("disconnect");
        }

        public Device GetConnectedDevice()
        {
            AndroidJavaObject javaDevice = ajo.Call<AndroidJavaObject>("getConnectedDevice");
            Device csDevice = devices.Find(x => javaDevice.Call<bool>("equals", x.GetAndroidJavaObject()));

            if (csDevice == null)
            {
                csDevice = new Device(javaDevice);
                devices.Add(csDevice);
            }

            return csDevice;
        }

        public IEnumerable<Device> GetDevices()
        {
            return devices;
        }

    }

    public abstract class ConnectionChangeObserver : AndroidJavaProxy
    {
        public ConnectionChangeObserver() :
            base("world.bilo.access.ConnectionChangeObserver")
        {
        }

        public void connected()
        {
            Connected();
        }

        public void disconnected()
        {
            Disconnected();
        }

        public abstract void Connected();
        public abstract void Disconnected();
    }

    public class BiloAccess : AndroidAdapter
    {
        private Bilo.Stack.Stack stack;
        private Devices devices;

        public BiloAccess(AndroidJavaObject ajo) : base(ajo)
        {
            stack = CallJavaObject("getApi", x => new Bilo.Stack.Stack(x));
            devices = CallJavaObject("getDevices", x => new Devices(x));
        }

        public void Calc()
        {
            ajo.Call("calc");
        }

        public Devices GetDevices()
        {
            return devices;
        }

        public void AddConnectionchangeObserver(ConnectionChangeObserver observer)
        {
            AndroidJavaObject observers = ajo.Call<AndroidJavaObject>("getConnectionChangeObserver");
            observers.Call("add", observer);
        }

        public Bilo.Stack.Stack GetStack()
        {
            return stack;
        }
    }

}