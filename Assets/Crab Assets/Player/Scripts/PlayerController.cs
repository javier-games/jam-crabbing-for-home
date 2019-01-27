using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player controller.
/// </summary>
[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (ObjectHandler))]
public class PlayerController: MonoBehaviour {


    #region Class Members


    [SerializeField]
    private State state = State.FALLING;    //  Sate of the current player.

    // Prefab References
    private CharacterAnimationcontroller animationController;
    private new Rigidbody2D rigidbody;
    private new Collider2D collider2D;     
    private ObjectHandler handler;
    private Transform child;
    private SpriteRenderer render;

    //  Game References
    [SerializeField]
    private GameMode gameMode;

    //  Parameters for forward movement.
    [Header ("Forward Variables")]
    [SerializeField]
    protected float forwardSpeed = 2.15f;   //  Speed to move forward.
    public static System.Action<bool> flipAction;

    //  Parameters for climbing movement.
    [Header ("Climbing Variables")]
    [SerializeField]
    protected float climbingMovement;
    [SerializeField, Range (0f, 90f)]
    protected float slopeAngle = 45f;
    [SerializeField]
    protected float slopeDistance = 1f;
    [SerializeField]
    protected Vector2 slopeOffset = Vector2.zero;
    [SerializeField]
    private MegaRay rightRay;
    [SerializeField]
    private MegaRay leftRay;
    protected RaycastHit2D hit;               //  Ray info.

    //  Parameters for falling movement.
    [Header ("Falling Variables")]
    [SerializeField, Range (0f, 2f)]
    protected float fallingMovement = 1f;   //  Forward movement at folling.
    protected bool facingLeft;
    protected bool facingRight;

    //  Parameters for jumping movement.
    [Header ("Jump Variables")]
    [SerializeField]
    protected float jumpForce = 10f;        //  Impulse Force at jump.
    [SerializeField]
    protected float extraForce = 0.01f;     //  Extra impulse at jump.
    [SerializeField]
    protected float maxJumpTime = 3f;       //  Maximum time for jump.
    [SerializeField]
    private MegaRay bottomRay;
    protected float jumpTime;                 //  Time since start jump.

    //  Parameters for jumping movement.
    [Header ("Rotation Variables")]
    [SerializeField]
    protected float smoothRotation = 0.1f;
    protected float angleRotationVelocity;
    protected float angleRotatioTarget;

    [Header ("Growing Variables")]
    [SerializeField]
    protected float growingFactor = 1.5f;
    [SerializeField]
    protected float growingDuration = 1;
    [SerializeField]
    protected AnimationCurve heightGrowingCurve;
    [SerializeField]
    protected AnimationCurve weidthGrowingCurve;
    protected bool isGrowing;
    protected float growingTime;
    protected Vector3 originalScale;
    public static System.Action<float> growUp;

    [Header ("Hurt Variables")]
    [SerializeField]
    private bool test;
    [SerializeField, Range(0,1)]
    private float testTransition;
    [SerializeField]
    protected Color hurtColor;
    [SerializeField]
    protected AnimationCurve hurtColorCurve;
    [SerializeField]
    protected bool canBeHurt;


    //  Inputs variables
    protected float horizontalAxis;           //  Amount to move horizontaly.
    protected bool jumpButton;
    protected bool fireButton;

    public bool HasShell {
        get {
            return handler.hasItem;
        }
    }


    #endregion


    #region MonoBehaviour Overrides

    private void Awake () {

        //  Getting references.
        rigidbody = GetComponent<Rigidbody2D> ();
        collider2D = GetComponent<Collider2D> ();
        handler = GetComponent<ObjectHandler> ();
        child = transform.GetChild (0);
        render = child.GetComponent<SpriteRenderer> ();
        animationController = child.GetComponent<CharacterAnimationcontroller> ();

    }

    public void Update () {
        ReadInputs ();
        UpdateStates ();
        UpdateAnimation ();
    }

    private void FixedUpdate () {

        //  Dying Effect.
        if (state == State.DYING || state == State.DEAD)
            return;

        float delta = Time.deltaTime;

        float x = horizontalAxis * forwardSpeed * delta;
        //  Falling Effect.
        x *= state == State.FALLING ? fallingMovement : 1;
        x = x < 0 && facingLeft ? 0 : x;
        x = x > 0 && facingRight ? 0 : x;

        float y = rigidbody.velocity.y;
        //  Jumping Extra Effect.
        if (state == State.JUMPING && jumpTime < maxJumpTime && Input.GetKey(KeyCode.Space)) {
            float tanh = Mathf.Atan (jumpTime);
            y += extraForce * tanh;
        }
        //  Climbing Effect.
        y += state == State.CLIMBING ? forwardSpeed * delta * climbingMovement : 0;

        rigidbody.velocity = new Vector2 (x, y);

        //  Jump.
        if (state == State.STARTJUMPING) {
            rigidbody.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
            state = State.JUMPING;
        }
    }

