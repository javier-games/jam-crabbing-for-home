using CrabAssets.Scripts.Shells;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CrabAssets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private new Rigidbody2D rigidbody2D;

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
        public System.Action<bool> HasShellChanged;
        
        private float _currentHorizontalInput;
        private float _currentVelocity;
        private float _currentSpeed;
        private float _currentAcceleration;
        private int _jumpCount;
        private bool _hasShell;

        private int HorizontalInputTarget { get; set; }

        private Vector2 _dPad;
        
        public bool IsFlipped { get; private set; }

        public bool IsGrounded { get; private set; }

        public bool HasShell
        {
            get => _hasShell;
            set
            {
                if (value == _hasShell)
                {
                    return;
                }

                _hasShell = value;
                HasShellChanged?.Invoke(_hasShell);
            }
        }

        public bool IsFalling => rigidbody2D && rigidbody2D.velocity.y < 0;


        #region Event Methods

#if UNITY_EDITOR
        public void Reset()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }
#endif

        private void FixedUpdate()
        {
            HorizontalMovement();
        }
        
        // Detects the ground and rigid objects consider as ground.
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger)
            {
                return;
            }
            
            IsGrounded = true;
            _jumpCount = ungroundedJumps;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.isTrigger)
            {
                return;
            }
            
            IsGrounded = false;
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

        #endregion
        

        #region Methods

        private void HorizontalMovement()
        {
            var inertia = 0f;
            var targetSpeed = 0f;
            
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
            
            rigidbody2D.velocity = new Vector2(
                x: _currentHorizontalInput * _currentSpeed * (HasShell ? shellFrictionFactor : 1), 
                y: rigidbody2D.velocity.y
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
            
            rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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
                HasShell = false;
            }
            else
            {
                HasShell = shellHandler.TryPickUp(direction, IsFlipped);
            }
        }

        public void PickUp(ShellController shell)
        {
            if (HasShell)
            {
                Pick();
            }

            HasShell = shellHandler.TryPickUp(shell, IsFlipped);
        }

        #endregion


#if UNITY_EDITOR
        
        [ContextMenu("GrowUp")]
        public void GrowUp() => Grow(0.25f);

        [ContextMenu("GrowDown")]
        public void GrowDown() => Grow(-0.25f);
        
#endif

        public void Grow(float scale)
        {
            sizeController.Grow(scale);
            HasShell = shellHandler.TryToGrow(sizeController.Scale);
        }
        
    }
}
