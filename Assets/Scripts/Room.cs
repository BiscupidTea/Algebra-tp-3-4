using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject floor;

    public bool isVisible = false;

    private Plane[] walls = new Plane[4];


    private void OnDrawGizmos()
    {
        //tomo la mesh filter del piso
        MeshFilter mesh = floor.GetComponent<MeshFilter>();
        Vector3[] corners = new Vector3[4];

        corners[0] = mesh.sharedMesh.vertices[0];
        corners[1] = mesh.sharedMesh.vertices[0];
        corners[2] = mesh.sharedMesh.vertices[0];
        corners[3] = mesh.sharedMesh.vertices[0];

        // vertice izq abajo
        for (int i = 0; i < mesh.sharedMesh.vertices.Length; i++)
        {
            if (mesh.sharedMesh.vertices[i].x > corners[0].x || mesh.sharedMesh.vertices[i].y > corners[0].y)
                continue;
            else
                corners[0] = mesh.sharedMesh.vertices[i];
        }

        // vertice der abajo
        for (int i = 0; i < mesh.sharedMesh.vertices.Length; i++)
        {
            if (mesh.sharedMesh.vertices[i].x > corners[1].x || mesh.sharedMesh.vertices[i].y < corners[1].y)
                corners[1] = mesh.sharedMesh.vertices[i];
        }

        // vertice izq arriba
        for (int i = 0; i < mesh.sharedMesh.vertices.Length; i++)
        {
            if (mesh.sharedMesh.vertices[i].x < corners[2].x || mesh.sharedMesh.vertices[i].y > corners[2].y)
                corners[2] = mesh.sharedMesh.vertices[i];
        }

        // vertice der arriba
        for (int i = 0; i < mesh.sharedMesh.vertices.Length; i++)
        {
            if (mesh.sharedMesh.vertices[i].x > corners[3].x || mesh.sharedMesh.vertices[i].y > corners[3].y)
                corners[3] = mesh.sharedMesh.vertices[i];
        }

        corners[0] = floor.transform.TransformPoint(corners[0]);
        corners[1] = floor.transform.TransformPoint(corners[1]);
        corners[2] = floor.transform.TransformPoint(corners[2]);
        corners[3] = floor.transform.TransformPoint(corners[3]);


        walls[0] = new Plane(corners[0], corners[1], corners[0] + Vector3.down);
        walls[1] = new Plane(corners[1], corners[3], corners[1] + Vector3.down);
        walls[2] = new Plane(corners[2], corners[0], corners[2] + Vector3.down);
        walls[3] = new Plane(corners[3], corners[2], corners[3] + Vector3.down);

        Debug.Log(corners[0]);
        Debug.Log(corners[1]);
        Debug.Log(corners[2]);
        Debug.Log(corners[3]);


        DrawPlane(corners[0], walls[0].normal);
        DrawPlane(corners[1], walls[1].normal);
        DrawPlane(corners[2], walls[2].normal);
        DrawPlane(corners[3], walls[3].normal);


        if (isVisible)
        {
            Show();
        }
        else
        {
            Hide();
        }

    }

    public void DrawPlane(Vector3 position, Vector3 normal)
    {
        Vector3 v3;
        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; ;
        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;
        Debug.DrawLine(corner0, corner2, Color.green);
        Debug.DrawLine(corner1, corner3, Color.green);
        Debug.DrawLine(corner0, corner1, Color.green);
        Debug.DrawLine(corner1, corner2, Color.green);
        Debug.DrawLine(corner2, corner3, Color.green);
        Debug.DrawLine(corner3, corner0, Color.green);
        Debug.DrawRay(position, normal, Color.red);
    }


    public bool PosInsideRoom(Vector3 pos)
    {
        for (int i = 0; i < walls.Length; i++)
        {
            //chequeo si la posicion esta del lado correcto de la pared
            if (!walls[i].GetSide(pos))
            {
                return false;
            }
        }

        return true;
    }

    public void Hide()
    {
        //desactivo la mesh

        MeshRenderer[] mesh = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < mesh.Length; i++)
        {
            mesh[i].enabled = false;
        }
    }

    public void Show()
    {
        //activo la mesh

        MeshRenderer[] mesh = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < mesh.Length; i++)
        {
            mesh[i].enabled = true;
        }
    }
}