    private void OnTriggerEnter2D (Collider2D other) {
        if (gameMode == null) {
            Debug.LogError ("Missing Reference Gamemode in PlayerController");
            return;
        }

        if (other.tag == "Checkpoint") {
            gameMode.SetCheckPoint (other.gameObject);
        }

        if(other.tag == "Collectable") {
            //gameMode.Collectable ();
            Destroy (other.gameObject);
        }
    }

    #endregion

    #region Class Implementation

    private void ReadInputs () {
        horizontalAxis = Input.GetAxis ("Horizontal");
        jumpButton = Input.GetKeyDown (KeyCode.Space);
        fireButton = Input.GetKeyDown (KeyCode.RightShift);

        if (Input.GetKeyDown (KeyCode.P)) {
               GrowUp ();
        }
        if (Input.GetKeyDown (KeyCode.O)) {
            Kill ();
        }
        if (Input.GetKeyDown (KeyCode.RightShift)) {
            if (!handler.hasItem) {
                if (leftRay.hittedObject != null && leftRay.hittedObject.CompareTag ("Pickable")) {
                    handler.Pick (leftRay.hittedObject.transform);
                    gameMode.StopTimer();
                }
                else if (rightRay.hittedObject != null && rightRay.hittedObject.CompareTag ("Pickable")) {
                    handler.Pick (rightRay.hittedObject.transform);
                    gameMode.StopTimer ();
                }
            }
            else if (handler.hasItem) {
                handler.Drop ();
                if (canBeHurt)
                    gameMode.BeginTimer ();
            }
        }

        if(test) {
            Hurt (testTransition);
        }
    }

    private void UpdateStates () {

        if (state == State.DYING || state == State.DEAD)
            return;

        State lastState = state;
        state = State.FALLING;

        if (bottomRay.IsHitting (transform)) {
            if (horizontalAxis > 0 || horizontalAxis < 0)
                state = State.WALKING;
            else
                state = State.IDLE;

            jumpTime = 0;
        }

        if(
            state != State.JUMPING &&
            (state == State.WALKING || state == State.IDLE) &&
            jumpButton && !handler.hasItem
        ) {
            state = State.STARTJUMPING;
            jumpTime = 0;
        }

        if (lastState == State.JUMPING && rigidbody.velocity.y > 0) {
            state = State.JUMPING;
            jumpTime += Time.deltaTime;
        }

        facingLeft = leftRay.IsHitting (transform);
        facingRight = rightRay.IsHitting (transform);
        if (
            (state == State.IDLE || state == State.WALKING) &&
            (facingLeft && CanClimb (-1) ||
            facingRight && CanClimb (1)) &&
            (Mathf.Abs(horizontalAxis) > 0 && rigidbody.velocity.x < 0.1f)
        )
            state = State.CLIMBING;

        if (state == State.FALLING)
            jumpTime = 0;
    }

    private void UpdateAnimation () {
        switch (state) {
            case State.FALLING:
            animationController.PlayAnimation (2);
            angleRotatioTarget = 0;
            break;
            case State.WALKING:
            animationController.PlayAnimation (1);
            angleRotatioTarget = 0;
            break;
            case State.IDLE:
            animationController.PlayAnimation (0);
            angleRotatioTarget = 0;
            break;
            case State.JUMPING:
            animationController.PlayAnimation (3);
            angleRotatioTarget = 0;
            break;
            case State.CLIMBING:
            if (horizontalAxis > 0 || horizontalAxis < 0) {
                animationController.PlayAnimation (1);
                angleRotatioTarget = horizontalAxis > 0 ? 90 : -90;
            }
            else {
                animationController.PlayAnimation (0);
                angleRotatioTarget = 0;
            }
            break;
            case State.DYING:
                animationController.PlayAnimation (4);
                angleRotatioTarget = 0;
                state = State.DEAD;
            break;
        }

        //  Set Animation.
        child.eulerAngles = Mathf.SmoothDampAngle (
            current: child.eulerAngles.z,
            target: angleRotatioTarget,
            currentVelocity: ref angleRotationVelocity,
            smoothTime: smoothRotation
        ) * Vector3.forward;


        //  Set Flip Animation.
        if (state == State.DYING || state == State.DEAD)
            return;
        render.flipX = horizontalAxis < 0;
        if (flipAction != null)
            flipAction.Invoke (render.flipX);

        //  Applying Growing Animation
        if (isGrowing) {

            growingTime += Time.deltaTime;
            float t = growingTime / growingDuration;

            transform.localScale = new Vector3 (
                originalScale.x * (1 + weidthGrowingCurve.Evaluate (t) * growingFactor),
                originalScale.y * (1 + heightGrowingCurve.Evaluate (t) * growingFactor),
                originalScale.z
            );

            if (growingTime > growingDuration) {
                isGrowing = false;
                growingTime = 0;
            }
        }
    }

