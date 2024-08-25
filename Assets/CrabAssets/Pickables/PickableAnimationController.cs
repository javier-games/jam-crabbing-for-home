using UnityEngine;

namespace CrabAssets.Pickables
{
    public class PickableAnimationController : MonoBehaviour
    { 
        [SerializeField]
        private GameObject anchorFlipped;
        
        [SerializeField] 
        private GameObject anchorUnFlipped;
        
        [SerializeField] 
        private GameObject[] onPickedIllustrations;

        public void Flip(bool flipX)
        {
            anchorFlipped.SetActive(flipX);
            anchorUnFlipped.SetActive(!flipX);
        }

        public void Picked(bool picked)
        {
            for (var i = 0; i < onPickedIllustrations.Length; i++)
            {
                onPickedIllustrations[i].SetActive(picked);
            }
        }
    }
}