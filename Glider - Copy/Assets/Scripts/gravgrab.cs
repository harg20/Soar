using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class gravgrab : MonoBehaviour
{
    public GameObject[] Planets;
    int i = 0;
    
    public float planetmass = 2;
   public float sunmass = 10000000000;
    Vector3 startvelocity = new Vector3(0.5f, 1, .5f);
    // Start is called before the first frame update
    void Start()
    {
       
        
        foreach (GameObject Planet in Planets)
        {
           Planet.GetComponent<Rigidbody>().AddForce(startvelocity, ForceMode.VelocityChange);
            
        }
        
    }
  
   
   
    
    public Vector3 calculateForce(GameObject Planet)
    {
        
            float distance = Vector3.Distance(transform.position, Planet.transform.position);

            float G = 6.67f * Mathf.Pow(10, -11);

            float force = G * sunmass * planetmass / (distance * distance);

            Vector3 fwd = (force * (transform.position - Planet.transform.position));
            return (fwd);

        
     

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (GameObject Planet in Planets)
        {
            Planet.GetComponent<Rigidbody>().AddForce(calculateForce(Planet), ForceMode.VelocityChange);

        }
        
    }
}
