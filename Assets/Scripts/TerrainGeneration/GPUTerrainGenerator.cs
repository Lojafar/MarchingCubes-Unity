using UnityEngine;

public class BufferForTerrainGen
{
	public readonly ComputeBuffer weightsBuffer;
	public BufferForTerrainGen(int weightsBufferSize)
	{
		weightsBuffer = new ComputeBuffer(weightsBufferSize, sizeof(float));
	}
}
public class GPUTerrainGenerator : TerrainGenerator
{
    ComputeShader terrainGenShader;
	readonly int kernelIndex;
	int shaderGroupesWidth;
	int shaderGroupesHeight;

	const int ShaderThreadsX = 8;
	const int ShaderThreadsY = 8;
	public GPUTerrainGenerator(int chunkWidth, int chunkHeight) : base(chunkWidth, chunkHeight) 
	{
		terrainGenShader = World.instance.ComputeShadersHolder.TerrainGeneratorShader;
		kernelIndex = terrainGenShader.FindKernel("GenerateTerrain");

		 shaderGroupesWidth = Mathf.CeilToInt((float)ChunkWidth / ShaderThreadsX);
		 shaderGroupesHeight = Mathf.CeilToInt((float)ChunkHeight / ShaderThreadsY);
	}

    public override float[] GenerateTerrainWeights(Vector3Int chunkPos, NoiseSettings noiseSettings)
	{
		if (noiseSettings == null) noiseSettings = deffaultNoiseSettings;

		//	BufferForTerrainGen buffer = new BufferForTerrainGen(TerrainArraySize);
		ComputeBuffer weightsBuffer = new ComputeBuffer(TerrainArraySize, sizeof(float));
		terrainGenShader.SetInt("ChunkWidth", ChunkWidth);
		terrainGenShader.SetInt("ChunkHeight", ChunkHeight);
		terrainGenShader.SetInt("ChunkPosX", chunkPos.x);
		terrainGenShader.SetInt("ChunkPosY", chunkPos.y);
		terrainGenShader.SetInt("ChunkPosZ", chunkPos.z);
		terrainGenShader.SetFloat("NoiseScale", noiseSettings.NoiseScale);
		terrainGenShader.SetFloat("Amplitude", noiseSettings.Amplitude);
		terrainGenShader.SetFloat("Frequency", noiseSettings.Frequency);
		terrainGenShader.SetFloat("Octaves", noiseSettings.Octaves);
		terrainGenShader.SetFloat("GroundPercent", noiseSettings.GroundPercent);
		terrainGenShader.SetBuffer(kernelIndex, "weights", weightsBuffer);

		terrainGenShader.Dispatch(kernelIndex, shaderGroupesWidth, shaderGroupesHeight, shaderGroupesWidth);

		float[] resultWeights = new float[ChunkWidth * ChunkHeight * ChunkWidth];
		weightsBuffer.GetData(resultWeights);

		weightsBuffer.Release();
		return resultWeights;

	}
}
