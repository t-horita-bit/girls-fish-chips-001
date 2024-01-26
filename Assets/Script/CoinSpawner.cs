using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject positiveCoinPrefab, trapCoinPrefab;

    [SerializeField]
    private float minYPos = -1f, maxYPos = -2f;

    [SerializeField]
    private float minSpawnWaitTime = 2f, maxSpawnWaitTime = 3.5f;

    private float spawnWaitTime;
    private Camera mainCamera;
    private Vector3 coinSpawnPos = Vector3.zero;

    [SerializeField]
    private List<GameObject> positiveCoinPool, trapCoinPool;

    [SerializeField]
    private int initialCoinsToSpawn = 5;

    [SerializeField]
    private GameObject lifeRecoveryPrefab;

    [SerializeField]
    private float lifeRecoveryYPos = 3f;

    [SerializeField]
    public List<GameObject> lifeRecoveryPool;


    private void Awake()
    {
        mainCamera = Camera.main;
        GenerateCoins();
    }

    void Update()
    {
        CoinSpawning();
    }

    void GenerateCoins()
    {
        for (int i = 0; i < initialCoinsToSpawn; i++)
        {
            GameObject positiveCoin = Instantiate(positiveCoinPrefab);
            positiveCoin.transform.SetParent(transform);
            positiveCoinPool.Add(positiveCoin);
            positiveCoin.SetActive(false);

            GameObject trapCoin = Instantiate(trapCoinPrefab);
            trapCoin.transform.SetParent(transform);
            trapCoinPool.Add(trapCoin);
            trapCoin.SetActive(false);

            GameObject lifeRecoveryItem = Instantiate(lifeRecoveryPrefab);
            lifeRecoveryItem.transform.SetParent(transform);
            lifeRecoveryPool.Add(lifeRecoveryItem);
            lifeRecoveryItem.SetActive(false);

        }
    }

    void CoinSpawning()
    {
        if (Time.time > spawnWaitTime)
        {
            SpawnCoinInGame();
            // プレイヤーのライフが最大でない場合のみライフ回復アイテムを生成する
            if (GameManager.Instance.GetCurrentLives() < GameManager.Instance.GetMaxLives() &&
                UnityEngine.Random.value < 0.1f) // 10%の確率でライフ回復のアイテムをスポーン
            {
                SpawnLifeRecoveryItem();
            }
            spawnWaitTime = Time.time + UnityEngine.Random.Range(minSpawnWaitTime, maxSpawnWaitTime);
        }
    }



    void SpawnCoinInGame()
    {
        coinSpawnPos.x = mainCamera.transform.position.x + 20f;
        coinSpawnPos.y = (UnityEngine.Random.value > 0.5) ? minYPos : maxYPos;
        bool spawnTrap = UnityEngine.Random.value <= 0.25f;

        List<GameObject> poolToUse = spawnTrap ? trapCoinPool : positiveCoinPool;

        foreach (GameObject coin in poolToUse)
        {
            if (!coin.activeInHierarchy)
            {
                coin.transform.position = coinSpawnPos;
                coin.SetActive(true);
                break;
            }
        }
    }

    void SpawnLifeRecoveryItem()
    {
        Vector3 lifeRecoverySpawnPos = coinSpawnPos;
        lifeRecoverySpawnPos.y = lifeRecoveryYPos;  // 修正: y位置を固定値に設定

        foreach (GameObject item in lifeRecoveryPool)
        {
            if (!item.activeInHierarchy)
            {
                item.transform.position = lifeRecoverySpawnPos;
                item.SetActive(true);
                break;
            }
        }
    }

}
