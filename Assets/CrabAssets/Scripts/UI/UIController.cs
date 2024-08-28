using CrabAssets.Scripts.Game;
using UnityEngine;

namespace CrabAssets.Scripts.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private LiveTimerController liveTimer;
        
        private void Awake()
        {
            liveTimer.enabled = false;
            
            GameController.liveTimeIn += LiveTimeIn;
            GameController.liveTimeUpdate += LiveTimeUpdate;
        }

        private void OnDestroy()
        {
            GameController.liveTimeIn -= LiveTimeIn;
            GameController.liveTimeUpdate += LiveTimeUpdate;
        }

        private void LiveTimeIn()
        {
            if (liveTimer.enabled)
            {
                return;
            }
            
            liveTimer.enabled = true;
        }

        private void LiveTimeUpdate(float progress)
        {
            liveTimer.LiveAmount = progress;
        }
    }
}