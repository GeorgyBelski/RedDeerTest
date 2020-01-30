using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public enum PlayerState {Move, Rotate, Freeze, TakeOff, GoDown };
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationTime = 0.3f;
    [SerializeField] PlayerState state = PlayerState.Move;
    public bool isTransferEnd = false;
    public float takingOffTime = 0.7f;
    float timerTakingOff;

    [Header("References")]
    public PlayerActions playerActions;
    public new Rigidbody rigidbody;
    public Transform playerGeometry;
    public MovingPlatform movingPlatform = null;
    public ParentConstraint parentConstraint;
    [HideInInspector]
    public Vector3 pipeDestination;
    public float goDownTime = 0.6f;
    float timerGoDown;

    Vector3 inputAxisVector;
    Vector3 moveDirection;
    public Vector3 newRotationAngle, previousRotationAngle;
    public Vector3 newPosition, previousPosition;
    Vector3 deltaPosition;

    Quaternion startRotation;
    float rotationFactor;
    bool isSnapping;
    float snappingFactor ,snappingTime;
    PlayerState snapFinalstate;
    float verticalInput;
    Vector3 snapPosition;
    ConstraintSource constraintSource = new ConstraintSource();
    //int transferEndframeOffset = 0;
    Vector3 lookDirection;
    

    void Start()
    {
        previousRotationAngle = transform.eulerAngles;
        deltaPosition = transform.position;
        startRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        Move();
        Rotate();
        Snaping();
      //  TransferEnd();
    }
    void Update()
    {
        TakingOff();
        GoingDown();
    }

    public void SetState(PlayerState newState) { state = newState; }
    void Move()
    {
        if (state != PlayerState.Move)
        {
            rigidbody.velocity = Vector3.zero;
            return;
        }

        verticalInput = Input.GetAxisRaw("Vertical");
        if (CheckGround())
        {
            inputAxisVector = new Vector3(0, 0, verticalInput);
            moveDirection = transform.rotation * inputAxisVector;
            rigidbody.velocity = moveDirection.normalized * moveSpeed;
          //  transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

    void Rotate()
    {
        if (state != PlayerState.Move && state != PlayerState.Rotate)
        { return; }

        float h = Input.GetAxisRaw("Horizontal");
        if (h != 0 && state != PlayerState.Rotate)
        {
            state = PlayerState.Rotate;
            newRotationAngle = transform.eulerAngles + new Vector3(0, Mathf.Sign(h) * 90, 0);
            rotationFactor = 0;
            DefineSnap(rotationTime, PlayerState.Rotate);
        }
        else if (state == PlayerState.Rotate)
        {
            rotationFactor += Time.deltaTime / rotationTime;
            transform.rotation = Quaternion.Slerp(Quaternion.Euler(previousRotationAngle), Quaternion.Euler(newRotationAngle), rotationFactor);
        //    transform.position = Vector3.Lerp(transform.position, snapPosition, rotationFactor);
            if (rotationFactor >= 1)
            {
                transform.eulerAngles = newRotationAngle;
                previousRotationAngle = newRotationAngle;
             //   transform.position = snapPosition;
                state = PlayerState.Move;
            }
        }
    }
    public void Snaping() 
    {
        if (!isSnapping) { return; }

        snappingFactor += Time.deltaTime / rotationTime;
        transform.position = Vector3.Lerp(transform.position, snapPosition, snappingFactor);
        if (snappingFactor >= 1)
        {
            transform.position = snapPosition;
            state = snapFinalstate;
            isSnapping = false;
        }
    } 

    public void DefineSnap( float snappingTime, PlayerState finalState = PlayerState.Move)
    {
        isSnapping = true;
        this.snappingTime = snappingTime;
        snapFinalstate = finalState;
        snappingFactor = 0;
        float snapX = Mathf.Round(transform.position.x - deltaPosition.x) + deltaPosition.x;
        float snapZ = Mathf.Round(transform.position.z - deltaPosition.z) + deltaPosition.z;
        float snapY = transform.position.y;
        snapPosition = new Vector3(snapX, snapY, snapZ);
    }
    bool CheckGround()
    {
        Vector3 front = transform.position + transform.rotation * Vector3.forward * 0.3f * Mathf.Sign(verticalInput) + Vector3.up * 0.3f;
        if (Physics.Raycast(front, Vector3.down, out RaycastHit hit, 1f, Globals.groundMask))
        {
            Debug.DrawLine(front, front + Vector3.down, Color.yellow);
            return true;
        }
        else
        {
            Debug.DrawLine(front, front + Vector3.down, Color.red);
            return false;
        }
    }

    public void ReconcileTransforms()
    {
        this.transform.position = playerGeometry.position;
        playerGeometry.position = Vector3.zero;
    }

    public void SetMovingPlatform(GameObject platform)
    {
        MovingPlatform.objectMovingPlatformMap.TryGetValue(platform.gameObject, out movingPlatform);
    }
    public void TakeOffToSpringboard() // Call from animation "JumpOnPlatform"
    {
        /*  previousPosition = transform.position;
          newPosition = previousPosition + movingPlatform.heightOfPlatform * Vector3.up + transform.rotation * Vector3.forward;
          timerTakingOff = 0;
          takingOffTime = 0.12f;
          state = PlayerState.TakeOff;*/

        SetTakeOff(movingPlatform.heightOfPlatform, 0.12f);
    }
    public void SetTakeOff(float height, float time) 
    {
        previousPosition = transform.position;
        newPosition = previousPosition + height * Vector3.up + transform.rotation * Vector3.forward * Mathf.Sign(verticalInput);
        timerTakingOff = 0;
        takingOffTime = time;
        state = PlayerState.TakeOff;
    }
    public void EnabletMovingPlatformConstraint(MovingPlatform movingPlatform)
    {
        movingPlatform.playerController = this;
        movingPlatform.SpringCharging();
        constraintSource.sourceTransform = movingPlatform.platform;
        constraintSource.weight = 1f;
        parentConstraint.translationAtRest = this.transform.position;
        parentConstraint.rotationAtRest = this.transform.eulerAngles;
        parentConstraint.enabled = true;
        parentConstraint.SetSource(0, constraintSource);
    }

    public void DisableMovingPlatformConstraint()///////////
    {
        parentConstraint.enabled = false;
    }

    public void LendedOnPlatform()
    {
        if (movingPlatform)
        { EnabletMovingPlatformConstraint(movingPlatform);}
    }

    public void SpringLaunch(Vector3 lookDirection)
    {
        parentConstraint.enabled = false;
        transform.position = movingPlatform.transform.position + Vector3.up * 0.5f;
        movingPlatform = null;
        this.lookDirection = lookDirection;
        SetTakeOff(1.7f, 0.5f);

    }
    void TakingOff()
    {
        if (state == PlayerState.TakeOff) 
        {
            timerTakingOff += Time.deltaTime;
            float takeOffFactor = timerTakingOff / takingOffTime * Mathf.PI * 0.5f;
            float right, up, forward;

            if (previousPosition.x != newPosition.x)
            { right = Mathf.Lerp(previousPosition.x, newPosition.x, 1 - Mathf.Cos(takeOffFactor)); }
            else { right = newPosition.x; }
            if (previousPosition.z != newPosition.z)
            { forward = Mathf.Lerp(previousPosition.z, newPosition.z, 1 - Mathf.Cos(takeOffFactor)); }
            else { forward = newPosition.z; }
            up = Mathf.Lerp(previousPosition.y, newPosition.y, Mathf.Sin(takeOffFactor));

            transform.position = Vector3.right * right + Vector3.up * up + Vector3.forward * forward;
            if (timerTakingOff >= takingOffTime) 
            {
                //timerTakingOff = 0;
               transform.position = newPosition;
                //state = PlayerState.GoDown;
            }
        }
    }
    public void StartGoDown()
    {
        state = PlayerState.GoDown;
        timerGoDown = goDownTime;
        previousPosition = transform.position;
        newPosition = pipeDestination;
    }
    void GoingDown() 
    {
        if (state == PlayerState.GoDown)
        {
            if (timerGoDown >= 0)
            {
                float goDownFactor = timerGoDown / goDownTime * Mathf.PI * 0.5f;
                transform.position = Vector3.Lerp(newPosition, previousPosition, Mathf.Sin(goDownFactor));
                timerGoDown -= Time.deltaTime;
            }
            else 
            {
                transform.position = newPosition;
                state = PlayerState.Move;
                playerActions.EndSqueeze();
            }
        }
    }

    public void Restart()
    {
        transform.position = deltaPosition;
        DefineSnap(rotationTime);
        transform.rotation = startRotation;
        previousRotationAngle = transform.eulerAngles;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + inputAxisVector);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * inputAxisVector);
        /**/
        Gizmos.DrawSphere(previousPosition,.2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(newPosition, .2f);
        
    }
}
