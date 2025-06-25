using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlayerShoot : MonoBehaviour
{
    // [SerializeField] private Gun currentGun; // Aktualnie wybrana broń
    private Gun currentGun;

    [SerializeField] private KeyCode reloadKey;

    private bool canShoot = true; 

    private void Update()
    {
        if (canShoot){
            HandleShooting();
            HandleReload();
        }

    }

    private void HandleShooting()
    {
        if (Input.GetMouseButton(0) && currentGun != null)
        {
            currentGun.Shoot();
        }
    }

    private void HandleReload()
    {
        if (Input.GetKeyDown(reloadKey) && currentGun != null)
        {
            currentGun.StartReload();
        }
    }


    public void SetActiveGun(Gun newGun)
    {
        currentGun = newGun;
    }

    public void SetCanShoot(bool value)
    {
        canShoot = value;
    }
}
