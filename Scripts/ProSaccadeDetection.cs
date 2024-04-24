using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
/// <summary>
/// 这是一个用来检测prosaccade的类
/// </summary>
public class ProSaccadeDetection : MonoBehaviour
{
    private StreamWriter _writer;
    public string csvFilePath;
    [HideInInspector]
    public bool isStartProSaccade = false;
    [HideInInspector]
    public bool isSpawning = false;

    public GameObject DotPrefab;
    private GameObject currentDot;

    private RaycastHit hit;
    private Vector3 target = Vector3.zero;
    private float detectionRadius = 10; // 设置检测半径
    float distance = 0;
    public float minX = 10f;
    public float maxX = 1240f;
    public float minY = 10f;
    public float maxY = 540f;
    private bool isProSaccade = false;

    // 初始化初始点
    Vector3 randomPosition = new Vector3(670f,275f,0f);


    private void Update()
    {
        if (isStartProSaccade)
        {
            StartProSaccadeDetection();  
        }
        if (isSpawning)
        {
            CancelInvoke("GenerateRandomPoints");
            OnApplicationQuit();
            if (currentDot != null)
            {
                Destroy(currentDot);
            }
            isSpawning = false;
        }
    }

    // 开启prosaccade检测
    void StartProSaccadeDetection()
    {
        CheckMousePosition();
        if (isStartProSaccade && _writer!=null) // 如果启用了 ProSaccade 检测
        {
            // 然后保存数据
            SaveProSaccadeDataToCSV(Time.time,target.x, target.y, randomPosition.x, randomPosition.y,distance, isProSaccade);
        }
    }

    // 检测prosaccade的状态下的位置关系
    void CheckMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //绘制出一条从相机射出的红色射线
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
            target = hit.point;
        }
        if(currentDot != null)
        {
            distance = Vector3.Distance(hit.point, currentDot.transform.position);

            // 判断距离是否在检测半径范围内，并打印结果
            if(distance < detectionRadius)
            {
                Debug.Log("Mouse is near the point: true");
                isProSaccade = true;
            }
            else
            {
                Debug.Log("Mouse is near the point: false");
                isProSaccade = false;
            }
        }
    }

    public void startProSaccade()
    {
        startSaveData();
        InvokeRepeating("GenerateRandomPoints", 0f, 2f);
    }

    void GenerateRandomPoints()
    {
        // 如果之前存在随机点，则销毁
        if (currentDot != null)
        {
            Destroy(currentDot);
        }

        // 计算上一个随机点的位置作为圆心
        Vector3 circleCenter = randomPosition;

        // 计算角度
        float angle = Random.Range(0, 360f);

        // 计算下一个随机点的位置
        
        float newX = circleCenter.x + 120f * Mathf.Cos(angle * Mathf.Deg2Rad);
        float newY = circleCenter.y + 120f * Mathf.Sin(angle * Mathf.Deg2Rad);

        while(newX < minX || newX > maxX || newY < minY || newY > maxY)
        {
            angle = Random.Range(0, 360f);
            newX = circleCenter.x + 120f * Mathf.Cos(angle * Mathf.Deg2Rad);
            newY = circleCenter.y + 120f * Mathf.Sin(angle * Mathf.Deg2Rad);
        }

        // 生成随机点的位置
        randomPosition = new Vector3(newX,newY,0);

        // 生成随机点
        currentDot = Instantiate(DotPrefab, randomPosition, Quaternion.identity);
    }


    // 创建CSV文件并写入表头
    public void startSaveData()
    {
        _writer = new StreamWriter(csvFilePath, false, Encoding.UTF8);
        _writer.WriteLine("Time,MouseX,MouseY,DotX,DotY,Distance,isProSaccade");
    }

    // 将数据写入文件
    public void SaveProSaccadeDataToCSV(float time,float MouseX, float MouseY, float DotX, float DotY,float Distance, bool isProSaccadeT)
    {
        _writer.WriteLine(time + "," + MouseX + "," + MouseY + "," + DotX + "," + DotY + "," + Distance + "," + isProSaccadeT);
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
