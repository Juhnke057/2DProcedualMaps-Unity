using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MapDisplay
{
    Height,
    Moisture,
    Heat,
    Biome
}

[ExecuteInEditMode]
public class Map : MonoBehaviour
{
    public MapDisplay displayType;

    public RawImage debugImage;

    public BiomePreset[] biomes;

    public GameObject tilePrefab;

    [Header("Dimensions")]
    public int width;
    public int height;
    public float scale;
    public Vector2 offset;

    [Header("Height Map")]
    public Wave[] heightWaves;
    public Gradient heightDebugColors;
    public float[,] heightMap;

    [Header("Moisture Map")]
    public Wave[] moistureWaves;
    public Gradient moistureDebugColors;
    public float[,] moistureMap;

    [Header("Heat Map")]
    public Wave[] heatWaves;
    public Gradient heatDebugColors;
    public float[,] heatMap;


    private float lastGenerateTime;

    private void Start()
    {
        if (Application.isPlaying)
        GenerateMap();
    }

    private void Update()
    {
        if (Application.isPlaying)
            return;
        if(Time.time - lastGenerateTime > 0.1f)
        {
            lastGenerateTime = Time.time;
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        heightMap = NoiseGenerator.Generate(width, height, scale, offset, heightWaves);

        moistureMap = NoiseGenerator.Generate(width, height, scale, offset, moistureWaves);

        heatMap = NoiseGenerator.Generate(width, height, scale, offset, heatWaves);

        Color[] pixels = new Color[width * height];
        int i = 0;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                switch (displayType)
                {
                    case MapDisplay.Height:
                        pixels[i] = heightDebugColors.Evaluate(heightMap[x, y]);
                        break;
                    case MapDisplay.Moisture:
                        pixels[i] = moistureDebugColors.Evaluate(moistureMap[x, y]);
                        break;
                    case MapDisplay.Heat:
                        pixels[i] = heatDebugColors.Evaluate(heatMap[x, y]);
                        break;
                    case MapDisplay.Biome:
                        {
                            BiomePreset biome = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y]);
                            pixels[i] = biome.debugColor;


                            if (Application.isPlaying)
                            {
                                GameObject tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                                tile.GetComponent<SpriteRenderer>().sprite = biome.GetTileSprite();

                            }

                            break;


                        }
                }

                i++;
            }
        }
        Texture2D tex = new Texture2D(width, height);
        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        debugImage.texture = tex;
    }
    BiomePreset GetBiome(float height, float moisture, float heat)
    {
        BiomePreset biomeToReturn = null;
        List<BiomePreset> tempBiomes = new List<BiomePreset>();

        foreach(BiomePreset biome in biomes)
        {
            if(biome.MatchCondition(height, moisture, heat))
            {
                tempBiomes.Add(biome);
            }
        }

        float curValue = 0.0f;

        foreach(BiomePreset biome in tempBiomes)
        {
            float diffValue = (height - biome.minHeight) + (moisture - biome.minMoisture) + (heat - biome.minHeat);

            if(biomeToReturn == null)
            {
                biomeToReturn = biome;
                curValue = diffValue;

            }
            else if(diffValue < curValue)
            {
                biomeToReturn = biome;
                curValue = diffValue;
            }

        }
        if(biomeToReturn == null)
        {
            return biomes[0];
        }

        return biomeToReturn;
    }

}

