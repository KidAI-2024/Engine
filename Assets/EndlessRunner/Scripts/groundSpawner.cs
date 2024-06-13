using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundSpawner : MonoBehaviour
{


    public GameObject groundTile;
 

    private Transform playerTransform;

    private float spawnZ = 0.0f;
    private float tileLength = 20f;
    private int tilesCount = 8;

    private float safeZone = 25f;
    private List<GameObject> activeTiles = new List<GameObject>();

    public GameObject[] obstacles;
    private int lastSpawnIndex;
    public GameObject coinPrefab;
    public bool toBeDeleted;
    GameObject spawning()
    {
        GameObject go = Instantiate(groundTile) as GameObject;
        go.transform.SetParent(transform);
        go.transform.position=Vector3.forward*spawnZ;
        spawnZ += tileLength;
        activeTiles.Add(go);
        return go;
    }

    void SpawnCoins(GameObject tile, List<Vector3> obstaclePositions)
    {
        // Number of coins to spawn
        int coinsToSpawn = Random.Range(3,5);

        // Iterate to spawn each coin
        for (int i = 0; i < coinsToSpawn; i++)
        {
            int spawnIndex = Random.Range(0, 3);
            float val = 0;
            if (spawnIndex == 0)
                val = -5.5f;
            else if (spawnIndex == 2)
                val = 5.5f;
            // Generate random position within the tile

            Vector3 coinPosition = new Vector3(val, transform.position.y + 4f, spawnZ - 20 + Random.Range(0, 20));

            // Check if the coin position overlaps with any obstacle position
            bool overlap = false;
            foreach (Vector3 obstaclePos in obstaclePositions)
            {
                if (Vector3.Distance(coinPosition, obstaclePos) < 2f)
                {
                    overlap = true;
                    break;
                }
            }

            // If no overlap, instantiate the coin
            if (!overlap)
            {
                Instantiate(coinPrefab, coinPosition, Quaternion.identity, tile.transform);
            }
        }
    }
        //void obstaclesCreation()
        //{
        //    int index = Random.Range(0, 8);
        //    Vector3 randomPosition = new Vector3();
        //    Instantiate(obstacles[index], randomPosition, Quaternion.identity);
        //}
    void SpawnObstacles(GameObject tile)
    {
        // Calculate the number of obstacles to spawn for the current tile
        int obstaclesToSpawn = Random.Range(1, 2);
        List<Vector3> obstaclePositions = new List<Vector3>();

        // Spawn obstacles at random positions within the tile
        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            // Choose a random position index along the Z-axis
            int spawnIndex = Random.Range(0, 3);
            if (spawnIndex == lastSpawnIndex)
            {
                // Ensure that obstacles are not spawned in the same position consecutively
                spawnIndex = (spawnIndex + 2) % 3;
            }
            float val = 0;
            if (spawnIndex == 0)
                val = -5.5f;
            else if (spawnIndex == 2)
                val = 5.5f;
            int distance = Random.Range(0, 20);
            // Calculate the position to spawn the obstacle
            Vector3 spawnPosition = new Vector3(val, transform.position.y+4f, spawnZ-20+distance);

            // Instantiate obstacle at the calculated position
            int obsctacleIndex= Random.Range(0, 10);
            Instantiate(obstacles[obsctacleIndex], spawnPosition, Quaternion.identity,tile.transform);


            obstaclePositions.Add(spawnPosition);
            
            // Update the last spawn index
            lastSpawnIndex = spawnIndex;
        }
        SpawnCoins(tile, obstaclePositions);


    }

    void Start()
    {
        lastSpawnIndex = 0;
        activeTiles =new List<GameObject>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        toBeDeleted = false;
       for (int i = 0; i < tilesCount; i++)
        {
           GameObject til= spawning();
            SpawnObstacles(til);
        }
        
    }
    void deleting()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);

    }
    private void Update()
    {
        if (playerTransform.position.z-safeZone > (spawnZ - tilesCount * tileLength))
        {
            GameObject til = spawning();
            SpawnObstacles(til);

            deleting();

            //if(toBeDeleted==true)
            //  {
            //      deleting();
            //      deleting();
            //      toBeDeleted = false;
            //  }
            //  else
            //  {
            //      toBeDeleted = true;
            //  }

        }
    }


}
