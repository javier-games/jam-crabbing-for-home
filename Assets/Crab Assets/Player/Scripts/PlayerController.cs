﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player controller.
/// </summary>
[RequireComponent (typeof (Rigidbody2D))]
public class PlayerController: MonoBehaviour {


    #region Class Members

    private new Rigidbody2D rigidbody;      //  Rigidbody of this Game Object.
    [SerializeField]
    private State state = State.FOLLING;    //  Sate of the current player.

    [Header ("Forward Parameters")]
    [SerializeField]
    protected float forwardSpeed = 2.15f;   //  Speed to move forward.
    [SerializeField]
    protected float forwardSmooth = 0.5f;   //  Smooth at stop moving forward.
    [SerializeField, Range (0f, 90f)]
    protected float slopeAngle = 45f;
    [SerializeField]
    protected float slopeDistance = 1f;
    [SerializeField]
    protected Vector2 slopeOffset = Vector2.zero;

    [Header ("Climbing Parameters")]
    [SerializeField]
    protected float climbingMovement;

    private float forwardSense;             //  Sense of movement.
    private float horizontalAxis;           //  Amount to move horizontaly.
    private float forwardVelocity;          //  Velocity for smooth damp.
    private RaycastHit2D hit;               //  Ray info.

    [Header ("Folling Parameters")]
    [SerializeField, Range (0f, 2f)]
    protected float fallingMovement = 1f;   //  Forward movement at folling.

    [Header ("Rays")]
    [SerializeField]
    protected float magnitude;
    [SerializeField]
    protected float angleGrad = 45;

    /*
    [Header ("Collision Parameters")]
    [SerializeField]
    protected FieldView2D leftField;        //  Left Field View.
    [SerializeField]
    protected FieldView2D rightField;       //  Right Field View.*/

    private List<Collider2D> leftColliders; //  List of facing elements.
    private List<Collider2D> rightColliders;//  List of facing elements.


    bool facingLeft;
    bool facingRight;

    #endregion


    #region MonoBehaviour Overrides

    //  Called on Awake.
    private void Awake () {
        //  Getting references.
        rigidbody = GetComponent<Rigidbody2D> ();

        //  Initializing lists.
        leftColliders = new List<Collider2D> ();
        rightColliders = new List<Collider2D> ();
    }

    //  Called each frame.
    private void Update () {
        UpdateHorizontalSense ();
        UpdateState ();
    }

    //  Called each frame fixed. Used it for physics.
    private void FixedUpdate () {

        //  Getting shortcuts.
        bool folling = state == State.FOLLING;
        bool climbing = state == State.CLIMBING;

        //  Getting horizontal velocity.
        float x = horizontalAxis * forwardSpeed * Time.deltaTime;
        x *= folling ? fallingMovement : 1;
        x = x > 0 && facingLeft ? 0 : x;
        x = x < 0 && facingRight ? 0 : x;

        //  Getting vertical velocity.
        float y = rigidbody.velocity.y + (climbing ? forwardSpeed * Time.deltaTime * climbingMovement : 0);

        //  Applying movement.
        rigidbody.velocity = new Vector2 (x, y);
    }

    //  Called on collision enter.
    /*
    private void OnCollisionEnter2D (Collision2D collision) {

        //  Adding collider to the lists.
        ContactPoint2D[] contacts = collision.contacts;

        Debug.Log (contacts.Length);
        foreach (ContactPoint2D contact in contacts) {

            Vector2 point = transform.InverseTransformPoint (contact.point);

            Debug.Log (point);

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
    }*/

    /*
    //  Called on collision exit.
    private void OnCollisionExit2D (Collision2D collision) {
        //  Removing the collision from the list.
        if (rightColliders.Contains (collision.collider))
            rightColliders.Remove (collision.collider);

        if (leftColliders.Contains (collision.collider))
            leftColliders.Remove (collision.collider);
    }*/

