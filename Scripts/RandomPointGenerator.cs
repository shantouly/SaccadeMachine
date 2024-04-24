using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RandomPointGenerator : MonoBehaviour
{
    public GameObject pointPrefab; // 点的预制体
    public GameObject initialPointPrefab; // 初始的点
    private GameObject currentPoint;
    private float spawnInterval = 2f; // 设定每隔多少时间就生成下一个随机点
    private float minX = 280f; // X轴最小值
    private float maxX = 800f; // X轴最大值
    private float minY = 128f;
    private float maxY = 355f;
    private bool isShow = true;
    int z = -380;
    //int x = -250;
    //float y = 280;

    Vector3 randomPosition;

    // 弹窗效果
    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;
    public float animationSpeed;
    public GameObject panel;

    // 获取鼠标的点击事件
    private Ray ray;
    private RaycastHit hit;
    Vector3 target;


    private void Update()
    {
        // 当按下键盘空格键，测试开始
        if(Input.GetKeyDown(KeyCode.Space))
        {
            HideInitialPrefab();
            InvokeRepeating("GenerateRandomPoints", 0f, spawnInterval);
        }
        if(Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(ShowPanel(panel));
        }else if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(HidePanel(panel));
        }

        MousePosition();

        isEqual();
    }

    void GenerateRandomPoints()
    {
        // 如果之前存在随机点，则销毁
        if(currentPoint != null)
        {
            Destroy(currentPoint);
        }

        // 生成随机点的位置
        randomPosition =  new Vector3(Random.Range(minX,maxX), Random.Range(minY, maxY),z);

        // 生成随机点
        currentPoint = Instantiate(pointPrefab,randomPosition,Quaternion.identity);
    }

    void HideInitialPrefab()
    {
        Destroy(initialPointPrefab);
        isShow = false; 
    }

    void MousePosition()
    {
        // 鼠标在屏幕的位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //绘制出一条从相机射出的红色射线
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
        }
        target = hit.point;//获取鼠标的坐标位置

        Debug.Log("获取鼠标的世界坐标位置:" + target);
    }

    void isEqual()
    {
        if(target == initialPointPrefab.transform.position)
        {
            HideInitialPrefab();
        }
    }

    IEnumerator ShowPanel(GameObject panel)
    {
        float timer = 0;
        while (timer <= 1)
        {
            panel.transform.localScale = Vector3.one * showCurve.Evaluate(timer);
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
    }

    IEnumerator HidePanel(GameObject panel)
    {
        float timer = 0;
        while (timer <= 1)
        {
            panel.transform.localScale = Vector3.one * hideCurve.Evaluate(timer);
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
    }
}




