using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;
    private Vector2 aimInput;
    private GamepadCursor gamepadCursor;
    private RaycastHit lastAimHit;

    [Header("Aim Control")]
    [SerializeField] private Transform aimTarget;

    [Header("Camera Control")]
    [SerializeField] private float cameraSensitivity = 4f;
    [SerializeField] private float minCameraDistance = 1f;
    [SerializeField] private float maxCameraDistance = 3f;
    [SerializeField] private Transform cameraTarget;

    [SerializeField] private LayerMask aimLayerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gamepadCursor =  GetComponent<GamepadCursor>();
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    // Update is called once per frame
    void Update()
    {
        aimTarget.position = GetAimHitInfo().point;
        aimTarget.position = new Vector3(aimTarget.position.x, transform.position.y + 1, aimTarget.position.z); 
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
    }

    public RaycastHit GetAimHitInfo() {

        if ( Gamepad.current != null ) {
            aimInput = gamepadCursor.GetWarpPosition();
        } else {
            aimInput = Mouse.current.position.ReadValue();
        }

        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, aimLayerMask)) {
            lastAimHit = hit;
            return hit;
        }
        
        return lastAimHit;
    }

    private Vector3 DesiredCameraPosition() {
        Vector3 desiredCameraPosition = GetAimHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float actualMaxCameraDistance = player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;

        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);
        desiredCameraPosition = transform.position + aimDirection * clampedDistance;

        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }

    private void AssignInputEvents() {
        controls = player.controls;

        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
    }
}
