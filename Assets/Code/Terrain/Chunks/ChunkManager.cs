using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour {

    Dictionary<Vector3i, Chunk> chunks = new Dictionary<Vector3i, Chunk>();
    GameObject player;

    void Awake()
    {
        player = GameObject.Find("Player");
    }

    public void GenerateChunksAroundPlayer()
    {
        Vector3 playerChunkIndex = Chunk.PositionToIndex(new Vector3i(player.transform.position)).ToVector3();
        int x, y, z, distance;
        for (x = -ChunkSettings.renderDistance; x <= ChunkSettings.renderDistance; x++)
        {
            for (y = -ChunkSettings.renderDistance; y <= ChunkSettings.renderDistance; y++)
            {
                for (z = -ChunkSettings.renderDistance; z <= ChunkSettings.renderDistance; z++)
                {
                    distance = Mathf.FloorToInt(Vector3.Distance(new Vector3(x, y, z), playerChunkIndex));
                    if (distance < ChunkSettings.renderDistance)
                    {
                        CreateChunkAt(new Vector3i(x, y, z));
                        GenerateChunkAt(new Vector3i(x, y, z));
                        MakeMeshOfChunkAt(new Vector3i(x, y, z));
                    }
                }
            }
        }
    }

    public void CreateChunkAt(Vector3i index)
    {
        GameObject chunkGO = new GameObject("Chunk " + index.ToString());
        chunkGO.transform.parent = gameObject.transform;
        chunkGO.transform.position = Chunk.IndexToPosition(index).ToVector3();
        chunkGO.AddComponent<MeshFilter>();
        chunkGO.AddComponent<MeshRenderer>();
        chunkGO.AddComponent<MeshCollider>();
        chunkGO.AddComponent<Chunk>();

        Chunk chunk = chunkGO.GetComponent<Chunk>();
        chunk.index = index;
        chunk.position = Chunk.IndexToPosition(index);
        chunks.Add(index, chunk);
    }

    public void DestroyChunkAt(Vector3i index)
    {
        chunks.Remove(index);
    }

    public Chunk GetChunkAt(Vector3i index)
    {
        return chunks[index];
    }

    public bool isChunkAt(Vector3i index)
    {
        return chunks.ContainsKey(index);
    }

    public void printExistingChunkIndizes()
    {
        foreach (KeyValuePair<Vector3i, Chunk> kvp in chunks)
        {
            Debug.Log(kvp.Key.ToString());
        }
    }

    public void GenerateChunkAt(Vector3i index)
    {
        chunks[index].Generate();
    }

    public void MakeMeshOfChunkAt(Vector3i index)
    {
        chunks[index].MakeMesh();
    }
}
