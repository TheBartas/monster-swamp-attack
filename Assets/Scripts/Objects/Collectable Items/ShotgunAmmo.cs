using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAmmo : CollectableItem
{
    protected override void Start() {
        base.Start();
        itemName = "ShotgunAmmo";
        amount = 7;
    }
}
