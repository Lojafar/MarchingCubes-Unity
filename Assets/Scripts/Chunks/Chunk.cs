using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] ComputeShader marchCubesShader;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    public const int Width = 16;
    public const int Height = 32;

    ChunkData rightBackChunkData, rightFrontChunkData, leftFrontChunkData, leftBackChunkData;
    ChunkData rightChunkData, leftChunkData, frontChunkData, backChunkData;
    BuffersForMarchingCubes buffersForGPUMarch;
    public BuffersForMarchingCubes BuffersForGPUMarch => buffersForGPUMarch;
    ChunkData chunkData;

    public Vector2Int ChunkPos { get; private set; }
    public bool Generated { get; private set; }
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        ChunkPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

        if (World.instance.GenerationType == GenerationType.GPU)
        buffersForGPUMarch = new BuffersForMarchingCubes(Width, Height);
    }
    private void OnDestroy()
    {
        if (buffersForGPUMarch != null)
        {
            buffersForGPUMarch.Dispose();
        }
    }
    public void SetData(ChunkData chunkData)
    {
        this.chunkData = chunkData;
       
        rightBackChunkData = World.instance.ChunksHolder.GetChunkDataByPos(new Vector2Int(ChunkPos.x + Width, ChunkPos.y - Width));
        rightFrontChunkData = World.instance.ChunksHolder.GetChunkDataByPos(new Vector2Int(ChunkPos.x + Width, ChunkPos.y + Width));
        leftFrontChunkData = World.instance.ChunksHolder.GetChunkDataByPos(new Vector2Int(ChunkPos.x - Width, ChunkPos.y + Width));
        leftBackChunkData = World.instance.ChunksHolder.GetChunkDataByPos(new Vector2Int(ChunkPos.x - Width, ChunkPos.y - Width));
        leftChunkData = World.instance.ChunksHolder.GetChunkDataByPos(new Vector2Int(ChunkPos.x - Width, ChunkPos.y));
        rightChunkData = World.instance.ChunksHolder.GetChunkDataByPos(new Vector2Int(ChunkPos.x + Width, ChunkPos.y));
        backChunkData = World.instance.ChunksHolder.GetChunkDataByPos(new Vector2Int(ChunkPos.x, ChunkPos.y - Width));
        frontChunkData = World.instance.ChunksHolder.GetChunkDataByPos(new Vector2Int(ChunkPos.x, ChunkPos.y + Width));
    }
    public void GenerateMesh()
    {
        meshFilter.mesh = World.instance.marchingCubes.GenerateMarch(this);
        meshCollider.sharedMesh = meshFilter.mesh;
        Generated = true;
    }
    public float[] GetWeightsForMarching() // Returns an array of weights, but with the addition of the weights of neighboring chunks
    {
        int WidthForThisCase = Width + 1;
        float[] weights = new float[Width * Height * Width + (Height * (2 * Width + 1))];
        
        for (int x = 0; x < WidthForThisCase; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < WidthForThisCase; z++)
                {
                    weights[x + WidthForThisCase * z + y * WidthForThisCase * WidthForThisCase] = GetWeightByPos(new Vector3Int(x, y, z));
                }
            }
        }
        return weights;
    }
    public float GetWeightByPos(Vector3Int pos)
    {
        if (pos.x < Width && pos.x > -1 && pos.y < Height && pos.y > -1 && pos.z < Width && pos.z > -1)
        {
            return chunkData.GetWeight(pos.x + Width * pos.z + pos.y * Width * Width);
        }
        else
        {
            if (pos.y >= Height || pos.y < 0) return 0.01f;
            ChunkData neighbourChunkData = null;
            int posX = pos.x;
            int posZ = pos.z;

            if (posX >= Width && posZ >= Width)
            {
                posX -= Width;
                posZ -= Width;
                neighbourChunkData = rightFrontChunkData;
            }
            else if (posX >= Width && posZ < 0)
            {
                posX -= Width;
                posZ += Width;
                neighbourChunkData = rightBackChunkData;
            }
            else if (posX < 0 && posZ >= Width)
            {
                posX += Width;
                posZ -= Width;
                neighbourChunkData = leftFrontChunkData;
            }
            else if (posX < 0 && posZ < 0)
            {
                posX += Width;
                posZ += Width;
                neighbourChunkData = leftBackChunkData;
            }
            else if (posX < 0 && posZ >= 0 && posZ < Width)
            {
                posX += Width;
                neighbourChunkData = leftChunkData;
            }
            else if (posX >= Width && posZ >= 0 && posZ < Width)
            {
                posX -= Width;
                neighbourChunkData = rightChunkData;
            }
            else if (posX >= 0 && posX < Width && posZ < 0)
            {
                posZ += Width;
                neighbourChunkData = backChunkData;
            }
            else if (posX >= 0 && posX < Width && posZ >= Width)
            {
                posZ -= Width;
                neighbourChunkData = frontChunkData;
            }

            if (neighbourChunkData != null)
            {
                return neighbourChunkData.GetWeightByPos(new Vector3Int(posX, pos.y, posZ));
            }
            else
            {
                return 0.01f;
            }
        }
    }
}
