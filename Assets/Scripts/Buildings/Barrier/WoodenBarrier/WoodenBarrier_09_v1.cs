using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenBarrier_09_v1 : Barrier
{
    private void Start() {
        health = 100f; // 240
    }

    public override void Damage(float amount)
    {
        base.Damage(amount); 
    }
}
