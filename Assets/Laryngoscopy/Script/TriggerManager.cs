using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    [Header("Material Change Delay")]
    public float materialChangeDelay = 2f;

    [Header("Materials")]
    public Material greenMaterial;
    public Material redMaterial;

    [Header("Audio Clips")]
    public AudioClip greenAudio;
    public AudioClip redAudio;

    private Dictionary<GameObject, AudioSource> objectAudioSources =
        new Dictionary<GameObject, AudioSource>();

    private Dictionary<GameObject, Material> originalMaterials =
        new Dictionary<GameObject, Material>();

    private void OnTriggerEnter(Collider other)
    {
        GameObject hitObject = other.gameObject;

        if (other.CompareTag("Green"))
        {
            SaveOriginalMaterial(hitObject);

            PlayObjectAudio(hitObject, greenAudio);

            StartCoroutine(
                ChangeMaterialAfterDelay(
                    hitObject,
                    greenMaterial
                )
            );
        }
        else if (other.CompareTag("Red"))
        {
            SaveOriginalMaterial(hitObject);

            PlayObjectAudio(hitObject, redAudio);

            StartCoroutine(
                ChangeMaterialAfterDelay(
                    hitObject,
                    redMaterial
                )
            );
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject hitObject = other.gameObject;

        if (objectAudioSources.ContainsKey(hitObject))
        {
            AudioSource source = objectAudioSources[hitObject];

            if (source != null)
            {
                source.Stop();
            }
        }
    }

    private IEnumerator ChangeMaterialAfterDelay(
        GameObject obj,
        Material mat)
    {
        yield return new WaitForSeconds(materialChangeDelay);

        if (obj == null)
            yield break;

        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer == null)
        {
            renderer = obj.GetComponentInChildren<Renderer>();
        }

        if (renderer != null && mat != null)
        {
            renderer.material = mat;
        }
    }

    private void SaveOriginalMaterial(GameObject obj)
    {
        if (originalMaterials.ContainsKey(obj))
            return;

        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer == null)
        {
            renderer = obj.GetComponentInChildren<Renderer>();
        }

        if (renderer != null)
        {
            originalMaterials.Add(
                obj,
                renderer.material
            );
        }
    }

    private void PlayObjectAudio(
        GameObject obj,
        AudioClip clip)
    {
        if (clip == null)
            return;

        if (!objectAudioSources.ContainsKey(obj))
        {
            AudioSource source =
                obj.GetComponent<AudioSource>();

            if (source == null)
            {
                source =
                    obj.AddComponent<AudioSource>();
            }

            objectAudioSources.Add(obj, source);
        }

        AudioSource audioSource =
            objectAudioSources[obj];

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void ResetAllObjects()
    {
        foreach (var item in originalMaterials)
        {
            if (item.Key == null)
                continue;

            Renderer renderer =
                item.Key.GetComponent<Renderer>();

            if (renderer == null)
            {
                renderer =
                    item.Key.GetComponentInChildren<Renderer>();
            }

            if (renderer != null)
            {
                renderer.material = item.Value;
            }
        }

        foreach (var item in objectAudioSources)
        {
            if (item.Value != null)
            {
                item.Value.Stop();
            }
        }

        Debug.Log("Trigger Objects Reset");
    }
}