using System.Collections.Generic;
using UnityEngine;

public class BuffersForMarchingCubes 
{
	public readonly ComputeBuffer trianglesBuffer;
	public readonly ComputeBuffer weightsBuffer;
	public readonly ComputeBuffer trianglesCountBuffer;
	public BuffersForMarchingCubes(int chunkWidth, int chunkHeight)
    {
		trianglesBuffer = new ComputeBuffer(chunkWidth * chunkHeight * chunkWidth * 5, sizeof(float) * 3 * 3 + sizeof(float) *2* 3, ComputeBufferType.Append);
		weightsBuffer = new ComputeBuffer(chunkWidth * chunkHeight * chunkWidth + (chunkHeight * (2 * chunkWidth + 1)), sizeof(float));
		trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
	}
    public void Dispose()
	{
		trianglesBuffer.Release();
		weightsBuffer.Release();
		trianglesCountBuffer.Release();
	}
}
public class GPUMarchingCubes : MarchingCubes
{
	ComputeShader marchCubesShader;
	readonly int kernelIndex;

	readonly int shadergroupesWidth;
	readonly int shaderGroupesHeight;
	const int ShaderThreadsX = 8;
	const int ShaderThreadsY = 8;
	public GPUMarchingCubes(float IsoLvl, int width, int height) : base(IsoLvl, width, height)
	{
		marchCubesShader = World.instance.ComputeShadersHolder.MarchingCubesShader;
		kernelIndex = marchCubesShader.FindKernel("March");
		shadergroupesWidth = Mathf.CeilToInt((float)ChunkWidth / ShaderThreadsX);
		shaderGroupesHeight = Mathf.CeilToInt((float)ChunkHeight / ShaderThreadsY);
	}

    public override void MarchCubesTerrain(MeshData meshData, Chunk chunk)
    {
		BuffersForMarchingCubes buffers = chunk.BuffersForGPUMarch;
		float[] weights = chunk.GetWeightsForMarching();

		buffers.weightsBuffer.SetData(weights);  
		marchCubesShader.SetFloat("IsoLevel", IsoLevel);
		marchCubesShader.SetInt("ChunkWidth", ChunkWidth);
		marchCubesShader.SetInt("ChunkHeight", ChunkHeight);
		marchCubesShader.SetBuffer(kernelIndex, "triangles", buffers.trianglesBuffer);
		marchCubesShader.SetBuffer(kernelIndex, "weights", buffers.weightsBuffer);
		 
		marchCubesShader.Dispatch(kernelIndex, shadergroupesWidth, shaderGroupesHeight, shadergroupesWidth);

		ComputeBuffer.CopyCount(buffers.trianglesBuffer, buffers.trianglesCountBuffer, 0);
		int[] trianglesCountArray = new int[1];
		buffers.trianglesCountBuffer.GetData(trianglesCountArray);
		int trianglesCount = trianglesCountArray[0];

		Triangle[] triangles = new Triangle[trianglesCount];
		buffers.trianglesBuffer.GetData(triangles, 0, 0, trianglesCount);
		buffers.trianglesBuffer.GetData(triangles, 0, 0, trianglesCount);
		meshData.triangles = new List<Triangle>(triangles);

	}
	
}
