using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEvent : MonoBehaviour {

    public GameObject mask;
    public int ID = 0;
    //普通按键
    float newtime = 0; 
    float oldtime = 0;
    //删除键
    float newtime2 = 0;
    float oldtime2 = 0;
    // Use this for initialization


    void Start () {
      
    }
	
	// Update is called once per frame
	void Update () {


        

    }
    void OnTriggerEnter(Collider other)
    {
        //若碰撞体不是食指指尖则返回
        if (TaskKeyboard.isSingle)
        {
            if (other.name != "bone3_index") return;
        }

        if (this.ID == 27 || this.ID == 44)
        {
            return;
        }
            newtime = Time.time;
        if (newtime - oldtime > 0.2f)
        {
            TaskKeyboard.currentTarget = this.ID;
            TaskKeyboard.UpdateOutputString();
            //Debug.Log(this.ID);
        }
        oldtime = newtime;
        
    }
    void OnTriggerStay(Collider other)
    {
        //若碰撞体不是食指指尖则返回
        if (TaskKeyboard.isSingle)
        {
            if (other.name != "bone3_index") return;
        }

        //激活mask
        MeshRenderer[] cs = this.GetComponentsInChildren<MeshRenderer>();
        cs[1].enabled = true;




        //根据时间定义删除键
        newtime2 = Time.time;
        if (newtime2 - oldtime2 > 0.5f)
        {
            //为删除键单独定义
            if (this.ID == 27 || this.ID == 44)
            {
            TaskKeyboard.currentTarget = this.ID;
            TaskKeyboard.UpdateOutputString();
            }
            oldtime2 = newtime2;
        }

    }
    void OnTriggerExit(Collider other)
    {

        MeshRenderer[] cs = this.GetComponentsInChildren<MeshRenderer>();
        cs[1].enabled = false;


    }

}
