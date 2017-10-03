using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskKeyboard : MonoBehaviour {

    public GameObject[] keyboardPos;
    public GameObject[] ninePos;
    public GameObject magicCube;
    public GameObject mask;
    public static GameObject keyboardBig;
    public static GameObject keyboardSmall;
    public static GameObject keyboardNine;
    static List<GameObject> cubeList = new List<GameObject>();
    static List<GameObject> cubeListForNine = new List<GameObject>();
    int modeIndex = 0;
    public static int currentTarget = 0;
    static Dictionary<int, string> dicBig = new Dictionary<int, string>();
    static Dictionary<int, string> dicSmall = new Dictionary<int, string>();
    static Dictionary<int, string> dicNine = new Dictionary<int, string>();
    public static string outputString = "";
	public static List<string> outTexts = new List<string>();

    enum InputMode {none, keyboardBig, keyboardSmall, keyboardNine };
    static InputMode inputMode = InputMode.none;
	static InputMode oldMode = InputMode.none;

    public static bool isSingle = true;

    void Start () {
		keyboardBig = GameObject.Find ("keyboardBig");
		keyboardSmall = GameObject.Find ("keyboardSmall");
		keyboardNine = GameObject.Find ("nine");
        //初始化texts
        Init();

        for (int i = 0; i < keyboardPos.Length; i++)
        {
            GameObject go = Instantiate(magicCube);
            cubeList.Add(go);
            go.GetComponent<CubeEvent>().ID = cubeList.Count - 1;
            GameObject go_mask = Instantiate(mask);
            //将生成的物体作为this的子物体
            go.transform.parent = this.transform;
            go_mask.transform.parent = go.transform;
            //设置位置
            go.transform.position = keyboardPos[i].transform.position;
            go_mask.transform.localPosition = Vector3.zero;
            //与父物体保持一致姿态
            go.transform.localRotation = Quaternion.identity;
            go_mask.transform.localRotation = Quaternion.identity;
            
            switch (i)
            {

                case 19:

                    go_mask.transform.localScale = new Vector3(1.85f, 2,1);
                    break;
                case 27:
                    go_mask.transform.localScale = new Vector3(1.85f, 2, 1);
                    break;
                case 28:
                    go_mask.transform.localScale = new Vector3(1.85f, 2, 1);
                    break;
                case 29:
                    go_mask.transform.localScale = new Vector3(1.85f, 2, 1);
                    break;
                case 31:
                    go.GetComponent<BoxCollider>().size = new Vector3(4, 0.5f, 1);
                    go_mask.transform.localScale = new Vector3(6f, 2, 1);
                    break;
                case 32:
                    go_mask.transform.localScale = new Vector3(3.7f, 2, 1);
                    break;
            }

    
        }

        for (int i = 0; i < ninePos.Length; i++)
        {
            GameObject go = Instantiate(magicCube);
            cubeListForNine.Add(go);
            go.GetComponent<CubeEvent>().ID = cubeListForNine.Count - 1 + cubeList.Count;
            GameObject go_mask = Instantiate(mask);
            //将生成的物体作为this的子物体
            go.transform.parent = this.transform;
            go_mask.transform.parent = go.transform;
            //设置位置
            go.transform.position = ninePos[i].transform.position;
            go_mask.transform.localPosition = Vector3.zero;
            //与父物体保持一致姿态
            go.transform.localRotation = Quaternion.identity;
            go_mask.transform.localRotation = Quaternion.identity;

            //重新设置每一个尺寸
            go.GetComponent<BoxCollider>().size = new Vector3(1.2f, 0.8f, 1);
            go_mask.transform.localScale = new Vector3(3.3f, 2, 1);

        }

        UpdateMode();
    }
	
	// Update is called once per frame
	void Update () {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            modeIndex++;
            if (modeIndex >= 4)
            {
                modeIndex = 0;
            }
            inputMode = (InputMode)modeIndex;
            //根据模式变化改变设置
            UpdateMode();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            isSingle = !isSingle;
        }
      
		
	}


    static void EnableList(List<GameObject> LG)
    {
        for (int i = 0; i < LG.Count; i++)
        {
            LG[i].SetActive(true);
        }
    }
    static void DisableList(List<GameObject> LG)
    {
        for (int i = 0; i < LG.Count; i++)
        {
            LG[i].SetActive(false);
        }
    }

    static void UpdateMode()
    {
        
        switch (inputMode)
        {
            case InputMode.none:
                keyboardBig.SetActive(false);
                keyboardSmall.SetActive(false);
                keyboardNine.SetActive(false);
                DisableList(cubeList);
                DisableList(cubeListForNine);
                break;
            case InputMode.keyboardBig:
                keyboardBig.SetActive(true);
                keyboardSmall.SetActive(false);
                keyboardNine.SetActive(false);
                EnableList(cubeList);
                DisableList(cubeListForNine);
                break;
            case InputMode.keyboardSmall:
                keyboardBig.SetActive(false);
                keyboardSmall.SetActive(true);
                keyboardNine.SetActive(false);
                EnableList(cubeList);
                DisableList(cubeListForNine);
                break;
            case InputMode.keyboardNine:
                keyboardBig.SetActive(false);
                keyboardSmall.SetActive(false);
                keyboardNine.SetActive(true);
                DisableList(cubeList);
                EnableList(cubeListForNine);
                break;

        }
    }

    void OnGUI()
    {
		outputString = "Input: ";
		for (int i=0; i < outTexts.Count; i++) {
			outputString += outTexts[i];
		}

        // GUI.Label(new Rect(100,30,1000,50),outputString);
        GUI.color = Color.green;
        GUI.Label(new Rect(600,10,100,20),"Mode: "+ (isSingle?"Single":"Multiple"));
    }
    public static void UpdateOutputString()
    {
        //未定义按键直接返回
        if (currentTarget == 29 || currentTarget == 30 || currentTarget == 32)
        {
            return;
        }
        //非常规情况，功能键
        if (currentTarget == 19) //shift，切换大小写键盘
        {
			if (inputMode == InputMode.keyboardBig) {
				inputMode = InputMode.keyboardSmall;
			} else if (inputMode == InputMode.keyboardSmall) {
				inputMode = InputMode.keyboardBig;
			}
            UpdateMode();
            return;

        }

        if (currentTarget == 27 || currentTarget == 44) //delete
        {
			if (outTexts.Count > 0) {
				outTexts.RemoveAt (outTexts.Count-1);
			}
			return;
        }

        if (currentTarget == 28) //切换到数字键盘
        {
			oldMode = inputMode;
			inputMode = InputMode.keyboardNine;
			UpdateMode();
            return;
        }

        if (currentTarget == 42) //切换回字母键盘
        {
			inputMode = oldMode;
			UpdateMode ();
            return;
        }

        //常规情况字符输入
        switch (inputMode)
        {
            case InputMode.keyboardBig:
			    outTexts.Add( dicBig[currentTarget]);
                break;
            case InputMode.keyboardSmall:
			    outTexts.Add( dicSmall[currentTarget]);
                break;
            case InputMode.keyboardNine:
			    outTexts.Add( dicNine[currentTarget]);
                break;
        }
        

        
     
    }
    void Init()
    {
        dicBig.Add(0, "Q");
        dicBig.Add(1, "W");
        dicBig.Add(2, "E");
        dicBig.Add(3, "R");
        dicBig.Add(4, "T");
        dicBig.Add(5, "Y");
        dicBig.Add(6, "U");
        dicBig.Add(7, "I");
        dicBig.Add(8, "O");
        dicBig.Add(9, "P");
        dicBig.Add(10, "A");
        dicBig.Add(11, "S");
        dicBig.Add(12, "D");
        dicBig.Add(13, "F");
        dicBig.Add(14, "G");
        dicBig.Add(15, "H");
        dicBig.Add(16, "J");
        dicBig.Add(17, "K");
        dicBig.Add(18, "L");
        dicBig.Add(19, "shift");
        dicBig.Add(20, "Z");
        dicBig.Add(21, "X");
        dicBig.Add(22, "C");
        dicBig.Add(23, "V");
        dicBig.Add(24, "B");
        dicBig.Add(25, "N");
        dicBig.Add(26, "M");
        dicBig.Add(27, "delete");
        dicBig.Add(28, "change");
        dicBig.Add(29, "earth");
        dicBig.Add(30, "voice");
        dicBig.Add(31, " ");
        dicBig.Add(32, "search");

        dicSmall.Add(0, "q");
        dicSmall.Add(1, "w");
        dicSmall.Add(2, "e");
        dicSmall.Add(3, "r");
        dicSmall.Add(4, "t");
        dicSmall.Add(5, "y");
        dicSmall.Add(6, "u");
        dicSmall.Add(7, "i");
        dicSmall.Add(8, "o");
        dicSmall.Add(9, "p");
        dicSmall.Add(10, "a");
        dicSmall.Add(11, "s");
        dicSmall.Add(12, "d");
        dicSmall.Add(13, "f");
        dicSmall.Add(14, "g");
        dicSmall.Add(15, "h");
        dicSmall.Add(16, "j");
        dicSmall.Add(17, "k");
        dicSmall.Add(18, "l");
        dicSmall.Add(19, "shift");
        dicSmall.Add(20, "z");
        dicSmall.Add(21, "x");
        dicSmall.Add(22, "c");
        dicSmall.Add(23, "v");
        dicSmall.Add(24, "b");
        dicSmall.Add(25, "n");
        dicSmall.Add(26, "m");
        dicSmall.Add(27, "delete");
        dicSmall.Add(28, "change");
        dicSmall.Add(29, "earth");
        dicSmall.Add(30, "voice");
        dicSmall.Add(31, " ");
        dicSmall.Add(32, "search");

        dicNine.Add(33, "1");
        dicNine.Add(34, "2");
        dicNine.Add(35, "3");
        dicNine.Add(36, "4");
        dicNine.Add(37, "5");
        dicNine.Add(38, "6");
        dicNine.Add(39, "7");
        dicNine.Add(40, "8");
        dicNine.Add(41, "9");
        dicNine.Add(42, "change");
        dicNine.Add(43, "0");
        dicNine.Add(44, "delete");
    }
}
