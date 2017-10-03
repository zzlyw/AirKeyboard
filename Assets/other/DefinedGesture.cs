using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefinedGesture : MonoBehaviour
{

    // Use this for initialization
    List<GameObject> objs = new List<GameObject>();
    GameObject handCenter;
    Vector3[] Tips;
    // public GUIText gesStateText;
    public GUIText gesCommandText;
    public enum GestureCommand
    {
        moveLeft,
        moveRight,
        moveForward,
        moveBack,
        moveUp,
        moveDown,
        rotationRight,
        rotationLeft,
        rotationUp,
        rotationDown,        
        none
    }
    enum HandMotion
    {
        up,
        down,
        left,
        right,
        forward,
        back,
        empty

    }
    enum GestureState
    {
        palm,
        pinch,
        empty
    }
    GestureState currentGestureState = GestureState.empty;
    HandMotion currentHandMotion = HandMotion.empty;
    public static GestureCommand currentGestureCommand = GestureCommand.none;
    float threshold = 0.856f;
    int frameCountX = 0;
    int frameCountY = 0;
    int frameCountZ = 0;
    int frameCountStay = 0;
    int countThreshold = 10;
    float stayFlag = 0.001f;
    Vector3 origin = Vector3.zero;
    Vector3 destination = Vector3.zero;


    void Start()
    {
        Tips = new Vector3[5];
        // gesStateText.text = currentGestureState.ToString();
        // gesStateText.fontSize = 40;

        gesCommandText.text = currentGestureCommand.ToString();
        gesCommandText.fontSize = 50;


    }

    // Update is called once per frame
    void Update()
    {


        if ( GameObject.Find("RigidRoundHandDefined(Clone)"))
        {

            handCenter = GameObject.Find("_palm");
            objs.Clear();
            foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                if (go.name == "_bone3")
                {
                    objs.Add(go);
                }
            }
            for (int i = 0; i < objs.Count && i < Tips.Length; i++)
            {
                Tips[i] = objs[i].transform.position;
            }

            float temp = SumOfFiveTipsToOthers(Tips);
           // Debug.Log("temp:"+temp.ToString());

            if (temp < threshold)
            {
                currentGestureState = GestureState.pinch;
            }
            else
            {
                currentGestureState = GestureState.palm;
            }



            //================
            destination = handCenter.transform.position;
            int direction = DetectMotionDirection(origin, destination);
            //when accumulate to the threshold, change the HandMotin state
            switch (direction)
            {
                case 0:
                    frameCountStay++;
                    if (frameCountStay > countThreshold)
                    {
                        ClearFrameCount();
                        currentHandMotion = HandMotion.empty;

                    }
                   // Debug.Log("empty");
                    break;
                case 1:
                    frameCountX++;
                    if (frameCountX > countThreshold)
                    {
                        ClearFrameCount();
                        currentHandMotion = destination.x - origin.x > 0 ? HandMotion.left : HandMotion.right;

                    }
                   // Debug.Log("X");
                    break;
                case 2:
                    frameCountY++;
                    if (frameCountY > countThreshold)
                    {
                        ClearFrameCount();
                        currentHandMotion = destination.y - origin.y > 0 ? HandMotion.forward : HandMotion.back;

                    }
                   // Debug.Log("Y");
                    break;
                case 3:
                    frameCountZ++;
                    if (frameCountZ > countThreshold)
                    {
                        ClearFrameCount();
                        currentHandMotion = destination.z - origin.z > 0 ? HandMotion.up : HandMotion.down;

                    }
                  //  Debug.Log("Z");
                    break;
            }


        }
        else
        {
            currentGestureState = GestureState.empty;

        }

        //according to  the current HandMotion state, compute the speed in the direction
        Vector3 speed = destination - origin;
        GetFinalCommand(currentGestureState, currentHandMotion, ref currentGestureCommand);

        origin = destination; //prepare data for the next cycle

        //gesStateText.text = currentGestureState.ToString();
        gesCommandText.text = currentGestureCommand.ToString();

        //Debug.Log("currentGestureState: "+ currentGestureState.ToString());
    }

    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(10,10,400,200), currentGestureCommand.ToString());
    }

    void GetFinalCommand(GestureState gs, HandMotion hm, ref GestureCommand gc)
    {
        switch (gs)
        {
            case GestureState.palm:
                switch (hm)
                {
                    case HandMotion.left:
                        gc = GestureCommand.moveLeft;
                        break;
                    case HandMotion.right:
                        gc = GestureCommand.moveRight;
                        break;
                    case HandMotion.forward:
                        gc = GestureCommand.moveForward;
                        break;
                    case HandMotion.back:
                        gc = GestureCommand.moveBack;
                        break;
                    case HandMotion.up:
                        gc = GestureCommand.moveUp;
                        break;
                    case HandMotion.down:
                        gc = GestureCommand.moveDown;
                        break;
                    case HandMotion.empty:
                        gc = GestureCommand.none;
                        break;
                }
                break;
            case GestureState.pinch:
                switch (hm)
                {
                    case HandMotion.left:
                        gc = GestureCommand.rotationLeft;
                        break;
                    case HandMotion.right:
                        gc = GestureCommand.rotationRight;
                        break;
                    case HandMotion.forward:
                        gc = GestureCommand.none;
                        break;
                    case HandMotion.back:
                        gc = GestureCommand.none;
                        break;
                    case HandMotion.up:
                        gc = GestureCommand.rotationUp;
                        break;
                    case HandMotion.down:
                        gc = GestureCommand.rotationDown;
                        break;
                    case HandMotion.empty:
                        gc = GestureCommand.none;
                        break;
                }
                break;
            case GestureState.empty:
                gc = GestureCommand.none;
                break;

        }

    }
    void ClearFrameCount()
    {
        frameCountX = 0;
        frameCountY = 0;
        frameCountZ = 0;
        frameCountStay = 0;

    }
    float TipsDistance(Vector3 a, Vector3 b)
    {
        float dis = 0;
        dis = Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2) + Mathf.Pow(a.z - b.z, 2));

        return dis;
    }
    float MeanDistanceOfFive(Vector3[] v)
    {
        float dis = 0;
        Vector3 v_sum = Vector3.zero;
        for (int i = 0; i < v.Length; i++)
        {
            v_sum = v_sum + v[i];
        }
        Vector3 v_mean = v_sum / v.Length;
        for (int i = 0; i < v.Length; i++)
        {
            dis = dis + TipsDistance(v_mean, v[i]);
        }


        return dis / v.Length;
    }

    float SumOfFiveTipsToOthers(Vector3[] v)
    {
        float sumDis = 0;
        for (int i = 0; i < v.Length; i++)
        {
            for (int j = 0; j < v.Length; j++)
            {
                sumDis += TipsDistance(v[i], v[j]);
            }

        }
        return sumDis;
    }

    int DetectMotionDirection(Vector3 origin, Vector3 destination)
    {
        Vector3 s = destination - origin;
        Vector3 t = new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));
        if (t.x + t.y + t.z < stayFlag) return 0; // not more than the stayflag, we consider it in a static state

        if (t.x >= t.y && t.x >= t.z)
        {
            return 1;
        }
        else if (t.y >= t.x && t.y >= t.z)
        {
            return 2;
        }
        else if (t.z >= t.x && t.z >= t.y)
        {
            return 3;
        }
        else
        {
            return 0;
        }
    }


}
