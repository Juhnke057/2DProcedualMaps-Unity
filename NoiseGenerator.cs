using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public static float[,] Generate (int width, int height, float scale, Vector2 offset, Wave[] waves)
    {
        float[,] noiseMap = new float[width, height];
        
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                // Calc. Sample Pos.
                float samplePosX = (float)x * scale + offset.x;
                float samplePosY = (float)y * scale + offset.y;

                float normalization = 0.0f;

                foreach(Wave wave in waves)
                {
                    float waveSamplePosX = samplePosX * wave.frequency + wave.seed;
                    float waveSamplePosY = samplePosY * wave.frequency + wave.seed;

                    noiseMap[x, y] += wave.amplitude * Mathf.PerlinNoise(waveSamplePosX, waveSamplePosY);
                    normalization += wave.amplitude;
                }
                noiseMap[x, y] /= normalization;
            }
        }

        return noiseMap;

    }
}

[System.Serializable]
public class Wave
{

    public float seed;
    public float frequency;
    public float amplitude;

}
