using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private new Rigidbody2D rigidbody2D;
        
        [SerializeField]
        private float jumpForce;

        [SerializeField] 
        private int ungroundedJumps = 1;

        [FormerlySerializedAs("fallingSpeed")]
        [SerializeField]
        private float ungroundedSpeed = 4;
        
        [FormerlySerializedAs("ungroundedSmoothness")]
        [FormerlySerializedAs("fallingSmoothness")]
        [SerializeField]
        private float ungroundedInertia = 0.12f;

        [FormerlySerializedAs("walkingSpeed")] 
        [SerializeField]
        private float groundedSpeed = 4;
        [FormerlySerializedAs("groundedSmoothness")]
        [FormerlySerializedAs("walkingSmoothness")] 
        [SerializeField]
        private float groundedInertia = 0.12f;

        [FormerlySerializedAs("accelerationSmoothness")] [SerializeField] 
        private float speedSmoothness = 0.5f;

        public System.Action Jumped;
        public System.Action<float> HorizontalMove;


        private float _currentHorizontalInput;
        private float _currentVelocity;
        private float _currentSpeed;
        private float _currentAcceleration;
        private int _jumpCount;

        private int HorizontalInputTarget { get; set; }

        public bool IsGrounded { get; private set; }

        public bool IsFalling => rigidbody2D && rigidbody2D.velocity.y < 0;


        #region Event Methods

#if UNITY_EDITOR
        public void Reset()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }
#endif

        public void FixedUpdate()
        {
            HorizontalMovement();
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            IsGrounded = true;
            _jumpCount = ungroundedJumps;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            IsGrounded = false;
        }

        public void OnHorizontalInput(InputAction.CallbackContext context)
        {
            HorizontalInputTarget = Mathf.RoundToInt(context.ReadValue<float>());
        }

        public void OnJumpInput(InputAction.CallbackContext context)
        {
            if (context.ReadValue<float>()>0)
            {
                Jump();
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
                x: _currentHorizontalInput * _currentSpeed, 
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

        #endregion
    }
}
