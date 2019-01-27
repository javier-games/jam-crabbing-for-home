using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundCollider : MonoBehaviour
{
    [SerializeField] private int soundIndex;
    [SerializeField] private MusicManager musicaManager;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            musicaManager.ChangeMusic(soundIndex);
            Collider2D col = GetComponent<Collider2D>();
            col.enabled = false;
        }
    }
}
