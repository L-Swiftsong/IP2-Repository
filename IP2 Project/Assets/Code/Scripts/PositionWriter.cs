using UnityEngine;

public class PositionWriter : MonoBehaviour
{
    [SerializeField] private Position _position;

    private void Update() => _position.Value = transform.position;
}
