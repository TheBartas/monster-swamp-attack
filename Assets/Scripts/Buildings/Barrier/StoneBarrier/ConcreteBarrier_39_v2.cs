using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteBarrier_39_v2 : Barrier
{
    private void Start() {
        health = 40f;
    }

    public override void Damage(float amount)
    {
        base.Damage(amount); 
    }
}
