using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class BlockTypeInfo : MonoBehaviour {

    string blockInfoPath = "Assets/Resources/Prefabs/Terrain/BlockTypes";
    static BlockTypeData[] blockTypeDataArray;
    static Dictionary<string, int> blockNameIdArray;

    public static BlockTypeData GetDataOfId(int id)
    {
        return blockTypeDataArray[id];
    }

    public static int GetBlockIDByName(string name)
    {
        return blockNameIdArray[name];
    }

    void Awake()
    {
        LoadBlockTypeData();
    }

    public void LoadBlockTypeData()
    {
        blockNameIdArray = new Dictionary<string, int>();
        blockTypeDataArray = new BlockTypeData[GetBlockTypeCount()];

        DirectoryInfo directory = new DirectoryInfo(blockInfoPath);
        FileInfo[] files = directory.GetFiles();
        FileInfo file;
        BlockTypeData blockTypeData;
        System.Array blockTypesArray = Enum.GetValues(typeof(BlockType));
        string blockName;
        for (int i = 0; i < files.Length; i++)
        {
            file = files[i];
            if (file.Extension == ".prefab")
            {
                blockName = file.Name.Substring(0, file.Name.Length - 7);
                // find out blockName index in BlockType enum
                int j;
                bool jFound = false;
                for (j = 0; j < blockTypesArray.Length; j++)
                {
                    if (blockTypesArray.GetValue(j).ToString() == blockName)
                    {
                        jFound = true;
                        break;
                    }
                }
                if (jFound)
                {
                    // add to blockNameIDArray
                    blockNameIdArray.Add(blockName, j);
                    // load the block type
                    blockTypeData = ((GameObject)Resources.Load("Prefabs/Terrain/BlockTypes/" + blockName)).GetComponent<BlockTypeData>();
                    // copy block type into array
                    blockTypeDataArray[j] = blockTypeData;
                }
            }
        }
    }

    int GetBlockTypeCount()
    {
        int count = 0;
        DirectoryInfo directory = new DirectoryInfo(blockInfoPath);
        FileInfo[] files = directory.GetFiles();
        FileInfo file;
        for (int i = 0; i < files.Length; i++)
        {
            file = files[i];
            if (file.Extension == ".prefab")
            {
                count++;
            }
        }
        return count;
    }
}
