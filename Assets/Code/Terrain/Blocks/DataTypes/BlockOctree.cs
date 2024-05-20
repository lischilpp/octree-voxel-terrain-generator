using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockOctree {
    public BlockOctree parent = null;
    public int indexInParent;
    public BlockType blockType = BlockType.None;
    public BlockOctree[] nodes = new BlockOctree[] { null, null, null, null, null, null, null, null };
    public bool[] sideIsAir = new bool[6];

    public void MergeNodes()
    {
        int size = ChunkSettings.chunkSize >> 1;
        for (int n = 0; n < 8; n++)
            TraverseNode(ref nodes[n], size);
    }

    public void TraverseNode(ref BlockOctree node, int size)
    {
        if (node != null)
        {
            if (size > 1)
            {
                size >>= 1;
                for (int n = 0; n < 8; n++)
                {
                    if (node != null)
                        TraverseNode(ref node.nodes[n], size);
                }  
            }
            else
                MergeNode(ref node.parent, node.blockType);
        }
    }

    int[,] neighbourAlignArray = new int[,] { { 0, 2, 4 },   // 0
                                              { 1, 2, 4 },   // 1
                                              { 1, 3, 4 },   // 2
                                              { 0, 3, 4 },   // 3
                                              { 0, 2, 5 },   // 4
                                              { 1, 2, 5 },   // 5
                                              { 1, 3, 5 },   // 6
                                              { 0, 3, 5 } }; // 7

    List<int>[] alignedIndizes = new List<int>[6] { new List<int> { 0, 3, 4, 7 },   // 0
                                                    new List<int> { 1, 2, 5, 6 },   // 1
                                                    new List<int> { 0, 1, 4, 5 },   // 2
                                                    new List<int> { 2, 3, 6, 7 },   // 3
                                                    new List<int> { 0, 1, 2, 3 },   // 4
                                                    new List<int> { 4, 5, 6, 7 } }; // 5

    public void MergeNode(ref BlockOctree node, BlockType blockType)
    {
        if (node != null)
        {
            int blockID = (int)blockType;
            int n, i;
            
            // check if all nodes have the same block type
            for (n = 0; n < 8; n++)
            {
                if ((int)node.nodes[n].blockType != blockID)
                    return;
            }

            node.blockType = blockType;

            // pass air side info to merged node
            int s;
            for (n = 0; n < 8; n++)
            {
                for (i = 0; i < 3; i++)
                {
                    s = neighbourAlignArray[n, i];
                    if (node.nodes[n].sideIsAir[s])
                    {
                        // subnode and node are on the same side in the octree
                        if (alignedIndizes[s].Contains(n) && alignedIndizes[s].Contains(node.indexInParent))
                            node.sideIsAir[s] = true;
                    }
                }

                node.nodes[n] = null;
            }


            MergeNode(ref node.parent, node.blockType);
        }
    }
}
