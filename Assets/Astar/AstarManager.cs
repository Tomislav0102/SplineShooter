using FirstCollection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AstarManager : MonoBehaviour
{
    public Transform pointA, pointB, pointC;
    public Vector2 vAB, vAC;
    public float dotFloat;

    private void Update()
    {
        vAB = (pointB.position - pointA.position).normalized;
        vAC = (pointC.position - pointA.position).normalized;
        dotFloat = Vector2.Dot(vAB, vAC);
    }
}
