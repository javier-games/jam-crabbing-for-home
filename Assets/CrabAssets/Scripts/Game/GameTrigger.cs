using UnityEngine;

namespace CrabAssets.Scripts.Game
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class GameTrigger : MonoBehaviour
    {
        private const string PlayerTag = "Player";
        
        [SerializeField]
        [HideInInspector] 
        private new Collider2D collider2D;

        protected Collider2D Collider2D => collider2D;
        
        public static OnTriggerInEvent OnTriggerIn { get; set; }
        
        public static OnTriggerOutEvent OnTriggerOut { get; set; }

#if UNITY_EDITOR
        private void Reset()
        {
            collider2D = GetComponent<Collider2D>();
        }
#endif
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!DidEnter(other, out var actor))
            {
                return;
            }
            
            OnTriggerIn?.Invoke(this, actor);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!DidExit(other, out var actor))
            {
                return;
            }
            
            OnTriggerOut?.Invoke(this, actor);
        }


        protected abstract bool DidEnter(Collider2D other, out Component actor);
        protected abstract bool DidExit(Collider2D other, out Component actor);
        
        protected bool IsPlayer(Collider2D other, out Component player)
        {
            var otherRigidBody = other.attachedRigidbody;
            if ((object) otherRigidBody == null)
            {
                player = null;
                return false;
            }
        
            if (!otherRigidBody.CompareTag(PlayerTag))
            {
                player = null;
                return false;
            }
        
            player = otherRigidBody; // not required tho.
            return true;
        }
    }
    
    public delegate void OnTriggerInEvent(GameTrigger trigger, Component actor);
    public delegate void OnTriggerOutEvent(GameTrigger trigger, Component actor);
}