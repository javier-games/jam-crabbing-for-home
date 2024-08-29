using UnityEngine;
using UnityEngine.UI;

namespace CrabAssets.Scripts.UI
{
    public class LiveTimerController : MonoBehaviour
    {
        [SerializeField] private Image imageFill;
        [SerializeField] private Animator animator;
        
        private static readonly int Active = Animator.StringToHash("Active");
        private static readonly int Live = Animator.StringToHash("Live");
        
        public float LiveAmount
        {
            get => imageFill.fillAmount;

            set
            {
                imageFill.fillAmount = value;
                animator.SetFloat(Live, value);
            }
        }

        private void OnEnable()
        {
            LiveAmount = 1;
            animator.SetBool(Active, true);
        }

        private void OnDisable()
        {
            animator.SetBool(Active, false);
        }
    }
}