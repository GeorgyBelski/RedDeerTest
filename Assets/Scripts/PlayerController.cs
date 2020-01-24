using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState {Move, Rotate};
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationTime = 0.3f;
    public PlayerState state = PlayerState.Move;

    [Header("References")]
    public new Rigidbody rigidbody;

    Vector3 inputAxisVector;
    Vector3 moveDirection;
    Vector3 newRotationAngle, previousRotationAngle;
    Vector3 deltaPosition;
    Quaternion startRotation;
    float rotationFactor;
    float verticalInput;
    Vector3 snapPosition;


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
    }
    
    void Move()
    {
     //   float h = Input.GetAxisRaw("Horizontal");
        if(state != PlayerState.Move) 
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
            rotationFactor += Time.deltaTime/rotationTime;
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
      //  transform.position = snapPosition;
    }
    bool CheckGround() 
    {
        Vector3 front = transform.position + transform.rotation * Vector3.forward * 0.3f * Mathf.Sign(verticalInput);
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

    public void Restart()
    {
        transform.position = deltaPosition;
        transform.rotation = startRotation;
        previousRotationAngle = transform.eulerAngles;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + inputAxisVector);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * inputAxisVector);
    }
}
