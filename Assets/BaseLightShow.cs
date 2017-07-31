/*
 * Copyright 2017 Urs Fässler
 * SPDX-License-Identifier: Apache-2.0
 */

using UnityEngine;
using Bilo.Stack;

class BaseLightShow
{
    private float timer;
    private int led;
    private Block baseBlock;

    public BaseLightShow(Block baseBlock)
    {
        this.baseBlock = baseBlock;
    }

    public void Start()
    {
        timer = 0;
        led = 0;
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1)
        {
            timer = 0;
            led = (led + 1) % baseBlock.GetLeds().Count;
        }

        int i = 0;
        foreach (RgbLed led in baseBlock.GetLeds())
        {
            Bilo.Stack.Color color = (i == this.led) ? Bilo.Stack.Color.Green : Bilo.Stack.Color.Black;
            led.Color = color;
            i++;
        }
    }
}
