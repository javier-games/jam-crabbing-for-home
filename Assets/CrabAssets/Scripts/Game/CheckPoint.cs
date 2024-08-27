using UnityEngine;

namespace CrabAssets.Scripts.Game
{
    public class CheckPoint : GameTrigger
    {
        private const string PlayerTag = "Player";

        [SerializeField] 
        private Transform spawnPosition;
    
        [SerializeField] 
        private Animator animator;

        private static readonly int Check = Animator.StringToHash("Check");
        private static readonly int Reset = Animator.StringToHash("Reset");

        protected override bool DidEnter(Collider2D other, out Component actor)
        {
            var isPlayer = IsPlayer(other, out actor);
        
            if (isPlayer)
            {
                animator.SetTrigger(Check);
                Collider2D.enabled = false;
            }
        
            return isPlayer;
        }

        protected override bool DidExit(Collider2D other, out Component actor)
        {
            return IsPlayer(other, out actor);
        }

        public void PlaceActor(Transform actorTransform)
        {
            actorTransform.SetPositionAndRotation(spawnPosition.position, Quaternion.identity);
        }

        private static bool IsPlayer(Collider2D other, out Component player)
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
}
