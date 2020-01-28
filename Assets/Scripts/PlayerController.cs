using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public enum PlayerState {Move, Rotate, Freeze, TakeOff};
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationTime = 0.3f;
    public PlayerState state = PlayerState.Move;
    public bool isTransferEnd = false;
    public float takingOffTime = 0.7f;
    float timerTakingOff;

    [Header("References")]
    public new Rigidbody rigidbody;
    public Transform playerGeometry;
    public MovingPlatform movingPlatform = null;
    public ParentConstraint parentConstraint;

    Vector3 inputAxisVector;
    Vector3 moveDirection;
    Vector3 newRotationAngle, previousRotationAngle;
    public Vector3 newPosition, previousPosition;
    Vector3 deltaPosition;

    Quaternion startRotation;
    float rotationFactor;
    float verticalInput;
    Vector3 snapPosition;
    ConstraintSource constraintSource = new ConstraintSource();
    int transferEndframeOffset = 1;
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
        TransferEnd();
    }
    void Update()
    {
        TakingOff();
    }
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
            DefineSnap();
        }
        else if (state == PlayerState.Rotate)
        {
            rotationFactor += Time.deltaTime / rotationTime;
            transform.rotation = Quaternion.Slerp(Quaternion.Euler(previousRotationAngle), Quaternion.Euler(newRotationAngle), rotationFactor);
            transform.position = Vector3.Lerp(transform.position, snapPosition, rotationFactor);
            if (rotationFactor >= 1)
            {
                transform.eulerAngles = newRotationAngle;
                previousRotationAngle = newRotationAngle;
                transform.position = snapPosition;
                state = PlayerState.Move;
            }
        }
    }

    void DefineSnap()
    {
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
    public void EnabletMovingPlatformConstraint(MovingPlatform movingPlatform)
    {

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

    void TransferEnd()
    {
        if (isTransferEnd && transferEndframeOffset > 0)
        { transferEndframeOffset--; }
        else if (isTransferEnd)
        {
            ReconcileTransforms();
            Debug.Log(movingPlatform);
            if (movingPlatform)
            {
                EnabletMovingPlatformConstraint(movingPlatform);
                movingPlatform.playerController = this;
                movingPlatform.SpringCharging();
            }
            isTransferEnd = false;
            transferEndframeOffset = 1;
        }
    }

    public void SpringLaunch(Vector3 lookDirection)
    {
        parentConstraint.enabled = false;
        movingPlatform = null;
        this.lookDirection = lookDirection;
        state = PlayerState.TakeOff;
        //previousRotationAngle = transform.eulerAngles;
        //rotationFactor = 0;
        previousPosition = transform.position;
        newPosition = previousPosition + Vector3.up * 1.7f + transform.rotation*Vector3.forward;
        timerTakingOff = 0;
    }
    void TakingOff()
    {
        if (state == PlayerState.TakeOff) 
        {
            //rotationFactor += Time.deltaTime / rotationTime;
            //transform.rotation = Quaternion.Slerp(Quaternion.Euler(previousRotationAngle), Quaternion.Euler(newRotationAngle), rotationFactor);
            
            timerTakingOff += Time.deltaTime;
            float takeOffFactor = timerTakingOff / takingOffTime * Mathf.PI * 0.5f;
            float right=0, up, forward=0;

            if (previousPosition.x != newPosition.x) 
            { right = Mathf.Lerp(previousPosition.x, newPosition.x,1-Mathf.Cos(takeOffFactor)); }

            if (previousPosition.z != newPosition.z)
            { forward = Mathf.Lerp(previousPosition.z, newPosition.z,1-Mathf.Cos(takeOffFactor)); }

            up = Mathf.Lerp(previousPosition.y, newPosition.y, Mathf.Sin(takeOffFactor));

            transform.position = Vector3.right * right + Vector3.up * up + Vector3.forward * forward;
            if (timerTakingOff >= takingOffTime) 
            {
                //timerTakingOff = 0;
               transform.position = newPosition;
            }
        }
    }

    public void Restart()
    {
        transform.position = deltaPosition;
        DefineSnap();
        transform.rotation = startRotation;
        previousRotationAngle = transform.eulerAngles;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + inputAxisVector);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * inputAxisVector);

        Gizmos.DrawSphere(previousPosition,.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(newPosition, .5f);
    }
}
