using System.Collections;
using CrabAssets.Scripts.Player;
using CrabAssets.Scripts.Shells;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] private bool startLiveTimingOnStart;

        [Header("Radiation Timer")] 
        [SerializeField]
        private float radiationTime;
        [SerializeField] 
        private float radiationSizeIncrement = 0.25f;

        public static TimeInEvent liveTimeIn;
        public static TimeOutEvent liveTimeOut;
        public static TimerUpdateEvent liveTimeUpdate;

        public static TimeInEvent radiationTimeIn;
        public static TimeOutEvent radiationTimeOut;
        public static TimerUpdateEvent radiationTimeUpdate;
        
        private PlayerController Player { get; set; }
        private Timer LiveTimer { get; set; }
        private Timer RadiationTimer { get; set; }

        private void Awake()
        {
            liveTimeOut += Kill;
            LiveTimer = new Timer(liveTime, liveTimeIn, liveTimeOut, liveTimeUpdate);
            radiationTimeOut += RadiationTimeOut;
            RadiationTimer = new Timer(radiationTime, radiationTimeIn, radiationTimeOut, radiationTimeUpdate);
            
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

        private void OnDestroy()
        {
            liveTimeOut -= Kill;
            radiationTimeOut -= RadiationTimeOut;
        }

        private void RadiationTimeOut()
        {
            Player.Grow(radiationSizeIncrement);
            RadiationTimer.Stop(this);
            RadiationTimer.Start(this);
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
            if (startLiveTimingOnStart)
            {
                LiveTimer.Start(this);
            }
        }

        private void OnEnable()
        {
            GameTrigger.OnTriggerIn += OnTriggerIn;
            GameTrigger.OnTriggerOut += OnTriggerOut;
        }

        private void OnDisable()
        {
            GameTrigger.OnTriggerIn -= OnTriggerIn;
            GameTrigger.OnTriggerOut -= OnTriggerOut;
        }

        private void OnTriggerIn(GameTrigger trigger, Component _)
        {
            switch (trigger)
            {
                case CheckPoint checkPoint:
                {
                    PlayerPrefs.SetString(LastCheckPointKey, checkPoint.gameObject.name); 
                    PlayerPrefs.SetFloat(LastPlayerScaleKey, Player.transform.localScale.x);
                    PlayerPrefs.Save();
                    break;
                }

                case ScaleModifier scaleModifier:
                {
                    Player.Grow(scaleModifier.Scale);
                    break;
                }

                case RadiationModifier radiationModifier:
                {
                    if (!RadiationTimer.IsRunning)
                    {
                        RadiationTimer.Start(this);
                    }
                    
                    RadiationTimer.IncreaseCurrentTime(radiationModifier.RadiationAmount);
                    break;
                }

                case EndGameTrigger endGameTrigger:
                {
                    StartCoroutine(LoadSceneCoroutine("EndScene"));
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
                var checkPointName = PlayerPrefs.GetString(LastCheckPointKey);
                var checkPoints = FindObjectsOfType<CheckPoint>();

                for (var i = 0; i < checkPoints.Length; i++)
                {
                    if (checkPoints[i].name != checkPointName)
                    {
                        continue;
                    }

                    checkPoint = checkPoints[i];
                }
            }

            if ((object) checkPoint != null)
            {
                checkPoint.PlaceActor(Player.transform);
                
                if (!RadiationTimer.IsRunning)
                {
                    RadiationTimer.Start(this);
                }

                if (!LiveTimer.IsRunning)
                {
                    LiveTimer.Start(this);
                }
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
            PlayerPrefs.DeleteAll();
        }

        private void Kill()
        {
            Player.Killed?.Invoke();
            RadiationTimer.Stop(this);
            LiveTimer.Stop(this);
            StartCoroutine(LoadSceneCoroutine("MenuScene"));
        }

        private IEnumerator LoadSceneCoroutine(string scene)
        {
            RadiationTimer.Stop(this);
            LiveTimer.Stop(this);
            yield return new WaitForSeconds(3);
            ClearPlayerPrefs();
            SceneManager.LoadScene(scene);
        }
    }
}