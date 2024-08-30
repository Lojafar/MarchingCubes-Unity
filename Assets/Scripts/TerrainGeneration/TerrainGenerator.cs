using UnityEngine;  
public class NoiseSettings
{
    public readonly float NoiseScale;
    public readonly float Amplitude;
    public readonly float Frequency;
    public readonly int Octaves;
    public readonly float GroundPercent;
    public NoiseSettings(float noiseScale, float amplitude, float frequency, int octaves, float groundPercent)
    {
        NoiseScale = noiseScale;
        Amplitude = amplitude;
        Frequency = frequency;
        Octaves = octaves;
        GroundPercent = groundPercent;
    }
}
public abstract class TerrainGenerator
{
    public readonly int ChunkWidth;
    public readonly int ChunkHeight;
    public readonly NoiseSettings deffaultNoiseSettings ;
    public int TerrainArraySize => ChunkWidth * ChunkHeight * ChunkWidth;

    public TerrainGenerator(int chunkWidth, int chunkHeight)
    {
        ChunkWidth = chunkWidth;
        ChunkHeight = chunkHeight;
        deffaultNoiseSettings = new NoiseSettings(1, 7, 0.034f, 8, 0.4f);
    }
    public abstract float[] GenerateTerrainWeights(Vector3Int chunkPos, NoiseSettings noiseSettings = null );

    public static TerrainGenerator CreateTerrainGeneratorObj(GenerationType generationType, int chunkWidth, int chunkHeight)
    {
        switch (generationType)
        {
            case GenerationType.CPU:
                return new CPUTerrainGenerator(chunkWidth, chunkHeight);
            case GenerationType.GPU:
                return new GPUTerrainGenerator(chunkWidth, chunkHeight);
        }
        Debug.LogError("This generation type is not setted for terrain generator! Type: " + generationType);
        return null;
    }
}
