using System;
using UnityEngine;

public class HeadingAnimationController : MonoBehaviour
{
    [Serializable]
    public class HeadingAnimation
    {
        [Header("Heading Index - matches JSON index")]
        public int headingIndex;

        [Header("Animator (leave empty if using prefabAnimator)")]
        public Animator animator;

        [Header("Use Runtime Prefab Animator")]
        public bool usePrefabAnimator;

        [Header("Animation State Name")]
        public string animationClipName;

        [Header("Enable GameObject for this step?")]
        public bool enableObject;

        [Header("Delay before playing (seconds)")]
        public float delayTime = 0f;
    }

    [Header("Runtime Prefab Animator")]
    public Animator prefabAnimator;

    [Header("Single Target GameObject")]
    public GameObject targetObject;

    [Header("Animations - Configure here in Inspector")]
    public HeadingAnimation[] animations;

    private Animator lastAnimator;

    /// <summary>
    /// Plays the animation for the given heading index after its delay.
    /// </summary>
    public void PlayAnimation(int headingIndex)
    {
        foreach (var anim in animations)
        {
            if (anim.headingIndex != headingIndex) continue;

            // Toggle the single GameObject immediately
            if (targetObject != null)
                targetObject.SetActive(anim.enableObject);

            // Start coroutine for delayed animation
            StartCoroutine(PlayAnimationWithDelay(anim));
            return;
        }
    }

    private System.Collections.IEnumerator PlayAnimationWithDelay(HeadingAnimation anim)
    {
        if (anim.delayTime > 0f)
            yield return new WaitForSeconds(anim.delayTime);

        Animator targetAnimator = anim.usePrefabAnimator ? prefabAnimator : anim.animator;

        if (targetAnimator != null)
        {
            lastAnimator = targetAnimator;
            targetAnimator.enabled = true;
            targetAnimator.Rebind();
            targetAnimator.Update(0f);
            targetAnimator.Play(anim.animationClipName, 0, 0f);
        }
    }

    /// <summary>
    /// Sets the animation to its last frame (no delay).
    /// </summary>
    public void SetAnimationToLastFrame(int headingIndex)
    {
        foreach (var anim in animations)
        {
            if (anim.headingIndex != headingIndex) continue;

            Animator targetAnimator = anim.usePrefabAnimator ? prefabAnimator : anim.animator;

            if (targetAnimator != null)
            {
                targetAnimator.enabled = true;
                targetAnimator.Play(anim.animationClipName, 0, 1f);
                targetAnimator.Update(0f);
            }

            if (targetObject != null)
                targetObject.SetActive(anim.enableObject);

            return;
        }
    }

    public void SetPrefabAnimator(Animator runtimeAnimator)
    {
        prefabAnimator = runtimeAnimator;
    }
}
