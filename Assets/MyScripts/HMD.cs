using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;


public class HMD : MonoBehaviour
{



    public Camera handCamera;

    public Texture2D cursor;
    
    public static Vector3 fingerPos = Vector3.zero;
    public bool calibrating;
    public float[,] calibCameraCoord;
    public float[,] calibUV;
    public int calibCount;
    public static bool trackState;
    public int calibrationNumbertotal;

   

    public static bool found;
    public int CalibrationNumber;
    public static string netString;


    int CalibrationCount =12;
   
   

    void Start()
    {
        

        Cursor.visible = false;


        calibrating = false;
        calibUV = new float[CalibrationCount, 2];
        calibCameraCoord = new float[CalibrationCount, 3];
        calibCount = 0;
        trackState = false;
        calibrationNumbertotal = 0;


		

       
     //   readTV();    


      //  position();


    }
    //void position()
    //{
    //    System.IO.StreamReader sr = new System.IO.StreamReader("RT.txt");
    //    float[] data = new float[12];
    //    string strTempRT;
    //    for (int i = 0; i < 12; i++)
    //    {

    //        strTempRT = sr.ReadLine();
    //        data[i] = float.Parse(strTempRT);
    //       // Debug.Log(data[i]);

    //    }
    //    sr.Close();



    //    rotMatrix[0, 0] = data[0];
    //    rotMatrix[0, 1] = data[1];
    //    rotMatrix[0, 2] = data[2];
    //    rotMatrix[1, 0] = data[3];
    //    rotMatrix[1, 1] = data[4];
    //    rotMatrix[1, 2] = data[5];
    //    rotMatrix[2, 0] = data[6];
    //    rotMatrix[2, 1] = data[7];
    //    rotMatrix[2, 2] = data[8];

    //    transVect[0, 0] = data[9];
    //    transVect[1, 0] = data[10];
    //    transVect[2, 0] = data[11];

