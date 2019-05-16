using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    
    public GameObject followCube;

    //void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Collision detected");
    //    Debug - draw all contact points and normals
    //    foreach (ContactPoint contact in collision.contacts)
    //    {
    //        Debug.Log("RayPoint");
    //        audioSource.Play();
    //        GameObject capsule = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Capsule), contact.point, transform.rotation);

    //        Debug.Log("Capsule instantiated");
    //        cube.GetComponent<BoxCollider>().enabled = false;
    //        followCube.GetComponent<BoxCollider>().enabled = false;
    //        followCube.SetActive(false);
    //        Debug.Log("Collider and cube  disabled");
    //        Debug.DrawRay(contact.point, contact.normal, Color.green);
    //    }

    //    Play a sound if the colliding objects had a big impact.
    //    if (collision.relativeVelocity)
    //        audioSource.Play();
    //}
    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Triggered!");

    }
}
