using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MoveCube : MonoBehaviour {

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
    bool success = false;

    void Start () {

		
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
        }
        else
        {
            DetectionDestination();

        }

		
	}

    void DetectionDestination()
    {
        if (Vector3.Distance(existingCubes[0].transform.position, destination[destinationIndex].transform.position) < 0.5f*destination[destinationIndex].transform.lossyScale.magnitude)
        {
            Destroy(existingCubes[0]);
            existingCubes.Clear();
            foreach(GameObject go in destination)
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
                CreateFile(Application.dataPath, "方块结果_手形.txt", "##########################");
                CreateFile(Application.dataPath, "方块结果_手形.txt", System.DateTime.Now.ToLocalTime().ToString() + "   手形");
                CreateFile(Application.dataPath, "方块结果_手形.txt", "--------------------------");

                for (int i = 0; i < 10; i++)
                {
                    CreateFile(Application.dataPath, "方块结果_手形.txt", UserTime[i].ToString());
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
        if (success)
        {
            GUI.Label(new Rect(700, 30, 200, 20), "任务完成！");
        }
    }
}
