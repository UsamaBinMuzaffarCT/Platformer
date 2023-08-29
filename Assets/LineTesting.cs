using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTesting : MonoBehaviour
{
    public LineRendererController controller;
    public Transform[] points;

    void Start()
    {
        controller.SetLine(points);
    }
}
