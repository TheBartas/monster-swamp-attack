using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ZombieGhoulController : MonoBehaviour, IDamageable
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
    private bool isDead = false;
    private float delayAfterDead = 0.9f;
    private bool isAnimationPlaying = false;
    private bool isPlayerInTrigger = false;
    private float timeInTrigger = 0.0f;
    private float attackThreshold = 0.5f; // Minimalny czas, po którym enemy zada dmg graczowi, jeśli ten zostanie w triggerze
    [Header("Attack")]
    [SerializeField] private AnimationClip[] animations = null; 
    private Animation animation;
    private int currentAnimIndex = 1;
    private bool isRunning = false;
    private EnemyHealthBar enemyHealthBar; // for UI

    private void Awake()
    {
        enemyData = Instantiate(templateData);
        isWater = false;
        enemyHealthBar = GetComponent<EnemyHealthBar>();
    }

    private void Start() {
        GetReferences();
        currentTarget = mainTarget; 
        damage = templateData.dmg;
        agent.speed = enemyData.speed;
    }

    private void Update() {
        if (isDead)
        {
            agent.isStopped = true;
            PlayAnimation(5);
            return;
        }

        if (isAnimationPlaying) return;

        if (isPlayerInTrigger) {
            if (canAttack) {
                RunAnimation();
            }
        }
        else {
            Charge();
            NeedForSpeed();
            DetectTargets();

            if (IsObstacleInFront()) {
                HandleObstacle();

            } else {
                MoveToTarget();
            }

            RotateTowardsTarget(currentTarget);
        }
    }

    private void GetReferences() {
        agent = GetComponent<NavMeshAgent>();
        animation = GetComponent<Animation>();
        mainTarget = GameManager.Instance.mainTarget.transform;
        houseCollider = GameManager.Instance.mainTarget.GetComponent<Collider>();
    }

    private void NeedForSpeed() {
        if (isWater == true) {
            agent.speed = enemyData.waterSpeed;
        } else {
            agent.speed = enemyData.speed;
        }
    }

    private void PlayAnimation(int index)
    {
        if (index >= 0 && index < animations.Length)
        {
            animation.Play(animations[index].name);
        }
    }

    private void Charge() {
        if (isRunning) {
            PlayAnimation(2);
        } else {
            PlayAnimation(1);
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
            GameManager.Instance.spawnedEnemies.Remove(gameObject);
        }
    }

    private IEnumerator DestroyAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
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
            }

            if (distanceToTarget <= stoppingDistance) {
                RotateTowardsTarget(currentTarget);
            }
        }
    }

    private void StopEnemy() {
        if (houseCollider.bounds.Contains(transform.position)) {
        agent.isStopped = true;  
        RotateToTarget();
        } else {
            agent.isStopped = false;  
        }
    }

    private void RotateToTarget() {
        Vector3 direction = currentTarget.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = rotation;
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
            isRunning = true;
            agent.speed = enemyData.charge;

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
        float distanceToObstacle = 1.5f; 
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distanceToObstacle, obstacleLayers)) {
            if (hit.transform.CompareTag("Barrier")) {
                currentTarget = hit.transform;
                return true;
            }
        }
        return false;
    }


    // ------------------------------------------------------------------------------
    private void OnDrawGizmosSelected() { // pomocniczo, tylko do rysowania
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Wizualizacja strefy wokół domu (Capsule Collider)
        if (houseCollider != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(houseCollider.bounds.center, houseCollider.bounds.size);
        }

        // Debugowanie Raycast (do gracza)
        if (currentTarget != null && currentTarget.CompareTag("Player")) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            timeInTrigger += Time.deltaTime;
        } else if (other.CompareTag("House")) {
            isAttackingHouse = true;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("isWater")) {
            isWater = true;
        } 
    } 

    // Attack 
    
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown) {
            isPlayerInTrigger = true;
            timeInTrigger += Time.deltaTime; 
            if (Time.time >= lastAttackTime + attackCooldown && timeInTrigger >= attackThreshold)
            {
                canAttack = true;
                RunAnimation();
            }

        } else if (other.CompareTag("House") && Time.time >= lastAttackTime + attackCooldown) {
            canAttack = true;
            if (agent.isStopped) RunAnimation();

        } else if (other.CompareTag("Barrier") && Time.time >= lastAttackTime + attackCooldown) {
            currentTarget = other.transform; // W razie, gdyby "zablkowował się" na jednej przeszkodzie, a target ustawił na main.
            RotateTowardsTarget(currentTarget);
            agent.isStopped = true;
            canAttack = true;
            if (agent.isStopped) RunAnimation();

        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            isPlayerInTrigger = false;
            canAttack = false;
            timeInTrigger = 0.0f; 
        } 
        if (other.gameObject.layer == LayerMask.NameToLayer("isWater")) {
            isWater = false;
        } 
    }
    // This is for AnimationEvent 
    public void OnAttackHit() { 
        if (currentTarget != null && currentTarget.CompareTag("Player")) {
            PlayerHealth playerHealth = currentTarget.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damage); 
        } else if (currentTarget != null && currentTarget.CompareTag("Barrier")) {
            IDamageable damageable = currentTarget.GetComponent<IDamageable>();
            damageable.Damage(damage);
        } else if (currentTarget != null && currentTarget.CompareTag("House")) {
            IDamageable damageable = currentTarget.GetComponent<IDamageable>();
            damageable.Damage(damage);
        } 
    }

    // Animacje
    private void RunAnimation() {
        if (!isAnimationPlaying)
        {
            isAnimationPlaying = true;
            currentAnimIndex = isAttackingHouse ? 3 : 4;
            animation.Play(animations[currentAnimIndex].name);

            StartCoroutine(ResetAnimationFlag(animations[currentAnimIndex].length));
        }
    }

    private IEnumerator ResetAnimationFlag(float animDuration)
    {
        yield return new WaitForSeconds(animDuration);

        isAnimationPlaying = false;
        if (!isPlayerInTrigger)
        {
            currentAnimIndex = 1;
            PlayAnimation(currentAnimIndex);
        }
    }
    private void StopAnimation() {
        currentAnimIndex = 1;
    }
}
