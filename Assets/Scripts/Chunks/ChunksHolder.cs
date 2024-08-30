using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChunksHolder : MonoBehaviour 
{
    [SerializeField] int chunkSpawnAmount;
    [SerializeField] bool DebugGenerationTime;
    public Chunk myChunkPrefab;
    Dictionary<Vector2Int, ChunkData> chunksDataDictionary = new Dictionary<Vector2Int, ChunkData>();
    List<Chunk> SpawnedChunks = new List<Chunk>();
    Vector2Int lastPlayerChunkNum = Vector2Int.zero;

    Stopwatch stopwatch = new Stopwatch();
    
    public void SpawnChunks()
    {
        CreateChunksDataIfNecessary();
        StartCoroutine(SpawnChunksRoutine());
    }
    void CreateChunksDataIfNecessary()
    {
        if (DebugGenerationTime)
        {
            stopwatch.Start();
        }

        for (int x = lastPlayerChunkNum.x - chunkSpawnAmount - 1; x < lastPlayerChunkNum.x + chunkSpawnAmount + 1; x++)
        {
            for (int z = lastPlayerChunkNum.y - chunkSpawnAmount - 1; z < lastPlayerChunkNum.y + chunkSpawnAmount + 1; z++)
            {
                Vector2Int chunkPos = new Vector2Int(x * Chunk.Width, z * Chunk.Width);
                if (chunksDataDictionary.ContainsKey(chunkPos)) continue;
                ChunkData chunkData = new ChunkData(chunkPos);
                chunksDataDictionary.Add(chunkPos, chunkData);

            }
        }
        if (DebugGenerationTime)
        {
            stopwatch.Stop();
            UnityEngine.Debug.Log("Terrain generation takes " + stopwatch.ElapsedMilliseconds + " ms. Average time is " + stopwatch.ElapsedMilliseconds / (float)(chunkSpawnAmount * chunkSpawnAmount * 4) + " ms");
            stopwatch.Reset();
        }
    }
  
    IEnumerator SpawnChunksRoutine()
    {
        for (int x = lastPlayerChunkNum.x - chunkSpawnAmount; x < lastPlayerChunkNum.x + chunkSpawnAmount; x++)
        {
            for (int z = lastPlayerChunkNum.y - chunkSpawnAmount; z < lastPlayerChunkNum.y + chunkSpawnAmount; z++)
            {
                Vector2Int chunkPos = new Vector2Int(x * Chunk.Width, z * Chunk.Width);
                ChunkData chunkData = chunksDataDictionary[chunkPos];

                if (chunkData.HasSpawnedChunk) continue;

                Chunk spawnedChunk = Instantiate(myChunkPrefab, new Vector3(x * Chunk.Width, 0, z * Chunk.Width), Quaternion.identity);
                SpawnedChunks.Add(spawnedChunk);
                chunkData.HasSpawnedChunk = true;
            }
        }
       
        foreach (Chunk chunk in SpawnedChunks)
        {
            if (chunk.Generated && lastPlayerChunkNum.x + chunkSpawnAmount != chunk.ChunkPos.x / Chunk.Width && lastPlayerChunkNum.y + chunkSpawnAmount != chunk.ChunkPos.y / Chunk.Width) continue;
            chunk.SetData(GetChunkDataByPos(chunk.ChunkPos));
            chunk.GenerateMesh();
            yield return new WaitForSeconds(0.01f);
        }
    }
    public ChunkData GetChunkDataByPos(Vector2Int key)
    {
        if (chunksDataDictionary.TryGetValue(key, out ChunkData chunk))
        {
            return chunk;
        }
        return null;
    }

}
