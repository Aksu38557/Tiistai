using UnityEngine;
using System.Collections.Generic;

public class CityGenerator : MonoBehaviour
{
    [Header("Buildings")]
    public GameObject[] buildings;

    [Header("Player")]
    public Transform player;

    [Header("World Settings")]
    public int chunkSize = 50;
    public int buildingsPerChunk = 8;
    public int renderDistance = 2;
    public float minimumDistance = 12f;

    private Dictionary<Vector2Int, GameObject> chunks =
        new Dictionary<Vector2Int, GameObject>();

    private Vector2Int lastPlayerChunk;
    void Start()
    {
        if (player == null)
        {
            Debug.LogError("CityGenerator: Player is not assigned!");
            return;
        }

        if (buildings == null || buildings.Length == 0)
        {
            Debug.LogError("CityGenerator: No buildings have been assigned!");
            return;
        }

        lastPlayerChunk = GetChunkCoordinate(player.position);

        GenerateChunksAroundPlayer();
    }
    void Update()
    {
        if (player == null)
            return;

        Vector2Int currentPlayerChunk =
            GetChunkCoordinate(player.position);

        // Only update when player enters a new chunk
        if (currentPlayerChunk != lastPlayerChunk)
        {
            lastPlayerChunk = currentPlayerChunk;

            GenerateChunksAroundPlayer();
        }
    }

    Vector2Int GetChunkCoordinate(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / chunkSize),
            Mathf.FloorToInt(position.z / chunkSize)
        );
    }
    void GenerateChunksAroundPlayer()
    {
        Vector2Int playerChunk =
            GetChunkCoordinate(player.position);

        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                Vector2Int chunkCoord = new Vector2Int(
                    playerChunk.x + x,
                    playerChunk.y + z
                );

                if (!chunks.ContainsKey(chunkCoord))
                {
                    GenerateChunk(chunkCoord);
                }
            }
        }

        RemoveFarChunks(playerChunk);
    }
    void GenerateChunk(Vector2Int chunkCoord)
    {
        GameObject chunkObject = new GameObject(
            "Chunk" + chunkCoord.x + "" + chunkCoord.y
        );

        chunks.Add(chunkCoord, chunkObject);

        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < buildingsPerChunk; i++)
        {
            Vector3 position = Vector3.zero;
            bool validPosition = false;

            // Try up to 50 times to find a suitable position
            for (int attempts = 0; attempts < 50; attempts++)
            {
                float x = Random.Range(
                    chunkCoord.x * chunkSize,
                    (chunkCoord.x + 1) * chunkSize
                );

                float z = Random.Range(
                    chunkCoord.y * chunkSize,
                    (chunkCoord.y + 1) * chunkSize
                );

                position = new Vector3(x, 0f, z);

                validPosition = true;

                // Check distance from other buildings in this chunk
                foreach (Vector3 otherPosition in positions)
                {
                    if (Vector3.Distance(position, otherPosition)
                        < minimumDistance)
                    {
                        validPosition = false;
                        break;
                    }
                }
                if (validPosition)
                {
                    positions.Add(position);
                    break;
                }
            }

            // Couldn't find a valid position
            if (!validPosition)
                continue;

            // Pick a random building
            GameObject building =
                buildings[Random.Range(0, buildings.Length)];

            // Random Y rotation while preserving
            // the prefab's original orientation
            Quaternion rotation =
                Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up)
                * building.transform.rotation;

            // Create the building
            GameObject clone = Instantiate(
                building,
                position,
                rotation
            );

            // Find the renderer on the building
            Renderer renderer = clone.GetComponentInChildren<Renderer>();

            if (renderer != null)
            {
                // Find the bottom of the building
                float bottom = renderer.bounds.min.y;

                // Move the building so its bottom is exactly at Y = 0
                clone.transform.position -= new Vector3(0f, bottom, 0f);
            }

            // Put building inside the chunk
            clone.transform.SetParent(chunkObject.transform);
        }
    }

    void RemoveFarChunks(Vector2Int playerChunk)
    {
        List<Vector2Int> chunksToRemove =
            new List<Vector2Int>();

        foreach (KeyValuePair<Vector2Int, GameObject> chunk in chunks)
        {
            int distanceX =
                Mathf.Abs(chunk.Key.x - playerChunk.x);

            int distanceZ =
                Mathf.Abs(chunk.Key.y - playerChunk.y);

            if (distanceX > renderDistance ||
                distanceZ > renderDistance)
            {
                chunksToRemove.Add(chunk.Key);
            }
        }
        foreach (Vector2Int chunkCoord in chunksToRemove)
        {
            if (chunks.ContainsKey(chunkCoord))
            {
                Destroy(chunks[chunkCoord]);
                chunks.Remove(chunkCoord);
            }
        }
    }
}
