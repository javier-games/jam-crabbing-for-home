using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CrabAssets.Scripts.Utils
{
    public class RandomRotation : MonoBehaviour
    {
        private void Awake()
        {
            transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        }
    }
}