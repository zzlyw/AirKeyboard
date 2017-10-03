using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectTouch : MonoBehaviour {

    
    bool finger1 = false;
    bool finger2 = false;
    public Material yellow_blue;
    public Material cyan_blue;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (finger1 && finger2)
        {
            GameObject go1 = GameObject.Find("bone3_thumb");
            GameObject go2 = GameObject.Find("bone3_index");

            Vector3 pos = 0.5f * (go1.transform.position + go2.transform.position);
            this.transform.position = pos;
            this.GetComponent<Renderer>().material = cyan_blue;
        }
        else
        {
            this.GetComponent<Renderer>().material = yellow_blue;
        }
		
	}
    void OnTriggerStay(Collider other)
    {
       // Debug.Log(other.name);
        
        if (other.name == "bone3_thumb")
        {
            finger1 = true;
        }
        if (other.name == "bone3_index")
        {
            finger2 = true;
        }
          
    }
    void OnTriggerExit(Collider other)
    {
        // Debug.Log(other.name);

        if (other.name == "bone3_thumb")
        {
            finger1 = false;
        }
        if (other.name == "bone3_index")
        {
            finger2 = false;
        }

    }
}
