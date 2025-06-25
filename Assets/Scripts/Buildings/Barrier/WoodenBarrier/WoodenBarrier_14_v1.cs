using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenBarrier_14_v1 : Barrier
{
    private void Start() {
        health = 200;
    }

    public override void Damage(float amount)
    {
        base.Damage(amount); 
    }
}
