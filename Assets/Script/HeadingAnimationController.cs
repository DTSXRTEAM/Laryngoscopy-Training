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
    }

    [Header("Runtime Prefab Animator")]
    public Animator prefabAnimator;

    [Header("Animations - Configure here in Inspector")]
    public HeadingAnimation[] animations;

    private Animator lastAnimator;

    /// <summary>
    /// Plays the animation for the given heading index.
    /// </summary>
    public void PlayAnimation(int headingIndex)
    {
        foreach (var anim in animations)
        {
            if (anim.headingIndex != headingIndex) continue;

            Animator targetAnimator = anim.usePrefabAnimator ? prefabAnimator : anim.animator;

            if (targetAnimator == null)
            {
                Debug.LogWarning("Animator Missing For Index : " + headingIndex);
                return;
            }

            lastAnimator = targetAnimator;

            targetAnimator.enabled = true;
            targetAnimator.Rebind();
            targetAnimator.Update(0f);

            targetAnimator.Play(anim.animationClipName, 0, 0f);

            Debug.Log("Playing Animation For Index : " + headingIndex);
            return;
        }
    }

    /// <summary>
    /// Sets the animation to its last frame for the given heading index.
    /// </summary>
    public void SetAnimationToLastFrame(int headingIndex)
    {
        foreach (var anim in animations)
        {
            if (anim.headingIndex != headingIndex) continue;

            Animator targetAnimator = anim.usePrefabAnimator ? prefabAnimator : anim.animator;

            if (targetAnimator == null) return;

            targetAnimator.enabled = true;
            targetAnimator.Play(anim.animationClipName, 0, 1f);
            targetAnimator.Update(0f);

            return;
        }
    }

    /// <summary>
    /// Assigns a runtime animator (e.g., from a prefab instance).
    /// </summary>
    public void SetPrefabAnimator(Animator runtimeAnimator)
    {
        prefabAnimator = runtimeAnimator;
    }
}
    