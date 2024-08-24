using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private Vector3 cameraOffset;


    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
    
    }
    private void FixedUpdate()
    {
        Vector3 pos = target.transform.position + cameraOffset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, pos, smoothSpeed);
        transform.position = smoothPos;
        
    }
}
