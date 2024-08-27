using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{

    [SerializeField]
    private Animator playerAnimator;
    
    [SerializeField] 
    private SpriteRenderer spriteRenderer;

    [SerializeField] 
    private GameObject anchorUnFlipped;
    
    [SerializeField] 
    private GameObject anchorFlipped;

    [SerializeField] 
    private Player.PlayerController playerController;

    private const float ZeroIntensityThreshold = 0.05f;

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
        FlipSprites(playerController.IsFlipped);

        var movementIntensity = Mathf.Abs(_currentMovement);
        var isHighestSpeed = Mathf.Abs(movementIntensity) >= 1;
        var isIdle = movementIntensity < ZeroIntensityThreshold;
        
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

    private void FlipSprites(bool flipX)
    {
        spriteRenderer.flipX = flipX;
        anchorUnFlipped.SetActive(!flipX);
        anchorFlipped.SetActive(flipX);
    }
}