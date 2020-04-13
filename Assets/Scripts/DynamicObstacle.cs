using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    public float speed = 3;
    public float distance = 5;

    private Vector3 startPos;
    
    void Start()
    {
        startPos = transform.position; 
    }

    void FixedUpdate()
    {
        Vector3 newPos = transform.position;
        newPos.x = startPos.x + (Mathf.Sin(Time.time * speed) * distance);

        transform.position = newPos; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            other.transform.parent = transform; 
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            other.transform.parent = transform; 
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            other.transform.parent = null; 
        }
    }

}
