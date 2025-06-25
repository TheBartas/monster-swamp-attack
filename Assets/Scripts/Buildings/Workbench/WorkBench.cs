using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class WorkBench : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private GameObject workbenchPanel;
    [Header("Gun")] 
    [SerializeField] private GunData revolerData;
    [SerializeField] private GunData shotgunData;
    [SerializeField] private GunData machinegunData;

    [Header("Inventory")]
    [SerializeField] private PlayerInventory playerInventory;

    [Header("Revolver Buttons")]
    [SerializeField] private TMP_Text revolverDmgButtonText;
    [SerializeField] private TMP_Text revolverRangeButtonText;

    [Header("Shotgun Buttons")] 
    [SerializeField] private TMP_Text shotgunDmgButtonText;
    [SerializeField] private TMP_Text shotgunRangeButtonText;

    [Header("Machinegun Buttons")]
    [SerializeField] private TMP_Text machinegunDmgButtonText;
    [SerializeField] private TMP_Text machinegunRangeButtonText;

    private string MAIN_TEXT = "B - Warsztat";
    private bool isOpen = false;
    private bool isTrigger = false;


    // Revolver
    private readonly Dictionary<int, int> damageUpgradesRevolver = new Dictionary<int, int> { // <lvl, dmg>
        { 2, 4 }, { 3, 7 }, { 4, 11 }, { 5, 19 }
    };

    private readonly Dictionary<int, int> rangeUpgradesRevolver = new Dictionary<int, int> { // <lvl, range>
        { 2, 10 }, { 3, 15 }, { 4, 20 }, { 5, 23 }
    };

    // Shotgun
    private readonly Dictionary<int, int> damageUpgradesShotgun = new Dictionary<int, int> { // <lvl, dmg>
        { 2, 10 }, { 3, 13 }, { 4, 17 }, { 5, 25 }
    };

    private readonly Dictionary<int, int> rangeUpgradesShotgun= new Dictionary<int, int> { // <lvl, range>
        { 2, 10 }, { 3, 14 }, { 4, 16 }, { 5, 25 }
    };

    // Machinegun
    private readonly Dictionary<int, int> damageUpgradesMachinegun = new Dictionary<int, int> { // <lvl, dmg>
        { 2, 3 }, { 3, 6 }, { 4, 10 }, { 5, 12 }
    };

    private readonly Dictionary<int, int> rangeUpgradesMachinegun = new Dictionary<int, int> { // <lvl, range>
        { 2, 20 }, { 3, 27 }, { 4, 39 }, { 5, 46 }
    };

    private readonly Dictionary<int, Dictionary<string, int>> costUpgradesRevolver = new Dictionary<int, Dictionary<string, int>> {
        { 2, new Dictionary<string, int> { { "swampcoin", 30 }, { "exp", 16 } } },
        { 3, new Dictionary<string, int> { { "swampcoin", 45 }, { "scrap", 15 } } },
        { 4, new Dictionary<string, int> { { "exp", 56}, { "scrap", 50 } } },
        { 5, new Dictionary<string, int> { { "swampcoin", 100 }, { "exp", 74 }, { "scrap", 79 } } }
    };

    private readonly Dictionary<int, Dictionary<string, int>> costUpgradesShotgun = new Dictionary<int, Dictionary<string, int>> {
        { 2, new Dictionary<string, int> { { "swampcoin", 43 }, { "exp", 26 } } },
        { 3, new Dictionary<string, int> { { "swampcoin", 56 }, { "scrap", 45 } } },
        { 4, new Dictionary<string, int> { { "exp", 51 }, { "scrap", 67 } } },
        { 5, new Dictionary<string, int> { { "swampcoin", 78 }, { "exp", 89 }, { "scrap", 101 } } }
    };

    private readonly Dictionary<int, Dictionary<string, int>> costUpgradesMachinegun = new Dictionary<int, Dictionary<string, int>> {
        { 2, new Dictionary<string, int> { { "swampcoin", 67 }, { "exp", 30 } } },
        { 3, new Dictionary<string, int> { { "swampcoin", 99 }, { "scrap", 56 } } },
        { 4, new Dictionary<string, int> { { "exp", 49 }, { "scrap", 80 } } },
        { 5, new Dictionary<string, int> { { "swampcoin", 180 }, { "exp", 100 }, { "scrap", 120 } } }
    };


    private void Update() {
        if (isTrigger) {
            mainText.text = MAIN_TEXT;
            if (Input.GetKeyDown(KeyCode.B)) {
                ToggleWorkbench();
            }
        } 
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            isTrigger = true;
        }
    }

    private void ToggleWorkbench() {
        isOpen = !isOpen; 

        if (isOpen) {
            workbenchPanel.SetActive(true); 
            RefreshUpgradeButtons();
            Time.timeScale = 0f; 
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            workbenchPanel.SetActive(false); 
            Time.timeScale = 1f; 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void UpgradeRevolverDMG() { // Button (UI) ---> DMG {revolver}
        UpgradeGun("dmg", revolerData, damageUpgradesRevolver, playerInventory.UpdateRevolverAmmoText, costUpgradesRevolver);
        RefreshUpgradeButtons();
    }

    public void UpgradeRevolverRange() { // Button (UI) ---> Range {revolver}
        UpgradeGun("range", revolerData, rangeUpgradesRevolver, playerInventory.UpdateRevolverRange, costUpgradesRevolver);
        RefreshUpgradeButtons();
    }

    public void UpgradeShotgunDMG() { // Button (UI) ---> DMG {shotgun}
       UpgradeGun("dmg", shotgunData, damageUpgradesShotgun, playerInventory.UpdateShotgunAmmoText, costUpgradesShotgun);
       RefreshUpgradeButtons();
    }

    public void UpgradeShotgunRange() { // Button (UI) ---> Range {shotgun}
       UpgradeGun("range", shotgunData, rangeUpgradesShotgun, playerInventory.UpdateShotgunRange, costUpgradesShotgun);
       RefreshUpgradeButtons();
    }

    public void UpgradeMachinegunDMG() { // Button (UI) ---> DMG {revolver}
       UpgradeGun("dmg", machinegunData, damageUpgradesMachinegun, playerInventory.UpdateMachinegunAmmoText, costUpgradesMachinegun);
       RefreshUpgradeButtons();
    }

    public void UpgradeMachinegunRange() { // Button (UI) ---> Range {shotgun}
       UpgradeGun("range", machinegunData, rangeUpgradesMachinegun, playerInventory.UpdateMachinegunRange, costUpgradesMachinegun);
       RefreshUpgradeButtons();
    }

    public void BuyMedKit() {
        playerInventory.medKitNumber += 1;
    }

    public void BuyRevolverAmmo() {
        playerInventory.BuyRevolverAmmo(6);
    }

    public void BuyShotgunAmmo() {
        playerInventory.BuyShotgunAmmo(8);
    }

    public void BuyMaschinegunAmmo() {
        playerInventory.BuyMaschinegunAmmo(100);
    }


    private void UpgradeGun(string upgradeType, GunData gunData, Dictionary<int, int> upgrades, System.Action<int> updateUI, Dictionary<int, Dictionary<string, int>> costs) { 
        switch (upgradeType) {
            case "dmg":
                UpgradeWeaponStat(ref gunData.damageLevel, ref gunData.damage, upgrades, updateUI, costs);
                break;
            case "range":
                UpgradeWeaponStat(ref gunData.rangeLevel, ref gunData.range, upgrades, updateUI, costs);
                break;
        } 
    }

    private void UpgradeWeaponStat(
        ref int level,
        ref int stat,
        Dictionary<int, int> upgrades,
        System.Action<int> updateUI,
        Dictionary<int, Dictionary<string, int>> costs)
    {
        int nextLevel = level + 1;

        if (upgrades.ContainsKey(nextLevel) && costs.ContainsKey(nextLevel))
        {
            int upgradeValue = upgrades[nextLevel];
            Dictionary<string, int> upgradeCosts = costs[nextLevel];

            if (playerInventory.HasEnoughResources(upgradeCosts))
            {
                playerInventory.SpendResources(upgradeCosts);

                level = nextLevel;
                stat += upgradeValue;
                updateUI(upgradeValue);
            }
        }
    }

    private void UpdateUpgradeButtonText(
        TMP_Text buttonText,
        string statName,
        int currentStat,
        int currentLevel,
        Dictionary<int, int> upgrades,
        Dictionary<int, Dictionary<string, int>> costs,
        PlayerInventory inventory)
    {
        int nextLevel = currentLevel + 1;

        if (!upgrades.ContainsKey(nextLevel))
        {
            buttonText.text = $"{statName}: <color=#ff5733>Max level reached! </color>";
            return;
        }

        int upgradeValue = upgrades[nextLevel];
        Dictionary<string, int> upgradeCosts = costs[nextLevel];

        bool canAfford = inventory.HasEnoughResources(upgradeCosts);

        string statLine = $"{statName}: {currentStat} (+{upgradeValue})";
        string costLine = string.Join("\n", upgradeCosts.Select(cost =>
        {
            string color = inventory.GetResourceAmount(cost.Key) >= cost.Value ? "#023020" : "#C70039";
            return $"<color={color}>{cost.Key}: {cost.Value}</color>";
        }));

        buttonText.text = canAfford ? $"{statLine}\n{costLine}" : $"{statName}: {currentStat}\n{costLine}";
    }

    private void RefreshUpgradeButtons()
    {
        // Revolver DMG
        UpdateUpgradeButtonText(
            revolverDmgButtonText,
            "Damage",
            revolerData.damage,
            revolerData.damageLevel,
            damageUpgradesRevolver,
            costUpgradesRevolver,
            playerInventory
        );

        // Revolver Range
        UpdateUpgradeButtonText(
            revolverRangeButtonText,
            "Range",
            revolerData.range,
            revolerData.rangeLevel,
            rangeUpgradesRevolver,
            costUpgradesRevolver,
            playerInventory
        );

        // Shotgun DMG
        UpdateUpgradeButtonText(
            shotgunDmgButtonText,
            "Damage",
            shotgunData.damage,
            shotgunData.damageLevel,
            damageUpgradesShotgun,
            costUpgradesShotgun,
            playerInventory
        );

        // Shotgun Range
        UpdateUpgradeButtonText(
            shotgunRangeButtonText,
            "Range",
            shotgunData.range,
            shotgunData.rangeLevel,
            rangeUpgradesShotgun,
            costUpgradesShotgun,
            playerInventory
        );

        // Machinegun DMG
        UpdateUpgradeButtonText(
            machinegunDmgButtonText,
            "Damage",
            machinegunData.damage,
            machinegunData.damageLevel,
            damageUpgradesMachinegun,
            costUpgradesMachinegun,
            playerInventory
        );

        // Machinegun Range
        UpdateUpgradeButtonText(
            machinegunRangeButtonText,
            "Range",
            machinegunData.range,
            machinegunData.rangeLevel,
            rangeUpgradesMachinegun,
            costUpgradesMachinegun,
            playerInventory
        );
    }


}
