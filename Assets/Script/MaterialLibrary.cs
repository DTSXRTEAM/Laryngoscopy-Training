using UnityEngine;

public class MaterialLibrary : MonoBehaviour
{
    public static MaterialLibrary Instance;

    public Material etTubeMaterial;
    public Material manikinMaterial;
    public Material styletMaterial;
    public Material syringeMaterial;

    private void Awake()
    {
        Instance = this;
    }
}