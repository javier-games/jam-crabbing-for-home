using UnityEngine;

namespace CrabAssets.Scripts.Game
{
    public class RadiationModifier : GameTrigger
    {
        [SerializeField] 
        private float radiationAmount;

        public float RadiationAmount => radiationAmount;
        
        protected override bool DidEnter(Collider2D other, out Component actor)
        {
            var isPlayer = IsPlayer(other, out actor);

            if (isPlayer)
            {
                Destroy(gameObject);
            }
            
            return isPlayer;
        }

        protected override bool DidExit(Collider2D other, out Component actor)
        {
            return IsPlayer(other, out actor);
        }
    }
}