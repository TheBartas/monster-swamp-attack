using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverAmmo : CollectableItem
{
    protected override void Start()
    {
        base.Start();
        itemName = "RevolverAmmo";
        amount = 16;
    }
}
