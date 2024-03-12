using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthUI : MonoBehaviour
{
    public abstract void UpdateHealth(HealthChangedValues newValues);
    public abstract void OnDead();
}
