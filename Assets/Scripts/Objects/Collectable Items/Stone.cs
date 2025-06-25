using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : CollectableItem
{
    protected override void Start() {
        base.Start();
        itemName = "Stone";
        amount = Random.Range(5, 40);
    }
}
