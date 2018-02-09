using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    float TurnSpeed = 200f;
    [SerializeField]
    float ZoomSpeed = 10;
    [SerializeField]
    GameObject CameraParent;
    [SerializeField]
    float StartCameraPosX = 0;
    [SerializeField]
    float StartCameraPosY = 10;
    [SerializeField]
    float StartCameraPosZ = -2;
    [SerializeField]
    float ZoomoutLimit = 10;
    [SerializeField]
    float ZoomPoint = 0;
    Vector3 StartRo;
    float DefaultZoom;
    Camera cam;
    Vector3 asd;
    void Start()
    {
        asd = new Vector3(StartCameraPosX, StartCameraPosY, StartCameraPosZ);
        cam = GetComponent<Camera>();
        //Player = GameObject.FindGameObjectWithTag("Player");
        transform.position = CameraParent.transform.position + asd;
        DefaultZoom = cam.fieldOfView;
        StartRo = transform.eulerAngles;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.RotateAround(CameraParent.transform.position, Vector3.up, 1 * Time.deltaTime * TurnSpeed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.RotateAround(CameraParent.transform.position, Vector3.up, -1 * Time.deltaTime * TurnSpeed);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (ZoomPoint >= 0)
            {
                CameraZoom(-1);
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {

            if (ZoomoutLimit >= ZoomPoint)
            {
                CameraZoom(1);
            }
        }
        if (Input.GetKeyDown(KeyCode.Home))
        {
            cam.fieldOfView = DefaultZoom;
            ZoomPoint = 0;
        }
        if (Input.GetKey(KeyCode.End))
        {
            Debug.Log(transform.position);
            Debug.Log(CameraParent.transform.position);
            Vector3 set = CameraParent.transform.position;
            transform.position = set + asd;
            transform.eulerAngles = CameraParent.transform.eulerAngles + StartRo;
        }

        if (Input.GetKey(KeyCode.Delete))
        {
            transform.position += transform.forward * ZoomPoint;
            cam.fieldOfView = DefaultZoom;
            ZoomPoint = 0;
        }
    }
    void CameraZoom(int x)
    {
        float set = x * Time.deltaTime * ZoomSpeed;
        ZoomPoint += set;
        cam.fieldOfView += set;
    }
}
