using System;
using Cinemachine;
using UnityEngine;

namespace CrabAssets.Scripts.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class CinemachinePlayerController : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private CinemachineVirtualCamera virtualCamera;

        [SerializeField]
        [HideInInspector]
        private PlayerController playerController;

#if UNITY_EDITOR
        private void Reset()
        {
            virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            playerController = GetComponent<PlayerController>();
        }
#endif

        private void OnEnable()
        {
            playerController.Killed += Killed;
        }

        private void Killed()
        {
            if (!virtualCamera)
            {
                return;
            }
            
            virtualCamera.enabled = false;
        }

        private void OnDisable()
        {
            playerController.Killed -= Killed;
        }
    }
}