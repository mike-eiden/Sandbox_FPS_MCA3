using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float moveSpeed;
    private Transform target; 
    
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Player").transform; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        float step = moveSpeed * Time.deltaTime;
        
        transform.LookAt(target);
        float y = transform.eulerAngles.y; 
        transform.eulerAngles = new Vector3(0,y,0);

        // Keep current y value no matter what
        Vector3 desiredPos = new Vector3(target.position.x, transform.position.y, target.position.z);

        transform.position = Vector3.MoveTowards(transform.position, desiredPos, step);
    }
}
