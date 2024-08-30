using UnityEngine;

public class ChunkData
{
    float[] Weights;
    Vector2Int chunkPos;
    Vector2Int ChunkPos => chunkPos;

    public bool HasSpawnedChunk;
    public ChunkData(Vector2Int chunkPos)
    {
        this.chunkPos = chunkPos;
        Weights = World.instance.terrainGenerator.GenerateTerrainWeights(new Vector3Int(this.chunkPos.x, 0, this.chunkPos.y));
    }
    public float GetWeight(int index)
    {
        return Weights[index];
    }
    public float GetWeightByPos(Vector3Int pos)
    {
        int index = pos.x + Chunk.Width * pos.z + pos.y * Chunk.Width * Chunk.Width;
        if (index < Weights.Length && index > 0) 
        {
            return Weights[pos.x + Chunk.Width * pos.z + pos.y * Chunk.Width * Chunk.Width]; 
        }
        else
        {
            return 0.01f;
        }
    }
    
}
