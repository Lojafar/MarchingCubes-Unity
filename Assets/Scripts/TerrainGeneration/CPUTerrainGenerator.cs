using UnityEngine;

public class CPUTerrainGenerator : TerrainGenerator
{
    public CPUTerrainGenerator(int chunkWidth, int chunkHeight) : base(chunkWidth, chunkHeight) { }

    public override float[] GenerateTerrainWeights(Vector3Int chunkPos, NoiseSettings noiseSettings)
    {
        if (noiseSettings == null) noiseSettings = deffaultNoiseSettings;
        float[] weights = new float[TerrainArraySize];
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
        noise.SetFrequency(noiseSettings.Frequency);
        noise.SetFractalOctaves(noiseSettings.Octaves);
        for (int x = 0; x < ChunkWidth; x++) 
        {
            for (int y = 0; y < ChunkHeight; y++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    Vector3 noisePos = new Vector3(chunkPos.x + x, chunkPos.y + y, chunkPos.z + z)  * noiseSettings.NoiseScale;
                    float groundLevel = -noisePos.y + (noiseSettings.GroundPercent * ChunkWidth);
                    float noiseValue = groundLevel + noise.GetNoise(noisePos.x, noisePos.y, noisePos.z) * noiseSettings.Amplitude;

                    weights[x + ChunkWidth * z + y * ChunkWidth * ChunkWidth] = noiseValue;
                }
            }
        }
    
        return weights;
    }
    public static float PerlinNoise3D(float x, float y, float z)
    {
        x += 50;
        y += 70;
        z += 90;

        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, y);
        float zx = Mathf.PerlinNoise(z, x);

        float result = (xy + yz + xz + yx + zy + zx) / 6;
        return result;
    }
}
