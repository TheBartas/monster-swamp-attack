using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public abstract class CollectableItem : MonoBehaviour
{
    [Header("Features")]
    [SerializeField] protected string itemName = string.Empty;
    [SerializeField] protected int amount;

    public string ItemName { get { return itemName; } }
    public int Amount { get { return amount;}}
    private TMP_Text uiText;
    
    private bool isPlayerNearby = false;

    private PlayerInventory playerInventory;
    
    protected virtual void Start()
    {
        if (uiText != null) {
            uiText.text = string.Empty;
        }

        uiText = GameObject.Find("CollectableItem")?.GetComponent<TMP_Text>();

        if (playerInventory == null) {
            GameObject player = GameObject.FindWithTag("Player"); 
            if (player != null) {
                playerInventory = player.GetComponent<PlayerInventory>();
            } 
        }
    }

    
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyUp(KeyCode.X)) {
            Collect();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerNearby = true;
            if (uiText != null) {
                uiText.text = $"<color=#F3EBE9>X - Zbierz {itemName}</color>";
                Debug.Log(uiText.text);
            }
            playerInventory.AddToList(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerNearby = false;
            if (uiText != null) {
                uiText.text = string.Empty;
            }
            playerInventory.RemoveFromList(this);
        }
    }

    public virtual void Collect() {
        if (uiText != null) {
            uiText.text = string.Empty;
            playerInventory.AddItem();
            GameManager.Instance.spawnedResources--;
        }
        Destroy(gameObject); 
    }

}
