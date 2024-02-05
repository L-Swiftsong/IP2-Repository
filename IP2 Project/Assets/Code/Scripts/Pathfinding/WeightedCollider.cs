using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WeightedCollider : MonoBehaviour
{
    [SerializeField, Range(1f, 2f)] private float _penalty;
    public float Penalty => _penalty;
}
