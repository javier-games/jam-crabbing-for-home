using System;
using CrabAssets.Pickables;
using Player;
using UnityEngine;

namespace Crabbing.Scripts.Game
{
    public class GameController : MonoBehaviour
    {

        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private ShellController startingShellPrefab;
        [SerializeField] private Transform startPoint;
        [SerializeField] private float initialScale = 1f;

        private PlayerController Player { get; set; }

        private void Awake()
        {
            var inScenePlayer = FindObjectOfType<PlayerController>();
            
            Player = inScenePlayer == null 
                ? Instantiate(playerPrefab) 
                : inScenePlayer;

            Player.transform.localScale = Vector3.one * initialScale; 
            Player.transform.SetPositionAndRotation(startPoint.position, Quaternion.identity);

            if (startingShellPrefab != null)
            {
                var shell = Instantiate(startingShellPrefab);
                Player.PickUp(shell);
            }
        }
    }
}