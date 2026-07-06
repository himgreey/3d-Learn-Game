using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{   
    public Transform lookat;
    public Vector3 offestPos;
    public float YMove;

    public float MoveSpeed;
    public float RotateSpeed;

    private Vector3 targetPos;
    private Quaternion targetRot;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (lookat == null)
        {
            return;
        }

        targetPos = lookat.position + lookat.forward * offestPos.z;
        targetPos += Vector3.up * offestPos.y;
        targetPos += lookat.right * offestPos.x;

        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, MoveSpeed * Time.deltaTime);

        targetRot = Quaternion.LookRotation(lookat.position + Vector3.up * YMove - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRot, RotateSpeed * Time.deltaTime);
    }

    public void SetLookAt(Transform target)
    {
        lookat = target;
    }
}
