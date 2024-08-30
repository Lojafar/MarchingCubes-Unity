using UnityEngine;

[CreateAssetMenu(fileName = "ComputeShadersHolder", menuName = "ScriptableObjects/ComputeShadersHolder")]
public class ComputeShadersHolder : ScriptableObject 
{
    [SerializeField] ComputeShader terrainGenShader;
    public ComputeShader TerrainGeneratorShader => terrainGenShader;

    [SerializeField] ComputeShader marchingCubesShader;
    public ComputeShader MarchingCubesShader => marchingCubesShader;

    
}
