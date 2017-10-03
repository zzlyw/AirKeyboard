
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TaskCubes : MonoBehaviour {
    public Material[] mats;
    public Material[] mats_mask;
    public GameObject cubes;
    public GameObject magicCube;
    public GameObject mask;
    public Camera cam;
    public GUISkin guiskin;
   
    //生成色块的行列数目
    int rows = 1;
    int cols = 1;
    //色块在虚拟空间中的深度
    const float depth = 0.3f;
    //每个level按键次数
    const int NUM = 10;
    //存储色块对象
    List<GameObject> cubeList = new List<GameObject>();
    //目标按键状态
    bool pressFinished = false;
    //存储目标按键的编号
    public static int currentTarget = 0;
    int oldCurrentTarget =0;
    //存储每次按键的位置
    public static List<int> pressedPositions = new List<int>();
    //存储每个按键所用的时间
    List<float> pressTimes = new List<float>();
    float lastTime = 0;
    //存储每个按键在按对之前的错误按键数
    List<int> wrongNums = new List<int>();
    //已经完成的按键数
    int totalPressedNum = 0;
    //正在检测按键
    bool isCheckingButton = false;
    //检测是否已经进入了某个任务
    bool isInTask = false;

    //检测当前任务是否已经完成规定次数
    bool finished = false;
    //设置几种显示模式
    enum MODE { No_Correction , Yes_Correction, No_Correction_Yes_Occlusion, Yes_Correction_Yes_Occlusion };
    MODE mode = MODE.No_Correction; //默认显示模式为无校正、无遮挡
    int modeIndex =0;

    enum LEVEL { F1,F2,F3,F4,F5,F6,F7};
    LEVEL level = LEVEL.F1;

    GameObject go;
    GameObject goClone;
    void Start () {
        go = GameObject.Find("eyeCameraClone");
        goClone = GameObject.Find("eyeCamera");
        go.transform.position = goClone.transform.position;
        go.transform.rotation = goClone.transform.rotation;
        this.transform.parent = go.transform;
      
	}
    void Init()
    {
        pressFinished = false;
        currentTarget = 0;
        oldCurrentTarget = 0;
        pressedPositions.Clear();
        pressTimes.Clear();
        wrongNums.Clear();
        totalPressedNum = 0;
        isCheckingButton = false;
        isInTask = true;
        finished = false;
    }
	// Update is called once per frame
	void Update () {
        go.transform.position = goClone.transform.position;
        go.transform.rotation = goClone.transform.rotation;
        //测试模式选择
        if (Input.GetKeyDown(KeyCode.M))
        {
            modeIndex++;
            if (modeIndex >= 2)
            {
                modeIndex = 0;
            }
            mode = (MODE)modeIndex;
            
        }
       // Debug.Log(mode);

        //方阵的密度设计
        if (Input.GetKeyDown(KeyCode.F1))
        {
            level = LEVEL.F1;
            rows = 1;
            cols = 1;
            task(rows,cols);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            level = LEVEL.F2;
            rows = 2;
            cols = 3;
            task(rows, cols);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            level = LEVEL.F3;
            rows = 3;
            cols = 4;
            task(rows, cols);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            level = LEVEL.F4;
            rows = 6;
            cols = 8;
            task(rows, cols);
            
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            level = LEVEL.F5;
            rows = 9;
            cols = 12;
            task(rows, cols);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            level = LEVEL.F6;
            rows = 12;
            cols = 16;
            task(rows, cols);
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            level = LEVEL.F7;
            rows = 15;
            cols = 20;
            task(rows, cols);
        }

        //遍历按键记录，检验是否按成功

        if (isCheckingButton)
        {
            for (int i = 0; i < pressedPositions.Count; i++)
            {
                if (pressedPositions[i] == currentTarget)
                {
                    pressFinished = true; //检测到正确按下按键
                    cubeList[currentTarget].GetComponent<Renderer>().material = mats[3]; //目标按键恢复原色
                    pressTimes.Add(Time.time - lastTime); //记录这次按键使用的时间
                    wrongNums.Add(i);//记录这次按键正确按下前，已经误按的次数

                    pressedPositions.Clear(); //这句话每次检测到正确按键按下后，都会执行
                    isCheckingButton = false;

                    totalPressedNum++; //按键总数加1
                    break;
                }

            }
        }

        //检测是否处于校正模式
        if (mode == MODE.Yes_Correction)
        {
            MatrixAdjust.startAutoCorrection = true;
        }
        if (mode == MODE.No_Correction)
        {
            MatrixAdjust.startAutoCorrection = false;
        }


        if (totalPressedNum >= NUM)
        {
            finished = true;
            totalPressedNum = 0;
            //收集满10次按键数据后，输出数据
            CreateFile(Application.dataPath, "按键实验结果.txt", "##########################");
            CreateFile(Application.dataPath, "按键实验结果.txt", System.DateTime.Now.ToLocalTime().ToString()+"    rows_cols: "+rows.ToString()+"*"+cols.ToString()+"   mode: "+ mode.ToString()+"  level: "+level.ToString());
            CreateFile(Application.dataPath, "按键实验结果.txt", "--------------------------");
            CreateFile(Application.dataPath, "按键实验结果.txt", "序号  按键时间  失误次数");
            for (int i = 0; i < NUM; i++)
            {
                CreateFile(Application.dataPath, "按键实验结果.txt", i.ToString() + "    " + pressTimes[i].ToString()+ "    " + wrongNums[i].ToString());
            }

        }



        //在不同位置生成新的色块
        if (isInTask && Input.GetKeyDown(KeyCode.Space))
        {
            cubeList[currentTarget].GetComponent<Renderer>().material = mats[3];
            pressedPositions.Clear();
            DoTask();
            isCheckingButton = true;

        }


    }

    void task(int _row, int _col)
    {
        Init(); //变量状态复位

        int rows = _row;
        int cols = _col;
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        cubeList.Clear();

        //复位cubes的姿态位置
        cubes.transform.position = cam.transform.position;
        cubes.transform.rotation = cam.transform.rotation;
        //获得每行每列的物体间距
        int gridx = Screen.width / cols;
        int gridy = Screen.height / rows;
        //根据物体屏幕间距近似计算空间间距
        Vector3 p1 = new Vector3(Screen.width / 2, Screen.height / 2, depth);
        Vector3 p2 = new Vector3(Screen.width / 2 + gridx, Screen.height / 2, depth);
        Vector3 p3 = new Vector3(Screen.width / 2, Screen.height / 2 + gridy, depth);
        float xSpaceDistance = Vector3.Distance(cam.ScreenToWorldPoint(p1), cam.ScreenToWorldPoint(p2));
        float ySpaceDistance = Vector3.Distance(cam.ScreenToWorldPoint(p1), cam.ScreenToWorldPoint(p3));

        //循环生成物体
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                //实例化物体
                GameObject go = Instantiate(magicCube);
                cubeList.Add(go);
                go.GetComponent<CubeEvent>().ID  = cubeList.Count - 1;

                GameObject go_mask = Instantiate(mask);
                //将生成的物体作为cubes的子物体
                go.transform.parent = cubes.transform;
                go_mask.transform.parent = go.transform;
                //根据屏幕位置获得空间位置
                Vector3 v = cam.ScreenToWorldPoint(new Vector3((i + 0.5f) * gridx, (j + 0.5f) * gridy, depth));
                go.transform.position = v;
                go_mask.transform.localPosition = Vector3.zero;
                //与父物体保持一致姿态
                go.transform.localRotation = Quaternion.identity;
                go_mask.transform.localRotation = Quaternion.identity;
                //更改材质
                //int c = (int)Mathf.Floor(Random.value * (mats.Length - 0.0001f));
                int c = 3;
                go.GetComponent<Renderer>().material = mats[c];
                go_mask.GetComponent<Renderer>().material = mats_mask[c];

                //设置物体尺寸
                go.transform.localScale = new Vector3(0.9f * xSpaceDistance, 0.9f * ySpaceDistance, 0.001f);
                go_mask.transform.localScale = new Vector3(1.1f, 1.1f, 0.0008f);
            }
        }

    }

    void DoTask()
    {
        if (level == LEVEL.F1)
        {
            currentTarget = 0;
        }
        else
        {
            do
            {
                currentTarget = (int)Mathf.Floor(Random.value * (cubeList.Count - 0.01f));
            }
            while (oldCurrentTarget == currentTarget);
        }


        //选择新的目标色块
        cubeList[currentTarget].GetComponent<Renderer>().material = mats[2];
        //更新原有的currenttarget
        oldCurrentTarget = currentTarget;

        //记录当前时间
        lastTime = Time.time;


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
        GUI.Label(new Rect(500,10,200,100), mode.ToString());

        
        if (finished)
        {
            GUI.color = Color.red;
            GUI.skin = guiskin;
            GUI.Label(new Rect(100,200,800,500),"FINISHED!!!");
        }
    }

 
       
 

}
