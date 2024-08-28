using System;
using System.Collections.Generic;
using CrabAssets.Scripts.Shells;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CrabAssets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D rigidBody2D;

        [SerializeField]
        private new Collider2D collider2D;

        [SerializeField] 
        private ShellHandler shellHandler;

        [SerializeField]
        [Range(0f, 1f)]
        private float shellFrictionFactor = 0.5f;

        [SerializeField] 
        private SizeController sizeController;
        
        [SerializeField]
        private float jumpForce;

        [SerializeField] 
        private int ungroundedJumps = 1;
        
        [SerializeField]
        private float ungroundedSpeed = 4;
        
        [SerializeField]
        private float ungroundedInertia = 0.12f;
        
        [SerializeField]
        private float groundedSpeed = 4;
        
        [SerializeField]
        private float groundedInertia = 0.12f;

        [SerializeField] 
        private float speedSmoothness = 0.5f;

        public System.Action Jumped;
        public System.Action<float> HorizontalMove;
        public System.Action Killed;
        
        private float _currentHorizontalInput;
        private float _currentVelocity;
        private float _currentSpeed;
        private float _currentAcceleration;
        private int _jumpCount;
        private bool _hasShell;
        private Vector2 _dPad;
        private readonly List<ContactPoint2D> _contacts = new List<ContactPoint2D>();
        
        private ContactFilter2D _leftContact = new ContactFilter2D()
        {
            useNormalAngle = true,
            minNormalAngle = -45,
            maxNormalAngle = 45
        };

        private readonly ContactFilter2D _rightContact = new ContactFilter2D()
        {
            useNormalAngle = true,
            minNormalAngle = 115,
            maxNormalAngle = 225,
        };

        private readonly ContactFilter2D _bottomContact = new ContactFilter2D()
        {
            useNormalAngle = true,
            minNormalAngle = 45,
            maxNormalAngle = 115
        };
        
        private int HorizontalInputTarget { get; set; }
        public bool IsFlipped { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool HasShell => shellHandler.HasShell;
        public bool IsFalling => rigidBody2D && rigidBody2D.velocity.y < 0;

        public ShellChangeEvent ShellChanged
        {
            get => shellHandler.ShellChanged;
            set => shellHandler.ShellChanged = value;
        }
        
#if UNITY_EDITOR
        public void Reset()
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
        }
#endif

        private void OnEnable()
        {
            Killed += OnKilled;
        }

        private void OnDisable()
        {
            Killed -= OnKilled;
        }
        
        private void FixedUpdate()
        {
            var touchingRight = collider2D.GetContacts(_rightContact, _contacts) > 0;
            var touchingLeft = collider2D.GetContacts(_leftContact, _contacts) > 0;
            var touchingBottom = collider2D.GetContacts(_bottomContact, _contacts) > 0;

            IsGrounded = touchingLeft || touchingRight || touchingBottom;
            if (IsGrounded)
            {
                _jumpCount = ungroundedJumps;
            }
            HorizontalMovement();
        }

        public void OnHorizontalInput(InputAction.CallbackContext context)
        {
            _dPad.x = context.ReadValue<float>();
            HorizontalInputTarget = Mathf.RoundToInt(_dPad.x);
        }

        public void OnVerticalInput(InputAction.CallbackContext context)
        {
            _dPad.y = context.ReadValue<float>();
        }

        public void OnJumpInput(InputAction.CallbackContext context)
        {
            if (HasShell)
            {
                return;
            }
            
            if (context.ReadValue<float>()>0)
            {
                Jump();
            }
        }

        public void OnPickInput(InputAction.CallbackContext context)
        {
            if (context.ReadValue<float>()>0)
            {
                Pick();
            }
        }

        private void HorizontalMovement()
        {
            float inertia;
            float targetSpeed;

            if (IsGrounded)
            {
                inertia = groundedInertia;
                targetSpeed = groundedSpeed;
            }
            else
            {
                inertia = ungroundedInertia;
                targetSpeed = ungroundedSpeed;
            }

            if (HorizontalInputTarget == 0)
            {
                targetSpeed = 0;
            }
            else
            {
                IsFlipped = HorizontalInputTarget < 0;
                shellHandler.Flip(IsFlipped);
            }
            
            _currentHorizontalInput = Mathf.SmoothDamp(
                _currentHorizontalInput,
                HorizontalInputTarget,
                ref _currentVelocity,
                inertia
            );

            _currentSpeed = Mathf.SmoothDamp(
                _currentSpeed,
                targetSpeed,
                ref _currentAcceleration,
                speedSmoothness
            );
            
            rigidBody2D.velocity = new Vector2(
                x: _currentHorizontalInput * _currentSpeed * (HasShell ? shellFrictionFactor : 1), 
                y: rigidBody2D.velocity.y
            );
            
            HorizontalMove?.Invoke(_currentHorizontalInput);
        }

        private void Jump()
        {
            if (!IsGrounded)
            {
                if (_jumpCount <= 0)
                {
                    return;
                }

                _jumpCount--;
            }
            
            rigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Jumped?.Invoke();
        }

        private void Pick()
        {
            var direction = _dPad == Vector2.zero
                ? IsFlipped ? Vector2.left : Vector2.right 
                : _dPad.normalized;
            
            if (HasShell)
            {
                shellHandler.DropOut(direction, IsGrounded);
            }
            else
            {
                shellHandler.TryPickUp(direction, IsFlipped);
            }
        }

        public void PickUp(ShellController shell)
        {
            if (HasShell)
            {
                Pick();
            }

            shellHandler.TryPickUp(shell, IsFlipped);
        }


#if UNITY_EDITOR
        
        [ContextMenu("GrowUp")]
        public void GrowUp() => Grow(0.25f);

        [ContextMenu("GrowDown")]
        public void GrowDown() => Grow(-0.25f);
        
#endif

        public void Grow(float scale)
        {
            shellHandler.HoldShellScale(sizeController.Scale + scale);
            sizeController.Grow(scale, shellHandler.ReleaseShellScale);
        }
        
        private void OnKilled()
        {
            rigidBody2D.AddForce(Vector2.up * 2, ForceMode2D.Impulse);
            collider2D.enabled = false;
        }
    }
}
