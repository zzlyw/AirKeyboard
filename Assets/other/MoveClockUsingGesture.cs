using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MoveClockUsingGesture : MonoBehaviour {

    public GameObject markerA; //分针
    public GameObject markerB; //时针

    public GameObject[] markersA;
    public GameObject[] markersB;
                               // Use this for initialization
    float DisThreshold = 0.003f;
    int clockTask = 0;
    bool fen = false;
    bool shi = false;

    public Material yellow_blue;
    public Material cyan_blue;
    public GameObject fenzhen;
    public GameObject shizhen;

    List<float> UserTime = new List<float>();
    float lastTime = 0;
    float currentTime = 0;
    bool timeRecorded = false;

    public Camera cam;
    Vector2 v2;
    public Texture2D cursor;
    bool shizhenIsSelected = false;
    bool fenzhenIsSelected = false;
    GameObject selectedObj;
    public Material selMatShi;
    public Material selMatFen;
    public Material unSelMatShi;
    public Material unSelMatFen;
    public GameObject shizhenButton;
    public GameObject fenzhenButton;
    bool selectionMode = true;
    bool success = false;

    public GameObject start;
    public GameObject end;
    void Start () {
        
        v2 = new Vector3(Screen.width / 2+100, Screen.height / 2);
    }
	
	// Update is called once per frame
	void Update () {

        if (selectionMode)
        {
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
        }



        //判断调整时针还是分针
        Vector3 shiPos = cam.WorldToScreenPoint(shizhenButton.transform.position);
        Vector3 fenPos = cam.WorldToScreenPoint(fenzhenButton.transform.position);
        // Debug.Log("v3: " + v3);
        // Debug.Log("v2: " + v2);
        if (Vector2.Distance(v2, new Vector2(shiPos.x, Screen.height - shiPos.y)) < 90)
        {

            shizhenIsSelected = true;
            selectedObj = shizhenButton;
            shizhenButton.GetComponent<Renderer>().material = selMatShi;
        }
        else
        {
            shizhenIsSelected = false;
            shizhenButton.GetComponent<Renderer>().material = unSelMatShi;
        }

        if (Vector2.Distance(v2, new Vector2(fenPos.x, Screen.height - fenPos.y)) < 90)
        {

            fenzhenIsSelected = true;
            selectedObj = fenzhenButton;
            fenzhenButton.GetComponent<Renderer>().material = selMatFen;
        }
        else
        {
            fenzhenIsSelected = false;
            fenzhenButton.GetComponent<Renderer>().material = unSelMatFen;
        }
        //非选择模式下进行调整
        if (selectionMode)
        {
            //调整时针位置
            if (shizhenIsSelected)
            {
                switch (DefinedGesture.currentGestureCommand)
                {
                    case DefinedGesture.GestureCommand.moveLeft:
                        shizhen.transform.RotateAround(shizhen.GetComponent<HingeJoint>().connectedAnchor, end.transform.position-start.transform.position, 1f);
                        break;
                    case DefinedGesture.GestureCommand.moveRight:
                        shizhen.transform.RotateAround(shizhen.GetComponent<HingeJoint>().connectedAnchor, end.transform.position - start.transform.position, -1f);
                        break;


                }
            }
            //调整分针位置
            if (fenzhenIsSelected)
            {
               

                switch (DefinedGesture.currentGestureCommand)
                {
                    case DefinedGesture.GestureCommand.moveLeft:
                        fenzhen.transform.RotateAround(fenzhen.GetComponent<HingeJoint>().connectedAnchor, end.transform.position - start.transform.position, 1f);
                        break;
                    case DefinedGesture.GestureCommand.moveRight:
                        fenzhen.transform.RotateAround(fenzhen.GetComponent<HingeJoint>().connectedAnchor, end.transform.position - start.transform.position, -1f);
                        break;


                }
            }
        }

        //切换选择模式和调整模式
        if (Input.GetKeyDown(KeyCode.L))
        {
           // selectionMode = !selectionMode;
        }

        //判断分针是否到位
        if (Vector3.Distance(markerA.transform.position, markersA[clockTask].transform.position) < DisThreshold)
        {
            fen = true;
            fenzhen.GetComponent<Renderer>().material = cyan_blue;
        }
        else 
        {
            fen = false;


            fenzhen.GetComponent<Renderer>().material = yellow_blue;
     
            
        }
        //判断时针是否到位
        if (Vector3.Distance(markerB.transform.position, markersB[clockTask].transform.position) < DisThreshold)
        {
            shi = true;
            shizhen.GetComponent<Renderer>().material = cyan_blue;
        }
        else
        {


            shizhen.GetComponent<Renderer>().material = yellow_blue;

        }

        if (!timeRecorded && shi && fen )
        {
            UserTime.Add(Time.time - lastTime);
            timeRecorded = true;
            
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            clockTask++;
            if (clockTask >= 4)
            {
                clockTask = 0;
                //收集满4次数据后，输出数据
                CreateFile(Application.dataPath, "钟表结果_手势.txt", "##########################");
                CreateFile(Application.dataPath, "钟表结果_手势.txt", System.DateTime.Now.ToLocalTime().ToString()+"   手势" );
                CreateFile(Application.dataPath, "钟表结果_手势.txt", "--------------------------");
               
                for (int i = 0; i < 4; i++)
                {
                    CreateFile(Application.dataPath, "钟表结果_手势.txt", UserTime[i].ToString());
                }
                UserTime.Clear();
                success = true;
            }
            
            lastTime = Time.time;
            timeRecorded = false;
            shizhenIsSelected = false;
            fenzhenIsSelected = false;
            v2 = new Vector3(Screen.width / 2, Screen.height / 2);
            selectionMode = true;
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
        GUI.Label(new Rect(700, 10, 20, 20), UserTime.Count.ToString());

        Rect _rect = new Rect(v2.x - 25,  v2.y - 25, 50, 50);
        GUI.DrawTexture(_rect, cursor);

 

        if (success)
        {
            GUI.Label(new Rect(700, 30, 200, 20), "任务完成！");
        }

    }
}
