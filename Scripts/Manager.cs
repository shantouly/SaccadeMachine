using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// </summary>
public class Manager : MonoBehaviour
{
    private ProSaccadeDetection proSaccadeDetection;
    private PanelAnimator panelanimator;
    private AntiSaccadeDetection antiSaccadeDetection;

    private void Start()
    {
        proSaccadeDetection = GetComponent<ProSaccadeDetection>();
        panelanimator = GetComponent<PanelAnimator>();
        antiSaccadeDetection = GetComponent<AntiSaccadeDetection>();
    }

    private void Update()
    {
        // 开启prosaccade检测
        if(Input.GetKeyDown(KeyCode.N))
        {
            proSaccadeDetection.startProSaccade();
            proSaccadeDetection.isStartProSaccade = true;
        }
        else
        {
            // 结束prosaccade检测
            if (Input.GetKeyDown(KeyCode.M))
            {
                proSaccadeDetection.isSpawning = true;
                proSaccadeDetection.isStartProSaccade = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            // 问题窗口弹出
            StartCoroutine(panelanimator.ShowPanel());
        }
        if(Input.GetKeyDown (KeyCode.S))
        {
            // 问题窗口关闭
            StartCoroutine (panelanimator.HidePanel());
        }
        // 开启antisaccade检测
        if (Input.GetKeyDown(KeyCode.Q))
        {
            antiSaccadeDetection.startAntiSaccade();
            antiSaccadeDetection.isStartAntiSaccade = true;
        }
        else
        {
            // 关闭antisaccade检测
            if (Input.GetKeyDown(KeyCode.W))
            {
                antiSaccadeDetection.isFinish = true;
                antiSaccadeDetection.isStartAntiSaccade= false;
            }
        }
    }
}
