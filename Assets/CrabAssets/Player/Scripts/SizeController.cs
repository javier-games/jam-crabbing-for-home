using UnityEngine;
using UnityEngine.Serialization;

public class SizeController : MonoBehaviour
{
    [SerializeField]
    protected float growingDuration = 1;
    
    [SerializeField]
    protected AnimationCurve heightGrowingCurve;
    
    [FormerlySerializedAs("weidthGrowingCurve")]
    [SerializeField]
    protected AnimationCurve widthGrowingCurve;

    private float _growingSize;
    private float _growingTime;
    private Vector3 _originalScale;
    
    public System.Action<float> Growed;

    private void Awake()
    {
        enabled = false;
    }

    private void Update()
    {
        _growingTime += Time.deltaTime;
        var t = _growingTime / growingDuration;

        transform.localScale = new Vector3 (
            _originalScale.x * (1 + widthGrowingCurve.Evaluate (t) * _growingSize),
            _originalScale.y * (1 + heightGrowingCurve.Evaluate (t) * _growingSize),
            _originalScale.z
        );

        if (!(_growingTime > growingDuration)) return;
        _growingTime = 0;
        enabled = false;
    }

    public void ResetScale()
    {
        transform.localScale = Vector3.one;
    }
    
    public void Grow (float size)
    {
        enabled = true;
        _growingTime = 0;
        _originalScale = transform.localScale;
        Growed?.Invoke (_originalScale.x + size);
    }
}
