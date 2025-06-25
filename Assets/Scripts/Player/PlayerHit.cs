using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    private HandWeapon handWeapon;

    private void Update()
    {
        HandleHitting();
    }

    private void HandleHitting()
    {
        if (Input.GetMouseButton(0) && handWeapon != null)
        {
            handWeapon.Attack();
        }
    }

    public void SetActiveHandWeapon(HandWeapon newHandWeapon)
    {
        handWeapon = newHandWeapon;
    }
}
