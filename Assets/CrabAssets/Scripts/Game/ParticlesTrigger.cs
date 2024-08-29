using UnityEngine;
using UnityEngine.Serialization;

namespace CrabAssets.Scripts.Game
{
    public class ParticlesTrigger : GameTrigger
    {
        [SerializeField] 
        private ParticlesType type;

        public ParticlesType ParticleType => type;
        
        protected override bool DidEnter(Collider2D other, out Component actor)
        {
            return IsPlayer(other, out actor);
        }

        protected override bool DidExit(Collider2D other, out Component actor)
        {
            return IsPlayer(other, out actor);
        }
    }
}
