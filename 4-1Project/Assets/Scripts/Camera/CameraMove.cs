using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject target;
    public float Speed;
    public float Distance;
    public float Height;

    Vector3 CameraPos;
   
    // Update is called once per frame
    void LateUpdate()
    {
        CameraPos = new Vector3(transform.position.x, target.transform.position.y + Height, Distance);
        transform.position = Vector3.Lerp(CameraPos, target.transform.position, Speed * Time.smoothDeltaTime);
    }
}
