using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermove : MonoBehaviour
{
    float rott;
    float xSpeed;
    float zSpeed;
    public float speed;
    public GameObject Glider;
    public ParticleSystem Crystalcounter;
    AirResistance.AirResistance resist;
    public float rotspeed;
    bool canjump = true;
    bool notrolling = true;
    int jumps = 2;
    Rigidbody rb;
    float reset = 0;
    bool grounded = false;
    bool gliding = false;
    Vector3 vel;
    public float maxangularvel = 3;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        resist = GetComponent<AirResistance.AirResistance>();
        resist.enabled = false;
        rb.maxAngularVelocity = maxangularvel;
        
    }
   public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "floor")
        {
            grounded = true;
        }
        if (other.gameObject.layer == 8)
        {
            notrolling = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "floor")
        {
            grounded = false ;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !gliding)
        {
            rb.AddForce(Vector3.up * 100, ForceMode.Impulse);

        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
       
        zSpeed = Input.GetAxis("Vertical") * (speed);
        Debug.Log(zSpeed);
        if (Input.GetKey(KeyCode.Space))
        {
            gliding = true;
            resist.enabled = true;
            rb.mass = 1;
            Glider.SetActive(true);
            //rb.velocity = new Vector3(rb.velocity.x-.05f, rb.velocity.y-.05f, rb.velocity.z - .05f);
            if (grounded == false)
            {
                //rb.AddRelativeForce(Vector3.forward * (Mathf.Abs(rb.velocity.y)*2f), ForceMode.Force);
                //transform.Translate(new Vector3(xSpeed, 0f, zSpeed));
            }
           
        }
        else
        {
            Glider.SetActive(false);
            rb.mass = 2;
            resist.enabled = false;
            gliding = false;
           // rb.AddRelativeForce(Vector3.forward*zSpeed,ForceMode.Impulse);
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            if (reset <= 1)
            {

                reset += .01f;
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0f), reset);
        }

        if (Input.GetKey(KeyCode.A))
            {
            if (grounded)
            {
                rott = rott - rotspeed;
                transform.rotation = Quaternion.Euler(0f, rott, 0f);
            }
            else
            {
                reset = 0;
                rb.AddRelativeTorque(0,0,.1f,ForceMode.VelocityChange);
                rott = rott - rotspeed;
                //transform.rotation = Quaternion.Euler(0f, rott, transform.eulerAngles.z);
            }
        }
            if (Input.GetKey(KeyCode.D))
            {
            if (grounded)
            {
                rott = rott + rotspeed;
                transform.rotation = Quaternion.Euler(0f, rott, 0f);
            }
            else
            {
                reset = 0;
                rb.AddRelativeTorque(0,0,-.1f,ForceMode.VelocityChange);
                rott = rott + rotspeed;
                //transform.rotation = Quaternion.Euler(0f, rott, transform.eulerAngles.z);
            }
        }
            transform.rotation = Quaternion.Euler(0, rott, transform.eulerAngles.z);

        // xSpeed = Input.GetAxis("Horizontal") * speed;

        
            transform.Translate(new Vector3(xSpeed, 0f, zSpeed) * .5f);
        
      
        if (Input.GetKey(KeyCode.S))
        {
            //rb.AddRelativeForce(new Vector3(0, 0, -20), ForceMode.Force);
        }
        /*if (rb.velocity.z > 15 || rb.velocity.x > 15 || rb.velocity.y > 15 )
        {
            rb.velocity =new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z -.05f);
        }*/
     
    }
}
