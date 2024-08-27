using System.Collections;
using UnityEngine;

namespace CrabAssets.Scripts.Shells
{
    [ExecuteInEditMode]
    public class ShellController : MonoBehaviour
    {
        [SerializeField]
        private PickableAnimationController animationController;

        [SerializeField] private float minResistance = 1;
        [SerializeField] private float maxResistance = 2;
        [SerializeField] private float destructionDelay = 4;

        [SerializeField] private new Rigidbody2D rigidbody2D;

        private bool _flip;

        public bool Flip
        {
            get => _flip;

            set
            {
                _flip = value;
                animationController.Flip(_flip);
            }
        }

        private void OnEnable()
        {
            animationController.Picked(false);
            rigidbody2D.simulated = true;
        }

        private void OnDisable()
        {
            animationController.Picked(true);
            rigidbody2D.simulated = false;
        }

        public bool TryToGrow(float scale)
        {
            if (scale > maxResistance)
            {
                Break();
                return false;
            }

            if (scale < minResistance )
            {
                Throw(Vector2.zero);
                return false;
            }

            return scale >= minResistance;
        }

        public bool CanBePicked(Vector2 scale)
        {
            return scale.x >= minResistance && scale.x <= maxResistance && enabled;
        }

        public void Throw(Vector2 force)
        {
            enabled = true;
            rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        }

        private void Break()
        {
            transform.parent = null;
            enabled = true;
            StartCoroutine (DestroyShell (destructionDelay));
        }

        private IEnumerator DestroyShell (float time) {
            // Apply destruction animation.
            yield return new WaitForSeconds (time);
            Destroy (gameObject);
        }

        [ContextMenu("Toggle")]
        private void ToggleFlip()
        {
            Flip = !Flip;
        }
    }
}
