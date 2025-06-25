using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private Transform weaponHolder; // to do weaponholder tego w canvas co mam
    private int currentWeaponIndex = 3; // Indeks aktualnie wybranej broni

    private void Start()
    {
        SelectWeapon(currentWeaponIndex); 
    }

    private void Update()
    {
        HandleWeaponSwitching();
    }

    private void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            currentWeaponIndex = 0;
            SelectWeapon(currentWeaponIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            currentWeaponIndex = 1;
            SelectWeapon(currentWeaponIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeaponIndex = 2;
            SelectWeapon(currentWeaponIndex);
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {

            currentWeaponIndex = 3;
            SelectWeapon(currentWeaponIndex);
        }
    }

    private void SelectWeapon(int weaponIndex)
    {
        for (int i = 0; i < weaponHolder.childCount; i++)
        {
            GameObject weapon = weaponHolder.GetChild(i).gameObject;
            bool isActive = i == weaponIndex;
            weapon.SetActive(isActive);

            if (isActive)
            {

                PlayerShoot playerShoot = GetComponent<PlayerShoot>();
                PlayerHit playerHit = GetComponent<PlayerHit>();

                if (playerShoot != null && weaponIndex != 3) {

                    playerShoot.enabled = true;
                    if (playerHit != null) playerHit.enabled = false;

                    Gun gun = weapon.GetComponent<Gun>();


                    if (gun != null) {
                        gun.UpdateAmmoDisplay();
                        playerShoot.SetActiveGun(gun);  
                    }
                }
                else if (playerHit != null && weaponIndex == 3) {
                    
                    playerHit.enabled = true;
                    if (playerShoot != null) playerShoot.enabled = false;
                
                    HandWeapon handWeapon = weapon.GetComponent<HandWeapon>();

                    if (handWeapon != null) {
                        handWeapon.UpdateAmmoDisplay();
                        playerHit.SetActiveHandWeapon(handWeapon); 
                    }
                }
            }
        }
    }

    public Gun GetActiveWeapon()
    {
        GameObject activeWeapon = null;

        for (int i = 0; i < weaponHolder.childCount; i++) {
            GameObject weapon = weaponHolder.GetChild(i).gameObject;
            if (weapon.activeSelf) {
                activeWeapon = weapon;
                break;
            }
        }

        if (activeWeapon != null) {
            return activeWeapon.GetComponent<Gun>();
        }

        return null;
    }

}