    //  Called to display gizmos.

#if UNITY_EDITOR
    private void OnDrawGizmos () {

        //  Drawing Field of views.
        /*leftField.Draw (transform);
        rightField.Draw (transform);*/

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
    private void UpdateHorizontalSense () {

        //  Read inputs.
        if (Input.GetKeyDown (KeyCode.RightArrow))
            forwardSense = 1;
        if (Input.GetKeyDown (KeyCode.LeftArrow))
            forwardSense = -1;
        if (Input.GetKeyUp (KeyCode.RightArrow) && forwardSense > 0)
            forwardSense = 0;
        if (Input.GetKeyUp (KeyCode.LeftArrow) && forwardSense < 0)
            forwardSense = 0;

        //  Applying Smooth.
        horizontalAxis = Mathf.SmoothDamp (
            current: horizontalAxis,
            target: forwardSense,
            currentVelocity: ref forwardVelocity,
            smoothTime: forwardSmooth
        );
    }

    //  Updates the state.
    private void UpdateState () {

        state = State.GROUNDED;
        //  Defining if the player is falling.
        if (rigidbody.velocity.y < 0 && state != State.CLIMBING)
            state = State.FOLLING;

        facingLeft = FacingLeft();
        facingRight = FacingRight ();
        if (state == State.GROUNDED && ((facingLeft && CanClimb (-1)) || (facingRight && CanClimb (1))))
            state = State.CLIMBING;
    }

    private bool CanClimb (float x) {
        float sense = x < 0 ? 1 : -1;

        float slopeRad = slopeAngle * Mathf.Deg2Rad;
        hit = Physics2D.Raycast (
            origin: transform.TransformPoint (slopeOffset),
            direction: new Vector2 (sense * Mathf.Cos (slopeRad), Mathf.Sin (slopeRad)),
            distance: slopeDistance
        );

        #if UNITY_EDITOR
        Debug.DrawRay (
            start: transform.TransformPoint (slopeOffset),
            dir: new Vector2 (sense * Mathf.Cos (slopeRad), Mathf.Sin (slopeRad)),
            color: Color.magenta
        );
        #endif

        return hit.collider == null;
    }

    private bool FacingLeft () {

        bool up = false;
        bool down = false;
        float angle = angleGrad * Mathf.Deg2Rad;

        hit = Physics2D.Raycast (
            origin: transform.position,
            direction: transform.InverseTransformPoint ((Vector2)transform.position + Vector2.right * Mathf.Cos (angle*0.5f) + Vector2.up * Mathf.Sin (angle * 0.5f)),
            distance: magnitude
        );
        #if UNITY_EDITOR
        Debug.DrawRay (
            start: transform.position,
            dir: transform.InverseTransformPoint((Vector2)transform.position + Vector2.right * Mathf.Cos (angle * 0.5f) + Vector2.up * Mathf.Sin (angle * 0.5f)).normalized * magnitude,
            color: Color.cyan
        );
        #endif
        up = hit.collider != null;


        hit = Physics2D.Raycast (
            origin: transform.position,
            direction: transform.InverseTransformPoint ((Vector2)transform.position + Vector2.right * Mathf.Cos (angle * 0.5f) - Vector2.up * Mathf.Sin (angle * 0.5f)),
            distance: magnitude
        );
        #if UNITY_EDITOR
        Debug.DrawRay (
            start: transform.position,
            dir: transform.InverseTransformPoint ((Vector2)transform.position + Vector2.right * Mathf.Cos (angle * 0.5f) - Vector2.up * Mathf.Sin (angle * 0.5f)).normalized * magnitude,
            color: Color.cyan
        );
        #endif
        down = hit.collider != null;

        return up || down;
    }

    private bool FacingRight () {

        bool up = false;
        bool down = false;
        float angle = angleGrad * Mathf.Deg2Rad;

        hit = Physics2D.Raycast (
            origin: transform.position,
            direction: transform.InverseTransformPoint ((Vector2)transform.position - Vector2.right * Mathf.Cos (angle * 0.5f) + Vector2.up * Mathf.Sin (angle * 0.5f)),
            distance: magnitude
        );
        #if UNITY_EDITOR
        Debug.DrawRay (
            start: transform.position,
            dir: transform.InverseTransformPoint ((Vector2)transform.position - Vector2.right * Mathf.Cos (angle * 0.5f) + Vector2.up * Mathf.Sin (angle * 0.5f)).normalized * magnitude,
            color: Color.cyan
        );
        #endif
        up = hit.collider != null;


        hit = Physics2D.Raycast (
            origin: transform.position,
            direction: transform.InverseTransformPoint ((Vector2)transform.position - Vector2.right * Mathf.Cos (angle * 0.5f) - Vector2.up * Mathf.Sin (angle * 0.5f)),
            distance: magnitude
        );
        #if UNITY_EDITOR
        Debug.DrawRay (
            start: transform.position,
            dir: transform.InverseTransformPoint ((Vector2)transform.position - Vector2.right * Mathf.Cos (angle * 0.5f) - Vector2.up * Mathf.Sin (angle * 0.5f)).normalized * magnitude,
            color: Color.cyan
        );
        #endif
        down = hit.collider != null;

        return up || down;
    }


    //  State of the player.
    private enum State {
        GROUNDED,
        FOLLING,
        CLIMBING
    }

    #endregion
}