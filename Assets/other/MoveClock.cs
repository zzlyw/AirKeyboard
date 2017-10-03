using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MoveClock : MonoBehaviour {

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
    bool success = false;

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

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
            shi = false;
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
                CreateFile(Application.dataPath, "钟表结果_手形.txt", "##########################");
                CreateFile(Application.dataPath, "钟表结果_手形.txt", System.DateTime.Now.ToLocalTime().ToString()+"   手形" );
                CreateFile(Application.dataPath, "钟表结果_手形.txt", "--------------------------");
               
                for (int i = 0; i < 4; i++)
                {
                    CreateFile(Application.dataPath, "钟表结果_手形.txt", UserTime[i].ToString());
                }
                UserTime.Clear();
                success = true;
            }
           

            lastTime = Time.time;
            timeRecorded = false;

        }
       
		
	}
    void OnGUI()
    {
        if (success)
        {
            GUI.Label(new Rect(700, 30, 200, 20), "任务完成！");
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
}
