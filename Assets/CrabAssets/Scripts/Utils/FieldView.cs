using UnityEngine;

[System.Serializable]
public struct FieldView2D {

    public Vector2  origin;
    [Range(0,360)]
    public float    angle;
    public float    magnitude;

    [Range (0, 360)]
    public float    aperture;

    public FieldView2D(Vector2 origin, float angle, float magnitude, float aperture){
        this.origin = origin;
        this.angle = angle;
        this.magnitude = magnitude;
        this.aperture = aperture;
    }

    public void Draw (Transform transform) {
        #if UNITY_EDITOR
        float half = aperture * 0.5f * Mathf.Deg2Rad;
        float segments = 8f;
        float increment = 2f / (float)segments;
        float directionAngle = angle * Mathf.Deg2Rad;

        for (int i = 0; i < segments + 1; i++){
            float alpha = directionAngle - half + (half * i * increment);
            Vector2 point = new Vector2 (
                Mathf.Cos (alpha),
                Mathf.Sin (alpha)
            ) * magnitude;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine (
                from: transform.TransformPoint (origin),
                to: transform.TransformPoint (origin + point)
            );
        }
        #endif
    }

    public bool Contains (Vector2 point) {
        Vector2 v = point - origin;
        if (v.magnitude > magnitude)
            return false;
        float sigma = Mathf.Atan2 (v.y, v.x) * Mathf.Rad2Deg;
        float half = aperture * 0.5f;
        return angle - half < sigma && angle + half > sigma;
    }

    public Vector2 GetUp () {
        float half = aperture * 0.5f * Mathf.Deg2Rad;
        float segments = 8f;
        float increment = 2f / (float)segments;
        float directionAngle = angle * Mathf.Deg2Rad;

            float alpha = directionAngle - half + (half * 0 * increment);
            Vector2 point = new Vector2 (
                Mathf.Cos (alpha),
                Mathf.Sin (alpha)
            ) * magnitude;
        return point;
    }
    public Vector2 GetDown () {
        float increment = 2f / 8f;
        float half = aperture * 0.5f * Mathf.Deg2Rad;
        float directionAngle = angle * Mathf.Deg2Rad;
        float alpha = directionAngle - half + (half * 0 * increment);
        return new Vector2 (
            Mathf.Cos (alpha),
            Mathf.Sin (alpha)
        ) * magnitude;
    }

}
