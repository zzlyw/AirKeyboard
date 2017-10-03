using UnityEngine;
using System.Collections;
using Leap;

public class HandControl : MonoBehaviour {

	// Use this for initialization
    Controller controller = new Controller();
    Vector3 tipPosition;

    public GameObject index;
    public GameObject targetCube;
   
    public static bool foundHand = false;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


        Frame frame = controller.Frame();
        HandList handList = frame.Hands;

        FingerList fingers = handList[0].Fingers;
        //食指
        tipPosition = fingers[1].TipPosition.ToUnityScaled();
        
        //用于标定任务
        HMD.fingerPos = tipPosition;
        //用于按键任务获取食指指尖位置
        index.transform.position = tipPosition;
        ReviseError.inputFingerPoint = tipPosition;
       

       // Debug.Log(tipPosition);

        if (handList.IsEmpty)
        {
            foundHand = false;
        }
        else
        {
            foundHand = true;
        }
	
	}
}
