using System;
using System.Collections;
using UnityEngine;

public class HeadingAnimationController : MonoBehaviour
{
    [Serializable]
    public class HeadingAnimation
    {
        [Header("Heading Index - matches JSON index")]
        public int headingIndex;

        [Header("Animator (Leave empty if using Prefab Animator)")]
        public Animator animator;

        [Header("Use Runtime Prefab Animator")]
        public bool usePrefabAnimator;

        [Header("Animation State Name")]
        public string animationClipName;

        [Header("Enable Video Player")]
        public bool enableVideoPlayer;

        [Header("Enable Header Object")]
        public bool enableHeaderObject;

        [Header("Quad For This Step")]
        public GameObject quad;

        [Header("Delay Before Playing (Seconds)")]
        public float delayTime = 0f;
    }

    [Header("Runtime Prefab Animator")]
    public Animator prefabAnimator;

    [Header("Common Video Player Object")]
    public GameObject videoPlayerObject;

    [Header("Common Header Object")]
    public GameObject headerObject;

    [Header("Animations")]
    public HeadingAnimation[] animations;

    private Animator lastAnimator;

    /// <summary>
    /// Plays the animation for the given heading index.
    /// </summary>
    public void PlayAnimation(int headingIndex)
    {
        foreach (var anim in animations)
        {
            if (anim.headingIndex != headingIndex)
                continue;

            // Video Player
            if (videoPlayerObject != null)
                videoPlayerObject.SetActive(anim.enableVideoPlayer);

            // Header Object
            if (headerObject != null)
                headerObject.SetActive(anim.enableHeaderObject);

            // Enable only the current Quad
            UpdateQuads(anim.quad);

            // Play animation
            StartCoroutine(PlayAnimationWithDelay(anim));

            return;
        }
    }

    private IEnumerator PlayAnimationWithDelay(HeadingAnimation anim)
    {
        if (anim.delayTime > 0f)
            yield return new WaitForSeconds(anim.delayTime);

        Animator targetAnimator =
            anim.usePrefabAnimator ? prefabAnimator : anim.animator;

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
    /// Sets animation to the last frame.
    /// </summary>
    public void SetAnimationToLastFrame(int headingIndex)
    {
        foreach (var anim in animations)
        {
            if (anim.headingIndex != headingIndex)
                continue;

            Animator targetAnimator =
                anim.usePrefabAnimator ? prefabAnimator : anim.animator;

            if (targetAnimator != null)
            {
                targetAnimator.enabled = true;
                targetAnimator.Play(anim.animationClipName, 0, 1f);
                targetAnimator.Update(0f);
            }

            if (videoPlayerObject != null)
                videoPlayerObject.SetActive(anim.enableVideoPlayer);

            if (headerObject != null)
                headerObject.SetActive(anim.enableHeaderObject);

            UpdateQuads(anim.quad);

            return;
        }
    }

    /// <summary>
    /// Enables only the current Quad and disables all others.
    /// </summary>
    private void UpdateQuads(GameObject activeQuad)
    {
        foreach (var anim in animations)
        {
            if (anim.quad != null)
            {
                anim.quad.SetActive(anim.quad == activeQuad);
            }
        }
    }

    /// <summary>
    /// Resets all animations and objects.
    /// </summary>
    public void ResetAllAnimations()
    {
        StopAllCoroutines();

        foreach (var anim in animations)
        {
            Animator targetAnimator =
                anim.usePrefabAnimator ? prefabAnimator : anim.animator;

            if (targetAnimator != null)
            {
                targetAnimator.enabled = true;
                targetAnimator.Rebind();
                targetAnimator.Update(0f);
            }

            if (anim.quad != null)
                anim.quad.SetActive(false);
        }

        if (videoPlayerObject != null)
            videoPlayerObject.SetActive(false);

        if (headerObject != null)
            headerObject.SetActive(false);
    }

    /// <summary>
    /// Assign runtime prefab animator.
    /// </summary>
    public void SetPrefabAnimator(Animator runtimeAnimator)
    {
        prefabAnimator = runtimeAnimator;
    }
}