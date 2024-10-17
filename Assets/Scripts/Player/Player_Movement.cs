using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Unity.Collections;
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
    public LayerMask groundMask;
    private float speed;
    private float verticalVelocity;

    public Vector2 moveInput { get; private set; }
    private Vector3 movementDirection;
    private List<Vector3> savedPositions = new List<Vector3>(20);//Stack<Vector3>(10);

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
        Vector3 lookingDirection = player.aim.GetAimHitInfo().point - transform.position;
        lookingDirection.y = 0f;
        lookingDirection.Normalize();

        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);

    }

    private bool IsGrounded() {
        int num = 10;
        Vector3 point = transform.position;
        float radius = 0.5f; // The distance around the player to check for ground
        float radians;
        float x,z;
        Vector3 spawnDir;
        Vector3 spawnPos;
        RaycastHit hit;

        /* Create a circle of points around the player, checking if they are all above the ground */
        for (int i = 0; i < num; i++){
            
            /* Distance around the circle */  
            radians = 2 * MathF.PI / num * i;
            
            /* Get the vector direction */ 
            z = MathF.Sin(radians);
            x = MathF.Cos(radians); 
            
            spawnDir = new Vector3 (x, 0, z);
            spawnPos = point + spawnDir * radius; // Radius is just the distance away from the point
            
            if (!Physics.Raycast(spawnPos, Vector3.down, out hit, 3)) {
                return false;
            }
        }
        return true;
    }
    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        
        if ( IsGrounded() == false ) {
            savedPositions.Reverse();
            foreach( Vector3 pos in savedPositions ) {
                transform.position = pos;
                if ( IsGrounded() == true ) {
                    Debug.Log("Moved to safe position: [" + transform.position + "]");
                    break;
                }
            } 

            movementDirection = (movementDirection.normalized *-1);
            characterController.Move(movementDirection.normalized * Time.deltaTime * (speed*2));
            movementDirection = (movementDirection.normalized *-1);
            savedPositions.Clear();

            return;
        } 
        
        ApplyGravity();

        if (movementDirection.magnitude > 0)
        {
            PlayFootstepsSFX();

            characterController.Move(movementDirection * Time.deltaTime * speed);
            savedPositions.Add(transform.position);
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