using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 velocity { get; set; }
    public Vector3 position { get { return transform.position; } set { transform.position = value; } }
    private void Update()
    {

    }


}
