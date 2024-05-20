using UnityEngine;
using System.Collections;

public class Terrain : MonoBehaviour {

    ChunkManager chunkManager;
	// Use this for initialization
	void Start () {
        chunkManager = gameObject.AddComponent<ChunkManager>();
        //chunkManager.GenerateChunksAroundPlayer();
        
        int x, y, z;
        int size = 8;
        for (z = 0; z < size; z++)
        {
            for (y = 0; y < size; y++)
            {
                for (x = 0; x < size; x++)
                {
                    chunkManager.CreateChunkAt(new Vector3i(x, y, z));
                    chunkManager.GenerateChunkAt(new Vector3i(x, y, z));
                    chunkManager.MakeMeshOfChunkAt(new Vector3i(x, y, z));
                }
            }
        }
    }
}
