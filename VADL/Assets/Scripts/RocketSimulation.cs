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
    private Vector3 windVelocity;
    private Vector3 CoM;
    private Vector3 CoD;
    private Vector3 CoT;

    private bool simulating;

    Rigidbody myRigidbody;

    // Use this for initialization
    void Start () {
        originalPos = this.transform.localPosition;
        windVelocity = new Vector3(windSpeed, 0f, 0f);
        CoM = this.transform.Find("CoM").transform.localPosition;
        CoD = this.transform.Find("CoD").transform.localPosition;
        CoT = this.transform.Find("CoT").transform.localPosition;
        myRigidbody = this.gameObject.GetComponent<Rigidbody>();
        OnReset();
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
            if (currentMass > dryMass)
            {
                currentMass -= 0.01f;
                if (currentMass < dryMass) currentMass = dryMass;
            }
            // simulate thrust
            if (currentMass > dryMass)
            {
                Vector3 thrustForce = new Vector3(0, thrust, 0);
                myRigidbody.AddForceAtPosition(thrustForce, CoT, ForceMode.Force);
            }
            // simulate gravity, don't need to do anything here
            // simulate drag
            Vector3 velocity = myRigidbody.velocity;
            float v2 = myRigidbody.velocity.sqrMagnitude;
            Vector3 dragForce = -velocity.normalized * v2 * drag / 2.0f;
            myRigidbody.AddForceAtPosition(dragForce, CoD, ForceMode.Force);
            // simulate wind
            Vector3 windForce = windVelocity.normalized * windSpeed * windSpeed * drag / 2.0f;
            myRigidbody.AddForceAtPosition(windForce, CoD, ForceMode.Force);
        }
    }

    // Called by GazeGestureManager when the user performs a Select gesture
    void OnLaunch()
    {
        // configure rigid body properly
        myRigidbody.isKinematic = false;
        myRigidbody.useGravity = true;
        myRigidbody.mass = currentMass;
        myRigidbody.centerOfMass = CoM;
        simulating = true;
    }

    void OnThrustIncrease()
    {
        thrust *= 2f;
    }

    void OnThrustDecrease()
    {
        thrust /= 2f;
    }

    void OnDragIncrease()
    {
        drag *= 2f;
    }

    void OnDragDecrease()
    {
        drag /= 2f;
    }

    // Called by SpeechManager when the user says the "Reset world" command
    void OnReset()
    {
        // Put the sphere back into its original local position.
        // Move rocket back to start platform
        this.transform.localPosition = originalPos;
        myRigidbody.transform.rotation = Quaternion.identity;
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.rotation = Quaternion.identity;
        myRigidbody.angularVelocity = Vector3.zero;
        myRigidbody.isKinematic = true;
        myRigidbody.useGravity = true;
        currentMass = wetMass;
        simulating = false;
    }
}
