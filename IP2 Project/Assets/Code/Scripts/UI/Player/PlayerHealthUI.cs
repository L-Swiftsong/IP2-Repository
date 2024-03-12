using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : HealthUI
{
    [SerializeField] private Transform _heartParent;
    [SerializeField] private GameObject _heartPrefab;

    private List<HealthHeart> _instancedHearts;


    private void Awake()
    {
        // Set HeartParent to this if it has not been set.
        if (_heartParent == null)
            _heartParent = this.transform;


        // Intiialise the Hearts.
        RemoveAllHearts();
    }


    // Destroy all existing hearts & reinitialise the _instancedHearts list.
    private void RemoveAllHearts()
    {
        foreach (Transform child in _heartParent)
            Destroy(child.gameObject);

        _instancedHearts = new List<HealthHeart>();
    }


    // Create and initialise a new UI Heart.
    private void CreateNewHeart()
    {
        HealthHeart newHeart = Instantiate<GameObject>(_heartPrefab, _heartParent).GetComponent<HealthHeart>();
        newHeart.SetHeartStatus(HeartStatus.Empty);
        _instancedHearts.Add(newHeart);
    }


    public override void UpdateHealth(HealthChangedValues newValues)
    {
        // Check if we need to change the number of existing hearts.
        int targetHeartCount = Mathf.CeilToInt(newValues.NewMax / 2f);
        if (targetHeartCount != _instancedHearts.Count)
        {
            // Clear and recreate the hearts.
            RemoveAllHearts();
            for (int i = 0; i < targetHeartCount; i++)
                CreateNewHeart();
        }


        // Update existing hearts.
        for (int i = 0; i < targetHeartCount; i++)
        {
            // Calculate whether this heart should be full, half, or empty.
            int heartStatus = Mathf.Clamp((newValues.NewHealth - (i * 2)), 0, 2);
            
            // Set the instanced heart's status to the calculated value.
            _instancedHearts[i].SetHeartStatus((HeartStatus)heartStatus);
        }
    }
    public override void OnDead() { }
}