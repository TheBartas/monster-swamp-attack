using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; 
public class Gun : MonoBehaviour
{

    [Header("References")]
    [SerializeField] public GunData gunData;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Camera fpsCam;

    [Header("Graphics")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject[] hitFlash;
    [SerializeField] private TextMeshProUGUI ammunitionDisplay;
    [SerializeField] private Image reloadProgressBar; // Referencja do paska postępu

    private bool allowInvoke = true;
    private bool readyToShoot;
    private bool reloading;
    private int bulletsLeft;
    private int bulletsShot;

    private void Awake() {
        bulletsLeft = gunData.magazineSize;
        readyToShoot = true;
        gunData.damageLevel = 1;
        gunData.rangeLevel = 1;
        UpdateAmmoDisplay();
    }

    public void Shoot()
    { // readyToShoot && !reloading && gunData.currentAmmo > 0
        if (readyToShoot && !reloading && gunData.currentAmmo > 0 && bulletsLeft > 0) // readyToShoot && !reloading && bulletsLeft > 0
        {
            readyToShoot = false;

            Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            // Ray ray = new Ray(attackPoint.position, fpsCam.transform.forward); // Zaczynamy promień od attackPoint
            RaycastHit hit;
            

            Vector3 directionWithoutSpread = ray.direction;

            // Rozrzut (spread)
            float x = Random.Range(-gunData.spread, gunData.spread);
            float y = Random.Range(-gunData.spread, gunData.spread);


            Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

            // int layerMask = ~LayerMask.GetMask("TransparentCollider");
            int layerMask = ~LayerMask.GetMask("MainTarget");

            if (Physics.Raycast(ray.origin, directionWithSpread, out hit, gunData.range, layerMask))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    GameObject effectInstanceInEnemy = Instantiate(hitFlash[1], hit.point, Quaternion.identity);
                    Destroy(effectInstanceInEnemy, 0.02f);
                    IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                    if (damageable != null) {
                        damageable.Damage(gunData.damage); 
                    }

                } else {
                    GameObject effectInstance = Instantiate(hitFlash[0], hit.point, Quaternion.identity);
                    Destroy(effectInstance, 0.02f);
                }
                Debug.Log("DMG: " + gunData.damage);
            }

            // if (muzzleFlash != null) {
            //     Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            // }
            muzzleFlash.Play();

            bulletsLeft--;
            bulletsShot++;
            gunData.currentAmmo--;

            UpdateAmmoDisplay();

            if (allowInvoke) {
                Invoke("ResetShot", gunData.timeBetweenShooting);
                allowInvoke = false;
                playerRb.AddForce(-directionWithSpread.normalized * gunData.recoilForce, ForceMode.Impulse);
            }
            if (bulletsShot < gunData.bulletsPerTap && bulletsLeft > 0) {
                Invoke("Shoot", gunData.timeBetweenShots);
            }
        }
    }


    private void ResetShot() {
        readyToShoot = true;
        allowInvoke = true;
    }

    public void StartReload()
    { // !reloading && (gunData.currentAmmo < gunData.magazineSize) && gunData.currentAmmo > 0
        if ((!reloading && gunData.currentAmmo < gunData.magazineSize && gunData.currentAmmo != 0) || (bulletsLeft == 0 && gunData.currentAmmo != 0)) //!reloading && gunData.currentAmmo < gunData.magazineSize
        {
            bulletsLeft = gunData.magazineSize;
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        reloading = true;

    
        float elapsedTime = 0f;
        float reloadDuration = gunData.reloadTime;
        reloadProgressBar.gameObject.SetActive(true);

        while (elapsedTime < reloadDuration) {
            elapsedTime += Time.deltaTime;
            // reloadProgressBar.fillAmount = 1 - (elapsedTime / reloadDuration);  
            reloadProgressBar.fillAmount = elapsedTime / reloadDuration; 
            yield return null;
        }
        // yield return new WaitForSeconds(gunData.reloadTime);

        // gunData.currentAmmo = Mathf.Clamp(gunData.currentAmmo, 0, gunData.magazineSize);
        bulletsLeft = gunData.magazineSize;
        reloading = false;

        yield return new WaitForSeconds(0.5F);
        reloadProgressBar.gameObject.SetActive(false);

        UpdateAmmoDisplay();
    }

    public void UpdateAmmoDisplay()
    {
        if (ammunitionDisplay != null) {
            int fullMagazines = gunData.currentAmmo / gunData.magazineSize; 
            ammunitionDisplay.text = bulletsLeft + " / " + fullMagazines + $" ({gunData.currentAmmo}) "; // Format: "ilość amunicji w magazynku / liczba magazynków (ilość pocisków)"
        }
    }

    public void ChangeBulletsLeft() {
        bulletsLeft = gunData.currentAmmo % gunData.magazineSize; 
    }

    public void AddAmmo(int amount)
    {
        gunData.currentAmmo += amount;
        ChangeBulletsLeft();
        if (gameObject.activeSelf) {
            UpdateAmmoDisplay();
        } 
    }



}
