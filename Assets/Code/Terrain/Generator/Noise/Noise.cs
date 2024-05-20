using UnityEngine;
using System.Collections;

public static class Noise {

    public enum Type
    {
        _2d,
        _3d,
        Octave2d,
        Octave3d
    }

    [System.Serializable]
    public class NoiseLevel
    {
        public Type type;
        public float[] options = new float[6] { 16, 16, 16, 1, 0.9f, 4 };
        public bool extendedInEditor;
    }

    static SimplexNoiseGenerator simplex = new SimplexNoiseGenerator("seed");

    public static float GetNoise2d(float x, float y, float scale, float amplitude)
    {
        return simplex.noise(x/scale, y/scale, 0) * amplitude;
    }

    public static float GetCoherentNoise2d(float x, float y, float scale, float amplitude, float lacunarity, float persistence, int octaves)
    {
        return simplex.coherentNoise(x, y, 0, scale, amplitude, lacunarity, persistence, octaves);
    }


}
