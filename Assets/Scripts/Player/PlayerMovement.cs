using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float sneakSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    bool readyToJump;
    bool isSneaking;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask isGround;
    bool grounded;

    [Header("Sprint control")]
    public float sprintSpeed;
    public float maxEnergy = 10;
    public float energyRegenRate = 0.5f; 
    public float energyDrainRate = 1.0f; 

    float energyThreshold = 0.01f;
    float currentEnergy;
    bool isSprinting;
    bool isRechargingEnergy; 

    [Header("UI Elements")]
    public Image energySliderFL;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        currentEnergy = maxEnergy;
    }

    private void FixedUpdate() {
        MovePlayer();
    }

    private void Update() {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, isGround);

        PlayerInput();
        SpeedControl();

        rb.drag = grounded ? groundDrag : 0;
    }

    private void PlayerInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Sprint();
        Sneak();
        Jump();
    }

    private void MovePlayer() {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        
        // float appliedSpeed = (isSprinting && currentEnergy > 0.01) ? sprintSpeed : moveSpeed;

        float appliedSpeed = moveSpeed;

        if (isSprinting && currentEnergy > energyThreshold) {
            appliedSpeed = sprintSpeed;
        } else if (isSneaking) {
            appliedSpeed = sneakSpeed;
        }

        if (grounded)
            rb.AddForce(moveDirection.normalized * appliedSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * appliedSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl() {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed) {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Sprint() {
        if (Input.GetKey(KeyCode.LeftShift) && currentEnergy > 0 && grounded) {
            isSprinting = true;
            currentEnergy -= energyDrainRate * Time.deltaTime;
            currentEnergy = Mathf.Max(0, currentEnergy);
            energySliderFL.fillAmount = currentEnergy / maxEnergy;

            if (isRechargingEnergy) {
                StopCoroutine(RechargeEnergy());
                isRechargingEnergy = false;
            }

        } else {
            isSprinting = false;

            
            if (currentEnergy < maxEnergy && !isRechargingEnergy) {
                StartCoroutine(RechargeEnergy());
            }
        }
    }

    private void Jump() {
        if (Input.GetKey(jumpKey) && readyToJump && grounded) {
            readyToJump = false;
            CalculateJump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }
    }

    private void CalculateJump() {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() {
        readyToJump = true;
    }

    private IEnumerator RechargeEnergy() {
        yield return new WaitForSeconds(2f); 

        isRechargingEnergy = true;
        
        while (currentEnergy < maxEnergy) {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            energySliderFL.fillAmount = currentEnergy / maxEnergy;

            yield return null;

            if (isSprinting) {
                isRechargingEnergy = false;
                yield break; 
            }
        }
        
        isRechargingEnergy = false; 
    }

    private void Sneak() {
        if (Input.GetKey(KeyCode.C) && grounded && !isSprinting) {
            isSneaking = true;
        } else {
            isSneaking = false;
        }
    }
}
