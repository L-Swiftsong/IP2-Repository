using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseSteeringBehaviour : ScriptableObject
{
    [SerializeField] protected bool ShowGizmos = true;

    
    // Note: If we require the use of LINQ, change from returning floats to a float[] to a List<float>.
    public abstract float[] GetInterestMap(Rigidbody2D movementBody, Vector2 targetPos, Vector2[] directions);
    public abstract float[] GetDangerMap(Rigidbody2D movementBody, Vector2 targetPos, Vector2[] directions);


    public virtual void DrawGizmos(Transform transform) { }
}