using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Frustrum : MonoBehaviour
{
    Camera cam;

    //max values
    private const int maxFrustrumPlanes = 6;
    private const int maxObjects = 15;
    private const int aabbPoints = 8;

    //planos
    Plane[] planes = new Plane[maxFrustrumPlanes];
    [SerializeField] GameObject[] TestObjests = new GameObject[maxObjects];
    [SerializeField] public bool showPoints;

    public struct Object
    {
        public GameObject gameObject;
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public Vector3[] aabb;
        public Vector3 v3Extents;
        public Vector3 scale;
    }

    [SerializeField] Vector3 nTLeft;
    [SerializeField] Vector3 nTRight;
    [SerializeField] Vector3 nBLeft;
    [SerializeField] Vector3 nBRight;

    [SerializeField] Vector3 fTLeft;
    [SerializeField] Vector3 fTRight;
    [SerializeField] Vector3 fBLeft;
    [SerializeField] Vector3 fBRight;

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
            Objects[i].meshRenderer = TestObjests[i].GetComponent<MeshRenderer>();
            Objects[i].meshFilter = TestObjests[i].GetComponent<MeshFilter>();
            Objects[i].aabb = new Vector3[aabbPoints];
            Objects[i].v3Extents = Objects[i].meshRenderer.bounds.extents;
            Objects[i].scale = Objects[i].meshRenderer.bounds.size;
        }

        for (int i = 0; i < maxFrustrumPlanes; i++)
        {
            planes[i] = new Plane();
        }
    }

    private void Update()
    {
        Vector3 frontMultFar = cam.farClipPlane * cam.transform.forward;

        Vector3 nearPos = cam.transform.position;
        nearPos += cam.transform.forward * cam.nearClipPlane;
        planes[0].SetNormalAndPosition(cam.transform.forward, nearPos);


        Vector3 farPos = cam.transform.position;
        farPos += (cam.transform.forward) * cam.farClipPlane;
        planes[1].SetNormalAndPosition(cam.transform.forward * -1, farPos);

        SetNearPoints(nearPos);
        SetFarPoints(farPos);

        planes[2].Set3Points(cam.transform.position, fBLeft, fTLeft);//left
        planes[3].Set3Points(cam.transform.position, fTRight, fBRight);//right
        planes[4].Set3Points(cam.transform.position, fTLeft, fTRight);//top
        planes[5].Set3Points(cam.transform.position, fBRight, fBLeft);//bottom

        for (int i = 2; i < maxFrustrumPlanes; i++)
        {
            planes[i].Flip();
        }
        for (int i = 0; i < maxObjects; i++)
        {
            CheckObjetColition(Objects[i]);
        }

        for (int i = 0; i < maxObjects; i++)
        {
            SetAABB(ref Objects[i]);
        }

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

    public void SetAABB(ref Object currentObject)
    {
        if (currentObject.scale != currentObject.gameObject.transform.localScale)
        {
            Quaternion rotation = currentObject.gameObject.transform.rotation;
            currentObject.gameObject.transform.rotation = Quaternion.identity;
            currentObject.v3Extents = currentObject.meshRenderer.bounds.extents;
            currentObject.scale = currentObject.gameObject.transform.localScale;
            currentObject.gameObject.transform.rotation = rotation;
        }

        Vector3 center = currentObject.meshRenderer.bounds.center;
        Vector3 size = currentObject.v3Extents;

        currentObject.aabb[0] = new Vector3(center.x - size.x, center.y + size.y, center.z - size.z);  // Front top left corner
        currentObject.aabb[1] = new Vector3(center.x + size.x, center.y + size.y, center.z - size.z);  // Front top right corner
        currentObject.aabb[2] = new Vector3(center.x - size.x, center.y - size.y, center.z - size.z);  // Front bottom left corner
        currentObject.aabb[3] = new Vector3(center.x + size.x, center.y - size.y, center.z - size.z);  // Front bottom right corner
        currentObject.aabb[4] = new Vector3(center.x - size.x, center.y + size.y, center.z + size.z);  // Back top left corner
        currentObject.aabb[5] = new Vector3(center.x + size.x, center.y + size.y, center.z + size.z);  // Back top right corner
        currentObject.aabb[6] = new Vector3(center.x - size.x, center.y - size.y, center.z + size.z);  // Back bottom left corner
        currentObject.aabb[7] = new Vector3(center.x + size.x, center.y - size.y, center.z + size.z);  // Back bottom right corner

        for (int i = 0; i < aabbPoints; i++)
        {
            currentObject.aabb[i] = transform.TransformPoint(currentObject.aabb[i]);
        }

        for (int i = 0; i < aabbPoints; i++)
        {
            currentObject.aabb[i] = RotatePointAroundPivot(currentObject.aabb[i], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        }
    }
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot;          
        dir = Quaternion.Euler(angles) * dir; 
        point = dir + pivot;                  
        return point;
    }
    public void CheckObjetColition(Object currentObject)
    {
        bool isInsideF = false;

        for (int i = 0; i < aabbPoints; i++)
        {
            int counter = maxFrustrumPlanes;

            for (int j = 0; j < maxFrustrumPlanes; j++)
            {
                if (planes[j].GetSide(currentObject.aabb[i]))
                {
                    counter--;
                }
            }

            if (counter == 0)
            {
                isInsideF = true;
                break;
            }
        }

        if (isInsideF)
        {
            for (int i = 0; i < currentObject.meshFilter.mesh.vertices.Length; i++)
            {
                int counter = maxFrustrumPlanes;

                for (int j = 0; j < maxFrustrumPlanes; j++)
                {
                    if (planes[j].GetSide(currentObject.gameObject.transform.TransformPoint(currentObject.meshFilter.mesh.vertices[i])))
                    {
                        counter--;
                    }
                }

                if (counter == 0)
                {
                    currentObject.gameObject.SetActive(true);
                    break;
                }
            }
        }
        else
        {
            if (currentObject.gameObject.activeSelf)
            {
                currentObject.gameObject.SetActive(false);
            }
        }
    }
    public void OnDrawGizmos()
    {
        Debug.DrawLine(nTLeft, fTLeft, Color.red);
        Debug.DrawLine(nTRight, fTRight, Color.red);
        Debug.DrawLine(nBLeft, fBLeft, Color.red);
        Debug.DrawLine(nBRight, fBRight, Color.red);

        Debug.DrawLine(nTLeft, nTRight, Color.blue);
        Debug.DrawLine(nTRight, nBRight, Color.blue);
        Debug.DrawLine(nBRight, nBLeft, Color.blue);
        Debug.DrawLine(nBLeft, nTLeft, Color.blue);

        Debug.DrawLine(fTLeft, fTRight, Color.blue);
        Debug.DrawLine(fTRight, fBRight, Color.blue);
        Debug.DrawLine(fBRight, fBLeft, Color.blue);
        Debug.DrawLine(fBLeft, fTLeft, Color.blue);

        for (int i = 0; i < maxObjects; i++)
        {
            DrawAABB(ref Objects[i]);
        }
    }

    public void DrawAABB(ref Object currentObject)
    {

        Gizmos.color = Color.blue;


        for (int i = 0; i < aabbPoints; i++)
        {
            Gizmos.DrawSphere(currentObject.aabb[i], 0.05f);
        }


        // Draw the AABB Box 
        Gizmos.DrawLine(currentObject.aabb[0], currentObject.aabb[1]);
        Gizmos.DrawLine(currentObject.aabb[1], currentObject.aabb[3]);
        Gizmos.DrawLine(currentObject.aabb[3], currentObject.aabb[2]);
        Gizmos.DrawLine(currentObject.aabb[2], currentObject.aabb[0]);
        Gizmos.DrawLine(currentObject.aabb[0], currentObject.aabb[4]);
        Gizmos.DrawLine(currentObject.aabb[4], currentObject.aabb[5]);
        Gizmos.DrawLine(currentObject.aabb[5], currentObject.aabb[7]);
        Gizmos.DrawLine(currentObject.aabb[7], currentObject.aabb[6]);
        Gizmos.DrawLine(currentObject.aabb[6], currentObject.aabb[4]);
        Gizmos.DrawLine(currentObject.aabb[7], currentObject.aabb[3]);
        Gizmos.DrawLine(currentObject.aabb[6], currentObject.aabb[2]);
        Gizmos.DrawLine(currentObject.aabb[5], currentObject.aabb[1]);

        Gizmos.color = Color.green;
    }
}