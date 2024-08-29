using System.Collections.Generic;
using UnityEngine;

namespace CrabAssets.Scripts.Shells
{
    public class ShellHandler: MonoBehaviour {
    
        [SerializeField] 
        private GameObject anchorUnFlipped;

        [SerializeField]
        private GameObject anchorFlipped;

        [SerializeField]
        private new Collider2D collider2D;

        [SerializeField] 
        private float rayDistance = 1f;

        [SerializeField]
        private Vector2 rayOffset;
    
        [SerializeField] 
        private float launchForce;

        [SerializeField] 
        private string shellTag;

        private ShellController _shellController;
        private Transform _previousShellAnchor;

        private ShellController ShellController
        {
            get => _shellController;
            set
            {
                if (_shellController == value)
                {
                    return;
                }

                _shellController = value;
                ShellChanged?.Invoke(value);
            }
        }

        public bool HasShell => (object) ShellController != null;

        public ShellChangeEvent ShellChanged { get; set; }

        private readonly List<ContactPoint2D> _contacts = new List<ContactPoint2D>();// = new List<ContactPoint2D>();
        private readonly List<ContactMatch> _matches = new List<ContactMatch>();

        private struct ContactMatch
        {
            public int Index;
            public float Match;
        }

        public bool TryPickUp(Vector2 direction, bool isFlipped)
        {
            var count = collider2D.GetContacts(_contacts);

            for (var i = 0; i < count; i++)
            {
                var contact = _contacts[i];

                var contactRigidBody = contact.collider.attachedRigidbody;
                if ((object)contactRigidBody == null || !contactRigidBody.CompareTag(shellTag))
                {
                    continue;
                }

                var matchValue = Mathf.Abs(Vector2.Dot(contact.normal, direction));
                _matches.Add(new ContactMatch(){Index = i, Match = matchValue});
            }

            if (_matches.Count > 1)
            {
                _matches.Sort(((a, b) => a.Match.CompareTo(b.Match)));
            }
        
            for (var i = 0; i < _matches.Count; i++)
            {
                if (!TryGetShellFromCollider(_contacts[_matches[i].Index].collider, out var collidedShell))
                {
                    continue;
                }
            
                _matches.Clear();
                
                return TryPickUp(collidedShell, isFlipped);
            }
        
            _matches.Clear();
        
            var rayCast = Physics2D.Raycast(transform.position + new Vector3(rayOffset.x, rayOffset.y), direction, rayDistance);

            return TryGetShellFromCollider(rayCast.collider, out var rayCastedShell) 
                   && TryPickUp(rayCastedShell, isFlipped);
        }
    
        private bool TryGetShellFromCollider(Collider2D other, out ShellController shell)
        {
            shell = null;
            if ((object) other == null)
            {
                return false;
            }
        
            var rigidBody = other.attachedRigidbody;
            if ((object) rigidBody == null)
            {
                return false;
            }

            if (!rigidBody.CompareTag(shellTag))
            {
                return false;
            }
        
            shell = rigidBody.GetComponent<ShellController>();
            return (object)shell != null && shell.CanBePicked(transform.localScale);
        }
    
        public bool TryPickUp(ShellController shell, bool isFlipped)
        {
            if ((object)shell == null || !shell.CanBePicked(transform.localScale))
            {
                return false;
            }
        
            ShellController = shell;
            Flip(isFlipped);
            ShellController.enabled = false;
            return true;
        }
    
        public void DropOut(Vector2 direction, bool isGrounded)
        {
            ShellController.transform.SetParent(null);
        
            var offset = Vector3.zero;
        
            if (direction.x != 0f)
            {
                offset.x = collider2D.bounds.size.x * 0.5f * direction.x / Mathf.Abs(direction.x);
            }

            if (direction.y > 0f)
            {
                offset.y = collider2D.bounds.size.y * 0.5f;
            }
        
            ShellController.transform.position += offset;

            if (direction.y < 0f)
            {
                collider2D.attachedRigidbody.AddForce(direction.y * Vector2.down * launchForce * 0.1f, ForceMode2D.Impulse);
                ShellController.Throw(direction.x * Vector2.right * launchForce);
            }
            else
            {
                ShellController.Throw(direction * launchForce);
            }
        
            ShellController = null;
        }
        
        public void HoldShellScale (float scale) {

            if (!HasShell)
            {
                return;
            }
            
            if (!ShellController.TryToGrow(scale))
            {
                ShellController = null;
                return;
            }

            _previousShellAnchor = ShellController.transform.parent;
            ShellController.transform.SetParent(null);
        }
        

        public void ReleaseShellScale()
        {
            if (!HasShell)
            {
                return;
            }
            
            ShellController.transform.SetParent(_previousShellAnchor);
            _previousShellAnchor = null;
        }
        
        public void Flip (bool flip) {
            if (!HasShell)
            {
                return;
            }
        
            ShellController.Flip = flip;
            ShellController.transform.SetParent(flip ? anchorFlipped.transform : anchorUnFlipped.transform);
            ShellController.transform.localPosition = Vector3.zero;
            ShellController.transform.localRotation = Quaternion.identity;
        }
    }

    public delegate void ShellChangeEvent(ShellController shell);
}
