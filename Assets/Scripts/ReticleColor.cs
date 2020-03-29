using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleColor : MonoBehaviour
{

    public Image reticle;
    public Color initialColor;

    private void Start()
    {
        reticle.color = initialColor; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Color currentColor = reticle.color; 
        RaycastHit hit; 
        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.tag.StartsWith("Enemy"))
            {
                reticle.color = Color.Lerp(currentColor, Color.red, 0.1f); 
            }
            else if (hit.collider.tag.Equals("Interactable"))
            {
                reticle.color = Color.Lerp(currentColor, Color.green, 0.1f);
            }
            else
            {
                reticle.color = Color.Lerp(currentColor, initialColor, 0.1f);
            }
        }
    }
}
