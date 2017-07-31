/*
 * Copyright 2017 Urs Fässler
 * SPDX-License-Identifier: Apache-2.0
 */

using UnityEngine.UI;
using Bilo.Stack;
using Bilo.Base;
using System.Collections.Generic;
using System;

class ChangeNotifier : ICollectionObserver<Block>
{
    private Text addedText;
    private Text removedText;
    private Func<Block, string> blockToString;

    public ChangeNotifier(Text addedText, Text removedText, Func<Block, string> blockToString)
    {
        this.addedText = addedText;
        this.removedText = removedText;
        this.blockToString = blockToString;
    }

    public void Added(IEnumerable<Block> items)
    {
        addedText.text = "last added blocks:" + BlockText(items);
    }

    public void Removed(IEnumerable<Block> items)
    {
        removedText.text = "last removed blocks:" + BlockText(items);
    }

    private string BlockText(IEnumerable<Block> items)
    {
        string text = "";

        foreach (Block block in items)
        {
            text += "\n" + blockToString(block);
        }

        return text;
    }
}
