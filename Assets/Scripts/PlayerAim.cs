using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;
    private Vector2 aimInput;
    private GamepadCursor gamepadCursor;
    private RaycastHit lastAimHit;

    [Header("Aim Visual")]
    [SerializeField] private LineRenderer aimLaser;
    
    [Header("Aim Control")]
    [SerializeField] private Transform aimTarget;
    [SerializeField] private bool aimPrecise = true;
    [SerializeField] private bool aimAssist = true;

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
        if ( Input.GetKeyDown(KeyCode.P) ) 
            aimPrecise = !aimPrecise;

        if ( Input.GetKeyDown(KeyCode.L) )
            aimAssist = !aimAssist;

        UpdateAimPosition();   
        UpdateCameraPosition();
        UpdateAimLaser();
    }
    
    private void UpdateAimLaser() {
        float aimLaserLength = 0.5f;
        float gunDistance = 4f;

        Transform gunPoint = player.weapon.GunPoint();
        Vector3 aimPoint = player.weapon.BulletDirection();
        Vector3 endPoint = gunPoint.position + aimPoint * gunDistance;

        if ( Physics.Raycast(gunPoint.position, aimPoint, out var hit, gunDistance) ) {
            endPoint = hit.point;
            aimLaserLength = 0f;
        }

        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + aimPoint * aimLaserLength);
    }
    
    private void UpdateAimPosition() {
        Transform target = Target();

        if ( target != null && aimAssist ) {
            aimTarget.position = target.position;
            return;
        }

        aimTarget.position = GetAimHitInfo().point;
        if ( !CanAimPrecisely() )
            aimTarget.position = new Vector3(aimTarget.position.x, transform.position.y + 1, aimTarget.position.z); 
    }

    public Transform Aim() => aimTarget;
    public Transform Target() {
        Transform target = null;

        if ( GetAimHitInfo().transform.GetComponent<Target>() != null )
            target = GetAimHitInfo().transform;

        return target;
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

    private void AssignInputEvents() {
        controls = player.controls;

        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
    }

    public bool CanAimPrecisely() => aimPrecise;

    #region Camrea Region
    private void UpdateCameraPosition() {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
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

    #endregion
}
