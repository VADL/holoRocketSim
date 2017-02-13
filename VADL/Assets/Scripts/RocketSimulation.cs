using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSimulation : MonoBehaviour {

    public float dryMass;
    public float wetMass;
    public float windSpeed;
    public float drag;
    public float thrust;

    private float currentMass;
    private float scale;
    private Vector3 originalPos;
    private Quaternion originalOrientation;
    private Vector3 windVelocity;
    private Vector3 CoM;
    private Vector3 CoD;
    private Vector3 CoT;

    private bool simulating;

    // Use this for initialization
    void Start () {
        currentMass = wetMass;
        originalPos = this.transform.localPosition;
        originalOrientation = this.transform.localRotation;
        simulating = false;
        windVelocity = new Vector3(windSpeed, windSpeed, windSpeed);
        CoM = this.transform.Find("CoM").transform.localPosition;
        CoD = this.transform.Find("CoD").transform.localPosition;
        CoT = this.transform.Find("CoT").transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
    }
    
	void FixedUpdate () {
        if (simulating)
        {
            float currentScale = this.gameObject.transform.root.transform.localScale.x; // they're all the same
            // update rigid body (mass)
            var rigidbody = this.GetComponent<Rigidbody>();
            if (currentMass > dryMass)
            {
                currentMass -= 0.01f;
                if (currentMass < dryMass) currentMass = dryMass;
            }
            // simulate thrust
            if (currentMass > dryMass)
            {
                Vector3 thrustForce = new Vector3(0, thrust, 0);
                rigidbody.AddForceAtPosition(thrustForce, CoT);
            }
            // simulate gravity, don't need to do anything here
            // simulate drag
            Vector3 velocity = rigidbody.velocity;
            float v2 = rigidbody.velocity.sqrMagnitude;
            Vector3 dragForce = -velocity.normalized * v2 * drag / 2.0f;
            rigidbody.AddForceAtPosition(dragForce, CoD);
            // simulate wind
            Vector3 windForce = windVelocity.normalized * windSpeed * windSpeed * drag / 2.0f;
            rigidbody.AddForceAtPosition(windForce, CoD);
        }
    }

    // Called by GazeGestureManager when the user performs a Select gesture
    void OnLaunch()
    {
        // If the rocket has no Rigidbody component, add one to enable physics.
        if (!this.GetComponent<Rigidbody>())
        {
            var rigidbody = this.gameObject.AddComponent<Rigidbody>();
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            // configure rigid body properly
            rigidbody.mass = currentMass;
            rigidbody.centerOfMass = CoM;
            rigidbody.drag = 0; // we'll do our own drag
        }
        simulating = true;
    }

    // Called by SpeechManager when the user says the "Reset world" command
    void OnReset()
    {
        // If the Rocket has a Rigidbody component, remove it to disable physics.
        var rigidbody = this.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            DestroyImmediate(rigidbody);
        }

        // Put the sphere back into its original local position.
        this.transform.localPosition = originalPos;
        this.transform.localRotation = originalOrientation;
        currentMass = wetMass;
        simulating = false;
    }
}
