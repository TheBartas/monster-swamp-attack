using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TMP_Text[] counterArray;
    // ----- How works counterArray?
    // id:   |      for:
    // 0     |      cntStone
    // 1     |      cntWood
    // 2     |      cntScrap
    // 3     |      cntMedKit
    // 4     |      cntRevolver (it means: range)
    // 5     |      cntShotgun (it means: range)
    // 6     |      cntMG (it means: range)  
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private TMP_Text revolverDmgText;
    [SerializeField] private TMP_Text shotgunDmgText;
    [SerializeField] private TMP_Text mgDmgText;
    [SerializeField] private TMP_Text ammunitionDisplay;

    [Header("References")]
    [SerializeField] private GunData revolverTemplate;
    [SerializeField] private GunData shotgunTemplate;
    [SerializeField] private GunData mgTemplate;
    [SerializeField] private Gun revolver;
    [SerializeField] private Gun shotgun;
    [SerializeField] private Gun mg;
    private PlayerHealth playerHealth;

    // For inventory

    public int woodNumber = 50;
    public int stoneNumber = 50;
    public int medKitNumber = 50;
    public int scrapNumber = 10000;
    private int revolverDmg = 5;
    private int shotgunDmg = 60;
    private int mgDmg = 20;

    // For collecting itmes to inventory in order
    private List<CollectableItem> itemsInRange = new List<CollectableItem>();
 
    // [Features]
    private float medKitHP = 30.0f;

    
    private bool isOpen;

    private void Start() {
        LoadInput();
        isOpen = false;
        inventoryPanel.SetActive(isOpen);
        mainText.text = string.Empty;
    }

    private void LoadInput() {
        LoadGunData(revolverTemplate, revolverDmg, 80, revolverDmgText);
        LoadGunData(shotgunTemplate,shotgunDmg, 70, shotgunDmgText);
        LoadGunData(mgTemplate, mgDmg, 150, mgDmgText);
        LoadBasicResources();
        LoadBasicWeapons();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void LoadGunData(GunData dataToLoad, int dmg, int range, TMP_Text toText) {
        dataToLoad.damage = dmg;
        dataToLoad.range = range;
        toText.text = dmg.ToString();
    }

    private void LoadBasicResources() {
        counterArray[0].text = stoneNumber.ToString();
        counterArray[1].text = woodNumber.ToString();
        counterArray[2].text = scrapNumber.ToString();
        counterArray[3].text = medKitNumber.ToString();
    }

    private void LoadBasicWeapons() {
        counterArray[4].text = revolverTemplate.range.ToString();
        counterArray[5].text = shotgunTemplate.range.ToString();
        counterArray[6].text = mgTemplate.range.ToString();
        revolverTemplate.currentAmmo = 12;
        shotgunTemplate.currentAmmo = 16;
        mgTemplate.currentAmmo = 160;
    }

    private void Update() {
        PlayerInput();
    }

    private void PlayerInput() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            ToggleInventory();
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            UseMedkit();
        }
    }

    private void ToggleInventory() {
        isOpen = !isOpen; 

        if (isOpen) {
            inventoryPanel.SetActive(true); 
            Time.timeScale = 0f; 
            LoadBasicResources();

        } else {
            inventoryPanel.SetActive(false); 
            Time.timeScale = 1f; 
        }
    }

    public void AddToList(CollectableItem item) {
        itemsInRange.Add(item);
    }

    public void RemoveFromList(CollectableItem item) {
        itemsInRange.Remove(item);
    }

    public void AddItem()
    {
        foreach (var item in itemsInRange) {
            string itemName = item.ItemName;
            int amount = item.Amount;

            switch (itemName) {
                case "Stone":
                    stoneNumber += amount;
                    counterArray[0].text = stoneNumber.ToString();
                    break;
                case "Wood":
                    woodNumber += amount;
                    counterArray[1].text = woodNumber.ToString();
                    break;

                case "Scrap":
                    scrapNumber += amount;
                    counterArray[2].text = scrapNumber.ToString();
                    break;

                case "MedKit":
                    medKitNumber += amount;
                    counterArray[3].text = medKitNumber.ToString();
                    break;

                case "RevolverAmmo":
                    AddRevolverAmmo(amount);
                    break;
                
                case "ShotgunAmmo":
                    AddShotgunAmmo(amount);
                    break;
                    
                case "MGAmmo":
                    AddMachinegunAmmo(amount);
                    break;

                default:
                    Debug.LogWarning("Something went wrong...");
                    break;
            }
        }
        itemsInRange.Clear(); 
    }

 public bool HasEnoughResources(Dictionary<string, int> costs)
    {
        foreach (var cost in costs)
        {
            switch (cost.Key)
            {
                case "exp":
                    if (GameManager.Instance.upgradePoints < cost.Value) return false;
                    break;
                case "swampcoin":
                    if (GameManager.Instance.swampCoins < cost.Value) return false;
                    break;
                case "scrap":
                    if (scrapNumber < cost.Value) return false;
                    break;
                default:
                    return false;
            }
        }
        return true;
    }

    public void SpendResources(Dictionary<string, int> costs)
    {
        foreach (var cost in costs)
        {
            switch (cost.Key)
            {
                case "exp":
                    GameManager.Instance.upgradePoints -= cost.Value;
                    break;
                case "swampcoin":
                    GameManager.Instance.swampCoins -= cost.Value;
                    break;
                case "scrap":
                    scrapNumber -= cost.Value;
                    break;
            }
        }
    }

    public int GetResourceAmount(string resource)
    {
        return resource switch
        {
            "exp" => GameManager.Instance.upgradePoints,
            "swampcoin" => GameManager.Instance.swampCoins,
            "scrap" => scrapNumber,
            _ => 0
        };
    }


    // Methods for using Inventory

    private void UseMedkit() {
        if (medKitNumber > 0) {
            playerHealth.AddHealth(medKitHP); // Synchronizacja zdrowia
            medKitNumber--;
            counterArray[3].text = medKitNumber.ToString(); // Aktualizacja UI
        }
    }

    public void UpdateRevolverAmmoText(int amount) {
        revolverDmgText.text = (int.Parse(revolverDmgText.text) + amount).ToString();
    }

    public void UpdateRevolverRange(int range) {
        counterArray[4].text = (int.Parse(counterArray[4].text) + range).ToString();
    }
    public void UpdateShotgunAmmoText(int amount) {
        shotgunDmgText.text = (int.Parse(shotgunDmgText.text) + amount).ToString();
    }
    public void UpdateShotgunRange(int range) {
        counterArray[5].text = (int.Parse(counterArray[5].text) + range).ToString();
    }
    public void UpdateMachinegunAmmoText(int amount) {
        mgDmgText.text = (int.Parse(mgDmgText.text) + amount).ToString();
    }
    public void UpdateMachinegunRange(int range) {
        counterArray[6].text = (int.Parse(counterArray[6].text) + range).ToString();
    }
    private void AddRevolverAmmo(int amount) {
        revolver.AddAmmo(amount);
        UpdateRevolverAmmoText(amount);
    }
    private void AddShotgunAmmo(int amount) {
        shotgun.AddAmmo(amount);
        UpdateShotgunAmmoText(amount);
    }
    private void AddMachinegunAmmo(int amount) {
        mg.AddAmmo(amount);
        UpdateMachinegunAmmoText(amount);
    }

    public void BuyRevolverAmmo(int amount) {
        revolverTemplate.currentAmmo += amount; 
        AddRevolverAmmo(amount);
    }

    public void BuyShotgunAmmo(int amount) {
        shotgunTemplate.currentAmmo += amount;
        AddShotgunAmmo(amount);
    }

    public void BuyMaschinegunAmmo(int amount) {
        mgTemplate.currentAmmo += amount;
        AddMachinegunAmmo(amount);
    }

}




