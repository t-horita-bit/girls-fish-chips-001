using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject groundPrefab, bambooPrefab;

    [SerializeField]
    private float groundToSpawn = 10, bambooToSpawn = 5;

    private List<GameObject> groundPool = new List<GameObject>();
    private List<GameObject> bambooPool = new List<GameObject>();

    [SerializeField]
    private float ground_Y_Pos = 0f, bamboo_Y_Pos = 0f;

    [SerializeField]
    private float ground_X_Distance = 17.98f, bamboo_X_Distance = 36f;

    private float nextGroundXPos, nextBambooXPos;

    [SerializeField]
    private float generateLevelWaitTime = 11f;

    private float nextSpawnTime;

    [SerializeField]
    private int maxGroundObjects = 20, maxBambooObjects = 10;  // 新しい変数を追加

    void Start()
    {
        GenerateInitialBackground();
        nextSpawnTime = Time.time + generateLevelWaitTime;
    }

    void Update()
    {
        CheckForGroundBamboo();
    }

    void GenerateInitialBackground()
    {
        /*
        Vector3 groundPosition = Vector3.zero;
        for (int i = 0; i < groundToSpawn; i++)
        {
            groundPosition = new Vector3(nextGroundXPos, ground_Y_Pos, 0f);
            GameObject newGround = Instantiate(groundPrefab, groundPosition, Quaternion.identity);
            newGround.transform.SetParent(transform);
            groundPool.Add(newGround);
            nextGroundXPos += ground_X_Distance;
        }*/

        Vector3 bambooPosition = Vector3.zero;
        for (int i = 0; i < bambooToSpawn; i++)
        {
            bambooPosition = new Vector3(nextBambooXPos, bamboo_Y_Pos, 0f);
            GameObject newBamboo = Instantiate(bambooPrefab, bambooPosition, Quaternion.identity);
            newBamboo.transform.SetParent(transform);
            bambooPool.Add(newBamboo);
            nextBambooXPos += bamboo_X_Distance;
        }
    }

    void CheckForGroundBamboo()
    {
        if (Time.time >= nextSpawnTime)
        {
            //SetNewGround();
            SetNewBamboo();
            nextSpawnTime = Time.time + generateLevelWaitTime;
        }
    }

    void SetNewGround()
    {
        Vector3 groundPosition = new Vector3(nextGroundXPos, ground_Y_Pos, 0f);
        GameObject ground = GetInactiveObjectFromPool(groundPool);

        if (!ground && groundPool.Count < maxGroundObjects)  // 利用可能なオブジェクトがなく、最大値を超えていない場合新しいオブジェクトを作成
        {
            ground = Instantiate(groundPrefab);
            ground.transform.SetParent(transform);
            groundPool.Add(ground);
        }

        if (ground)
        {
            ground.transform.position = groundPosition;
            ground.SetActive(true);
            nextGroundXPos += ground_X_Distance;
        }
    }

    void SetNewBamboo()
    {
        Vector3 bambooPosition = new Vector3(nextBambooXPos, bamboo_Y_Pos, 0f);
        GameObject bamboo = GetInactiveObjectFromPool(bambooPool);

        if (!bamboo && bambooPool.Count < maxBambooObjects)  // 同様の変更を竹の部分にも適用
        {
            bamboo = Instantiate(bambooPrefab);
            bamboo.transform.SetParent(transform);
            bambooPool.Add(bamboo);
        }

        if (bamboo)
        {
            bamboo.transform.position = bambooPosition;
            bamboo.SetActive(true);
            nextBambooXPos += bamboo_X_Distance;
        }
    }

    GameObject GetInactiveObjectFromPool(List<GameObject> pool)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }
}
