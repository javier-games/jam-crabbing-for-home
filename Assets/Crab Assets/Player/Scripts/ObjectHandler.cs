using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandler: MonoBehaviour {
    [SerializeField] private GameObject homeAnchor;
    [SerializeField] private GameObject homeAnchorFliped;

    [HideInInspector]
    public bool hasItem = false;
    public static GameObject pickedObject;
    private SpriteRenderer[] sprites;

    [SerializeField]
    float launchForce;




    private void OnEnable () {
        PlayerController.flipAction += Flipping;
        PlayerController.growUp += GrowingUp;
    }
    private void OnDisable () {
        PlayerController.flipAction -= Flipping;
        PlayerController.growUp -= GrowingUp;

    }

    private void Start () {
        sprites = new SpriteRenderer[2];
    }
    public void Pick (Transform collision) {

        if (collision.tag == "Pickable") {
            if (!hasItem) {
                pickedObject = collision.gameObject;
                sprites = collision.GetComponentsInChildren<SpriteRenderer> ();
                hasItem = true;
                SetHome ();
            }
        }
    }

    public void Drop () {
        if (!hasItem)
            return;
        pickedObject.transform.parent = transform.parent;
        StartCoroutine (Launch (0.1f, pickedObject));
        hasItem = false;
        GetComponent<PlayerController> ().gameMode.BeginTimerNekedness ();
    }

    public void GrowingUp (float scale) {
        if (!hasItem)
            return;
        pickedObject.transform.parent = transform.parent;
        StartCoroutine (DestroyPickable (4f, pickedObject));
        hasItem = false;
        GetComponent<PlayerController> ().gameMode.BeginTimerNekedness ();
    }

    IEnumerator Launch (float time, GameObject gameObj) {
        Rigidbody2D myRigid = GetComponent<Rigidbody2D> ();
        Rigidbody2D rigid = pickedObject.GetComponent<Rigidbody2D> ();
        Collider2D coll = pickedObject.GetComponent<Collider2D> ();
        coll.enabled = false;

        rigid.AddForce ((Vector2.up + 0.3f * myRigid.velocity).normalized * launchForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds (time);
        coll.enabled = true;
        pickedObject = null;
    }

    IEnumerator DestroyPickable (float time, GameObject gameObj) {
        yield return new WaitForSeconds (time);
        Destroy (gameObj);
    }

    private void SetHome () {
        Collider2D coll = pickedObject.GetComponent<Collider2D> ();
        coll.enabled = false;
        pickedObject.transform.parent = homeAnchor.transform;
        pickedObject.transform.position = homeAnchor.transform.position;
        pickedObject.transform.rotation = homeAnchor.transform.rotation;
    }

    private void Flipping (bool flip) {
        if (!hasItem)
            return;
        if (flip) {
            pickedObject.transform.parent = homeAnchorFliped.transform;
            pickedObject.transform.position = homeAnchorFliped.transform.position;
            pickedObject.transform.rotation = homeAnchorFliped.transform.rotation;
            for (int i = 0; i < sprites.Length; i++) {
                sprites[i].flipX = true;
            }
        }
        else {
            pickedObject.transform.parent = homeAnchor.transform;
            pickedObject.transform.position = homeAnchor.transform.position;
            pickedObject.transform.rotation = homeAnchor.transform.rotation;
            for (int i = 0; i < sprites.Length; i++) {
                sprites[i].flipX = false;
            }
        }
    }
}
