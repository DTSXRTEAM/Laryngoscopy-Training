using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    [Tooltip("Delay before disabling the green-tagged object")]
    public float disableDelay = 2f;

    [Header("Materials")]
    public Material greenMaterial;
    public Material redMaterial;

    [Header("Audio Clips")]
    public AudioClip greenAudio;
    public AudioClip redAudio;

    private Dictionary<GameObject, AudioSource> objectAudioSources = new Dictionary<GameObject, AudioSource>();
    private HashSet<GameObject> disabledObjects = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        GameObject hitObject = other.gameObject;

        if (other.CompareTag("Green"))
        {
            Debug.Log("Green triggered: " + hitObject.name);

            SetObjectMaterial(hitObject, greenMaterial);
            PlayObjectAudio(hitObject, greenAudio, true);

            if (!disabledObjects.Contains(hitObject))
            {
                disabledObjects.Add(hitObject);
                StartCoroutine(DisableAfterDelay(hitObject));
            }
        }
        else if (other.CompareTag("Red"))
        {
            Debug.Log("Red triggered: " + hitObject.name);

            SetObjectMaterial(hitObject, redMaterial);
            PlayObjectAudio(hitObject, redAudio, true);
            // Red objects stay active
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject hitObject = other.gameObject;

        // Stop audio immediately when collider exits
        if (objectAudioSources.ContainsKey(hitObject) && objectAudioSources[hitObject] != null)
        {
            objectAudioSources[hitObject].Stop();
        }
    }

    private IEnumerator DisableAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(disableDelay);

        if (obj != null)
        {
            // Stop audio before disabling
            if (objectAudioSources.ContainsKey(obj) && objectAudioSources[obj] != null)
            {
                objectAudioSources[obj].Stop();
            }

            obj.SetActive(false);
            Debug.Log(obj.name + " disabled after " + disableDelay + " seconds.");

            disabledObjects.Remove(obj);
        }
    }

    private void SetObjectMaterial(GameObject obj, Material mat)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && mat != null)
        {
            renderer.material = mat;
        }
    }

    private void PlayObjectAudio(GameObject obj, AudioClip clip, bool loop)
    {
        if (!objectAudioSources.ContainsKey(obj) || objectAudioSources[obj] == null)
        {
            AudioSource source = obj.GetComponent<AudioSource>();
            if (source == null) source = obj.AddComponent<AudioSource>();
            objectAudioSources[obj] = source;
        }

        AudioSource audioSource = objectAudioSources[obj];
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }
}
