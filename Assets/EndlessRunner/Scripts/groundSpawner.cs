using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundSpawner : MonoBehaviour
{
    public GameObject groundTile;
    private Transform playerTransform;
    private float spawnZ = 0f;
    public float tileLength = 487f;
    public int tilesCount = 1;
    private float safeZone = 25f;
    private List<GameObject> activeTiles = new List<GameObject>();
    public GameObject player;
    public GameObject[] obstacles;
    private int lastSpawnIndex;
    public GameObject coinPrefab;
    public bool toBeDeleted;
    private Vector3 initial_po_player;
    public int obstaclesToSpawn = 30;
    public float off = 4.1f;
    GameObject spawning()
    {
        GameObject go = Instantiate(groundTile) as GameObject;
        go.transform.SetParent(transform);
        go.transform.position = Vector3.forward * spawnZ;
        spawnZ += tileLength;
        activeTiles.Add(go);
        return go;
        
    }

    //void SpawnCoins(GameObject tile, List<Vector3> obstaclePositions)
    //{
    //    int coinsToSpawn =300;

    //    for (int i = 0; i < coinsToSpawn; i++)
    //    {
    //        int spawnIndex = Random.Range(0, 3);
    //        float val = 0;
    //        if (spawnIndex == 0)
    //            val = -4.1f;
    //        else if (spawnIndex == 2)
    //            val = 4.1f;

    //        Vector3 coinPosition = new Vector3(initial_po_player.x+val, initial_po_player.y + 1f, spawnZ - 20 + Random.Range(0, 400));

    //        bool overlap = false;
    //        foreach (Vector3 obstaclePos in obstaclePositions)
    //        {
    //            if (Vector3.Distance(coinPosition, obstaclePos) < 2f)
    //            {
    //                overlap = true;
    //                break;
    //            }
    //        }

    //        if (!overlap)
    //        {
    //            GameObject coin = Instantiate(coinPrefab, coinPosition, Quaternion.identity, tile.transform);
    //            coin.transform.localScale = new Vector3(0.5f, 10f, 0.5f); // Set the coin scale to 1
    //        }
    //    }
    //}


    void SpawnCoins(GameObject tile, List<Vector3> obstaclePositions)
    {
        int coinsToSpawn = 80; // Number of coins to spawn
        //float tileLength = 400f; // Length of the tile

        for (int i = 0; i < coinsToSpawn; i++)
        {
            // Determine the lane (left, center, right)
            int spawnIndex = Random.Range(0, 3);
            float val = 0;
            if (spawnIndex == 0)
                val = -off;
            else if (spawnIndex == 2)
                val =off;

            // Calculate coin position within the tile length
            Vector3 coinPosition = new Vector3(initial_po_player.x + val, initial_po_player.y + 1f, tile.transform.position.z + Random.Range(0, tileLength));

            // Check for overlaps with obstacles
            bool overlap = false;
            foreach (Vector3 obstaclePos in obstaclePositions)
            {
                if (Vector3.Distance(coinPosition, obstaclePos) < 2f)
                {
                    overlap = true;
                    break;
                }
            }

            // Spawn the coin if there's no overlap
            if (!overlap)
            {
                GameObject coin = Instantiate(coinPrefab, coinPosition, Quaternion.identity, tile.transform);
                coin.transform.localScale = new Vector3(0.5f, 3f, 0.5f); // Adjust the coin scale as needed
            }
        }
    }

    void SpawnObstacles(GameObject tile)
    {
       
        List<Vector3> obstaclePositions = new List<Vector3>();

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            int spawnIndex =Random.Range(0,3);
            if (spawnIndex == lastSpawnIndex)
            {
                spawnIndex = (spawnIndex + 2) % 3;
            }
            float val = 0;
            if (spawnIndex == 0)
                val = -off;
            else if (spawnIndex == 2)
                val =off;
            int distance = Random.Range(0, 20);
            Vector3 spawnPosition = new Vector3(initial_po_player.x + val, initial_po_player.y + 0.5f, tile.transform.position.z + Random.Range(0, tileLength));
            //Vector3 coinPosition = new Vector3(initial_po_player.x + val, initial_po_player.y + 1f, tile.transform.position.z + Random.Range(0, tileLength));


            GameObject obs=Instantiate(obstacles[2], spawnPosition, Quaternion.identity, tile.transform);
            //obs.transform.localScale = new Vector3(0.0005f, 0.006f, 0.0005f); // Adjust the coin scale as needed
            obstaclePositions.Add(spawnPosition);
            lastSpawnIndex = spawnIndex;
        }
        SpawnCoins(tile, obstaclePositions);
    }

    void Start()
    {
        lastSpawnIndex = 0;
        activeTiles = new List<GameObject>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        toBeDeleted = false;

        for (int i = 0; i < tilesCount; i++)
        {
            GameObject tile = spawning();
            SpawnObstacles(tile);
        }
        initial_po_player = player.GetComponent<playerMovement>().initial_po;
    }

    void deleting()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    private void Update()
    {
        if (playerTransform.position.z - safeZone > (spawnZ - tilesCount * tileLength))
        {
            GameObject tile = spawning();
            SpawnObstacles(tile);
            if (activeTiles.Count > 2)
                deleting();
        }
    }
}
