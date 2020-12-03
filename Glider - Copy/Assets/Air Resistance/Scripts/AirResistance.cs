using UnityEngine;
using UnityEngine.Assertions;

namespace AirResistance
{
    [RequireComponent(typeof(Rigidbody))]
    public class AirResistance : MonoBehaviour
    {
        //Are we debugging?
        public bool debug = false;
        [Range(1f, 5f)] //How many raycasts per m^2
        public float raycastDensity = 1f;
        private Rigidbody rig; //Our Rigidbody
        //The radius of the object, the increment for raycasting, and the density of the air per raycast
        private float radius, increment, airDensityPerRay;
        //These are used to check if the lossy scale or gravity have changed
        private Vector3 scale = Vector3.zero, gravity = Vector3.zero;
        //Cache the transform, because SPEED IS KEY
        private new Transform transform;
        //Reference to the settings asset
        private AirResistanceSettings settings;
        //Storage for up, right, and forward based on the gravity being down
        private Vector3 up, right, forward;
        //Buffer for raycasting
        private RaycastHit[] raycastBuffer = new RaycastHit[16];

        void Start()
        {
            LoadSettings();

            //Cache our rigidbody
            rig = GetComponent<Rigidbody>();
            //Set it's drag to zero - We'll be handling that.
            rig.drag = 0;
            //Cache our transform
            transform = GetComponent<Transform>();
            //Calculate the increment for raycasting
            increment = 1.0f / raycastDensity;
            //Calculate air density per raycast
            airDensityPerRay = settings.airDensity * increment * increment;
        }

        void FixedUpdate()
        {
            CheckValues();
            ApplyAirResistance();
        }

        private void ApplyAirResistance()
        {
            Vector3 airResistanceForce = CalculateAirResistance();
            //Loop through the area below this gameobject
            for(float rightf = -radius; rightf < radius; rightf += increment)
            {
                for(float forwardf = -radius; forwardf < radius; forwardf += increment)
                {
                    //Calculate origin of ray
                    Vector3 origin = transform.position + (right * rightf + forward * forwardf - up * radius);
                    //Create ray
                    Ray ray = new Ray(origin, up);

                    //Raycast
                    int hits = Physics.RaycastNonAlloc(ray, raycastBuffer);
                    for(int i = 0; i < hits; i++)
                    {
                        //If the hit is this gameobject,
                        if(raycastBuffer[i].transform == transform)
                        {
                            //add air resistance force
                            rig.AddForceAtPosition(airResistanceForce, raycastBuffer[i].point);
                            //and if we're debugging, show the ray
                            if (debug) Debug.DrawLine(origin, raycastBuffer[i].point, Color.red);
                            //Break out of the loop
                            break;
                        }
                    }

                }
            }
        }

        private Vector3 CalculateAirResistance()
        {
            //Yes, this isn't the proper way to calculate air resistance - However, it's a believable approximation
            return new Vector3(rig.velocity.x * up.x, rig.velocity.y * up.y, rig.velocity.z * up.z) * -airDensityPerRay;
        }

        private void CheckValues()
        {
            //If our lossy scale has changed,
            if (scale != transform.lossyScale)
            {
                //recalculate our radius
                scale = transform.lossyScale;
                radius = (transform.lossyScale.magnitude / 2.0f) + 1.0f;
            }

            //If gravity has changed,
            if(gravity != Physics.gravity)
            {
                //recalculate all the directions
                gravity = Physics.gravity;
                up = -Physics.gravity.normalized;
                forward = new Vector3(up.z, up.x, up.y);
                right = Vector3.Cross(up, forward);
            }

        }

        private void LoadSettings()
        {
            settings = Resources.Load("Air Resistance Settings") as AirResistanceSettings;
            Assert.IsNotNull(settings, "Air Resistance Settings is not in the Resources folder!");
        }

    }

}