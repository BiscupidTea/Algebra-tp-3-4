using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Frustrum : MonoBehaviour
{
    Camera cam;

    //max values
    private const int maxFrustrumPlanes = 6;
    private const int maxObjects = 4;
    private const int aabbPoints = 8;

    //planos
    Plane[] planes = new Plane[maxFrustrumPlanes];
    [SerializeField] GameObject[] TestObjests = new GameObject[maxObjects];

    public struct Object
    {
        public GameObject gameObject;
        public Vector3[] aabb;
    }

    [SerializeField] Vector3 nTLeft;
    [SerializeField] Vector3 nTRight;
    [SerializeField] Vector3 nBLeft;
    [SerializeField] Vector3 nBRight;

    [SerializeField] Vector3 fTLeft;
    [SerializeField] Vector3 fTRight;
    [SerializeField] Vector3 fBLeft;
    [SerializeField] Vector3 fBRight;

    [SerializeField] Vector3[] aabb = new Vector3[aabbPoints];

    [SerializeField] Object[] Objects = new Object[maxObjects];

    private void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        for (int i = 0; i < maxObjects; i++)
        {
            Objects[i].gameObject = TestObjests[i];
            Objects[i].aabb = new Vector3[aabbPoints];
        }

        float halfCameraHeight = cam.farClipPlane * MathF.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float CameraWidth = (halfCameraHeight * 2) * cam.aspect;
        Vector3 frontMultFar = cam.farClipPlane * cam.transform.forward;

        //near position
        Vector3 nearPos = cam.transform.position;
        nearPos += cam.transform.forward * cam.nearClipPlane;
        planes[0] = new Plane(cam.transform.forward, nearPos);

        //far position
        Vector3 farPos = cam.transform.position;
        farPos += (cam.transform.forward) * cam.farClipPlane;
        planes[1] = new Plane(cam.transform.forward * -1, farPos);

        planes[2] = new Plane(cam.transform.position, -Vector3.Cross(cam.transform.up, frontMultFar + cam.transform.right * CameraWidth));

        planes[3] = new Plane(cam.transform.position, -Vector3.Cross(frontMultFar - cam.transform.right * CameraWidth, cam.transform.up));

        planes[4] = new Plane(cam.transform.position, -Vector3.Cross(cam.transform.right, frontMultFar - cam.transform.up * halfCameraHeight));

        planes[5] = new Plane(cam.transform.position, -Vector3.Cross(frontMultFar + cam.transform.up * halfCameraHeight, cam.transform.right));
    }

    private void Update()
    {
        
    }

    public void SetNearPoints(Vector3 nearPos)
    {

        float halfCameraHeightNear = Mathf.Tan((cam.fieldOfView / 2) * Mathf.Deg2Rad) * cam.nearClipPlane;
        float CameraHalfWidthNear = (cam.aspect * halfCameraHeightNear);

        Vector3 nearPlaneDistance = cam.transform.position + (cam.transform.forward * cam.nearClipPlane);

        nTLeft = nearPlaneDistance + (cam.transform.up * halfCameraHeightNear) - (cam.transform.right * CameraHalfWidthNear);

        nTRight = nearPlaneDistance + (cam.transform.up * halfCameraHeightNear) + (cam.transform.right * CameraHalfWidthNear);

        nBLeft = nearPlaneDistance - (cam.transform.up * halfCameraHeightNear) - (cam.transform.right * CameraHalfWidthNear);

        nBRight = nearPlaneDistance - (cam.transform.up * halfCameraHeightNear) + (cam.transform.right * CameraHalfWidthNear);
    }

    public void SetFarPoints(Vector3 farPos)
    {
        float halfCameraHeightfar = Mathf.Tan((cam.fieldOfView / 2) * Mathf.Deg2Rad) * cam.farClipPlane;
        float CameraHalfWidthFar = (cam.aspect * halfCameraHeightfar);

        Vector3 farPlaneDistance = cam.transform.position + (cam.transform.forward * cam.farClipPlane);


        fTLeft = farPlaneDistance + (cam.transform.up * halfCameraHeightfar) - (cam.transform.right * CameraHalfWidthFar);

        fTRight = farPlaneDistance + (cam.transform.up * halfCameraHeightfar) + (cam.transform.right * CameraHalfWidthFar);

        fBLeft = farPlaneDistance - (cam.transform.up * halfCameraHeightfar) - (cam.transform.right * CameraHalfWidthFar);

        fBRight = farPlaneDistance - (cam.transform.up * halfCameraHeightfar) + (cam.transform.right * CameraHalfWidthFar);
    }
}