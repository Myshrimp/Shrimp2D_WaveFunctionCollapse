using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputHandler : MonoBehaviour
{
    public static Vector2 ScreenPointToWorldPoint()
    {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                Camera.main.transform.position.z > 0 ? Camera.main.transform.position.z : -Camera.main.transform.position.z));//屏幕坐标转换世界坐标
            return worldPos;
    }

    public GameObject MouseSelectedGameObject()
    {       
        //需要碰撞到物体才可以
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isCollider = Physics.Raycast(ray, out hit);
        return hit.collider.gameObject;
    }

    public Vector2 MousePositionScreen()
    {
        return Input.mousePosition;
    }

}
