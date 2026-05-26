using System;
using UnityEngine;

public class HeadingAnimationController : MonoBehaviour
{
    [Serializable]
    public class HeadingAnimation
    {
        [Header("Heading")]
        public string headingName;

        [Header("Normal Object Animator")]
        public Animator animator;

        [Header("Prefab Animation")]
        public bool usePrefabAnimator;

        [Header("Animation State Name")]
        public string animationClipName;
    }

    [Header("Spawned Prefab Animator")]
    public Animator prefabAnimator;

    [Header("All Animations")]
    public HeadingAnimation[] animations;

    public void PlayAnimation(string heading)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].headingName == heading)
            {
                Animator targetAnimator = null;

                // NORMAL OBJECT ANIMATOR
                if (!animations[i].usePrefabAnimator)
                {
                    targetAnimator =
                        animations[i].animator;
                }
                // SPAWNED PREFAB ANIMATOR
                else
                {
                    targetAnimator =
                        prefabAnimator;
                }

                // PLAY
                if (targetAnimator != null)
                {
                    targetAnimator.enabled = true;

                    targetAnimator.Play(
                        animations[i].animationClipName,
                        0,
                        0f
                    );

                    Debug.Log(
                        "Playing Animation : " +
                        animations[i].animationClipName
                    );
                }
                else
                {
                    Debug.LogWarning(
                        "Animator Missing For : " +
                        heading
                    );
                }
            }
        }
    }

    // RUNTIME PREFAB ANIMATOR
    public void SetPrefabAnimator(
        Animator runtimeAnimator)
    {
        prefabAnimator = runtimeAnimator;
    }
}