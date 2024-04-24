using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
/// <summary>
/// 
/// </summary>
public class SaveDataToCSV : MonoBehaviour
{
    private StreamWriter _writer;
    [HideInInspector]
    public string csvFilePath;

    [HideInInspector]
    public bool isStartPro = false;
    [HideInInspector]
    public bool isStartAnti = false;

    [HideInInspector]
    public struct SaveProSaccadeData
    {
        public float mouseX;
        public float mouseY;
        public float DotX;
        public float DotY;
        public float time;
        public bool isProSaccade;

        public SaveProSaccadeData(float x, float y,float DotX,float DotY,float time,bool isProSaccade)
        {
            mouseX = x;
            mouseY = y;
            this.DotX = DotX;
            this.DotY = DotY;
            this.time = time;
            this.isProSaccade = isProSaccade;
        }
    }

    [HideInInspector]
    public struct SaveAntiSaccadeData
    {
        public float mouseX;
        public float mouseY;
        public float DotX;
        public float DotY;
        public float time;
        public bool isAntiSaccade;

        public SaveAntiSaccadeData(float x,float y,float DotX,float DotY,float time,bool isAntiSaccade)
        {
            mouseX = x;
            mouseY = y;
            this.DotX = DotX;
            this.DotY = DotY;
            this.time = time;
            this.isAntiSaccade = isAntiSaccade;
        }
    }

    public void startSaveData()
    {
        // 创建CSV文件并写入表头
        _writer = new StreamWriter(csvFilePath,false,Encoding.UTF8);
        if (isStartAnti)
        {
            _writer.WriteLine("MouseX,MouseY,DotX,DotY,time,isAntiSaccade");
        }
        if (isStartPro)
        {
            _writer.WriteLine("MouseX,MouseY,DotX,DotY,time,isProSaccade");
        }
    }

    public void SaveProSaccadeDataToCSV(float MouseX,float MouseY,float DotX,float DotY,float time,bool isProSaccade)
    {
        // 将数据写入文件
        _writer.WriteLine(MouseX + ","+ MouseY + ","+DotX+","+DotY+","+time+","+isProSaccade);
    }

    public void SaveAntiSaccadeDataToCSV(SaveAntiSaccadeData data)
    {
        // 将数据存入文件
        _writer.WriteLine(data.mouseX + "," + data.mouseY + "," + data.DotX + "," + data.DotY + "," + data.time + "," + data.isAntiSaccade);
    }

    public void OnApplicationQuit()
    {
        if (_writer != null)
        {
            _writer.Close();
        }
    }
}
