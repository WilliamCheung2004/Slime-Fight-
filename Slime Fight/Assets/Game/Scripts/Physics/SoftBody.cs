using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SoftBody : MonoBehaviour
{
    [Range(0.0f, 2f)]        
    public float softness = 0.08f;

    [Range(0.01f, 1.0f)]
    public float damping = 0.1f;

    [Range(0.0f, 1.0f)]
    public float stiffness = 0.2f;

    public bool overrideClothSettings = true;

    private void Start()
    {
        CreateSoftBodyPhysics();
    }

    void CreateSoftBodyPhysics()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
        {
            Debug.LogError("SoftBody requires a SkinnedMeshRenderer with a sharedMesh.");
            return;
        }

        Cloth cloth = GetComponent<Cloth>();
        if (cloth == null)
            cloth = gameObject.AddComponent<Cloth>();

        if (overrideClothSettings)
        {
            cloth.damping = damping;
            cloth.useTethers = true;
        }

        cloth.coefficients = GenerateClothCoefficients(skinnedMeshRenderer.sharedMesh.vertexCount);
    }

    private ClothSkinningCoefficient[] GenerateClothCoefficients(int vertexCount)
    {
        ClothSkinningCoefficient[] coefficients = new ClothSkinningCoefficient[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            coefficients[i].maxDistance = softness;
            coefficients[i].collisionSphereDistance = 0f;
        }

        return coefficients;
    }
}
//