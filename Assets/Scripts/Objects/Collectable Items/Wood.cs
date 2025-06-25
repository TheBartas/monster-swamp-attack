using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Wood : CollectableItem
{
    protected override void Start() {
        base.Start();
        itemName = "Wood";
        amount = 10;
    }
}
