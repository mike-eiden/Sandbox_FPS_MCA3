using UnityEngine;

public class ShootProjectiles : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletsParent; 
    public int bulletSpeed;
    public Animator anim; 

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("Shoot");
            
            GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward, bulletPrefab.transform.rotation) as GameObject;

            if (!bullet.GetComponent<Rigidbody>())
            {
                bullet.AddComponent<Rigidbody>(); 
            }

            bullet.transform.parent = bulletsParent; 
            bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
        }
    }
}