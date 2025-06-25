using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int totalEnemiesDefeated { get; private set; }
    public int swampCoins { get; set; } = 1000;
    public int upgradePoints { get; set; } = 1000;

    [Header("UI")]
    [SerializeField] private TMP_Text killedEnemyCounter;
    [SerializeField] private TMP_Text upgradePointCounter;
    [SerializeField] private TMP_Text swampCoinsCounter;
    [SerializeField] private TMP_Text currentRoundText;

    [Header("Reward Settings")]
    [SerializeField] private int swampCoinsMin = 5;
    [SerializeField] private int swampCoinsMax = 10; 
    [SerializeField] private int expPointsMin = 10; 
    [SerializeField] private int expPointsMax = 20; 

    [Header("Targets")]
    [SerializeField] public GameObject mainTarget;
    [SerializeField] public Transform playerTarget;

    [Header("Enemy Spanwer Settings")]
    [SerializeField] private GameObject[] enemiesPrefab; 
    [SerializeField] private int maxEnemiesPerRound = 40; // It's basic number 
    [SerializeField] private float spawnInterval = 0.001f; 
    [SerializeField] private Transform[] spawnPoints;
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    [Header("Resource Spawner Settings")]
    [SerializeField] private GameObject[] stonePrefabs; 
    [SerializeField] private GameObject woodPrefab; 
    [SerializeField] private GameObject scrapPrefab; 
    [SerializeField] private int maxResourcesPerMoment = 10; 
    [SerializeField] public int spawnedResources = 0;
    [SerializeField] private float resourceSpawnInterval = 3.0f;
    [SerializeField] private Transform[] resourceSpawnPoints; 




    // Game controllr 
    [Header("Round Settings")]
    [SerializeField] private int currentRound = 1;
    [SerializeField] private float roundTransitionDelay = 5.0f;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentRoundText.text = currentRound.ToString();
        StartCoroutine(SpawnZombies());
        StartCoroutine(SpawnResources());
    }

    // Spawning enemies

    private IEnumerator SpawnZombies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (spawnedEnemies.Count < maxEnemiesPerRound)
            {
                SpawnZombie();
            }
        }
    }

    private void SpawnZombie()
    {
        if (spawnPoints.Length == 0 || enemiesPrefab.Length == 0) return;

        if (spawnedEnemies.Count >= maxEnemiesPerRound) return;

        Transform spawnPoint1 = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject selectedZombie1 = enemiesPrefab[Random.Range(0, enemiesPrefab.Length)];
        GameObject selectedZombie2 = enemiesPrefab[Random.Range(0, enemiesPrefab.Length)];
        GameObject selectedZombie3 = enemiesPrefab[Random.Range(0, enemiesPrefab.Length)];
        GameObject selectedZombie4 = enemiesPrefab[Random.Range(0, enemiesPrefab.Length)];

        GameObject newZombie1 = Instantiate(selectedZombie1, spawnPoint1.position, Quaternion.identity);
        GameObject newZombie2 = Instantiate(selectedZombie2, spawnPoint1.position, Quaternion.identity);
        GameObject newZombie3 = Instantiate(selectedZombie3, spawnPoint1.position, Quaternion.identity);
        GameObject newZombie4 = Instantiate(selectedZombie4, spawnPoint1.position, Quaternion.identity);

        spawnedEnemies.Add(newZombie1); // Removing is in EnemyController 
        spawnedEnemies.Add(newZombie2); 
        spawnedEnemies.Add(newZombie3); 
        spawnedEnemies.Add(newZombie4); 
    }

    // spawning resources

    private IEnumerator SpawnResources() { 
        while (true) {
            yield return new WaitForSeconds(resourceSpawnInterval);
            if (spawnedResources < maxResourcesPerMoment) {
                SpawnResource();
            }
        }  
    }

    private void SpawnResource() {
        Transform spawnPoint1 = resourceSpawnPoints[Random.Range(0, resourceSpawnPoints.Length)];
        int ind = Random.Range(0, 3); // 0, 1, 2
        switch (ind) {
            case 0:
                Instantiate(stonePrefabs[Random.Range(0, stonePrefabs.Length)], spawnPoint1.position, Quaternion.identity);
                break;
            case 1:
                Instantiate(woodPrefab, spawnPoint1.position, Quaternion.identity);
                break;
            case 2:
                Instantiate(scrapPrefab, spawnPoint1.position, Quaternion.identity);
                break;
        }
    }

    // Killing enemies

    public void EnemyDefeated()
    {
        totalEnemiesDefeated++;

        int coinsReward = GenerateSwampCoinsReward();
        int expReward = GenerateExpPointsReward();

        swampCoins += coinsReward;
        upgradePoints += expReward;

        if (killedEnemyCounter != null)
            killedEnemyCounter.text = totalEnemiesDefeated.ToString();
        if (swampCoinsCounter != null)
            swampCoinsCounter.text = swampCoins.ToString();
        if (upgradePointCounter != null)
            upgradePointCounter.text = upgradePoints.ToString();

        if (spawnedEnemies.Count == 0) {
            StartCoroutine(NextRound());
        }
    }


    private IEnumerator NextRound()
    {
        yield return new WaitForSeconds(roundTransitionDelay); // Oczekiwanie przed nową rundą

        currentRound++;
        currentRoundText.text = currentRound.ToString();

        switch (currentRound)
        {
            case 2:
                maxEnemiesPerRound = 72;
                break;
            case 3:
                maxEnemiesPerRound = 84;
                break;
            case 4:
                maxEnemiesPerRound = 100;
                break;
            case 5:
                maxEnemiesPerRound = 136;
                break;
            case 6:
                maxEnemiesPerRound = 164;
                break;
            case 7:
                maxEnemiesPerRound = 192;
                break;
            case 8:
                maxEnemiesPerRound = 212;
                break;
            case 9:
                maxEnemiesPerRound = 236;
                break;
            case 10:
                maxEnemiesPerRound = 300;
                break;
            case 11:
                Debug.Log("Koniec gry!"); // TO DO
                yield break;
        }

        StartCoroutine(SpawnZombies()); 
    }

    private int GenerateSwampCoinsReward()
    {
        return Random.Range(swampCoinsMin, swampCoinsMax + 1);
    }

    private int GenerateExpPointsReward()
    {
        return Random.Range(expPointsMin, expPointsMax + 1);
    }
}
