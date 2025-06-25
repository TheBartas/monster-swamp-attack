using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : CollectableItem
{
    protected override void Start()
    {
        base.Start();
        itemName = "Scrap";
        amount = Random.Range(10, 30);
    }
}
