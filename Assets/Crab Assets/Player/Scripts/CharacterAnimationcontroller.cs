using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CharacterAnimationcontroller : MonoBehaviour
{

    private Animator playerAnimator;

     private void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    public void PlayAnimation( int animIndex)
    {
        switch (animIndex)
        {
            // idle
            case 0:
                break;
                //walk
            case 1:
                break;
                // glide
            case 2:
                break;
            //jump

            case 3:
                break;
            //death
            case 4:
                break;
                //default
            default:
                break;
        }
    }
}
