using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
/// <summary>
/// 
/// </summary>
public class CsvManager : MonoBehaviour
{
    //public string fileName;
    //public List<Book> books = new List<Book>();

    //private void Start()
    //{
    //    // 文件路径
    //    string path = Application.streamingAssetsPath + "/" + fileName + ".csv";
    //    // 检验文件夹是否存在
    //    if (!Directory.Exists(path))
    //    {
    //        Directory.CreateDirectory(Application.streamingAssetsPath);
    //    }
    //    StreamWriter sw = new StreamWriter(path,false,Encoding.UTF8);
    //    sw.WriteLine("id,name,author");

    //    // 存储内容
    //    for (int i = 0; i < books.Count; i++)
    //    {
    //        sw.WriteLine($"{books[i].id},{books[i].name},{books[i].author}");
    //    }

    //    sw.Flush();
    //    sw.Close();
    //}

    private StreamWriter csvWriter;
    private string csvFilePath = "Assets/MouseMovementData.csv"; // CSV文件保存路径

    private struct MousePositionData
    {
        public float mouseX;
        public float mouseY;

        public MousePositionData(float x, float y)
        {
            mouseX = x;
            mouseY = y;
        }
    }

    void Start()
    {
        // 创建CSV文件并写入表头
        csvWriter = new StreamWriter(csvFilePath,false,Encoding.UTF8);
        csvWriter.WriteLine("Time,X,Y"); // 表头：时间，鼠标X位置，鼠标Y位置
    }

    void Update()
    {
        // 获取鼠标当前位置
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        // 创建一个MousePositionData实例并将其写入CSV文件
        MousePositionData data = new MousePositionData(mouseX, mouseY);
        WriteDataToCSV(data);
    }

    void WriteDataToCSV(MousePositionData data)
    {
        // 将鼠标位置写入CSV文件
        csvWriter.WriteLine(Time.time + "," + data.mouseX + "," + data.mouseY);
    }

    void OnApplicationQuit()
    {
        // 关闭CSV文件
        if (csvWriter != null)
        {
            csvWriter.Close();
        }
    }

}
[System.Serializable]
public class Book
{
    public int id;
    public string name;
    public string author;
}

