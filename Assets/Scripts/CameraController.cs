using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public Transform player;
    public float dotProductX, dotProductZ;
    void Start()
    {

    }

    void LateUpdate()
    {
        AutoRotate();
    }

    void AutoRotate()
    {
        Vector3 focusToPlayer = player.position - transform.position;
        dotProductX = Vector3.Dot(focusToPlayer, transform.rotation * Vector3.right);
        dotProductZ = Vector3.Dot(focusToPlayer, transform.rotation * Vector3.forward);

        if (dotProductX >= 0 && dotProductZ >= 0)
        { return; }
        else if (dotProductX < 0 && dotProductZ > 0)
        {
            float ratio = -dotProductX / dotProductZ;
            transform.Rotate(Vector3.down, rotationSpeed * Time.deltaTime * Mathf.Min(0.5f, ratio));
        }
        else if (dotProductX < 0 && dotProductZ < 0)
        { transform.Rotate(Vector3.down, rotationSpeed * Time.deltaTime * 0.7f); }
        else //(dotProductX > 0 && dotProductZ < 0)
        {
            float ratio = dotProductX / -dotProductZ;
            transform.Rotate(Vector3.down, rotationSpeed * Time.deltaTime * Mathf.Max(1, ratio));
        }

        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * Vector3.forward * dotProductZ);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * Vector3.right * dotProductX);
    }
}
