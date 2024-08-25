using System;
using System.Collections.Generic;
using CrabAssets.Pickables;
using UnityEngine;
using UnityEngine.Serialization;

public class ShellHandler: MonoBehaviour {
    
    [FormerlySerializedAs("homeAnchor")] 
    [SerializeField] 
    private GameObject anchorUnFlipped;

    [FormerlySerializedAs("homeAnchorFliped")] 
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
    
    private ShellController ShellController { get; set; }

    public bool HasShell => (object) ShellController != null;


    private List<ContactPoint2D> _contacts = new List<ContactPoint2D>();// = new List<ContactPoint2D>();
    private List<ContactMatch> _matches = new List<ContactMatch>();
    struct ContactMatch
    {
        public int index;
        public float match;
    }

    public bool TryPickUp(Vector2 direction, bool isFlipped)
    {
        var count = collider2D.GetContacts(_contacts);
        
        for (var i = 0; i < count; i++)
        {
            var contact = _contacts[i];
            
            if (!contact.collider.CompareTag(shellTag))
            {
                continue;
            }

            var matchValue = Mathf.Abs(Vector2.Dot(contact.normal, direction));
            _matches.Add(new ContactMatch(){index = i, match = matchValue});
        }

        if (_matches.Count > 1)
        {
            _matches.Sort(((a, b) => a.match.CompareTo(b.match)));
        }
        
        for (var i = 0; i < _matches.Count; i++)
        {
            if (!TryGetInShell(_contacts[_matches[i].index].collider, isFlipped))
            {
                continue;
            }
            
            _matches.Clear();
            return true;
        }
        
        _matches.Clear();
        
        var raycast = Physics2D.Raycast(transform.position + new Vector3(rayOffset.x, rayOffset.y), direction, rayDistance);
        
        if ((object)raycast.collider == null)
        {
            return false;
        }
        
        if (!raycast.collider.CompareTag(shellTag))
        {
            return false;
        }
        
        return TryGetInShell(raycast.collider, isFlipped);
    }

    private bool TryGetInShell(Collider2D other, bool isFlipped)
    {
        var shell = other.GetComponent<ShellController>();
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
    
    public void Grow (float scale) {

        if (!HasShell)
        {
            return;
        }
        
        if (ShellController.TryToGrow(scale))
        {
            return;
        }

        ShellController = null;
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
