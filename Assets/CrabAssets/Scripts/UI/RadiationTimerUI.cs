using UnityEngine;
using UnityEngine.UI;

namespace CrabAssets.Scripts.UI
{
    public class RadiationTimerUI : MonoBehaviour
    {
        [SerializeField] 
        private Animator animator;
        
        [SerializeField] 
        private Slider slider;
        
        
        private static readonly int Active = Animator.StringToHash("Active");
        private static readonly int Radiation = Animator.StringToHash("Radiation");
        
        public float RadiationAmount
        {
            get => 1 - slider.value;

            set
            {
                slider.value = 1 - value;
                animator.SetFloat(Radiation, slider.value);
            }
        }

        private void OnEnable()
        {
            RadiationAmount = 0;
            animator.SetBool(Active, true);
        }

        private void OnDisable()
        {
            animator.SetBool(Active, false);
        }
    }
}