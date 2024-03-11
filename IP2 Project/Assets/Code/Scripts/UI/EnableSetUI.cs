using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> A script that ensures that whenever this object is enabled, only set children are enabled.</summary>
public class EnableSetUI : MonoBehaviour
{
    [SerializeField] private List<Transform> _childrenToStartEnabled;

    private void OnEnable()
    {
        foreach (Transform child in transform)
        {
            if (_childrenToStartEnabled.Contains(child))
                child.gameObject.SetActive(true);
            else
                child.gameObject.SetActive(false);
        }
    }
}
