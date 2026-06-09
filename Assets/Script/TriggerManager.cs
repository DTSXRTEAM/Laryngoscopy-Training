using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    [Tooltip("Delay before disabling the triggered object")]
    public float delay = 2f;

    // Keep track of objects already disabled
    private HashSet<GameObject> disabledObjects = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the tag "Bell"
        if (!other.CompareTag("Bell")) return;

        GameObject hitObject = other.gameObject;

        // Prevent disabling the same object multiple times
        if (disabledObjects.Contains(hitObject)) return;

        Debug.Log("Triggered: " + hitObject.name);

        disabledObjects.Add(hitObject);
        StartCoroutine(DisableAfterDelay(hitObject));
    }

    private IEnumerator DisableAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            obj.SetActive(false);
            Debug.Log(obj.name + " disabled after " + delay + " seconds.");
        }
    }
}
