using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private GameObject homeAnchor;
    [SerializeField] private GameObject homeAnchorFliped;

    [HideInInspector]
    public bool hasItem = false;
    public static GameObject pickedObject;
    private SpriteRenderer[] sprites;
   



    private void OnEnable()
    {
        PlayerController.flipAction += Flipping;
    }
    private void OnDisable()
    {
        PlayerController.flipAction -= Flipping;

    }

    private void Start()
    {
        sprites = new SpriteRenderer[2];
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.transform.tag == "Pickable")
        {
            if (!hasItem)
            {
                pickedObject = collision.transform.gameObject;
                sprites = collision.GetComponentsInChildren<SpriteRenderer>();
                hasItem = true;
                SetHome();
            }          
        }
    }

    private void SetHome()
    {
       Collider2D coll = pickedObject.GetComponent<Collider2D>();
        coll.enabled = false;
        pickedObject.transform.parent = homeAnchor.transform;
        pickedObject.transform.position = homeAnchor.transform.position;
        pickedObject.transform.rotation = homeAnchor.transform.rotation;
    }

    private void Flipping(bool flip)
    {
        if (!hasItem)
            return;
        if (flip)
        {
            pickedObject.transform.parent = homeAnchorFliped.transform;
            pickedObject.transform.position = homeAnchorFliped.transform.position;
            pickedObject.transform.rotation = homeAnchorFliped.transform.rotation;
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].flipX = true;
            }
        }
        else
        {
            pickedObject.transform.parent = homeAnchor.transform;
            pickedObject.transform.position = homeAnchor.transform.position;
            pickedObject.transform.rotation = homeAnchor.transform.rotation;
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].flipX = false;
            }
        }
    }
}
