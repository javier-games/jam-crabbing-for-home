using UnityEngine;

[System.Serializable]
public enum Direction {
    Horizontal,
    Vertical
}

[System.Serializable]
public struct Capsule {
    Vector2 offset;
    Vector2 size;
    Direction direction;

    public void Draw (Vector2 origin) {
        float sense = direction == Direction.Horizontal ? 1 : -1;
        UnityEditor.Handles.DrawWireDisc (origin, Vector3.back, 1);
    }
}



