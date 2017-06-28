using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollFeedback : MonoBehaviour
{

    Quaternion rot;

    // Use this for initialization
    void Start()
    {
        rot = gameObject.transform.rotation;
    }

    // Update is called once per frame
    private void Update()
    {
        gameObject.transform.rotation = rot;
        Debug.Log(rot.ToString());
        Debug.Log(rot.eulerAngles);
    }


    public void SetOrientation(float xval, float yval, float zval, float wval)
    {

        //Debug.Log("In Set Orientation Euler Quat");
        rot.Set(xval, yval, zval, wval);
    }

    public void SetOrientation(float xval, float yval, float zval)
    {
        //Debug.Log("In Set Orientation Euler Angles");

        rot.eulerAngles = new Vector3(xval, yval, zval);
    }
}
