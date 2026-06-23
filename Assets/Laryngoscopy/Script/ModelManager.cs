using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ModelManager : MonoBehaviour
{
    //public Transform spawnRoot;

    private Dictionary<string, GameObject> loadedModels =
        new Dictionary<string, GameObject>();

    public async void ShowModels(ModelData[] models)
    {
        Debug.Log("=================================");
        Debug.Log("ShowModels Called");

        ClearModels();

        foreach (ModelData model in models)
        {
            Debug.Log("Loading : " + model.address);

            var handle =
                Addressables.LoadAssetAsync<GameObject>(
                    model.address);

            GameObject prefab =
                await handle.Task;

            if (prefab == null)
            {
                Debug.LogError(
                    "Failed To Load : " +
                    model.address);

                continue;
            }

            Vector3 position =
                new Vector3(
                    model.position[0],
                    model.position[1],
                    model.position[2]);

            GameObject obj =
                Instantiate(
                    prefab,
                    position,
                    Quaternion.identity);

            ApplyLocalMaterials(obj);

            loadedModels.Add(
                model.address,
                obj);

            Debug.Log(
                "Spawned : " +
                model.address +
                " at " +
                position);
        }

        Debug.Log("=================================");
    }

    private void ApplyLocalMaterials(GameObject root)
    {
        MaterialID id =
            root.GetComponent<MaterialID>();

        if (id == null)
        {
            Debug.LogWarning(
                root.name +
                " does not contain MaterialID");

            return;
        }

        Debug.Log(
            "Material ID : " +
            id.materialID);

        Material localMaterial =
            MaterialResolver.GetMaterial(
                id.materialID);

        if (localMaterial == null)
        {
            Debug.LogError(
                "No Material Found For : " +
                id.materialID);

            return;
        }

        Renderer[] renderers =
            root.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            renderer.material = localMaterial;
        }

        Debug.Log(
            "Assigned Material : " +
            localMaterial.name);
    }

    private void ClearModels()
    {
        foreach (var item in loadedModels)
        {
            Destroy(item.Value);
        }

        loadedModels.Clear();
    }
}