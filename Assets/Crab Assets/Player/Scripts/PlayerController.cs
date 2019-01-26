using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player controller.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour{


    #region Class Members

    private new Rigidbody2D rigidbody;      //  Rigidbody of this Game Object.
    private State state = State.FOLLING;    //  Sate of the current player.

    [Header("Forward Parameters")]
    [SerializeField]
    protected float forwardSpeed = 2.15f;   //  Speed to move forward.
    [SerializeField]
    protected float forwardSmooth = 0.5f;   //  Smooth at stop moving forward.
    [SerializeField, Range(0f, 90f)]
    protected float slopeAngle = 45f;
    [SerializeField]
    protected float slopeDistance = 1f;
    [SerializeField]
    protected Vector2 slopeOffset = Vector2.zero;

    private float forwardSense;             //  Sense of movement.
    private float horizontalAxis;           //  Amount to move horizontaly.
    private float forwardVelocity;          //  Velocity for smooth damp.
    private RaycastHit2D hit;               //  Ray info.

    [Header("Folling Parameters")]
    [SerializeField, Range (0f, 2f)]
    protected float fallingMovement = 1f;   //  Forward movement at folling.

    [Header("Collision Parameters")]
    [SerializeField]
    protected FieldView2D leftField;        //  Left Field View.
    [SerializeField]
    protected FieldView2D rightField;       //  Right Field View.

    private List<Collider2D> leftColliders; //  List of facing elements.
    private List<Collider2D> rightColliders;//  List of facing elements.

    #endregion


    #region MonoBehaviour Overrides

    //  Called on Awake.
    private void Awake(){
        //  Getting references.
        rigidbody = GetComponent<Rigidbody2D>();

        //  Initializing lists.
        leftColliders = new List<Collider2D>();
        rightColliders = new List<Collider2D>();
    }

    //  Called each frame.
    private void Update(){
        UpdateHorizontalSense();
        UpdateState();
    }

    //  Called each frame fixed. Used it for physics.
    private void FixedUpdate(){

        //  Getting shortcuts.
        bool folling = state == State.FOLLING;

        //  Getting horizontal velocity.
        float x = horizontalAxis * forwardSpeed * Time.deltaTime;
        x *= folling ? fallingMovement : 1;
        x = x > 0 && leftColliders.Count > 0 ? 0 : x;
        x = x < 0 && rightColliders.Count > 0 ? 0 : x;




        //  Getting vertical velocity.
        float y = rigidbody.velocity.y;

        //  Applying movement.
        rigidbody.velocity = new Vector2(x, y);
    }

    //  Called on collision enter.
    private void OnCollisionEnter2D(Collision2D collision){

        //  Adding collider to the lists.
        ContactPoint2D[] contacts = collision.contacts;
        foreach(ContactPoint2D contact in contacts){

            Vector2 point = transform.InverseTransformPoint (contact.point);

            if (
                leftField.Contains (point) &&
                !leftColliders.Contains (collision.collider)
            )
                leftColliders.Add (collision.collider);

            if (
                rightField.Contains (point) &&
                !rightColliders.Contains (collision.collider)
            )
                rightColliders.Add (collision.collider);
        }
    }

    //  Called on collision exit.
    private void OnCollisionExit2D(Collision2D collision){
        //  Removing the collision from the list.
        if(rightColliders.Contains(collision.collider))
            rightColliders.Remove(collision.collider);

        if(leftColliders.Contains(collision.collider))
            leftColliders.Remove(collision.collider);
    }

    //  Called to display gizmos.

    #if UNITY_EDITOR
    private void OnDrawGizmos () {

        //  Drawing Field of views.
        leftField.Draw (transform);
        rightField.Draw (transform);

        //  Drawing Slope.
        float slopeRad = slopeAngle * Mathf.Deg2Rad;
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine (
            from: transform.TransformPoint (slopeOffset),
            to: transform.TransformPoint (slopeOffset + new Vector2 (Mathf.Cos (slopeRad), Mathf.Sin (slopeRad)))
        );
    }
    #endif

    #endregion


    #region Class Implementation

    //  Updates the horizontal variable.
    private void UpdateHorizontalSense(){

        //  Read inputs.
        if (Input.GetKeyDown(KeyCode.RightArrow))
            forwardSense = 1;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            forwardSense = -1;
        if (Input.GetKeyUp(KeyCode.RightArrow) && forwardSense > 0)
            forwardSense = 0;
        if (Input.GetKeyUp(KeyCode.LeftArrow) && forwardSense < 0)
            forwardSense = 0;

        //  Applying Smooth.
        horizontalAxis = Mathf.SmoothDamp(
            current: horizontalAxis,
            target: forwardSense,
            currentVelocity: ref forwardVelocity,
            smoothTime: forwardSmooth
        );
    }

    //  Updates the state.
    private void UpdateState(){

        state = State.GROUNDED;
        //  Defining if the player is falling.
        if (rigidbody.velocity.y < 0)
            state = State.FOLLING;

        if(state == State.GROUNDED) {
            if (leftColliders.Count > 0) {
            }
            if (rightColliders.Count > 0) {
            }
        }
    }

    private bool CanClimb (float x) {
        float sense = x / Mathf.Abs (x);

        float slopeRad = slopeAngle * Mathf.Deg2Rad;
        hit = Physics2D.Raycast (
            origin: transform.TransformPoint(slopeOffset),
            direction: new Vector2 (Mathf.Cos (slopeRad), Mathf.Sin (slopeRad)),
            distance: slopeDistance
        );
        if(hit.collider!= null)
        Debug.Log (hit.collider.name);

        return hit.collider == null;
    }


    //  State of the player.
    private enum State{
        GROUNDED,
        FOLLING,
        CLIMBING
    }

#endregion
}