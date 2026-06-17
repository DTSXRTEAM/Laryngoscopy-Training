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
    }

    [Header("Runtime Prefab Animator")]
    public Animator prefabAnimator;

    [Header("Single Target GameObject")]
    public GameObject targetObject;

    [Header("Animations - Configure here in Inspector")]
    public HeadingAnimation[] animations;

    private Animator lastAnimator;

    public void PlayAnimation(int headingIndex)
    {
        foreach (var anim in animations)
        {
            if (anim.headingIndex != headingIndex) continue;

            Animator targetAnimator = anim.usePrefabAnimator ? prefabAnimator : anim.animator;

            if (targetAnimator != null)
            {
                lastAnimator = targetAnimator;
                targetAnimator.enabled = true;
                targetAnimator.Rebind();
                targetAnimator.Update(0f);
                targetAnimator.Play(anim.animationClipName, 0, 0f);
            }

            // Toggle the single GameObject
            if (targetObject != null)
                targetObject.SetActive(anim.enableObject);

            return;
        }
    }

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

            // Toggle the single GameObject
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
