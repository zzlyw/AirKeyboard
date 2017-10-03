using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ReviseError : MonoBehaviour {

	// Use this for initialization

    public static Vector3 inputFingerPoint = new Vector3(1,1,1); //当前手指的屏幕位置
    public static Vector3 recommendedPoint =Vector3.zero; //最佳手指屏幕位置
   
    public static bool check = false; //撞击任务中
    float threshold = 0.02f; //校正阈值

    List<float> projectionErrors = new List<float>(); //投影点误差
    List<Vector2> projectionErrorsXY = new List<Vector2>(); //投影点XY方向误差
    const int errorNum = 5; //记录的误差数据个数
    
    public Camera eyeCam; //人眼摄像机

    public static Vector2 screenError = Vector2.zero; //屏幕误差，用于确定校正数值

    List<Vector2> storeData = new List<Vector2>(); 
    List<Vector2> storeReviseValue = new List<Vector2>();
    List<int> storeRevisePos = new List<int>();
    bool isRevise = false;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
 

        //是否处于检测误差状态

        if (check)
        {
            Debug.Log("check");
            //获得屏幕XY方向点击误差
            Vector2 v2;
            v2.x = eyeCam.WorldToScreenPoint(inputFingerPoint).x-eyeCam.WorldToScreenPoint(recommendedPoint).x;
            v2.y = eyeCam.WorldToScreenPoint(inputFingerPoint).y-eyeCam.WorldToScreenPoint(recommendedPoint).y;
            projectionErrorsXY.Add(v2);
            //计算实际误差
            float projectionError = v2.magnitude;
            projectionErrors.Add(projectionError);

            //如果记录的误差个数超过了阈值，就把最早的删除
            if (projectionErrors.Count == errorNum+1)
            {
                projectionErrorsXY.RemoveAt(0);
                projectionErrors.RemoveAt(0);
            }


            check = false;

            //记录当前的误差
            storeData.Add(v2);
            //记录当前的屏幕位置修正值，这个值只在满足一定条件后更新
            storeReviseValue.Add(screenError);

            if (isRevise)
            {
                storeRevisePos.Add(1);
                isRevise = false;
            }
            else
            {
                storeRevisePos.Add(0);
            }

        }

        //持续更新调整数据
        if (projectionErrorsXY.Count == errorNum)
        {
            Vector2 v2 = Vector2.zero;
            for (int i = 0; i < errorNum; i++)
            {
                v2.x += projectionErrorsXY[i].x;
                v2.y += projectionErrorsXY[i].y;
            }
            screenError = new Vector2(v2.x/errorNum, v2.y/ errorNum);
            Debug.Log("screenError: " + screenError.x + "  " + screenError.y);

            
        }

        //一旦误差超过最大允许误差值，则进行一次校正
        if (projectionErrors.Count == errorNum)
        {
            float acc = 0;
            for (int i = 0; i < projectionErrors.Count; i++)
            {
                acc += projectionErrors[i];
            }
            if (acc > 40* errorNum)
            {
                isRevise = true;
                MatrixAdjust.isRevise = true;
                projectionErrors.Clear();
                projectionErrorsXY.Clear();
                
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
    float Dis(Vector3 a, Vector3 b)
    {
        float d;
        d = Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));
        return d;
    }

    //void medianFiltering(List<float> a, List<float> b)
    //{

    //    for (int i = 0; i < a.Count; i++)
    //    {
    //        if (i == 0 )
    //        { 
    //            b[0] = a[0]<a[1]?a[0]:a[1];
    //        }
    //        else if (i == a.Count - 1)
    //        {
    //            b[a.Count - 1] = a[a.Count - 2] < a[a.Count - 1] ? a[a.Count - 2] : a[a.Count - 1];
    //        }
    //        else
    //        { 

    //        }

    //    }
    //}
    void OnDestroy()
    {
        //存储误差
      //  if (MatrixAdjust.startAutoCorrection)
        {
            CreateFile(Application.dataPath, "偏差数据.txt", "##########################");
            CreateFile(Application.dataPath, "偏差数据.txt", System.DateTime.Now.ToLocalTime().ToString());
            CreateFile(Application.dataPath, "偏差数据.txt", "--------------------------");

            for (int i = 0; i < storeData.Count; i++)
            {
                CreateFile(Application.dataPath, "偏差数据.txt", storeData[i].x + "  " + storeData[i].y + "  " + storeReviseValue[i].x + "  " + storeReviseValue[i].y+"  "+ storeRevisePos[i]);
            }


        }
    }
}