    //    float[,] rotMatrix_turn = new float[3, 3];
    //    //求R的转置
    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 0; j < 3; j++)
    //        {
    //            rotMatrix_turn[i, j] = (float)rotMatrix[j, i];
    //        }


    //    }
    //    Vector3 w_start;
    //    w_start.x = -(float)(rotMatrix_turn[0, 0] * transVect[0, 0] + rotMatrix_turn[0, 1] * transVect[1, 0] + rotMatrix_turn[0, 2] * transVect[2, 0]);
    //    w_start.y = -(float)(rotMatrix_turn[1, 0] * transVect[0, 0] + rotMatrix_turn[1, 1] * transVect[1, 0] + rotMatrix_turn[1, 2] * transVect[2, 0]);
    //    w_start.z = -(float)(rotMatrix_turn[2, 0] * transVect[0, 0] + rotMatrix_turn[2, 1] * transVect[1, 0] + rotMatrix_turn[2, 2] * transVect[2, 0]);
    //    //调整虚拟摄像机在世界坐标系中的位置
    //    Vector3 backPoint = new Vector3(w_start.x, w_start.y, w_start.z);

    //    Matrix4x4 M;

    //    M.m00 = (float)rotMatrix[0, 0];
    //    M.m01 = (float)rotMatrix[1, 0];
    //    M.m02 = (float)rotMatrix[2, 0];
    //    M.m10 = (float)rotMatrix[0, 1];
    //    M.m11 = (float)rotMatrix[1, 1];
    //    M.m12 = (float)rotMatrix[2, 1];
    //    M.m20 = (float)rotMatrix[0, 2];
    //    M.m21 = (float)rotMatrix[1, 2];
    //    M.m22 = (float)rotMatrix[2, 2];

 

      

    //    M.m03 = backPoint.x;
    //    M.m13 = backPoint.y ;
    //    M.m23 = backPoint.z ;

    //    M.m30 = 0;
    //    M.m31 = 0;
    //    M.m32 = 0;
    //    M.m33 = 1;




    //}

    //读取txt转换矩阵，
    //void readTV()
    //{
    //    netString = "";
    //    FileStream fs = new FileStream("T_V.txt", FileMode.Open);

    //    StreamReader sr = new StreamReader(fs, System.Text.UnicodeEncoding.Default);


    //    string str = sr.ReadLine();


    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 0; j < 4; j++)
    //        {
    //            if (str != null)
    //            {
    //                string temp;
    //                float result;
    //                float.TryParse(str.ToString(), out result);
    //                if (i == 2 && j == 3)
    //                {
    //                    temp = str.ToString();
    //                }
    //                else
    //                {
    //                    temp = str.ToString() + ",";
    //                }

    //                netString += temp;
    //               // Debug.Log("netstring " + netString);
    //                T_V[i, j] = (result);
    //                //	Debug.Log("tv"+i+" "+j+" "+T_V[i,j]);
    //                str = sr.ReadLine();

    //            }
    //        }
    //    }



    //    fs.Close();
    //    sr.Close();
    //}

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
    
    //public Camera drawCamera;
    void Update()
    {





        
        //检测跟踪状态
        bool isTracking = HandControl.foundHand;
        if (isTracking )
      
        {
            trackState = true;
        }
        else 
        {
            trackState = false;        
        }



      


       // 按下F12进行标定
        if (Input.GetKeyDown(KeyCode.F12))
        {
            calibrating = true;
           
            calibCount = 0;
            Debug.Log("开始标定！");
            Debug.Log("#########请标定calibCount:" + calibCount);
        }

        if (calibrating == true && trackState == true)
        {
            if (Input.GetMouseButtonDown(0) )
            {
                if (calibCount < CalibrationCount)
                {


                    Vector3 temp = handCamera.transform.InverseTransformPoint(fingerPos);
                    calibCameraCoord[calibCount, 0] = temp.x;
                    calibCameraCoord[calibCount, 1] = temp.y;
                    calibCameraCoord[calibCount, 2] = temp.z;
                    calibUV[calibCount, 0] = Input.mousePosition.x;
                    calibUV[calibCount, 1] = Input.mousePosition.y;

                    calibCount++;
                   
                    Debug.Log("#########请标定calibCount:" + calibCount);

                }
                if (calibCount == CalibrationCount)
                {
                    Debug.Log("标定完成！");
                    calibCount = 0;
                    calibrating = false;
                    if (calibrationNumbertotal < 10000)
                    { calibrationNumbertotal++; }
                    else { Debug.Log("Error!!!!!!!!!!!   Too many calibration times!"); }
                   
                   
                    //输出标定点
                    CreateFile(Application.dataPath, "最新标定坐标.txt", "##########################");
                    CreateFile(Application.dataPath, "最新标定坐标.txt",System.DateTime.Now.ToLocalTime().ToString() );
                    CreateFile(Application.dataPath, "最新标定坐标.txt", "--------------------------");
                    for (int i = 0; i < CalibrationCount; i++)
                    {
                        CreateFile(Application.dataPath, "最新标定坐标.txt", calibCameraCoord[i, 0].ToString()+"  "+calibCameraCoord[i, 1].ToString()+"  "+calibCameraCoord[i, 2].ToString());
                    }
                    for (int i = 0; i < CalibrationCount; i++)
                    {
                        CreateFile(Application.dataPath, "最新标定坐标.txt", calibUV[i, 0].ToString() + "  " + calibUV[i, 1].ToString() );
                    }
                }
            }
        }
    
       
       


    }
    void OnGUI()
   {


        Vector3 msPos = Input.mousePosition;
      //  Rect _rect = new Rect(msPos.x - 25, Screen.height - msPos.y - 25, 50, 50);
      //  GUI.DrawTexture(_rect, cursor);

        if (HandControl.foundHand)
        {
            GUI.color = Color.green;
            Rect rect0 = new Rect(300, 10, 300, 20);
            GUI.Label(rect0, "Tracked: Successful!");
        }
        else
        {
            GUI.color = Color.red;
            Rect rect0 = new Rect(300, 10, 300, 20);
            GUI.Label(rect0, "Tracked: Failed!");
        }




        if (calibrating == true)
        {
            GUI.color = Color.red;
            Rect rect = new Rect(50, 10, 300, 20);
            GUI.Label(rect, "In calibration...");
            Rect rect2 = new Rect(50, 30, 300, 40);
            GUI.Label(rect2, "Please select calibCount:" + calibCount);
        }
        else
        {
           // GUI.color = Color.red;
          //  Rect rect = new Rect(50, 10, 300, 40);
           // GUI.Label(rect, "Not in Calibration!");
        }






    }

  
}
