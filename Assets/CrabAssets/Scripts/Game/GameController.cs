using CrabAssets.Scripts.Player;
using CrabAssets.Scripts.Shells;
using UnityEngine;

namespace CrabAssets.Scripts.Game
{
    public class GameController : MonoBehaviour
    {
        private const string LastCheckPointKey = "CheckPoint-Last";
        private const string LastPlayerScaleKey = "CheckPoint-Scale";

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

            Respawn();

            if (startingShellPrefab != null)
            {
                var shell = Instantiate(startingShellPrefab);
                Player.PickUp(shell);
            }
        }

        private void OnEnable()
        {
            GameTrigger.OnTriggerIn += OnTriggerIn;
        }

        private void OnDisable()
        {
            GameTrigger.OnTriggerOut -= OnTriggerOut;
        }

        private void OnTriggerIn(GameTrigger trigger, Component _)
        {
            switch (trigger)
            {
                case CheckPoint checkPoint:
                {
                    PlayerPrefs.SetInt(LastCheckPointKey, trigger.GetHashCode()); 
                    PlayerPrefs.SetFloat(LastPlayerScaleKey, Player.transform.localScale.x);
                    PlayerPrefs.Save();
                    break;
                }
            }
        }

        private void OnTriggerOut(GameTrigger trigger, Component actor)
        {
            switch (trigger)
            {
                case CheckPoint checkPoint:
                    break;
            }
        }


        private void Respawn()
        {
            CheckPoint checkPoint = null;
            if (PlayerPrefs.HasKey(LastCheckPointKey))
            {
                var hashCode = PlayerPrefs.GetInt(LastCheckPointKey);
                var checkPoints = FindObjectsOfType<CheckPoint>();

                for (var i = 0; i < checkPoints.Length; i++)
                {
                    if (checkPoints[i].GetHashCode() != hashCode)
                    {
                        continue;
                    }

                    checkPoint = checkPoints[i];
                }
            }

            if ((object) checkPoint != null)
            {
                checkPoint.PlaceActor(Player.transform);
            }
            else
            {
                Player.transform.SetPositionAndRotation(startPoint.position, Quaternion.identity);
            }

            var savedScale = PlayerPrefs.GetFloat(LastPlayerScaleKey, initialScale);
            Player.transform.localScale = Vector3.one * savedScale; 
        }

        
        [ContextMenu("Clear PlayerPrefs")]
        public void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteKey(LastPlayerScaleKey);
            PlayerPrefs.DeleteKey(LastCheckPointKey);
        }
    }
}