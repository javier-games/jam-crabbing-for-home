using UnityEngine;

public class SoundCollider : MonoBehaviour
{
    [SerializeField] private int soundIndex;
    [SerializeField] private MusicManager musicManager;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.attachedRigidbody.CompareTag("Player")) return;
        musicManager.ChangeMusic(soundIndex);
        var col = GetComponent<Collider2D>();
        col.enabled = false;
    }
}
