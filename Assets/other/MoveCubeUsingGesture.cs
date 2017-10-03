using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MoveCubeUsingGesture : MonoBehaviour {

    public GameObject cube_prefab;
    public GameObject[] initPos;
    int initPosIndex=0;
    int oldInitPosIndex = 0;
    public GameObject[] destination;
    int destinationIndex=0;
    int oldDestinationIndex = 0;

    List<GameObject> existingCubes =  new List<GameObject>();
    // Use this for initialization
    List<float> UserTime = new List<float>();
    float lastTime = 0;
    float currentTime = 0;
    public Camera cam;
    Vector2 v2 ;
    public Texture2D cursor;
    bool isSelecting = false;
    GameObject selectedObj;
    public Material selMat;
    bool success = false;
    void Start () {

        v2 = new Vector3(Screen.width/2, Screen.height/2);
	}
	
	// Update is called once per frame
	void Update () {
        if (existingCubes.Count == 0)
        {
            do
            { initPosIndex = (int)Mathf.Floor(Random.value * (initPos.Length - 0.001f)); }
            while(initPosIndex== oldInitPosIndex);
            do
            { destinationIndex = (int)Mathf.Floor(Random.value * (destination.Length - 0.001f)); }
            while (destinationIndex== oldDestinationIndex);

            destination[destinationIndex].GetComponent<MeshRenderer>().enabled = true;

            GameObject go = Instantiate(cube_prefab);
            go.transform.position = initPos[initPosIndex].transform.position;
            existingCubes.Add(go);
            lastTime = Time.time;
            isSelecting = true;
            v2 = new Vector3(Screen.width / 2, Screen.height / 2);
        }
        else
        {
            DetectionDestination();

        }

        //通过鼠标点击选中要操作的物体
        if (isSelecting)
        {


            Vector3 v3 = cam.WorldToScreenPoint(existingCubes[0].transform.position);
           // Debug.Log("v3: " + v3);
           // Debug.Log("v2: " + v2);
            if (Vector2.Distance(v2, new Vector2(v3.x, Screen.height- v3.y))<100)
            {

                isSelecting = false;
                selectedObj = existingCubes[0];
                selectedObj.GetComponent<Renderer>().material = selMat;
            }

           // Debug.Log(Vector2.Distance(v2, new Vector2(v3.x, v3.y)));



        }

        switch (DefinedGesture.currentGestureCommand)
        {
            case DefinedGesture.GestureCommand.rotationLeft:
                v2.x -= 2;
                break;
            case DefinedGesture.GestureCommand.rotationRight:
                v2.x += 2;
                break;
            case DefinedGesture.GestureCommand.rotationUp:
                v2.y -= 2;
                break;
            case DefinedGesture.GestureCommand.rotationDown:
                v2.y += 2;
                break;

        }
        if(selectedObj !=null)
        {
            switch (DefinedGesture.currentGestureCommand)
            {

                case DefinedGesture.GestureCommand.moveLeft:
                    selectedObj.transform.Translate(Vector3.right*0.0005f,Space.Self);
                    break;
                case DefinedGesture.GestureCommand.moveRight:
                    selectedObj.transform.Translate(-Vector3.right * 0.0005f, Space.Self);
                    break;
                case DefinedGesture.GestureCommand.moveUp:
                    selectedObj.transform.Translate(Vector3.forward * 0.0005f, Space.Self);
                    break;
                case DefinedGesture.GestureCommand.moveDown:
                    selectedObj.transform.Translate(-Vector3.forward * 0.0005f, Space.Self);
                    break;
                case DefinedGesture.GestureCommand.moveForward:
                    //selectedObj.transform.Translate(Vector3.forward * 0.001f, Space.Self);
                    break;
                case DefinedGesture.GestureCommand.moveBack:
                    //selectedObj.transform.Translate(-Vector3.forward * 0.001f, Space.Self);
                    break;
            }
        }
 

    }

    void DetectionDestination()
    {
        if (Vector3.Distance(existingCubes[0].transform.position, destination[destinationIndex].transform.position) < 0.5f*destination[destinationIndex].transform.lossyScale.magnitude)
        {
            Destroy(existingCubes[0]);
            existingCubes.Clear();
            selectedObj = null;
            foreach (GameObject go in destination)
            {
                go.GetComponent<MeshRenderer>().enabled = false;
            }
            oldInitPosIndex = initPosIndex;
            oldDestinationIndex = destinationIndex;

            currentTime = Time.time;
            UserTime.Add(currentTime-lastTime);

            if (UserTime.Count >= 10)
            {
                //收集满10次数据后，输出数据
                CreateFile(Application.dataPath, "方块结果_手势.txt", "##########################");
                CreateFile(Application.dataPath, "方块结果_手势.txt", System.DateTime.Now.ToLocalTime().ToString() + "   手势");
                CreateFile(Application.dataPath, "方块结果_手势.txt", "--------------------------");

                for (int i = 0; i < 10; i++)
                {
                    CreateFile(Application.dataPath, "方块结果_手势.txt", UserTime[i].ToString());
                }
                UserTime.Clear();
                success = true;
            }

        }

        
    }

    void CreateFile(string path, string name, string info)
    {
        StreamWriter sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if (!t.Exists)
        {
            sw = t.CreateText();
        }
        else
        {
            sw = t.AppendText();

        }

        sw.WriteLine(info);
        sw.Close();
        sw.Dispose();

    }

    void OnGUI()
    {
        GUI.Label(new Rect(700,10,20,20),UserTime.Count.ToString());

        Rect _rect = new Rect(v2.x - 25, v2.y - 25, 50, 50);
        GUI.DrawTexture(_rect, cursor);

        if (success)
        {
            GUI.Label(new Rect(700,30,200,20),"任务完成！");
        }
    }
}
