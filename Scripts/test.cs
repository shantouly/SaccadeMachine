using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// </summary>
public class test : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    private Ray ray;
    private RaycastHit hit;
    private bool isTrue = false;

    private void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //绘制出一条从相机射出的红色射线
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
        }
             // 计算向量AB和向量BC
            Vector3 vectorAB = pointA.transform.position - pointB.transform.position;
            Vector3 vectorBC = pointA.transform.position - hit.point;

            // 计算AB和BC的点乘
            float dotProduct = Vector3.Dot(vectorAB.normalized, vectorBC.normalized);

            // 使用余弦公式计算夹角（弧度）
            float angleRad = Mathf.Acos(dotProduct);

            // 将弧度转换为角度
            float angleDeg = angleRad * Mathf.Rad2Deg;
        if(angleDeg < 20)
        {
            isTrue = true;
        }
        else
        {
            isTrue = false;
        }

            Debug.Log("AB和BC的夹角（度）：" + angleDeg + "......" + isTrue);
    }
}
