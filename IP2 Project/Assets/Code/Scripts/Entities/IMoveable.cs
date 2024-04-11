using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    public void ApplyKnockback(Vector2 force, float duration, ForceMode2D forceMode);
}
