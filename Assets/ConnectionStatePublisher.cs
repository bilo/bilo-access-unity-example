/*
 * Copyright 2017 Urs Fässler
 * SPDX-License-Identifier: Apache-2.0
 */

using UnityEngine.UI;
using Bilo.Access;

class ConnectionStatePublisher : ConnectionChangeObserver
{
    public Text text;

    public ConnectionStatePublisher(Text text)
    {
        this.text = text;
    }

    public override void Connected()
    {
        text.text = "Connected";
    }

    public override void Disconnected()
    {
        text.text = "Disconnected";
    }
}
