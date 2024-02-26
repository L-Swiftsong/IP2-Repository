using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField, ReadOnly] private string _currentAnimationName;
    [SerializeField, ReadOnly] private string _previousAnimationName;


    [Header("Animation")]
    [SerializeField] private bool _animate = true;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Sprite[] _currentAnimation;
    private int _currentSpriteIndex;
    private int _currentAnimationID;

    
    // Current animation parameters.
    private bool _currentIsSoft; // Soft animations are automatically overrided by new animations.
    private bool _currentLoops; // Should this animation loop.
    private bool _hasCurrentLooped = false;
    private bool _currentIsTemporary; // Temporary animations aren't saved as previous animations when set.
    
    private System.Func<bool> _currentResetCondition;
    private Coroutine _revertToPreviousCoroutine;


    private (Sprite[] animation, int ID, bool isSoft, bool loops) _previousAnimationParameters; // The relevant parameters for the previous animation.

    private bool _destroyOnComplete = false;


    [SerializeField] private int _framerate;
    private float _timeTillNextSprite;


    [Header("Animation Containter Parameters")]
    [SerializeField] private EntityType _entityType;
    [SerializeField] private AnimationContainerSO _idleAnim;
    [SerializeField] private AnimationContainerSO _runningAnim;

    [Space(5)]
    [SerializeField] private AnimationContainerSO _hurtAnim;
    [SerializeField] private AnimationContainerSO _deadAnim;



    private void Start() => SetNextAnimation(_idleAnim, hardOverride: true, isTemporary: false);

    private void Update()
    {
        if (!_animate)
            return;
        
        
        // Decrement timeTillNextSprite.
        _timeTillNextSprite -= Time.deltaTime;
        
        // Loop until we shouldn't set the next sprite.
        float timeBetweenSprites = 1f / (float)_framerate;
        while (_timeTillNextSprite <= 0)
        {
            // If there is no current animation set, stop here (Note: We stop and add the timeBetweenSprites here so that timeTillNextSprite doesn't go below 0 too often).
            if (_currentAnimation == null || _currentAnimation.Length <= 0)
            {
                _timeTillNextSprite += timeBetweenSprites;
                return;
            }

            // Set the next sprite.
            SetNextSprite();

            // Increment the time till next sprite.
            _timeTillNextSprite += timeBetweenSprites;
        }
    }
    private void SetNextSprite()
    {
        // If the current animation is temporary, has looped at least once, and the reset condition has been met, reset it as opposed to waiting until the animation fully completes.
        if (_hasCurrentLooped && (_currentIsTemporary && _currentResetCondition()))
        {
            if (_revertToPreviousCoroutine != null)
                StopCoroutine(_revertToPreviousCoroutine);
            _revertToPreviousCoroutine = StartCoroutine(RevertToPreviousAfterCondition(_currentResetCondition));

            // Stop here to prevent errors.
            return;
        }

        
        // Increment current sprite index.
        _currentSpriteIndex++;
        if (_currentSpriteIndex >= _currentAnimation.Length)
        {
            // If we are to destroy ourself when the current animation completes, do so now.
            if (_destroyOnComplete)
                Destroy(this);

            // If this animation loops AND (is not temporary OR the condition has not been met), constrain the index and allow it to loop.
            if (_currentLoops)
            {
                _currentSpriteIndex = 0;
                _hasCurrentLooped = true;
            }
            // Otherwise, revert to the previous animation.
            else
            {
                if (_revertToPreviousCoroutine != null)
                    StopCoroutine(_revertToPreviousCoroutine);
                _revertToPreviousCoroutine = StartCoroutine(RevertToPreviousAfterCondition(_currentResetCondition));

                // Stop here to prevent errors.
                return;
            }
        }


        // Set the sprite of the sprite renderer.
        _spriteRenderer.sprite = _currentAnimation[_currentSpriteIndex];
    }


    #region Shortcut functions for Common Animations
    public void PlayIdle() => SetNextAnimation(_idleAnim, softOverridable: true, hardOverride: false, loops: true, isTemporary: false);
    public void PlayRunning() => SetNextAnimation(_runningAnim, softOverridable: true, hardOverride: false, loops: true, isTemporary: false);

    
    public void HandleHealthChanged(HealthChangedValues newValues)
    {
        // If the entity's health has been reduced, then play the hurt animation.
        if (newValues.NewHealth < newValues.OldHealth)
            PlayHurt();
    }
    public void PlayHurt() => SetNextAnimation(_hurtAnim, softOverridable: false, hardOverride: true, loops: false, isTemporary: true);
    
    
    public void PlayDead()
    { 
        // Play the death animation, and schedule this component for deletion.
        SetNextAnimation(_deadAnim, softOverridable: false, hardOverride: true, loops: false, isTemporary: false);
        _destroyOnComplete = true;
    }


    public void PlayAttackAnimation(AnimationContainerSO attackContainer, float minRevertDuration = 0f) => SetNextAnimation(attackContainer, softOverridable: false, hardOverride: true, loops: false, isTemporary: true, minRevertDuration: minRevertDuration);
    #endregion


    /// <summary>
    /// Set the next animation to be played.
    /// </summary>
    /// <param name="animationContainer"> The animation container where the animation is stored.</param>
    /// <param name="softOverridable"> Can this animation be overridable by soft animations?</param>
    /// <param name="hardOverride"> Should this animation override all other animations?</param>
    /// <param name="isTemporary"> When the next animation is set, should we save this animation in the case that we need to revert?</param>
    public void SetNextAnimation(AnimationContainerSO animationContainer, bool softOverridable = true, bool hardOverride = false, bool loops = true, bool isTemporary = true, System.Func<bool> customRevertCondition = null, float minRevertDuration = 0f)
    {
        if (hardOverride || _currentIsSoft)
        {
            // If the animation container does not exist OR doesn't contain a value for this entity, then stop here.
            if (animationContainer == null || !animationContainer.PairedAnimations.ContainsKey(_entityType) || animationContainer.GetInstanceID() == _currentAnimationID)
                return;


            // In case an animation is currently trying to revert to previous, stop the RevertAfterDelay coroutine.
            if (_revertToPreviousCoroutine != null)
                StopCoroutine(_revertToPreviousCoroutine);


            // If the current (Now previous) animation was NOT a temporary animation, then cache it as the previous animation.
            if (!_currentIsTemporary)
            {
                _previousAnimationParameters.animation = _currentAnimation;
                _previousAnimationParameters.isSoft = _currentIsSoft;
                _previousAnimationParameters.loops = _currentLoops;
                _previousAnimationParameters.ID = _currentAnimationID;

                _previousAnimationName = _currentAnimationName;
            }
            
            // Debug.
            Debug.Log("New Animation: " + animationContainer.name);
            _currentAnimationName = animationContainer.name;


            // Set new animation parameters.
            _currentIsSoft = softOverridable;
            _currentLoops = loops;
            _hasCurrentLooped = false;
            _currentIsTemporary = isTemporary;
            _currentAnimationID = animationContainer.GetInstanceID();


            // Set revert condition to either the passed in custom one, or to the revert time.
            if (customRevertCondition != null)
                _currentResetCondition = customRevertCondition;
            else
            {
                float revertTime = Time.time + minRevertDuration;
                _currentResetCondition = () => Time.time > revertTime;
            }

            // Set the next sprite animation.
            _currentAnimation = animationContainer.PairedAnimations[_entityType];
            _currentSpriteIndex = 0;
        }
    }

    public void StopCurrentAnimation()
    {
        if (_previousAnimationParameters.animation == null)
            return;

        RevertToPreviousAnimation();
    }


    private IEnumerator RevertToPreviousAfterCondition(System.Func<bool> predicate)
    {
        _currentAnimation = null;
        yield return new WaitUntil(predicate);

        RevertToPreviousAnimation();
        _revertToPreviousCoroutine = null;
    }
    private void RevertToPreviousAnimation()
    {
        Debug.Log("Reverting to Previous Animation");

        // Revert to the previous animation.
        _currentAnimation = _previousAnimationParameters.animation;
        _currentSpriteIndex = 0;

        // Revert Debug Info.
        _currentAnimationName = _previousAnimationName;

        // Revert to the previous animation's parameters.
        _currentIsSoft = _previousAnimationParameters.isSoft;
        _currentLoops = _previousAnimationParameters.loops;
        _hasCurrentLooped = false;
        _currentAnimationID = _previousAnimationParameters.ID;
        _currentIsTemporary = false;
    }
}
