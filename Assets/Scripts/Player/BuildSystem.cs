using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class BuildSystem : MonoBehaviour
{
    [Header("Obstacles")]
    [SerializeField] private GameObject objectToPlace;
    [SerializeField] private GameObject tempObject;
    [SerializeField] private GameObject[] wireFence; 
    [SerializeField] private GameObject[] tempWireFence;

    [Header("References")]
    [SerializeField] private TMP_Text panel;
    [SerializeField] private TMP_Text costsText;
    private PlayerInventory playerInventory;

    private Vector3 place;
    private RaycastHit _Hit;
    public bool placeNow;
    public bool placeWireFence;
    public bool tempObjectExists;


    // Obrót 
    public bool rotateLeft, rotateRight;


    // ---- 


    // Zasięg 
    public int range;
    public float distance;
    public bool objectIsInRange;
    public GameObject player;
    // ----


    private int index;
    private bool canChose = false;
    private bool canDestroy = false;

    public bool CanChose { get { return canChose; } }

    private PlayerShoot playerShoot;


    // Costs 
    private int woodenBarbedWireBarrier_Wood = 25;
    private int woodenBarbedWireBarrier_Scrap = 40;

    private int barbedWireBarrier_Wood = 10;
    private int barbedWireBarrier_Scrap = 30;

    private int concreteBarrier_Stone = 45;

    private int woodenPlankBarrier_Wood = 20;

    private int woodenBarrier_Wood = 50;

    private void Start() {
        index = 0;
        playerInventory = GetComponent<PlayerInventory>();
        playerShoot = GetComponent<PlayerShoot>();
        costsText.text = string.Empty;

    }

    private void Update() {

        if (canChose) {
            ChoseObject();
            ShowCosts();
        }

        if (placeNow == true) {
            SendRay();

        } if (placeWireFence == true) {
            objectToPlace = wireFence[index];

        } if (Input.GetKeyDown("e")) {
            canChose = true;
            playerShoot.SetCanShoot(false); // Blokuj strzelanie
            PlaceWireFence();
        }


        // Obrót
        if (Input.GetKeyDown("q")) {
            rotateLeft = true;
        } if (Input.GetKeyUp("q")) {
            rotateLeft = false;
        }

        if (Input.GetKeyDown("f")) {
            rotateRight = true;
        } if (Input.GetKeyUp("f")) {
            rotateRight = false;
        }

        if (rotateLeft == true) {
            RotateLeft();

        } if (rotateRight == true) {
            RotateRight();
        }
        // ----



        // Zasięg 

        if (distance < range) {
            objectIsInRange = true;

        } else {
            objectIsInRange = false;

            placeNow = false;
            placeWireFence = false;
            Destroy(tempObject);
            tempObjectExists = false;

            distance = 0;
        }

        // ----
    }

    private void ChoseObject() {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput > 0f && index < 4) {
            index += 1;
            canDestroy = true;
        } else if (scrollInput < 0f && index > 0) {
            index -= 1;
            canDestroy = true;
        } 

        if (canDestroy == true) {
            Destroy(tempObject);
            tempObject = Instantiate(tempWireFence[index], place, Quaternion.identity);
            canDestroy = false;
        }
    }

    public void SendRay() {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _Hit)) {
            place = new Vector3(_Hit.point.x, _Hit.point.y, _Hit.point.z);

            distance = Vector3.Distance(place, player.transform.position);

            if (_Hit.transform.tag == "isGroundTag" && objectIsInRange == true) {
                if (tempObjectExists == false) {
                    tempObject = Instantiate(tempWireFence[index], place, Quaternion.identity);
                    tempObjectExists = true;
                }

                if (Input.GetMouseButtonDown(0)) {
                    if (index == 0 && playerInventory.woodNumber >= woodenBarbedWireBarrier_Wood && playerInventory.scrapNumber >= woodenBarbedWireBarrier_Scrap) {
                        PlaceObject();
                        playerInventory.woodNumber -= woodenBarbedWireBarrier_Wood;
                        playerInventory.scrapNumber -= woodenBarbedWireBarrier_Scrap;
                        ResetCostText();

                    } else if (index == 1 && playerInventory.woodNumber >= barbedWireBarrier_Wood && playerInventory.scrapNumber >= barbedWireBarrier_Scrap) {
                        PlaceObject();
                        playerInventory.woodNumber -= barbedWireBarrier_Wood;
                        playerInventory.scrapNumber -= barbedWireBarrier_Scrap;
                        ResetCostText();

                    } else if (index == 2 && playerInventory.stoneNumber >= concreteBarrier_Stone) {
                        PlaceObject();
                        playerInventory.stoneNumber -= concreteBarrier_Stone;
                        ResetCostText();

                    } else if (index == 3 && playerInventory.woodNumber >=  woodenPlankBarrier_Wood) {
                        PlaceObject();
                        playerInventory.woodNumber -= woodenPlankBarrier_Wood;
                        ResetCostText();
                        
                    } else if (index == 4 && playerInventory.woodNumber >= woodenBarrier_Wood) {
                        PlaceObject();
                        playerInventory.woodNumber -= woodenBarrier_Wood;
                        ResetCostText();

                    } else {
                        StartCoroutine(ShowBadInfo());
                    }

                }

                if (tempObject != null) {
                    tempObject.transform.position = place;
                }
            }


            if (Input.GetMouseButtonDown(1)) {
                placeNow = false;
                placeWireFence = false;
                Destroy(tempObject);
                tempObjectExists = false;
                ResetCostText();
            }
        }
    } 


    private void ResetCostText() {
        costsText.text = string.Empty;
        canChose = false;
        Invoke(nameof(EnableShooting), 0.2f); // Odblokuj po 0.2 sekundy
    }

    private void EnableShooting()
    {
        playerShoot.SetCanShoot(true);
    }


    private void ShowCosts() {
        switch (index) {
            case 0: 
                costsText.text = "Wood: " + woodenBarbedWireBarrier_Wood + "\nScrap: " + woodenBarbedWireBarrier_Scrap;
                break;
            case 1:
                costsText.text = "Wood: " + barbedWireBarrier_Wood + "\nScrap: " + barbedWireBarrier_Scrap;
                break;
            case 2:
                costsText.text = "Stone: " + concreteBarrier_Stone;
                break;
            case 3:
                costsText.text = "Wood: " + woodenPlankBarrier_Wood;
                break;
            case 4:
                costsText.text = "Wood: " + woodenBarrier_Wood;
                break;
        }
    }


    private void PlaceObject() {
        Instantiate(objectToPlace, place, /*Quaternion.identity*/ tempObject.transform.rotation);
        placeNow = false;
        placeWireFence = false;

        Destroy(tempObject);
        tempObjectExists = false;
    }

    private IEnumerator ShowBadInfo() {
        panel.text = "Ah, not enough resources...";
        yield return new WaitForSeconds(2.0f);
        panel.text = string.Empty;
    }



    public void PlaceWireFence() {
        placeNow = true;
        placeWireFence = true;
    }


    public void RotateLeft() {
        tempObject.transform.Rotate(0f, -0.5f, 0f, Space.World);
    }

    public void RotateRight() {
        tempObject.transform.Rotate(0f, 0.5f, 0f, Space.World);
    }

}




