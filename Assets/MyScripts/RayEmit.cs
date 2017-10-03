using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayEmit : MonoBehaviour {

    GameObject gameObj;
    public Camera cam;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

      //  if (Input.GetMouseButton(0))

        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线

            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))

            {

                Debug.DrawLine(ray.origin, hitInfo.point);//划出射线，只有在scene视图中才能看到

                gameObj = hitInfo.collider.gameObject;

                Vector3 hitColliderPos = hitInfo.transform.position; //碰到的碰撞体位置

                Vector3 hitPointPos = hitInfo.point; //射线碰到碰撞器的碰撞点

                Debug.Log("click object name is " + gameObj.GetComponent<CubeEvent>().ID );


                if (gameObj.GetComponent<CubeEvent>().ID == 0)//当射线碰撞目标为boot类型的物品 ，执行拾取操作

                {

                  //  gameObj.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);

                    Debug.Log("pick up!");

                }

            }

        }

    }
}
