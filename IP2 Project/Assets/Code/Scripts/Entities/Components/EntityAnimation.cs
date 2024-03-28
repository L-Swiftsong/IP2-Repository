using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour
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
    private const int ANGLE_COUNT = 4;


    public void PlayAnimation(AnimationType animationType) => _anim.SetTrigger(animationType.ToString());
    public void StopAnimation(AnimationType animationType) => _anim.SetTrigger(string.Format("Stop{0}", animationType.ToString()));

    
    private void Update() =>_anim.SetFloat(ANIMATOR_ROTATION_VARIABLE, GetRotationForAnimator());


    #region Animation Shortcuts
    public void PlayAttackAnimation() => PlayAnimation(AnimationType.Attacking);

    public void PlayDashingAnimation() => PlayAnimation(AnimationType.Dashing);
    public void StopDashingAnimation() => StopAnimation(AnimationType.Dashing);

    public void PlayMovementAnimation(Vector2 movementInput) => PlayAnimation(movementInput == Vector2.zero ? AnimationType.Idle : AnimationType.Walking);

    public void PlayStunnedAnimation() => PlayAnimation(AnimationType.Stunned);
    public void PlayDeadAnimation() => PlayAnimation(AnimationType.Dead);
    #endregion


    private float GetCurrentRotation()
    {
        float roundingValue = ANGLE_COUNT > 0f ? 360f / ANGLE_COUNT : 1;
        return Mathf.Round(_rotationPivot.eulerAngles.z / roundingValue) * roundingValue;
    }
    private int GetRotationForAnimator()
    {
        float currentRotation = GetCurrentRotation();
        currentRotation /= 360f; // E.g. 'currentRotation/360f = 0.75' and '0.75 * ANGLE_COUNT = 3' when currentRotation = 270* and ANGLE_COUNT = 4
        return currentRotation < 1f ? Mathf.FloorToInt(currentRotation * ANGLE_COUNT) : 0;
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