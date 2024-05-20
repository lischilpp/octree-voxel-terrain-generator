using UnityEngine;
using System.Collections;

public class Chunk : MonoBehaviour {

    BlockGeometry geometry;
    public BlockOctree blocks;
    public Chunk[] neighbours;

    public bool isGenerated;

    public Vector3i index, position;

    void Awake()
    {
        geometry = new BlockGeometry(this, GetComponent<MeshFilter>().mesh);//, GetComponent<MeshCollider>());
        gameObject.GetComponent<Renderer>().material = GameObject.Find("BlockMaterial").GetComponent<BlockMaterial>().mainMaterial;
    }

    public void Generate()
    {
        blocks = new BlockOctree();
        TerrainGenerator.Generate(position, ref blocks);
        isGenerated = true;
    }

    public void MakeMesh()
    {
        geometry.CreateFromData();
        geometry.Update();
    }

    public bool BlockInRange(int x, int y, int z)
    {
        return (x >= 0 && x < ChunkSettings.chunkSize &&
                y >= 0 && y < ChunkSettings.chunkSize &&
                z >= 0 && z < ChunkSettings.chunkSize);
    }

    public static Vector3i IndexToPosition(Vector3i index)
    {
        return new Vector3i(index.x << ChunkSettings.logChunkSize,
                               index.y << ChunkSettings.logChunkSize,
                               index.z << ChunkSettings.logChunkSize);
    }

    public static Vector3i PositionToIndex(Vector3i position)
    {
        return new Vector3i(position.x >> ChunkSettings.logChunkSize,
                               position.y >> ChunkSettings.logChunkSize,
                               position.z >> ChunkSettings.logChunkSize);
    }
}
