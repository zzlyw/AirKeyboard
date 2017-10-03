using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Typing : MonoBehaviour {
    public Text t1;
    public Text t2;
    List<string> t1_str = new List<string>();
    int cursorCount = 0;
    bool showCursor = true;
	// Use this for initialization
	void Start () {

        string temp = "T,h,e, ,p,r,o,g,r,a,m, ,w,o,u,l,d, ,l,a,y, ,t,h,e, ,f,o,u,n,d,a,t,i,o,n, ,o,f, ,m,i,x,e,d, ,r,e,a,l,i,t,y, ,a,p,p,l,i,c,a,t,i,o,n,s, ,b,a,s,e,d, ,o,n, ,O,S,T,H,M,D,s, ,b,e,f,o,r,e, ,2,0,2,1";
        t1.text = "The program would lay the foundation of mixed reality applications based on OSTHMDs before 2021";
        t2.text = "";

        var liststr = temp.Split(',');
        foreach (string str in liststr)
        {
            t1_str.Add(str);
        }

        Debug.Log(t1_str.Count);


    }
	
	// Update is called once per frame
	void Update () {

        t2.text = "<color=green>";
        t2.text += "Input: ";
        t2.text += "</color>";
        for (int i = 0; i < TaskKeyboard.outTexts.Count; i++)
        {
            if (TaskKeyboard.outTexts[i] == t1_str[i])
            {
                t2.text += "<color=yellow>";
                t2.text += TaskKeyboard.outTexts[i];
                t2.text += "</color>";
            }
            else
            {
                t2.text += "<color=red>";
                t2.text += TaskKeyboard.outTexts[i];
                t2.text += "</color>";
            }
        }
        cursorCount++;
        if (cursorCount >= 30)
        {
            cursorCount = 0;
            showCursor = !showCursor;
        }

        if (showCursor)
        {
            t2.text += "<color=white>";
            t2.text += "|";
            t2.text += "</color>";
        }



    }

    void OnGUI()
    {

    }
}
