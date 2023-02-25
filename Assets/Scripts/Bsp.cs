using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bsp : MonoBehaviour
{
    struct Line
    {
        public Vector3 start;
        public Vector3 dir;
    }

    [SerializeField] private Room[] rooms;
    [SerializeField] private float lineMargin;
    [SerializeField] private float renderDis;
    [SerializeField] private float LineFreqColition;

    private Line[] lines = new Line[25];

    private void OnDrawGizmos()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            //el player esta en el cuarto?
            if (rooms[i].PosInsideRoom(transform.position))
            {
                rooms[i].isVisible = true;
            }
            else
            {
                rooms[i].isVisible = false;
            }
        }

        InitLines();

        DrawLines();

        for (int i = 0; i < lines.Length; i++)
        {
            CheckLine(lines[i]);
        }
    }

    private void CheckLine(Line line)
    {
        Vector3[] points = new Vector3[(int)(renderDis / LineFreqColition)];

        Gizmos.color = Color.black;

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = line.start + line.dir * LineFreqColition * i;
            Gizmos.DrawSphere(points[i], 0.1f);

            for (int j = 0; j < rooms.Length; j++)
            {
                //colision de linea esta en el cuarto
                if (rooms[j].PosInsideRoom(points[i]))
                {
                    rooms[j].isVisible = true;
                }

            }
        }
    }

    public void InitLines()
    {
        //setear inicio y fin de la linea
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].start = transform.position;
            lines[i].dir = (transform.forward * renderDis + transform.right * lineMargin * (-lines.Length / 2) + i * transform.right * lineMargin).normalized;

        }
    }

    public void DrawLines()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            Vector3[] points = new Vector3[(int)(renderDis / LineFreqColition)];

            Gizmos.color = Color.red;

            for (int j = 0; j < points.Length; j++)
            {
                points[j] = lines[i].start + lines[i].dir * LineFreqColition * j;
                Gizmos.DrawSphere(points[j], 0.1f);
            }
        }
    }

}
