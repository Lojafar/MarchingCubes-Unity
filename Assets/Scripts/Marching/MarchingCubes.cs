using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
	public List<Triangle> triangles = new List<Triangle>();
}
public struct Triangle
{
	public Vector3 vertex1;
	public Vector3 vertex2;
	public Vector3 vertex3;

	public Vector2 uv1;
	public Vector2 uv2;
	public Vector2 uv3;
}

public abstract class MarchingCubes 
{
	public readonly float IsoLevel;
	public readonly int ChunkWidth;
	public readonly int ChunkHeight;
	public MarchingCubes(float isoLevel, int chunkWidth, int chunkHeight)
    {
		IsoLevel = isoLevel;
		ChunkWidth = chunkWidth;
		ChunkHeight = chunkHeight;
	}
	public Mesh GenerateMarch(Chunk chunk)
    {
		MeshData meshData = new MeshData();

		MarchCubesTerrain(meshData, chunk);

		return CreateMesh(meshData);
	}


    public abstract void MarchCubesTerrain(MeshData meshData, Chunk chunk);
	Mesh CreateMesh(MeshData meshData)
	{
		Mesh mesh = new Mesh();

		Vector3[] vertices = new Vector3[meshData.triangles.Count * 3];
		int[] triangles = new int[meshData.triangles.Count * 3];
		Vector2[] uvs = new Vector2[meshData.triangles.Count * 3];

		for (int i = 0; i < meshData.triangles.Count; i++)
		{
			int currentIndex = i * 3;

			vertices[currentIndex] = meshData.triangles[i].vertex1;
			vertices[currentIndex + 1] = meshData.triangles[i].vertex2;
			vertices[currentIndex + 2] = meshData.triangles[i].vertex3;

			triangles[currentIndex] = currentIndex;
			triangles[currentIndex + 1] = currentIndex + 1;
			triangles[currentIndex + 2] = currentIndex + 2;

			uvs[currentIndex] = meshData.triangles[i].uv1;
			uvs[currentIndex + 1] = meshData.triangles[i].uv2;
			uvs[currentIndex + 2] = meshData.triangles[i].uv3;
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		return mesh;
	}
	
    public static MarchingCubes CreateMarchCubesObj(GenerationType generationType, float isoLevel, int chunkWidth, int chunkHeight)
    {
        switch (generationType)
        {
			case GenerationType.CPU:
				return new CPUMarchingCubes(isoLevel, chunkWidth, chunkHeight);
			case GenerationType.GPU:
				return new GPUMarchingCubes(isoLevel, chunkWidth, chunkHeight);
		}
		Debug.LogError("This generation type is not setted for marching cubes! Type: " + generationType);
		return null;
    }
}
