using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Animations/Directional Animation Container")]
public class DirectionalAnimationContainerSO : ScriptableObject
{
    [SerializedDictionary(keyName: "Entity Type", valueName: "Animations")]
    //public SerializedDictionary<EntityType, DirectionalAnimation[]> PairedAnimations;
    public SerializedDictionary<EntityType, SerializedDictionary<Direction, Sprite[]>> PairedAnimations;
}

[System.Serializable]
public struct DirectionalAnimation
{
    public Direction Direction;
    public Sprite[] Sprites;
}

[System.Serializable]
public enum Direction
{
    North,
    East,
    South,
    West,
}
public static class DirectionExtensions
{
    public static Vector2? DirectionToVector2(this Direction direction)
    {
        return direction switch
        {
            Direction.North => new Vector2(x: 0, y: 1),
            //Direction.NorthEast => new Vector2(x: 1, y: 1).normalized,
            Direction.East => new Vector2(x: 1, y: 0),
            //Direction.SouthEast => new Vector2(x: 1, y: -1).normalized,
            Direction.South => new Vector2(x: 0, y: -1),
            //Direction.SouthWest => new Vector2(x: -1, y: -1).normalized,
            Direction.West => new Vector2(x: -1, y: 0),
            //Direction.NorthWest => new Vector2(x: -1, y: 1).normalized,
            _ => null
        };
    }

    private const float NORTH_SOUTH_THRESHOLD = 1f;
    public static Direction GetDirectionFromVector(Vector2 vector)
    {
        float upDot = Vector2.Dot(Vector2.up, vector);
        float rightDot = Vector2.Dot(Vector2.right, vector);

        if (upDot >= NORTH_SOUTH_THRESHOLD)
            return Direction.North;
        else if (upDot <= -NORTH_SOUTH_THRESHOLD)
            return Direction.South;
        {
            if (rightDot > 0)
                return Direction.East;
            else
                return Direction.West;
        }
    }
}