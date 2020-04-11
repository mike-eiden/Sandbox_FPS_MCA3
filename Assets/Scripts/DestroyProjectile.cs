using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyProjectile : MonoBehaviour
{
    private LevelManager lm; 
    
    // Start is called before the first frame update
    void Start()
    {
        lm = GameObject.FindWithTag("LM").GetComponent<LevelManager>(); 
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.StartsWith("Enemy"))
        {
            lm.incrementScore();
            if (other.gameObject.GetComponent<MeshRenderer>())
            {
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                MeshRenderer[] meshes = other.gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mesh in meshes)
                {
                    mesh.enabled = false; 
                }
            }

            if (other.gameObject.GetComponent<Collider>())
            {
                other.gameObject.GetComponent<Collider>().enabled = false; 
            }
            if (other.gameObject.GetComponent<ParticleSystem>())
            {
                other.gameObject.GetComponent<ParticleSystem>().Play();
            }

            if (other.gameObject.GetComponent<EnemyAI>())
            {
                other.gameObject.GetComponent<EnemyAI>().Die();
            }
            else
            {
                Destroy(other.gameObject, 1f);
                Destroy(gameObject);
            }
            
            
        }
    }
}
