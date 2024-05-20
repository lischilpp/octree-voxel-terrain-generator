using UnityEngine;
using UnityEditor;
using System.Collections;

public class ChunkSettings : MonoBehaviour {

    [Range(3, 6)]
    public int _logChunkSize;

    [Range(3, 6)]
    public int _renderDistance;

    public static int logChunkSize;
    public static int renderDistance;

    public static int chunkSize;

    void Awake()
    {
        logChunkSize = _logChunkSize;
        chunkSize = 1 << logChunkSize;
        renderDistance = _renderDistance;
    }
}
