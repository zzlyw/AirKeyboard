using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPose : MonoBehaviour {

    public GameObject go;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = go.transform.position;
        this.transform.rotation = go.transform.rotation;
		
	}
}
