using UnityEngine;
using UnityEngine.UIElements;

public class Player_Movement : MonoBehaviour
{
    private Player player;

    private CharacterController characterController;
    private InputSystem_Actions.CharacterActions controls;
    private Animator animator;

    [Header("Movement info")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSpeed;
    private float speed;
    private float verticalVelocity;

    public Vector2 moveInput { get; private set; }
    private Vector3 movementDirection;

    private bool isRunning;

    private AudioSource walkSFX;
    private AudioSource runSFX;
    private bool canPlayFootsteps;

    private void Start()
    {
        player = GetComponent<Player>();

        walkSFX = player.sound.walkSFX;
        runSFX = player.sound.runSFX;
        Invoke(nameof(AllowfootstepsSFX), 1f);

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;


        AssignInputEvents();
    }


    private void Update()
    {
        if (player.health.isDead)
            return;

        ApplyMovement();
        ApplyRotation();
        AnimatorControllers();
    }

    private void AnimatorControllers()
    {
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);

        bool playRunAnimation = isRunning & movementDirection.magnitude > 0;
        animator.SetBool("isRunning", playRunAnimation);
    }
    private void ApplyRotation()
    {
        Vector3 lookingDirection = player.aim.GetMouseHitInfo().point - transform.position;
        lookingDirection.y = 0f;
        lookingDirection.Normalize();

        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);

    }
    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();

        if (movementDirection.magnitude > 0)
        {
            PlayFootstepsSFX();

            characterController.Move(movementDirection * Time.deltaTime * speed);
        }
    }

    private void PlayFootstepsSFX()
    {
        if (canPlayFootsteps == false)
            return;

        if (isRunning)
        {
            if (runSFX.isPlaying == false)
                runSFX.Play();
        }
        else
        {
            if (walkSFX.isPlaying == false)
                walkSFX.Play();
        }
    }
    private void StopFootstepsSFX()
    {
        walkSFX.Stop();
        runSFX.Stop();
    }
    private void AllowfootstepsSFX() => canPlayFootsteps = true;

    private void ApplyGravity()
    {
        if (characterController.isGrounded == false)
        {
            verticalVelocity -= 9.81f * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
            verticalVelocity = -.5f;
    }
    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Movement.canceled += context =>
        {
            StopFootstepsSFX();
            moveInput = Vector2.zero;
        };

        controls.Run.performed += context =>
        {
            speed = runSpeed;
            isRunning = true;
        };


        controls.Run.canceled += context =>
        {
            speed = walkSpeed;
            isRunning = false;
        };
    }
}