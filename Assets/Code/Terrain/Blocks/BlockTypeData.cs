using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BlockTypeData : MonoBehaviour
{
    public enum Texture {
        Air,
        Bedrock,
        Stone,
        Dirt,
        GrassSide,
        GrassTop,
        WoodSide,
        WoodTop,
        Leaves,
        Glass,
        Water,
        Sand,
        IronOre,
        NickelOre,
        GoldOre
    }

    public Texture textureLeft;
    public Texture textureRight;
    public Texture textureBottom;
    public Texture textureTop;
    public Texture textureBack;
    public Texture textureFront;

    public bool isTranslucent;
    public bool isTransparent;

    [Range(0, 1)]
    public float transparency;

    public bool isSolid;
}
