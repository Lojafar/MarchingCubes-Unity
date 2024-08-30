using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] GenerationType generationType;
    public GenerationType GenerationType => generationType;
    public TerrainGenerator terrainGenerator;
    public MarchingCubes marchingCubes;
    
    [SerializeField] ChunksHolder chunksHolder;
    public ChunksHolder ChunksHolder => chunksHolder;

    [SerializeField] ComputeShadersHolder computeShadersHolder;
    public ComputeShadersHolder ComputeShadersHolder => computeShadersHolder;

    public static World instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        terrainGenerator = TerrainGenerator.CreateTerrainGeneratorObj(generationType, Chunk.Width, Chunk.Height);
        marchingCubes = MarchingCubes.CreateMarchCubesObj(generationType, 0.5f, Chunk.Width, Chunk.Height);
    }
    void Start()
    {
        ChunksHolder.SpawnChunks();
    }
}
