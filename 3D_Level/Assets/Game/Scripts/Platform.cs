using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{

    public GameObject Plataform;
    public Transform[] movePoints;
    public float speed;
    public float RotationSpeed;

    private int pointSelection;
    private Transform curretPoint;

    // Start is called before the first frame update
    void Start()
    {
        curretPoint = movePoints[pointSelection];

    }

    // Update is called once per frame
    void Update()
    {
        Plataform.transform.position = Vector3.MoveTowards(Plataform.transform.position, curretPoint.position, Time.deltaTime * speed);

        if(Plataform.transform.position == curretPoint.position)
        {
            pointSelection++;
            if(pointSelection == movePoints.Length)
            {
                pointSelection = 0;
                Plataform.transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);

            }
            curretPoint = movePoints[pointSelection];
        }



    }
}
