using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Platform: MonoBehaviour {

    [SerializeField]
    private string tagGround;

    void Awake () {
        tag = tagGround;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer> ();
        BoxCollider2D coll = this.GetComponent<BoxCollider2D> ();
        coll.size = spriteRenderer.size;
    }
}
