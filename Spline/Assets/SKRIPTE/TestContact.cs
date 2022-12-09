using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestContact : MonoBehaviour
{
    public enum Smjer
    {
        Up, Down, Left, Right
    }
    public Smjer smjer;
    public float dotX, dotY;
    public Transform krug;
    Vector2 razlika;

    private void Update()
    {
        razlika = (krug.position - transform.position).normalized;
        dotX = Vector2.Dot(transform.right, razlika);
        dotY = Vector2.Dot(transform.up, razlika);

        if(Mathf.Abs(dotX) > Mathf.Abs(dotY))
        {
            if (dotX > 0) smjer = Smjer.Right; 
            else smjer=Smjer.Left;
        }
        else
        {
            if (dotY > 0) smjer = Smjer.Up;
            else smjer = Smjer.Down;

        }
    }
}
