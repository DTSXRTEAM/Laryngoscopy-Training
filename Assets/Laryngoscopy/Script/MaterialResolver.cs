using UnityEngine;

public static class MaterialResolver
{
    public static Material GetMaterial(string id)
    {
        switch (id)
        {
            case "EtTube":
                return MaterialLibrary.Instance.etTubeMaterial;

            case "Manikin":
                return MaterialLibrary.Instance.manikinMaterial;

            case "Stylet":
                return MaterialLibrary.Instance.styletMaterial;

            case "Syringe":
                return MaterialLibrary.Instance.syringeMaterial;

            default:
                return null;
        }
    }
}