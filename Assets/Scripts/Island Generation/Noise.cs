﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
    // Simple Perlin Noise Generator
    public static float[,] Generate(int width, int height, float xOff, float yOff, float scale, float influence)
    {
        if(influence < 0)
            throw new System.Exception();

        float[,] map = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = (x + xOff) / scale;
                float dy = (y + yOff) / scale;

                map[x, y] = Mathf.PerlinNoise(dx, dy) * influence;
            }
        }

        return map;
    }

    // Perlin Noise Generator w/ Texture Influence (uses alpha of pixels)
    public static float[,] Generate(Texture2D txt, int padding, float xOff, float yOff, float baseIntesity, float txtIntensity, float extScale, float intScale)
    {
        int w = txt.width + (padding * 2) + 2;
        int h = txt.height + (padding * 2) + 2;

        int halfTxtW = txt.width / 2;
        int halfTxtH = txt.height / 2;

        int halfFullW = w / 2;
        int halfFullH = h / 2;

        int txtX;
        int txtY;

        float[,] map = Generate(w, h, xOff, yOff, extScale, baseIntesity);

        for (int y = halfFullH - halfTxtH; y < halfFullH + halfTxtH; y++)
        {
            for (int x = halfFullW - halfTxtW; x < halfFullW + halfTxtW; x++)
            {
                txtX = (x + halfTxtW) - halfFullW;
                txtY = (y + halfTxtH) - halfFullH;

                float dx = (x + xOff) / intScale;
                float dy = (y + yOff) / intScale;

                float sample = txt.GetPixel(txtX, txtY).a;

                map[x, y] += sample * Mathf.PerlinNoise(dx,dy) * txtIntensity;
            }
        }

        return map;
    }
}

/*
public static float[,] Generate(int width, int height, float scale, int octaves, float lacunarity, float persistence)
{
    if (scale <= 0)
        scale = 0.0001f;
    // Disperse octaves around the perlin noise. The scroll param allows offsetting traversal in the editor. 

    // Create noise map.
    float[,] map = new float[width, height];

    float maxNoiseHeight = float.MinValue;
    float minNoiseHeight = float.MaxValue;

    // Centering noise scaling.
    float halfWidth = width * 0.5f;
    float halfHeight = height * 0.5f;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            float frequency = 1;
            float amplitude = 1;
            float noiseHeight = 0;

            for (int i = 0; i < octaves; i++)
            {
                // Collect a sample coordinate based on frequency that increases per octave.
                float sampleX = (((x - halfWidth) / scale) * frequency);
                float sampleY = (((y - halfHeight) / scale) * frequency);

                // Collect sample. The * 2 - 1 allows [-1, 1] range.
                float sample = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                // Add noiseheight value based on sample and of increasing amplitude/complexity from persistence.
                noiseHeight += (sample * amplitude);

                frequency *= lacunarity;
                amplitude *= persistence;
            }

            if (noiseHeight > maxNoiseHeight)
                maxNoiseHeight = noiseHeight;
            else if (noiseHeight < minNoiseHeight)
                minNoiseHeight = noiseHeight;

            map[x, y] = noiseHeight;
        }
    }

    // Normalizes map
    for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x, y]);

    return map;
}

//////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////BEWARE/////////////////////////////////////////////
//////////////////////////////THE DARK ARTS LIVE BELOW////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////



public static float[,] Generate(int width, int height, System.Random rnd, float scale, int octaves, float lacunarity, float persistence, Vector2 scroll, Texture2D[] radialNoises)
{
    if (scale <= 0)
        scale = 0.0001f;

    Texture2D radialNoise = radialNoises[rnd.Next(0, radialNoises.Length)];

    // Every octave will be placed in a random location of our perlin noise.
    Vector2[] octaveOffsets = new Vector2[octaves];

    // Disperse octaves around the perlin noise. The scroll param allows offsetting traversal in the editor. 
    for (int i = 0; i < octaves; i++)
        octaveOffsets[i] = new Vector2(rnd.Next(-100000, 100000) + scroll.x, rnd.Next(-100000, 100000) + scroll.y);

    // Create noise map.
    float[,] map = new float[width, height];

    float maxNoiseHeight = float.MinValue;
    float minNoiseHeight = float.MaxValue;

    // Centering noise scaling.
    float halfWidth = width * 0.5f;
    float halfHeight = height * 0.5f;
    int halfRadialW = radialNoise.width / 2;
    int halfRadialH = radialNoise.height / 2;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            float frequency = 1;
            float amplitude = 1;
            float noiseHeight = 0;

            for (int i = 0; i < octaves; i++)
            {
                // Collect a sample coordinate based on frequency that increases per octave.
                float sampleX = (((x - halfWidth) / scale) * frequency) + octaveOffsets[i].x;
                float sampleY = (((y - halfHeight) / scale) * frequency) + octaveOffsets[i].y;

                // Centering radial noise.
                int rx = (halfRadialW - x) + (int)halfWidth;
                int ry = (halfRadialH - y) + (int)halfHeight;

                // Collect sample. The * 2 - 1 allows [-1, 1] range.
                float sample = (Mathf.PerlinNoise(sampleX, sampleY) * radialNoise.GetPixel(rx, ry).a) * 2 - 1;

                // Add noiseheight value based on sample and of increasing amplitude/complexity from persistence.
                noiseHeight += (sample * amplitude);

                frequency *= lacunarity;
                amplitude *= persistence;
            }

            if (noiseHeight > maxNoiseHeight)
                maxNoiseHeight = noiseHeight;
            else if (noiseHeight < minNoiseHeight)
                minNoiseHeight = noiseHeight;

            map[x, y] = noiseHeight;
        }
    }

    // Normalizes map
    for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x, y]);

    return map;
}

*/
