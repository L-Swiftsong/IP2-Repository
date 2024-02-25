using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Sprite[] _currentAnimation;
    private int _currentSpriteIndex;

    [SerializeField] private int _framerate;
    private float _timeTillNextSprite;


    [Header("Animation Containter Parameters")]
    [SerializeField] private EntityType _entityType;
    [SerializeField] private AnimationContainerSO _defaultContainer;



    private void Start() => SetNextAnimation(_defaultContainer);

    private void Update()
    {
        if (_currentAnimation == null || _currentAnimation.Length <= 0)
            return;

        
        // Decrement timeTillNextSprite.
        _timeTillNextSprite -= Time.deltaTime;


        // Loop until we shouldn't set the next sprite.
        float timeBetweenSprites = 1f / (float)_framerate;
        while(_timeTillNextSprite <= 0)
        {
            // Set the next sprite.
            SetNextSprite();

            // Increment the time till next sprite.
            _timeTillNextSprite += timeBetweenSprites;
        }
    }
    private void SetNextSprite()
    {
        // Increment current sprite index.
        _currentSpriteIndex++;
        if (_currentSpriteIndex >= _currentAnimation.Length)
            _currentSpriteIndex = 0;


        // Set the sprite of the sprite renderer.
        _spriteRenderer.sprite = _currentAnimation[_currentSpriteIndex];
    }


    public void SetNextAnimation(AnimationContainerSO animationContainer)
    {
        // Set the next sprite animation.
        _currentAnimation = animationContainer.PairedAnimations[_entityType];
        _currentSpriteIndex = 0;
    }
}
