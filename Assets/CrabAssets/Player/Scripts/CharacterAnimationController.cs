using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{

    [SerializeField]
    private Animator playerAnimator;
    
    [SerializeField] 
    private SpriteRenderer spriteRenderer;

    [SerializeField] 
    private Player.PlayerController playerController;

    private float _currentMovement;
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Glide = Animator.StringToHash("Glide");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Reset = Animator.StringToHash("Reset");

    private void OnEnable()
    {
        playerController.Jumped += Jumped;
        playerController.HorizontalMove += HorizontalMove;
    }

    private void Jumped()
    {
        playerAnimator.SetTrigger(Jump);
    }

    private void HorizontalMove(float movement)
    {
        _currentMovement = movement;
    }

    private void Update()
    {
        spriteRenderer.flipX = _currentMovement < 0;
        var isHighestSpeed = Mathf.Abs(_currentMovement) >= 1;
        var isIdle = _currentMovement == 0;
        
        if (playerController.IsFalling)
        {
            playerAnimator.SetTrigger(Glide);
        }

        else if (playerController.IsGrounded)
        {
            if (isIdle)
            {
                playerAnimator.Play(Idle);
            }
            else if (isHighestSpeed)
            {
                playerAnimator.SetTrigger(Glide);
            }
            else
            {
                playerAnimator.SetTrigger(Walk);
            }
        }
    }

    private void Kill()
    {
        playerAnimator.SetTrigger(Death);
    }

    private void ResetAnimation()
    {
        playerAnimator.SetTrigger(Reset);
    }

    private void OnDisable()
    {
        playerController.Jumped -= Jumped;
        playerController.HorizontalMove -= HorizontalMove;
    }
}