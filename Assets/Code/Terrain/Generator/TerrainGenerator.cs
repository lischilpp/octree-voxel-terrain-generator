using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TerrainGenerator : EditorWindow
{

    [System.Serializable]
    public class Layer
    {
        public BlockType blockType;
        public List<Noise.NoiseLevel> noiseLevels = new List<Noise.NoiseLevel>();
        public bool extendedInEditor;
    }

    [SerializeField]
    public List<Layer> layers = new List<Layer>();
    static BlockType[,,] blockData;

    public static void Generate(Vector3i chunkPosition, ref BlockOctree blocks)
    {
        //float startTime = Time.realtimeSinceStartup;
        blockData = new BlockType[ChunkSettings.chunkSize, ChunkSettings.chunkSize, ChunkSettings.chunkSize];
        GenerateData(chunkPosition);
        GenerateDivision(0, chunkPosition, ref blocks, chunkPosition);
        blocks.MergeNodes();
        //Debug.Log("generating took " + (Time.realtimeSinceStartup - startTime));
    }

    public static void GenerateDivision(int d, Vector3i pos, ref BlockOctree node, Vector3i chunkPosition)
    {
        int size = ChunkSettings.chunkSize >> d;
        if (size > 1)
        {
            int halfSize = size >> 1;
            Vector3i[] points = new Vector3i[] { pos,
                                                 new Vector3i(pos.x + halfSize, pos.y, pos.z),
                                                 new Vector3i(pos.x + halfSize, pos.y + halfSize, pos.z),
                                                 new Vector3i(pos.x, pos.y + halfSize, pos.z),
                                                 new Vector3i(pos.x, pos.y, pos.z + halfSize),
                                                 new Vector3i(pos.x + halfSize, pos.y, pos.z + halfSize),
                                                 new Vector3i(pos.x + halfSize, pos.y + halfSize, pos.z + halfSize),
                                                 new Vector3i(pos.x, pos.y + halfSize, pos.z + halfSize) };
            d++;
            for (int n = 0; n < 8; n++)
            {
                node.nodes[n] = new BlockOctree();
                node.nodes[n].parent = node;
                node.nodes[n].indexInParent = n;
                GenerateDivision(d, points[n], ref node.nodes[n], chunkPosition);
            }
        }
        else
        {
            int chunkX = pos.x - chunkPosition.x,
                chunkY = pos.y - chunkPosition.y,
                chunkZ = pos.z - chunkPosition.z;

            node.blockType = blockData[chunkX, chunkY, chunkZ];

            // calculate air side
            if (chunkX != 0 && chunkY != 0 && chunkZ != 0 && chunkX != ChunkSettings.chunkSize - 1 && chunkY != ChunkSettings.chunkSize - 1 && chunkZ != ChunkSettings.chunkSize - 1)
            {
                Vector3i[] neighbourPositions = new Vector3i[] { new Vector3i(chunkX - 1, chunkY, chunkZ),
                                                                 new Vector3i(chunkX + 1, chunkY, chunkZ),
                                                                 new Vector3i(chunkX, chunkY - 1, chunkZ),
                                                                 new Vector3i(chunkX, chunkY + 1, chunkZ),
                                                                 new Vector3i(chunkX, chunkY, chunkZ - 1),
                                                                 new Vector3i(chunkX, chunkY, chunkZ + 1) };
                for (int n = 0; n < 6; n++)
                {
                    node.sideIsAir[n] = blockData[neighbourPositions[n].x, neighbourPositions[n].y, neighbourPositions[n].z] == BlockType.Air;
                }
            }
        }
    }



    static void GenerateData(Vector3i chunkPosition)
    {
        List<Layer> layers = EditorWindow.GetWindow<TerrainGenerator>().layers;
        int x, y, z, realX, realZ, realY, lay, lv, noiseValue;
        Noise.NoiseLevel noiseLevel;
        
        for (x = 0; x < ChunkSettings.chunkSize; x++)
        {
            for (z = 0; z < ChunkSettings.chunkSize; z++)
            {
                realX = chunkPosition.x + x;
                realZ = chunkPosition.z + z;
                for (lay = 0; lay < layers.Count; lay++)
                {
                    noiseValue = 0;
                    for (lv = 0; lv < layers[lay].noiseLevels.Count; lv++)
                    {
                        noiseLevel = layers[lay].noiseLevels[lv];
                        switch (noiseLevel.type)
                        {
                            case Noise.Type._2d:
                                noiseValue += (int)noiseLevel.options[0] + (int)Noise.GetNoise2d(realX, realZ, noiseLevel.options[1], noiseLevel.options[2]);
                                break;
                            case Noise.Type.Octave2d:
                                noiseValue += (int)noiseLevel.options[0] + (int)Noise.GetCoherentNoise2d(realX, realZ, noiseLevel.options[1], noiseLevel.options[2], noiseLevel.options[3], noiseLevel.options[4], (int)noiseLevel.options[5]);
                                break;
                        }
                    }
                    for (y = 0; y < ChunkSettings.chunkSize; y++)
                    {
                        realY = chunkPosition.y + y;
                        if (realY < noiseValue)
                            blockData[x, y, z] = layers[lay].blockType;
                    }
                }
            }
        }
    }

    // GUI

    Vector2 scrollPosition = new Vector2(0, 0);

    bool livePreview = false;
    Texture2D livePreviewTexture;
    int livePreviewPixelSize = 16;
    int livePreviewWorldHeight = 64;

    [MenuItem("Window/TerrainGenerator")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TerrainGenerator));
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
        DrawLifePreviewSettings();
        DrawTerrainOptions();
        EditorGUILayout.EndScrollView();
    }

    public void DrawTerrainOptions()
    {
        // Add Layer Button
        GUILayout.Label("Layers", EditorStyles.boldLabel);
        if (GUILayout.Button("Add Layer"))
        {
            layers.Add(new Layer());
        }

        // Settings for each layer
        Layer layer;
        for (int i = layers.Count - 1; i >= 0; i--)
        {
            layer = layers[i];
            layer.extendedInEditor = EditorGUILayout.Foldout(layer.extendedInEditor, "Layer " + i);
            if (layer.extendedInEditor)
            {
                if (i > 0)
                {
                    if (GUILayout.Button("Move Layer " + i + " up"))
                    {
                        Layer tmp = layers[i];
                        layers[i] = layers[i - 1];
                        layers[i - 1] = tmp;
                    }
                }
                EditorGUILayout.BeginHorizontal();
                layer.blockType = (BlockType)EditorGUILayout.EnumPopup("Block Type", layer.blockType);
                EditorGUILayout.EndHorizontal();
                GUILayout.Label("Noise Levels");

                // Add Noise Level Button
                if (GUILayout.Button("Add Noise Level"))
                {
                    layer.noiseLevels.Add(new Noise.NoiseLevel());
                }

                Noise.NoiseLevel level;
                for (int j = 0; j < layer.noiseLevels.Count; j++)
                {
                    level = layer.noiseLevels[j];
                    level.extendedInEditor = EditorGUILayout.Foldout(level.extendedInEditor, "Level " + j);
                    if (level.extendedInEditor)
                    {
                        level.type = (Noise.Type)EditorGUILayout.EnumPopup("Noise Type", level.type);

                        switch (level.type)
                        {
                            case Noise.Type._2d:
                                DrawDefaultNoiseOptions(level.options);
                                break;
                            case Noise.Type._3d:
                                DrawDefaultNoiseOptions(level.options);
                                break;
                            case Noise.Type.Octave2d:
                                DrawDefaultNoiseOptions(level.options);
                                DrawCoherentNoiseOptions(level.options);
                                break;
                            case Noise.Type.Octave3d:
                                DrawDefaultNoiseOptions(level.options);
                                DrawCoherentNoiseOptions(level.options);
                                break;
                        }
                    }
                    if (GUILayout.Button("Remove Noise Level "+j))
                    {
                        layer.noiseLevels.RemoveAt(j);
                    }
                }
            }
            if (GUILayout.Button("Remove Layer" + i))
            {
                layers.RemoveAt(i);
            }
        }
    }

    public void DrawLifePreviewSettings()
    {
        GUILayout.Label("LivePreview Settings", EditorStyles.boldLabel);
        livePreview = GUILayout.Toggle(livePreview, "Auto Refresh");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Pixel Size");
        livePreviewPixelSize = EditorGUILayout.IntSlider(livePreviewPixelSize, 1, 64);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("World Height");
        livePreviewWorldHeight = EditorGUILayout.IntField(livePreviewWorldHeight);
        EditorGUILayout.EndHorizontal();
        if (livePreview || GUILayout.Button("Generate Preview"))
        {
            GenerateLifePreview();
        }
        GUILayout.Box(livePreviewTexture);
    }

    public void GenerateLifePreview()
    {
        int x, z, lay, lv, p, noiseValue;
        Noise.NoiseLevel noiseLevel;
        Color color;
        Color[] colors = new Color[livePreviewPixelSize * livePreviewPixelSize];

        livePreviewTexture = new Texture2D((int)(position.width * 0.8f), (int)(position.width * 0.8f));

        for (x = 0; x < livePreviewTexture.width - livePreviewPixelSize; x += livePreviewPixelSize)
        {
            for (z = 0; z < livePreviewTexture.height - livePreviewPixelSize; z += livePreviewPixelSize)
            {
                for (lay = 0; lay < layers.Count; lay++)
                {
                    noiseValue = 0;
                    for (lv = 0; lv < layers[lay].noiseLevels.Count; lv++)
                    {
                        noiseLevel = layers[lay].noiseLevels[lv];
                        switch (noiseLevel.type)
                        {
                            case Noise.Type._2d:
                                noiseValue += (int)noiseLevel.options[0] + (int)Noise.GetNoise2d((int)x, (int)z, noiseLevel.options[1], noiseLevel.options[2]);
                                break;
                            case Noise.Type.Octave2d:
                                noiseValue += (int)noiseLevel.options[0] + (int)Noise.GetCoherentNoise2d((int)x, (int)z, noiseLevel.options[1], noiseLevel.options[2], noiseLevel.options[3], noiseLevel.options[4], (int)noiseLevel.options[5]);
                                break;
                        }
                    }
                    color = GetColorForHeight(noiseValue);
                    for (p = 0; p < livePreviewPixelSize * livePreviewPixelSize; p++)
                        colors[p] = color;
                    livePreviewTexture.SetPixels(x, z, livePreviewPixelSize, livePreviewPixelSize, colors);
                }
            }
        }
        livePreviewTexture.Apply();
    }

    public Color GetColorForHeight(int height)
    {
        float h = height / (float)livePreviewWorldHeight;
        if (h < 0.25f)
        {
            return Color.Lerp(new Color(0.59f, 0.39f, 0), new Color(0, 1.0f, 1.0f), h / 0.25f);
        }
        else if (h < 0.5f)
        {
            return Color.Lerp(new Color(0, 1.0f, 1.0f), new Color(0, 1.0f, 0), (h - 0.25f) / 0.25f);
        }
        else if (h < 0.75f)
        {
            return Color.Lerp(new Color(0, 1.0f, 0), new Color(1.0f, 0, 0), (h - 0.5f) / 0.25f);
        }
        else if (h < 1.0f)
        {
            return Color.Lerp(new Color(1.0f, 0, 0), new Color(1.0f, 1.0f, 0), (h - 0.75f) / 0.25f);
        }
        else
            return new Color(h, h, h);
    }

    public void DrawDefaultNoiseOptions(float[] options)
    {
        DrawSliderSetting("Constant Height", ref options[0], 1, 128);
        DrawSliderSetting("Scale", ref options[1], 1, 128);
        DrawSliderSetting("Magnitude", ref options[2], 1, 128);
    }

    public void DrawCoherentNoiseOptions(float[] options)
    {
        DrawSliderSetting("Lacunarity", ref options[3], 0.25f, 2);
        DrawSliderSetting("Persistence", ref options[4], 0.25f, 2);
        DrawSliderSetting("Octaves", ref options[5], 2, 8);
    }

    public void DrawSliderSetting(string label, ref float option, float sliderMIN, float sliderMAX)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);
        option = GUILayout.HorizontalSlider(option, sliderMIN, sliderMAX);
        EditorGUILayout.EndHorizontal();
    }
}
