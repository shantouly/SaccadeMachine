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
        // 开启proSaccade检测
        if(Input.GetKeyDown(KeyCode.N))
        {
            proSaccadeDetection.startProSaccade();
            proSaccadeDetection.isStartProSaccade = true;
        }
        else
        {
            // 结束proSaccade检测
            if (Input.GetKeyDown(KeyCode.M))
            {
                proSaccadeDetection.isSpawning = true;
                proSaccadeDetection.isStartProSaccade = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            // 开启问题面板
            StartCoroutine(panelanimator.ShowPanel());
        }
        if(Input.GetKeyDown (KeyCode.S))
        {
            // 隐藏问题面板
            StartCoroutine (panelanimator.HidePanel());
        }
        // 开启antiSaccade检测
        if (Input.GetKeyDown(KeyCode.Q))
        {
            antiSaccadeDetection.startAntiSaccade();
            antiSaccadeDetection.isStartAntiSaccade = true;
        }
        else
        {
            // 结束antisaccade检测
            if (Input.GetKeyDown(KeyCode.W))
            {
                antiSaccadeDetection.isFinish = true;
                antiSaccadeDetection.isStartAntiSaccade= false;
            }
        }
    }
}
