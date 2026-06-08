using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    public List<GameObject> objectsToDisable; // Assign B, C, D
    public float delay = 2f;

    private HashSet<GameObject> disabledObjects = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        GameObject hitObject = other.gameObject;

        // Check if this object is in our list
        if (!objectsToDisable.Contains(hitObject)) return;

        // Prevent disabling same object multiple times
        if (disabledObjects.Contains(hitObject)) return;

        Debug.Log("Touched: " + hitObject.name);

        disabledObjects.Add(hitObject);
        StartCoroutine(DisableAfterDelay(hitObject));
    }

    IEnumerator DisableAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
            obj.SetActive(false);
    }
}