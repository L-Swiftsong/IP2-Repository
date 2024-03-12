using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : HealthUI
{
    [SerializeField] private ProgressBar _healthBar;
    

    // To be called when created via AddComponent.
    public void Init() => _healthBar = GetComponent<ProgressBar>();
    public void Init(ProgressBar healthBar) => _healthBar = healthBar;


    public override void UpdateHealth(HealthChangedValues newValues) => _healthBar.SetValues(newValues.NewHealth, newValues.NewMax);
    public override void OnDead()
    {
        Debug.Log("HealthBar: Destroying self as target is dead");
        Destroy(this.gameObject);
    }
}