    private bool CanClimb (float x) {
        float sense = x < 0 ? -1 : 1;
        float mult = (transform.localScale.x + transform.localScale.y) * 0.5f;
        float slopeRad = slopeAngle * Mathf.Deg2Rad;
        hit = Physics2D.Raycast (
            origin: transform.TransformPoint (slopeOffset),
            direction: new Vector2 (
                x: sense * Mathf.Cos (slopeRad),
                y: Mathf.Sin (slopeRad)),
            distance: slopeDistance * mult
        );

        #if UNITY_EDITOR
        Debug.DrawRay (
            start: transform.TransformPoint (slopeOffset),
            dir: new Vector2 (
                x: sense * Mathf.Cos (slopeRad),
                y: Mathf.Sin (slopeRad)).normalized * slopeDistance * mult,
            color: Color.magenta
        );
        #endif

        return hit.collider == null;
    }

    public void Kill () {
        state = State.DYING;
    }

    public void GrowUp () {
        isGrowing = true;
        growingTime = 0;
        originalScale = transform.localScale;
        if (growUp != null)
            growUp.Invoke((transform.localScale.x + transform.localScale.y) * 0.5f);
    }

    public void Hurt (float t) {
        if (!canBeHurt)
            return;

        if (Camera.main != null) {
            CameraVibe cameraVibe = Camera.main.GetComponent<CameraVibe> ();
            if (cameraVibe != null) {
                cameraVibe.MoveCamera (t);
            }
        }
        render.color = Color.Lerp (Color.white, hurtColor, hurtColorCurve.Evaluate (t));
    }

    public void SetCanBeHurtable (bool canBeHurt) {
        this.canBeHurt = canBeHurt;
    }
    #endregion


    #region Nested Members

    //  State of the player.
    private enum State {
        IDLE,
        WALKING,
        FALLING,
        CLIMBING,
        JUMPING,
        STARTJUMPING,
        DYING,
        DEAD
    }

    [System.Serializable]
    private struct MegaRay{

        public Vector2 origen;
        public Vector2 offset;
        public Vector2 direction;
        public float magnitude;
        public float bulk;
        public Color color;

        [HideInInspector]
        public GameObject hittedObject;

        public bool IsHitting (Transform transform) {

            Vector2 normal = Vector2.Perpendicular (direction);
            Vector2 position = transform.position;
            float mult = (transform.localScale.x + transform.localScale.y) * 0.5f;
            Collider2D collider;

            collider = GetHit (
                origin: position - normal * 0.5f * mult * bulk,
                dir: direction,
                mult: mult
            ).collider;

            if (collider != null) {
                hittedObject = collider.gameObject;
                return true;
            }

            collider = GetHit (
                origin: position + normal * 0f * mult * bulk,
                dir: direction,
                mult: mult
            ).collider;

            if (collider != null) {
                hittedObject = collider.gameObject;
                return true;
            }

            collider = GetHit (
                origin: position + normal * 0.5f * mult * bulk,
                dir: direction,
                mult: mult
            ).collider;

            if (collider != null) {
                hittedObject = collider.gameObject;
                return true;
            }

            return false;
        }

        private RaycastHit2D GetHit (Vector2 origin, Vector2 dir, float mult) {

            #if UNITY_EDITOR
            Debug.DrawRay (
                start: origin + offset,
                dir: dir.normalized * magnitude * mult,
                color: color
            );
            #endif

            return Physics2D.Raycast (
                origin: origin + offset,
                direction: dir,
                distance: magnitude * mult
            );
        }
    }

    #endregion
}