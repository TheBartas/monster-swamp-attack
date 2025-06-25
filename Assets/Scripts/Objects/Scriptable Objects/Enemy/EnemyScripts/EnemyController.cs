using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IDamageable
{
    private NavMeshAgent agent = null;
    
    [Header("References")]
    [SerializeField] private EnemyData templateData;
    private EnemyData enemyData;
    private Transform mainTarget; // House
    private Transform currentTarget = null;
    private Collider houseCollider; 

    [Header("Detection")]
    [SerializeField] private LayerMask detectionLayers; // Warstwy do detekcji
    [SerializeField] private LayerMask obstacleLayers;  // Warstwy przeszkód 
    [SerializeField] private float detectionRadius; 
    private bool isPlayerInRange = false;
    private bool isWater;

    [Header("Features")]
    [SerializeField] private float stoppingDistance;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 2.0f; 
    private float lastAttackTime; // Ostatni czas ataku
    private float damage = 0.0f;
    private bool canAttack = false;
    private bool isAttackingHouse = false;
    private bool isAttackingBarrier = false;
    private bool isDead = false;
    private Animator animator;
    private float delayAfterDead = 3.0f;
    private EnemyHealthBar enemyHealthBar; // for UI


    private void Awake()
    {
        enemyData = Instantiate(templateData);
        enemyHealthBar = GetComponent<EnemyHealthBar>();
        isWater = false;
    }

    private void Start() {
        GetReferences();
        currentTarget = mainTarget; 
        damage = templateData.dmg;
        agent.speed = enemyData.speed;
    }

    private void Update() {

        if (isDead) {
            animator.SetTrigger("triggerDead");
            agent.isStopped = true;
            return;
        }

        if (isAttackingBarrier) 
            detectionRadius = 2.0f;
        else 
            detectionRadius = 30.0f;
 
        WhatIsGround();
        DetectTargets();
        

        if (IsObstacleInFront()) {
            HandleObstacle();
        } else {
            MoveToTarget();
        }

        RotateTowardsTarget(currentTarget);
    }

    private void GetReferences() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mainTarget = GameManager.Instance.mainTarget.transform;
        houseCollider = GameManager.Instance.mainTarget.GetComponent<Collider>();
    }

    private void WhatIsGround() {
        if (isWater == true) {
            agent.speed = enemyData.waterSpeed;
        } else {
            agent.speed = enemyData.speed;
        }
    }

    // Dead or Not

    public void Damage(float damage) {
        enemyData.health -= damage; 
        enemyHealthBar.UpdateEnemyHealthBar(templateData.health, enemyData.health);
        if (enemyData.health <= 0) {
            isDead = true;
            enemyHealthBar.KillBar();
            StartCoroutine(DestroyAfterDelay(delayAfterDead));
        }
    }

    private IEnumerator DestroyAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        GameManager.Instance.EnemyDefeated();
        GameManager.Instance.spawnedEnemies.Remove(gameObject);
    }


    private void HandleObstacle() {
        if (currentTarget.CompareTag("Barrier")) {
            RotateTowardsTarget(currentTarget);
            agent.isStopped = true; 
        }
    }

    private void MoveToTarget() {
        if (currentTarget != null) {
            agent.isStopped = false;
            agent.SetDestination(currentTarget.position);

            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (houseCollider.bounds.Contains(transform.position)) {
                agent.isStopped = true; 
                isAttackingHouse = true;
            }

            if (distanceToTarget <= stoppingDistance) {
                RotateTowardsTarget(currentTarget);
            }
        }
    }

    private void RotateTowardsTarget(Transform target)
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Wyeliminowanie komponentu Y, aby obracać tylko w płaszczyźnie poziomej
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); 
    }

    private void DetectTargets()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayers);

        Transform playerTarget = null;
        Transform obstacleTarget = null;

        if (isAttackingHouse == false) {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player") && IsPlayerVisible(hitCollider.transform))
                {
                    playerTarget = hitCollider.transform;
                    break; 
                }
                else if (hitCollider.CompareTag("Barrier"))
                {
                    obstacleTarget = hitCollider.transform;
                }
            }
        }

        if (playerTarget != null) {
            currentTarget = playerTarget;
            isPlayerInRange = true;
            isAttackingBarrier = false; 

        } else if (obstacleTarget != null && !isPlayerInRange) {
            currentTarget = obstacleTarget;

        } else {
            currentTarget = mainTarget; 
            isPlayerInRange = false;
        }
    }


    private bool IsPlayerVisible(Transform player) {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distanceToPlayer, obstacleLayers)) {
            if (hit.transform.CompareTag("Player")) {
                return true;
            } else {
                return false;
            }
        }

        return true;
    }

    private bool IsObstacleInFront() {
        if (isPlayerInRange) {
            return false; // Ignoruj przeszkody, jeśli gracz jest widoczny
        }
        float distanceToObstacle = 1.5f; 
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distanceToObstacle, obstacleLayers)) {
            if (hit.transform.CompareTag("Barrier")) {
                currentTarget = hit.transform;
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("isWater")) {
            isWater = true;
            //agent.baseOffset = -1f; 
        } 
    } 

    // Attack 
    
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown) {
            canAttack = true;
            RunAnimation();
            StartCoroutine(AttackPlayer(other.transform));

        } else if (other.CompareTag("House") && Time.time >= lastAttackTime + attackCooldown) {
            canAttack = true;
            if (agent.isStopped) {
                RunAnimation();
            }
            StartCoroutine(AttackMainTarget(other));

        } else if (other.CompareTag("Barrier") && !isPlayerInRange && Time.time >= lastAttackTime + attackCooldown) {
            // Debug.Log("Something is wrong!");
            currentTarget = other.transform; // W razie, gdyby "zablkowował się" na jednej przeszkodzie, a target ustawił na main.
            RotateTowardsTarget(currentTarget);
            isAttackingBarrier = true;
            agent.isStopped = true;
            canAttack = true;
            RunAnimation();
            StartCoroutine(AttackBarrier(other));

        } else if (other.CompareTag("Dead")) {
            isAttackingBarrier = false;
            Debug.Log("Test: " + isAttackingBarrier);
            canAttack = false;
            StopAnimation();
        }
    }

    private void RunAnimation() {
        animator.SetBool("isAttacking", true);
        animator.SetBool("Attack", true);
        animator.SetBool("endAttack", false);
    }

    private void StopAnimation() {
        animator.SetBool("isAttacking", false);
        animator.SetBool("endAttack", true);
        animator.SetBool("Attack", false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Barrier")) {
            canAttack = false;
            StopAnimation();
            if (other.CompareTag("Barrier")) {
                isAttackingBarrier = false;
                agent.isStopped = false;
            }
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("isWater")) {
            isWater = false;
        } 
    }

    private IEnumerator AttackPlayer(Transform player)
    {
        while (canAttack) {
            float animTime = 0.2f;
            yield return new WaitForSeconds(animTime);
            canAttack = false;
            lastAttackTime = Time.time;
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    private IEnumerator AttackMainTarget(Collider other) {
        while (canAttack) {

            if (other == null) {
                yield break;
            }
            lastAttackTime = Time.time;
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    private IEnumerator AttackBarrier(Collider other) 
    {
        while (canAttack) {
            if (other == null) {
                currentTarget = mainTarget;
                agent.isStopped = false;
                canAttack = false;
                yield break;
            }
            lastAttackTime = Time.time;
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    // This is for AnimationEvent 
    public void OnAttackHit() { 
        if (currentTarget != null && currentTarget.CompareTag("Player")) {
            PlayerHealth playerHealth = currentTarget.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                playerHealth.TakeDamage(damage);  
            }
        } else if (currentTarget != null && currentTarget.CompareTag("Barrier")) {
            IDamageable damageable = currentTarget.GetComponent<IDamageable>();
            damageable.Damage(damage);
        } else if (currentTarget != null && currentTarget.CompareTag("House")) {
            IDamageable damageable = currentTarget.GetComponent<IDamageable>();
            damageable.Damage(damage);
        }
    }
}


