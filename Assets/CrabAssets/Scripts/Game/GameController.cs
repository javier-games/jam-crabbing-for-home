using CrabAssets.Scripts.Player;
using CrabAssets.Scripts.Shells;
using UnityEngine;

namespace CrabAssets.Scripts.Game
{
    public class GameController : MonoBehaviour
    {
        private const string LastCheckPointKey = "CheckPoint-Last";
        private const string LastPlayerScaleKey = "CheckPoint-Scale";

        [Header("Spawning")]
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private ShellController startingShellPrefab;
        [SerializeField] private Transform startPoint;
        [SerializeField] private float initialScale = 1f;

        [Header("Live Timer")]
        [SerializeField] private float liveTime;
        [SerializeField] private bool startLimeTimingOnStart;

        public static TimeInEvent liveTimeIn;
        public static TimeOutEvent liveTimeOut;
        public static TimerUpdateEvent liveTimeUpdate;
        
        private PlayerController Player { get; set; }
        private Timer LiveTimer { get; set; }

        private void Awake()
        {
            liveTimeOut += LiveTimeOut;
            LiveTimer = new Timer(liveTime, liveTimeIn, liveTimeOut, liveTimeUpdate);
            
            var inScenePlayer = FindObjectOfType<PlayerController>();
            
            Player = inScenePlayer == null 
                ? Instantiate(playerPrefab) 
                : inScenePlayer;
            
            Player.ShellChanged += ShellChanged;

            Respawn();

            if (startingShellPrefab != null)
            {
                var shell = Instantiate(startingShellPrefab);
                Player.PickUp(shell);
            }

        }

        private void LiveTimeOut()
        {
            Player.Killed?.Invoke();
        }

        private void ShellChanged(ShellController shell)
        {
            if (Player.HasShell)
            {
                LiveTimer.Stop(this);
                liveTimeUpdate?.Invoke(1);
            }
            else if (!LiveTimer.IsRunning) 
            {
                LiveTimer.Start(this);
            }
        }

        private void Start()
        {
            if (startLimeTimingOnStart)
            {
                LiveTimer.Start(this);
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
                    PlayerPrefs.SetInt(LastCheckPointKey, checkPoint.GetHashCode()); 
                    PlayerPrefs.SetFloat(LastPlayerScaleKey, Player.transform.localScale.x);
                    PlayerPrefs.Save();
                    break;
                }

                case ScaleModifier scaleModifier:
                {
                    Player.Grow(scaleModifier.Scale);
                    break;
                }
            }
        }

        private void OnTriggerOut(GameTrigger trigger, Component actor)
        {
            // switch (trigger)
            // {
            //     case CheckPoint checkPoint:
            //         break;
            // }
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