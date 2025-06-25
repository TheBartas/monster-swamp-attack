using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGAmmo : CollectableItem
{
    protected override void Start()
    {
        base.Start();
        itemName = "MGAmmo";
        amount = 80;
    }
}
