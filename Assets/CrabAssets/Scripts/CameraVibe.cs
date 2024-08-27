using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraVibe : MonoBehaviour{

    [SerializeField]
    AnimationCurve frequency;
    [SerializeField]
    AnimationCurve amplitude;
    [SerializeField]
    AnimationCurve closeUp;

    public void MoveCamera (float t) {

        transform.eulerAngles = new Vector3 (0,0,amplitude.Evaluate(t) * Mathf.Sin(Time.time * frequency.Evaluate(t)));

        GetComponent<Camera> ().orthographicSize = closeUp.Evaluate (t); 
    }
}
