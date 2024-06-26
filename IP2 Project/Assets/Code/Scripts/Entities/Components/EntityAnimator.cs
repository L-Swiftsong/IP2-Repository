using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    /*
    Animations (All Directional):
        - Idle
        - Walking
        - Dashing
        - Attacking
        - Stunned
        - Dead
     */
    [SerializeField] private Animator _anim;
    private const string ANIMATOR_ROTATION_VARIABLE = "RotationIndex";


    [Header("Rotation")]
    [SerializeField] private Transform _rotationPivot;
    [SerializeField] private bool _facePivotDirection = true;
    private Vector2 _movementInput;
    private const int ANGLE_COUNT = 4;


    public void PlayAnimation(AnimationType animationType)
    {
        // Prevent us from setting the "Walking" trigger while transitioning into the Walking state (Was causing us to get stuck).
        if (animationType == AnimationType.Walking && _anim.GetCurrentAnimatorStateInfo(0).IsName(animationType.ToString()))
            return;
        
        _anim.SetTrigger(animationType.ToString());
    }

    public void StopAnimation(AnimationType animationType) => _anim.SetTrigger(string.Format("Stop{0}", animationType.ToString()));


    private void Update()
    {
        // Set the value of the Animator's Rotation Variable.
        if (_movementInput != Vector2.zero || _facePivotDirection)
            _anim.SetFloat(ANIMATOR_ROTATION_VARIABLE, GetRotationForAnimator());
    }


    #region Animation Shortcuts
    public void PlayAttackAnimation() => PlayAnimation(AnimationType.Attacking);
    

    public void PlayDashingAnimation() => PlayAnimation(AnimationType.Dashing);
    public void StopDashingAnimation() => StopAnimation(AnimationType.Dashing);

    public void PlayMovementAnimation(Vector2 movementInput) => PlayAnimation(movementInput == Vector2.zero ? AnimationType.Idle : AnimationType.Walking);

    public void PlayStunnedAnimation() => PlayAnimation(AnimationType.Stunned);
    public void PlayDeadAnimation()
    {
        PlayAnimation(AnimationType.Dead);
        Destroy(this);
    }
    #endregion


    public void SetMovementInputValue(Vector2 movementInput) => _movementInput = movementInput;


    private float GetCurrentRotation()
    {
        float roundingValue = ANGLE_COUNT > 0f ? 360f / ANGLE_COUNT : 1;

        if (_movementInput != Vector2.zero)
        {
            // Calculate the angle.
            float rotationAngle = Vector2.Angle(Vector2.up, _movementInput);

            // If the x value of the movementInput is greater than 0, subtract it from 360f to get a non-symmetrical angle between 0 & 360f.
            rotationAngle = _movementInput.x > 0f ? 360f - rotationAngle : rotationAngle;

            // Return the constrained angle
            return rotationAngle;
        }
        else
            return Mathf.Round(_rotationPivot.eulerAngles.z / roundingValue) * roundingValue;
    }
    private int GetRotationForAnimator()
    {
        float currentRotation = GetCurrentRotation();

        // If the current rotation is 360, then treat it as 0.
        if (Mathf.Approximately(currentRotation, 360f))
            currentRotation = 0f;

        currentRotation /= 360f; // E.g. 'currentRotation/360f = 0.75' and '0.75 * ANGLE_COUNT = 3' when currentRotation = 270* and ANGLE_COUNT = 4
        float maxValue = 1f - (1f / ANGLE_COUNT);
        return currentRotation <= maxValue ? RoundTowardsOdd(currentRotation * ANGLE_COUNT) : ANGLE_COUNT - 1;
    }

    private int RoundTowardsEven(float value)
    {
        int intValue = Mathf.RoundToInt(value);
        
        // If the value is already whole, then return the value.
        if (Mathf.Approximately(intValue, value))
            return intValue;
        // Otherwise, round to the closest even value.
        else
        {
            float evenFloat = Mathf.Round(value / 2f) * 2f;
            return Mathf.RoundToInt(evenFloat);
        }
    }
    private int RoundTowardsOdd(float value)
    {
        int intValue = Mathf.RoundToInt(value);

        // If the value is already whole, then return the value.
        if (Mathf.Approximately(intValue, value))
            return intValue;
        // Otherwise, round to the closest even value.
        else
        {
            bool roundUp = Mathf.CeilToInt(value) % 2 != 0;
            return roundUp ? Mathf.CeilToInt(value) : Mathf.FloorToInt(value);
        }
    }
}

public enum AnimationType
{
    Idle,
    Walking,
    Dashing,
    Attacking,
    Stunned,
    Dead
}