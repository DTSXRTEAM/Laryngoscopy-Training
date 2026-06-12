using System;
using UnityEngine;

public class HeadingAnimationController : MonoBehaviour
{
    [Serializable]
    public class HeadingAnimation
    {
        [Header("Heading Index - matches JSON index")]
        public int headingIndex;

        [Header("Animator")]
        public Animator animator;

        [Header("Use Runtime Prefab Animator")]
        public bool usePrefabAnimator;

        [Header("Animation State Name")]
        public string animationClipName;
    }

    [Header("Runtime Prefab Animator")]
    public Animator prefabAnimator;

    [Header("Animations - Configure here in Inspector with heading indices 0-24")]
    public HeadingAnimation[] animations;

    private Animator lastAnimator;

    public void PlayAnimation(int headingIndex)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].headingIndex != headingIndex)
                continue;

            Animator targetAnimator =
                animations[i].usePrefabAnimator
                ? prefabAnimator
                : animations[i].animator;

            if (targetAnimator == null)
            {
                Debug.LogWarning(
                    "Animator Missing For Index : " +
                    headingIndex
                );
                return;
            }

            lastAnimator = targetAnimator;

            targetAnimator.enabled = true;

            targetAnimator.Rebind();
            targetAnimator.Update(0f);

            targetAnimator.Play(
                animations[i].animationClipName,
                0,
                0f
            );

            Debug.Log(
                "Playing Animation For Index : " +
                headingIndex
            );

            return;
        }
    }

    public void SetAnimationToLastFrame(int headingIndex)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].headingIndex != headingIndex)
                continue;

            Animator targetAnimator =
                animations[i].usePrefabAnimator
                ? prefabAnimator
                : animations[i].animator;

            if (targetAnimator == null)
                return;

            targetAnimator.enabled = true;

            targetAnimator.Play(
                animations[i].animationClipName,
                0,
                1f
            );

            targetAnimator.Update(0f);

            return;
        }
    }

    public void SetPrefabAnimator(Animator runtimeAnimator)
    {
        prefabAnimator = runtimeAnimator;
    }
}