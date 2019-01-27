using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CharacterAnimationcontroller : MonoBehaviour
{

    private Animator playerAnimator;
    [SerializeField] private Animator anchorAnimator;

     private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        PlayAnimation(99);
    }

/*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            PlayAnimation(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayAnimation(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayAnimation(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayAnimation(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayAnimation(4);
        }
    }
    
    */

    public void PlayAnimation( int animIndex)
    {
        switch (animIndex)
        {
            // idle
            case 0:                
                playerAnimator.Play("Idle");
                anchorAnimator.Play("AnchorIdle");
                break;

                //walk
            case 1:
                playerAnimator.SetTrigger("Walk");                
                break;

                // glide
            case 2:
                playerAnimator.SetTrigger("Glide");
                break;

            //jump
            case 3:
                playerAnimator.SetTrigger("Jump");
                anchorAnimator.SetTrigger("Jump");
                break;

            //death
            case 4:
                playerAnimator.SetTrigger("Death");
                break;

                //default
            default:
                playerAnimator.SetTrigger("Reset");
                break;
        }
    }
}
