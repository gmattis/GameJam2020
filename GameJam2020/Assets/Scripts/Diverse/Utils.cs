using UnityEngine;

public enum PlayerID
{
    Player1,
    Player2
}

public static class Utils
{
    public static Vector3 Vector2to3(Vector2 vect)
    {
        return new Vector3(vect.x, 0, vect.y);
    }
}