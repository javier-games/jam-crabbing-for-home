using UnityEngine;

namespace CrabAssets.Scripts.Game
{
    public class ScaleModifier : GameTrigger
    {
        [Range(0f,2f)]
        [SerializeField]
        private float scaleToAdd;

        [SerializeField]
        private bool disableOnTrigger;

        public float Scale => scaleToAdd;
        
        protected override bool DidEnter(Collider2D other, out Component actor)
        {
            var isPlayer = IsPlayer(other, out actor);
            if (isPlayer && disableOnTrigger)
            {
                // TODO: Save and reset modifiers to their saved state.
                Collider2D.enabled = false;
            }
            return isPlayer;
        }

        protected override bool DidExit(Collider2D other, out Component actor)
        {
            return IsPlayer(other, out actor);
        }
    }
}