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

    [Header("Audio")]
    public AudioClip greenAudio;
    public AudioClip redAudio;

    private Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();
    private Dictionary<GameObject, AudioSource> objectAudioSources = new Dictionary<GameObject, AudioSource>();
    private Dictionary<GameObject, Coroutine> runningCoroutines = new Dictionary<GameObject, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;

        if (!other.CompareTag("Green") && !other.CompareTag("Red"))
            return;

        SaveOriginalMaterial(obj);

        if (runningCoroutines.ContainsKey(obj))
        {
            StopCoroutine(runningCoroutines[obj]);
            runningCoroutines.Remove(obj);
        }

        if (other.CompareTag("Green"))
        {
            PlayObjectAudio(obj, greenAudio);
            runningCoroutines[obj] =
                StartCoroutine(ChangeMaterialAfterDelay(obj, greenMaterial));
        }
        else
        {
            PlayObjectAudio(obj, redAudio);
            runningCoroutines[obj] =
                StartCoroutine(ChangeMaterialAfterDelay(obj, redMaterial));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;

        if (objectAudioSources.ContainsKey(obj))
        {
            AudioSource source = objectAudioSources[obj];

            if (source != null)
            {
                source.Stop();
            }
        }
    }

    IEnumerator ChangeMaterialAfterDelay(GameObject obj, Material targetMaterial)
    {
        yield return new WaitForSeconds(materialChangeDelay);

        if (obj == null)
            yield break;

        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer == null)
            renderer = obj.GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            renderer.material = targetMaterial;
        }

        if (runningCoroutines.ContainsKey(obj))
            runningCoroutines.Remove(obj);
    }

    void SaveOriginalMaterial(GameObject obj)
    {
        if (originalMaterials.ContainsKey(obj))
            return;

        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer == null)
            renderer = obj.GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            originalMaterials.Add(obj, renderer.material);
        }
    }

    void PlayObjectAudio(GameObject obj, AudioClip clip)
    {
        if (clip == null)
            return;

        AudioSource source;

        if (!objectAudioSources.TryGetValue(obj, out source))
        {
            source = obj.GetComponent<AudioSource>();

            if (source == null)
                source = obj.AddComponent<AudioSource>();

            objectAudioSources.Add(obj, source);
        }

        source.Stop();
        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    public void ResetAllObjects()
    {
        // Stop all delayed material changes
        StopAllCoroutines();
        runningCoroutines.Clear();

        // Restore original materials
        foreach (var item in originalMaterials)
        {
            if (item.Key == null)
                continue;

            Renderer renderer = item.Key.GetComponent<Renderer>();

            if (renderer == null)
                renderer = item.Key.GetComponentInChildren<Renderer>();

            if (renderer != null)
            {
                renderer.material = item.Value;
            }
        }

        // Stop all looping audio
        foreach (var item in objectAudioSources)
        {
            if (item.Value == null)
                continue;

            item.Value.Stop();
            item.Value.clip = null;
            item.Value.loop = false;
        }

        // Force trigger to fire again
        StartCoroutine(ResetTriggerState());

        Debug.Log("Trigger Manager Reset Complete");
    }

    IEnumerator ResetTriggerState()
    {
        Collider trigger = GetComponent<Collider>();

        if (trigger != null)
        {
            trigger.enabled = false;
            yield return null;
            trigger.enabled = true;
        }
    }
}