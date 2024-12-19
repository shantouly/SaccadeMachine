using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShowVideo : MonoBehaviour
{
    public RawImage rawImage;
    private Connect connect;
    Texture2D texture;

    int width = 640;
    int height = 480;

    private void Start()
    {
        texture = new Texture2D(width, height);
        connect = GetComponent<Connect>();
    }

    private void Update()
    {
        // 接收图像数据大小
        byte[] sizeInfo = new byte[4];
        int bytesRead = 0;
        while (bytesRead < sizeInfo.Length)
        {
            bytesRead += connect.stream.Read(sizeInfo, bytesRead, sizeInfo.Length - bytesRead);
        }
        int size = BitConverter.ToInt32(sizeInfo, 0);

        // 接收图像数据
        byte[] imageData = new byte[size];
        bytesRead = 0;
        while (bytesRead < size)
        {
            bytesRead += connect.stream.Read(imageData, bytesRead, size - bytesRead);
        }

        // 创建新的Texture2D对象
        texture = new Texture2D(width, height);
        texture.LoadImage(imageData);   // 这里imageData是JPEG格式的图像数据
        texture.Apply();

        // 将图像数据应用于RawImage
        rawImage.texture = texture;
    }

    
}
