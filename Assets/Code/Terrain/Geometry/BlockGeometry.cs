using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockGeometry {

    Chunk chunk;
    Mesh visualMesh;//, colliderMesh;
    MeshCollider collider;

    // vertices
    List<Vector3> newVisualVertices = new List<Vector3>();/*,
                  newColliderVertices = new List<Vector3>();*/
    // triangles
    List<int> newVisualTriangles = new List<int>();/*,
              newColliderTriangles = new List<int>();*/
    // uv
    List<Vector2> newUV = new List<Vector2>();
    int textureElements;
    float uvUnit;

    int visualVerticesCount = 0;/*,
        colliderVerticesCount = 0;*/


    public BlockGeometry(Chunk chunk, Mesh visualMesh)//, MeshCollider collider)
    {
        this.chunk = chunk;
        this.visualMesh = visualMesh;
        //this.collider = collider;
        //this.colliderMesh = collider.sharedMesh;
        textureElements = GameObject.Find("BlockMaterial").GetComponent<BlockMaterial>().textureElements;
        uvUnit = 1 / (float)textureElements;
    }

    public void Update()
    {
        visualMesh.Clear();
        visualMesh.vertices = newVisualVertices.ToArray();
        visualMesh.triangles = newVisualTriangles.ToArray();
        visualMesh.uv = newUV.ToArray();
        visualMesh.RecalculateNormals();
    }

    public void Clear()
    {
        newVisualVertices.Clear();
        newVisualTriangles.Clear();
        newUV.Clear();

        visualVerticesCount = 0;
    }

    public void CreateFromData()
    {
        TraverseNode(ref chunk.blocks, 0, 0, 0, ChunkSettings.chunkSize);
    }

    void TraverseNode(ref BlockOctree node, float x, float y, float z, float size)
    {
        if (node == null)
            return;

        // check sub nodes
        int n;
        for (n = 0; n < 8; n++)
        {
            if (node.nodes[n] != null)
            {
                size *= 0.5f;
                TraverseNode(ref node.nodes[0], x, y, z, size); // left bottom front
                if (n < 2)
                    TraverseNode(ref node.nodes[1], x + size, y, z, size); // right bottom front
                if (n < 3)
                    TraverseNode(ref node.nodes[2], x + size, y + size, z, size); // right top front
                if (n < 4)
                    TraverseNode(ref node.nodes[3], x, y + size, z, size); // left top front
                if (n < 5)
                    TraverseNode(ref node.nodes[4], x, y, z + size, size); // left bottom back
                if (n < 6)
                    TraverseNode(ref node.nodes[5], x + size, y, z + size, size); // right bottom back
                if (n < 7)
                    TraverseNode(ref node.nodes[6], x + size, y + size, z + size, size); // right top back
                if (n < 8)
                    TraverseNode(ref node.nodes[7], x, y + size, z + size, size); // left top back
                return;
            }
        }

        if (node.blockType != BlockType.Air)
            CreateBlockDirty(x, y, z, size, node.blockType);
        /*if (node.blockType != BlockType.Air)
        {
            BlockTypeData blockData = BlockTypeInfo.GetDataOfId((int)node.blockType);
            if (node.sideIsAir[0])
                CreateVisualLeft(x, y, z, size, blockData.textureLeft);
            if (node.sideIsAir[1])
                CreateVisualRight(x, y, z, size, blockData.textureRight);
            if (node.sideIsAir[2])
                CreateVisualBottom(x, y, z, size, blockData.textureBottom);
            if (node.sideIsAir[3])
                CreateVisualTop(x, y, z, size, blockData.textureTop);
            if (node.sideIsAir[4])
                CreateVisualFront(x, y, z, size, blockData.textureFront);
            if (node.sideIsAir[5])
                CreateVisualBack(x, y, z, size, blockData.textureBack);
        }*/
    }

    public void CreateBlockDirty(float x, float y, float z, float size, BlockType blockType)
    {
        BlockTypeData blockData = BlockTypeInfo.GetDataOfId((int)blockType);
        CreateVisualLeft(x, y, z, size, blockData.textureLeft);
        CreateVisualRight(x, y, z, size, blockData.textureRight);
        CreateVisualBottom(x, y, z, size, blockData.textureBottom);
        CreateVisualTop(x, y, z, size, blockData.textureTop);
        CreateVisualFront(x, y, z, size, blockData.textureFront);
        CreateVisualBack(x, y, z, size, blockData.textureBack);
    }

    // visual sides
    public void CreateVisualLeft(float x, float y, float z, float size, BlockTypeData.Texture texture)
    {
        newVisualVertices.AddRange(new List<Vector3>() {
            new Vector3 (x, y, z + size),
            new Vector3 (x, y + size, z + size),
            new Vector3 (x, y + size, z),
            new Vector3 (x, y, z)
        });
        AddVisualTriangles();
        createUV(texture, size);
        visualVerticesCount += 4;
    }

    public void CreateVisualRight(float x, float y, float z, float size, BlockTypeData.Texture texture)
    {
        newVisualVertices.AddRange(new List<Vector3>() {
            new Vector3 (x + size, y, z),
            new Vector3 (x + size, y + size, z),
            new Vector3 (x + size, y + size, z + size),
            new Vector3 (x + size, y, z + size)
        });
        AddVisualTriangles();
        createUV(texture, size);
        visualVerticesCount += 4;
    }

    public void CreateVisualFront(float x, float y, float z, float size, BlockTypeData.Texture texture)
    {
        newVisualVertices.AddRange(new List<Vector3>() {
            new Vector3 (x + size, y, z + size),
            new Vector3 (x + size, y + size, z + size),
            new Vector3 (x, y + size, z + size),
            new Vector3 (x, y, z + size),
        });
        AddVisualTriangles();
        createUV(texture, size);
        visualVerticesCount += 4;
    }

    public void CreateVisualBack(float x, float y, float z, float size, BlockTypeData.Texture texture)
    {
        newVisualVertices.AddRange(new List<Vector3>() {
            new Vector3 (x, y, z),
            new Vector3 (x, y + size, z),
            new Vector3 (x + size, y + size, z),
            new Vector3 (x + size, y, z)
        });
        AddVisualTriangles();
        createUV(texture, size);
        visualVerticesCount += 4;
    }

    public void CreateVisualBottom(float x, float y, float z, float size, BlockTypeData.Texture texture)
    {
        newVisualVertices.AddRange(new List<Vector3>() {
            new Vector3 (x, y, z),
            new Vector3 (x + size, y, z),
            new Vector3 (x + size, y, z + size),
            new Vector3 (x, y, z + size)
        });
        AddVisualTriangles();
        createUV(texture, size);
        visualVerticesCount += 4;
    }

    public void CreateVisualTop(float x, float y, float z, float size, BlockTypeData.Texture texture)
    {
        newVisualVertices.AddRange(new List<Vector3>() {
            new Vector3 (x, y + size, z),
            new Vector3 (x, y + size, z + size),
            new Vector3 (x + size, y + size, z + size),
            new Vector3 (x + size, y + size, z)
        });
        AddVisualTriangles();
        createUV(texture, size);
        visualVerticesCount += 4;
    }

    public void AddVisualTriangles()
    {
        newVisualTriangles.AddRange(new List<int>() {
            visualVerticesCount, visualVerticesCount + 1, visualVerticesCount + 2,
            visualVerticesCount, visualVerticesCount + 2, visualVerticesCount + 3
        });
    }

    public void createUV(BlockTypeData.Texture texture, float size)
    {
        float uvPosX = ((int)texture % textureElements) * uvUnit,
              uvPosY = Mathf.Floor((int)texture / textureElements) * uvUnit;
        newUV.Add(new Vector2(uvPosX, uvPosY));
        newUV.Add(new Vector2(uvPosX, uvPosY + uvUnit));
        newUV.Add(new Vector2(uvPosX + uvUnit, uvPosY + uvUnit));
        newUV.Add(new Vector2(uvPosX + uvUnit, uvPosY));
    }
}
