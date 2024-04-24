using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
/// <summary>
/// 
/// </summary>
public class AntiSaccadeDetection : MonoBehaviour
{
    private StreamWriter _writer;
    public string csvFilePath;
    [HideInInspector]
    public bool isStartAntiSaccade = false;
    [HideInInspector]
    public bool isFinish = false;

    public GameObject PointPrefab;
    private GameObject currentPoint;

    private RaycastHit hit;
    private Vector3 target = Vector3.zero;
    private bool isTrue = false;
    public float minX = 10f;
    public float maxX = 1240f;
    public float minY = 10f;
    public float maxY = 540f;
    private float angleDeg;
    private bool isAntiSaccade = false;

    Vector3 randomPosition = new Vector3(670f,275f,0);



    private void Update()
    {
        if (isStartAntiSaccade)
        {
            StartAntiSaccadeDetection();
        }
        if (isFinish)
        {
            CancelInvoke("GenerateRandomPointsOfAnti");
            OnApplicationQuit();
            if (currentPoint != null)
            {
                Destroy(currentPoint);
            }
            isFinish = false;
        }
    }

    // 开启prosaccade检测
    void StartAntiSaccadeDetection()
    {
        if(currentPoint != null)
        {
            CheckMouseAngle();
        }      
        if(isStartAntiSaccade && _writer != null)
        {
            // 保存数据
            SaveAntiSaccadeDataToCSV(Time.time, target.x, target.y,randomPosition.x,randomPosition.y,angleDeg,isAntiSaccade);
        }
    }

    // 检测prosaccade的状态下的位置关系
    void CheckMouseAngle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //绘制出一条从相机射出的红色射线
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
        }
        Vector3 vectorLine = randomPosition - new Vector3(randomPosition.x - 200, randomPosition.y, randomPosition.z);
        Vector3 mouseTocuttenPoint = currentPoint.transform.position - hit.point;

        // 计算两条直线的点乘
        float dotProduct = Vector3.Dot(vectorLine.normalized, mouseTocuttenPoint.normalized);

        // 使用余弦公式计算夹角（弧度）
        float angleRad = Mathf.Acos(dotProduct);

        // 将弧度转换为角度
        angleDeg = angleRad * Mathf.Rad2Deg;

        // 判断角度是否在判断的范围内
        if(angleDeg < 25 || angleDeg > 155)
        {
            isTrue = true;
            isAntiSaccade = true;
        }
        else
        {
            isTrue = false;
            isAntiSaccade = false;
        }
        Debug.Log("角度为：" + angleDeg + "是否正确" + isTrue);
    }
    public void startAntiSaccade()
    {
        startSaveData();
        InvokeRepeating("GenerateRandomPointsOfAnti", 0f, 3f);
    }

    void GenerateRandomPointsOfAnti()
    {
        // 如果之前存在随机点，则销毁
        if (currentPoint != null)
        {
            Destroy(currentPoint);
        }
        // 计算上一个随机点的位置作为圆心
        Vector3 circleCenter = randomPosition;

        // 计算角度
        float angle = Random.Range(0, 360f);

        // 计算下一个随机点的位置

        float newX = circleCenter.x + 120f * Mathf.Cos(angle * Mathf.Deg2Rad);
        float newY = circleCenter.y + 120f * Mathf.Sin(angle * Mathf.Deg2Rad);

        while (newX < minX || newX > maxX || newY < minY || newY > maxY)
        {
            angle = Random.Range(0, 360f);
            newX = circleCenter.x + 120f * Mathf.Cos(angle * Mathf.Deg2Rad);
            newY = circleCenter.y + 120f * Mathf.Sin(angle * Mathf.Deg2Rad);
        }

        // 生成随机点的位置
        randomPosition = new Vector3(newX, newY, 0);

        // 生成随机点
        currentPoint = Instantiate(PointPrefab, randomPosition, Quaternion.identity);
    }


    // 创建CSV文件并写入表头
    public void startSaveData()
    {
        _writer = new StreamWriter(csvFilePath, false, Encoding.UTF8);
        _writer.WriteLine("Time,MouseX,MouseY,DotX,DotY,Angle,isAntiSaccade");
    }

    // 将数据写入文件
    public void SaveAntiSaccadeDataToCSV(float time,float MouseX, float MouseY, float DotX, float DotY,float angle, bool isAntiSaccadeT)
    {
        _writer.WriteLine(time + "," + MouseX + "," + MouseY + "," + DotX + "," + DotY + "," + angle + ","  +isAntiSaccadeT);
        //Debug.Log("已经保存");
    }

    public void OnApplicationQuit()
    {
        if (_writer != null)
        {
            _writer.Close();
        }
    }
}
