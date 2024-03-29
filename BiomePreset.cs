﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName ="Biome Preset", menuName ="New Biome Preset")]
public class BiomePreset : ScriptableObject
{
    public Color debugColor;
    public Sprite[] tileSprites;

    public float minHeight;
    public float minMoisture;
    public float minHeat;

    public bool MatchCondition(float height, float moisture, float heat)
    {
        return height >= minHeight && moisture >= minMoisture && heat >= minHeat;
    }

    public Sprite GetTileSprite()
    {
        return tileSprites[Random.Range(0, tileSprites.Length)];
    }
 
}