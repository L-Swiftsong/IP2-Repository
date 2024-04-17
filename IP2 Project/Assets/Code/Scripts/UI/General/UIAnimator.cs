using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Q: Why are we not using the Unity Animator?
// A: If we use the Unity Animator on UI, the entire Canvas refreshes each frame, even if nothing is changing.

/// <summary>
/// A script to animate UI in unity without using the Animator.
/// </summary>
[RequireComponent(typeof(RectTransform), typeof(Image))]
public class UIAnimator : MonoBehaviour
{
    private Sprite _resetSprite;
    private Coroutine _animationCoroutine;

    
    [Header("References")]
    RectTransform _rectTransform;
    private Image _image;

    private void Awake()
    {
        // Find all dependent components
        _rectTransform = GetComponent<RectTransform>();
        _image = _rectTransform.GetComponent<Image>();
    }


    public void StartAnimation(Sprite[] animation, int framerate) => _animationCoroutine = StartCoroutine(PlayAnimation(animation, framerate)); 
    public void StartReversedAnimation(Sprite[] animation, int framerate) => _animationCoroutine = StartCoroutine(PlayReverseAnimation(animation, framerate));


    private IEnumerator PlayAnimation(Sprite[] animation, int framerate)
    {
        // Cache & Reset Values.
        int length = animation.Length;
        int spriteIndex = 0;
        float timeBetweenFrames = 1f / framerate;

        // Play the animation
        while (spriteIndex < length)
        {
            // Set the sprite.
            _image.sprite = animation[spriteIndex];

            // Wait to transition to the next sprite.
            yield return new WaitForSeconds(timeBetweenFrames);
            spriteIndex++;
        }

        // Ensure we are on the final sprite at the end of the animation.
        _image.sprite = animation[length - 1];
    }
    private IEnumerator PlayReverseAnimation(Sprite[] animation, int framerate)
    {
        // Cache & Reset Values.
        int spriteIndex = animation.Length - 1;
        float timeBetweenFrames = 1f / framerate;

        // Play the animation
        while (spriteIndex >= 0)
        {
            // Set the sprite.
            _image.sprite = animation[spriteIndex];

            // Wait to transition to the next sprite.
            yield return new WaitForSeconds(timeBetweenFrames);
            spriteIndex--;
        }

        // Ensure we are on the final sprite at the end of the animation.
        _image.sprite = animation[0];
    }
}
