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
    private Vector3 windDirection;
    private Vector3 CoM;
    private Vector3 CoD;
    private Vector3 CoT;

    private bool simulating;

    Rigidbody myRigidbody;

    // Use this for initialization
    void Start () {
        originalPos = this.transform.localPosition;
        windDirection = new Vector3(1f, 0f, 0f);
        CoM = transform.Find("CoM").transform.localPosition;
        CoD = transform.Find("CoD").transform.localPosition;
        CoT = transform.Find("CoT").transform.localPosition;
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
            scale = transform.root.transform.localScale.x;
            var scale2 = scale * scale;
            // update rigid body (mass)
            if (currentMass > dryMass)
            {
                currentMass -= 0.01f;
                if (currentMass < dryMass) currentMass = dryMass;
            }
            //myRigidbody.mass = currentMass;

            // simulate thrust
            if (currentMass > dryMass)
            {
                Vector3 thrustForce = transform.TransformDirection(Vector3.up) * thrust * scale;
                var ct = transform.TransformPoint(CoT / scale2);
                myRigidbody.AddForceAtPosition(thrustForce, ct, ForceMode.Force);
            }

            // simulate gravity
            //Physics.gravity = Vector3.down * 9.81f * scale;
            var cg = transform.TransformPoint(CoM / scale2);
            Vector3 gravity = Vector3.down * 9.81f * scale * currentMass;
            myRigidbody.AddForceAtPosition(gravity, cg, ForceMode.Force);

            var cp = transform.TransformPoint(CoD / scale2);

            // simulate drag
            Vector3 velocity = myRigidbody.velocity * scale;
            float v2 = velocity.sqrMagnitude;
            Vector3 dragForce = -velocity.normalized * v2 * drag / 2.0f;
            myRigidbody.AddForceAtPosition(dragForce, cp, ForceMode.Force);

            // simulate wind
            var windScale = windSpeed * windSpeed * scale2;
            Vector3 windForce = windDirection.normalized * windScale * drag / 2.0f;
            myRigidbody.AddForceAtPosition(windForce, cp, ForceMode.Force);
        }
    }

    // Called by GazeGestureManager when the user performs a Select gesture
    void OnLaunch()
    {
        CoM = transform.Find("CoM").transform.localPosition;
        CoD = transform.Find("CoD").transform.localPosition;
        CoT = transform.Find("CoT").transform.localPosition;
        // configure rigid body properly
        myRigidbody.isKinematic = false;
        myRigidbody.useGravity = false;
        myRigidbody.mass = currentMass;
        myRigidbody.centerOfMass = CoM;
        simulating = true;
    }

    private void OnMouseDown()
    {
        OnReset();
        OnLaunch();
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
        myRigidbody.useGravity = false;
        currentMass = wetMass;
        simulating = false;
    }
}
