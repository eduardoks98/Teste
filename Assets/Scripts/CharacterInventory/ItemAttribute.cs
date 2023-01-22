using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Scriptables;
using UnityEngine;

[Serializable]

public class ItemAttribute
{
    public ItemData itemData;
    public float percentage;

    public ItemAttribute(ItemData item, float perc)
    {
        itemData = item;
        percentage = perc;
    }
}