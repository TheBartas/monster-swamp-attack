using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedKit : CollectableItem
{
    protected override void Start()
    {
        base.Start();
        itemName = "MedKit";
        amount = 1;
    }
}
