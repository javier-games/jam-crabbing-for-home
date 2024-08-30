using UnityEngine;
using UnityEngine.UI;

namespace CrabAssets.Scripts.UI
{
    public class RadiationTimerUI : MonoBehaviour
    {
        [SerializeField] 
        private Animator animator;
        
        [SerializeField] 
        private Image imageFill;
        
        
        private static readonly int Active = Animator.StringToHash("Active");
        private static readonly int Radiation = Animator.StringToHash("Radiation");
        
        public float RadiationAmount
        {
            get => 1 - imageFill.fillAmount;

            set
            {
                imageFill.fillAmount = 1 - value;
                animator.SetFloat(Radiation, imageFill.fillAmount);
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