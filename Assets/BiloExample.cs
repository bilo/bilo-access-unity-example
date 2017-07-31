/*
 * Copyright 2017 Urs Fässler
 * SPDX-License-Identifier: Apache-2.0
 */

using UnityEngine;
using UnityEngine.UI;
using Bilo.Access;
using Bilo.Access.Android;
using Bilo.Stack;
using System.Collections.Generic;

public class BiloExample : MonoBehaviour
{
    public Dropdown deviceSelection;
    public Button connectButton;
    public Button disconnectButton;
    public Text connectText;
    public Text connectDeviceText;
    public Text addedBlocksText;
    public Text removedBlocksText;

    public Button redButton;
    public Button greenButton;
    public Button blueButton;
    public Button blackButton;

    private BiloAccessAndroid androidAccess;
    private BiloAccess access;
    private Devices devices;
    private Stack stack;

    private BaseLightShow baseLightShow;

    private void Awake()
    {
        androidAccess = new BiloAccessAndroid();
    }

    private string BlockToText(Block block)
    {
        BlockId id = block.getId();
        return id.type.ToString() + " " + id.position.ToString() + " " + id.rotation.ToString() + " " + block.GetLeds()[0].Color.ToString();
    }

    private void Start ()
    {
        androidAccess.Create();

        access = androidAccess.GetAccess();
        devices = access.GetDevices();
        stack = access.GetStack();
        baseLightShow = new BaseLightShow(stack.GetBase());
        stack.GetBlocks().AddObserver(new ChangeNotifier(addedBlocksText, removedBlocksText, BlockToText));

        access.AddConnectionchangeObserver(new ConnectionStatePublisher(connectText));

        deviceSelection.options.Clear();
        foreach (Device device in devices.GetDevices())
        {
            Dropdown.OptionData item = new Dropdown.OptionData() { text = device.GetName() };
            deviceSelection.options.Add(item);
        }
        deviceSelection.value = 1;
        deviceSelection.value = 0;

        connectButton.onClick.AddListener(Connect);
        disconnectButton.onClick.AddListener(Disconnect);

        ConnectColorButton(redButton, Bilo.Stack.Color.Red);
        ConnectColorButton(greenButton, Bilo.Stack.Color.Green);
        ConnectColorButton(blueButton, Bilo.Stack.Color.Blue);
        ConnectColorButton(blackButton, Bilo.Stack.Color.Black);

        baseLightShow.Start();
    }

    private void ConnectColorButton(Button button, Bilo.Stack.Color color)
    {
        button.onClick.AddListener(() => SetColor(color));
    }

    private Device GetDeviceByName(string name)
    {
        foreach (Device device in devices.GetDevices())
        {
            if (device.GetName() == name)
            {
                return device;
            }
        }

        return null;
    }

    private void Connect()
    {
        Device device = GetDeviceByName(deviceSelection.captionText.text);
        if (device == null)
        {
            Debug.LogError("bilo example: " + "device not found :(");
        }
        else
        {
            Debug.Log("bilo example: " + "Connecting to: " + device.GetName());
            devices.Connect(device);
        }
    }

    private void Disconnect()
    {
        Debug.Log("bilo example: " + "Disconnecting");
        devices.Disconnect();
    }

    private void SetColor(Bilo.Stack.Color color)
    {
        ICollection<Block> blocks = stack.GetBlocks().Items();

        foreach (Block block in blocks)
        {
            foreach (RgbLed led in block.GetLeds())
            {
                led.Color = color;
            }
        }
    }

    private void Update ()
    {
        baseLightShow.Update();

        access.Calc();

        connectDeviceText.text = "Connected device: " + devices.GetConnectedDevice().GetName();
    }
}
