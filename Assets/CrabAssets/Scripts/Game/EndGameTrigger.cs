using UnityEngine;

namespace CrabAssets.Scripts.Game
{
    public class EndGameTrigger : GameTrigger
    {
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
