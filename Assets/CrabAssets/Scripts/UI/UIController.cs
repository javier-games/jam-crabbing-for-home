using CrabAssets.Scripts.Game;
using UnityEngine;

namespace CrabAssets.Scripts.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private LiveTimerController liveTimer;
        [SerializeField] private RadiationTimerUI radiationTimer;
        
        private void Awake()
        {
            liveTimer.enabled = false;
            radiationTimer.enabled = false;
            
            GameController.LiveTimeIn += LiveTimeIn;
            GameController.LiveTimeUpdate += LiveTimeUpdate;
            GameController.RadiationTimeIn += RadiationTimeIn;
            GameController.RadiationTimeUpdate += RadiationTimeUpdate;
        }

        private void RadiationTimeUpdate(float progress)
        {
            radiationTimer.RadiationAmount = progress;
        }

        private void RadiationTimeIn()
        {
            if (radiationTimer.enabled)
            {
                return;
            }

            radiationTimer.enabled = true;
        }

        private void OnDestroy()
        {
            GameController.LiveTimeIn -= LiveTimeIn;
            GameController.LiveTimeUpdate -= LiveTimeUpdate;
            GameController.RadiationTimeIn -= RadiationTimeIn;
            GameController.RadiationTimeUpdate -= RadiationTimeUpdate;
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