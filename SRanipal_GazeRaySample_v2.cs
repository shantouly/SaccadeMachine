//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class SRanipal_GazeRaySample_v2 : MonoBehaviour
            {
                public int LengthOfRay = 25;
                [SerializeField] private LineRenderer GazeRayRenderer;
                private static EyeData_v2 eyeData = new EyeData_v2();
                private static bool eye_callback_registered = false;

                //此处为增加的变量
                [HideInInspector]
                public float pupilDiameterLeft, pupilDiameterRight;
                [HideInInspector]
                public Vector2 pupilPositionLeft, pupilPositionRight;
                [HideInInspector]
                public float eyeOpenLeft, eyeOpenRight;

                [HideInInspector]
                public long timeStamp;
                [HideInInspector]
                public Vector2 Gaze_Origin_Left, Gaze_Origin_Right;
                [HideInInspector]
                public Vector2 Gaze_Direction_Left, Gaze_Direction_Right;
                [HideInInspector]
                public static Vector3 GazeDirectionCombined;

                [HideInInspector]
                public RaycastHit hit;
                [HideInInspector]
                public RaycastHit hit2;
                [HideInInspector]
                public RaycastHit hit1;


                public GameObject ProSaccadeDotPre;
                public GameObject PrefabToSpawn; // 新增的预制体变量
                private GameObject lastSpawnedPrefab; // 跟踪最近生成的预制体
                private GameObject currentDot;
                public GameObject OriginDotPre;    // 一开始出现在屏幕中央的预制体
                private GameObject OriginDot;       // 托管该预制体
                Vector3 gazeHitPosition;
                Vector3 randomPosition = new Vector3(0,0, 2.5f);
                private float radius = 0.6f;
                private float detectionRadius = 0.06f;

                private bool begin = false;
                private static bool isStart = false;

                //public string csvFilePath;
                //private StreamWriter _writer;

                public string csvFilePath2;
                private StreamWriter _writer2;
                private static int count = 0;

                public SaccadeDetection saccadedetection;

                // 数据的获取的存储
                private static List<Vector3> LocalDirection = new List<Vector3>();
                private static List<int> eyetimeStamp = new List<int>();
                private static List<int> eyeTrail = new List<int>();
                private static List<Vector2> gaze_Origin_Left = new List<Vector2>();
                private static List<Vector2> gaze_Origin_Right = new List<Vector2>();
                private static List<Vector2> gaze_Direction_Left = new List<Vector2>();
                private static List<Vector2> gaze_Direction_Right = new List<Vector2>();
                private static List<Vector2> pupil_Pos_Left = new List<Vector2>();
                private static List<Vector2> pupil_Pos_Right = new List<Vector2>();
                private static List<float> pupil_Size_Left = new List<float>();
                private static List<float> pupil_Size_Right = new List<float>();
                private static List<float> openness_Left = new List<float>();
                private static List<float> openness_Right = new List<float>();
                // 判断条件，用于将原始数据写进文件中
                private bool Process = false;
                private bool isFinish = false;
                private static int indexArray = 0;

                //新添加
                int pro_n_trial = 5;                   // Number of practice trials for pro-saccade task. private GameObject RightTarget;
                private GameObject RightTarget;
                private GameObject CenterTarget;
                private GameObject LeftTarget;

                private float Targetduration = 1;       // Duration (seconds) until a targets appears or disappears.    Recommended: 1s
                private float Angle = 8;                // Angle (degrees) between center target and L/R target.        Recommended: 8-10°
                private float Distance = 2.5f;             // Distance between origin of coordinate and targets.           Recommended: 10m
                private float Distance_camera = 7f;      // Distance between camera and targets.
                private float target_size = 2f;       // Size of target in degree.                                    Recommended: 0.5°
                private float threshold = 0.02f;            // 最大差值
                private string GROUP = "";
                private int Number = 1;
                public bool IsPractice = true;         // 是否是训练状态
                public bool ProSaccadeTest;             // 是否是Prosaccade


                float[] pro_trial = new float[10] { 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f };
                int[] pro_trial_direct = new int[10] { 1, -1, -1, 1, -1, 1, -1, 1, 1, -1 };             // 正式的顺序
                int[] trial_direct_prc = new int[10] { 1, -1, 1, -1, 1, -1, 1, -1, 1, -1 };             // 联系时候的顺序
                private void Start()
                {
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }
                    Assert.IsNotNull(GazeRayRenderer);

                    TargetPosition();
                    Register();
                    //startSaveData();
                    if(IsPractice == false)
                        CreateFile();
                }

                private void Update()
                {/*
                    Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;

                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else return;
                    }
                    else
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else return;
                    }

                    GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);*/

                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        if (IsPractice)
                        {
                            StartCoroutine(sequence());
                        }
                        else
                        {
                            GROUP = "第" + Number + "组";
                            Number++;
                            StartCoroutine(sequence());
                            begin = true;
                        }
                    }
  
                    if (begin)
                    {
                        EyePosition();
                        isStart = true;
                        Process = false;
                    }
                    
                    if (!Process && isFinish)
                    {
                        if (ProSaccadeTest)
                        {
                            Vector3 GazeDirectionWorld = Camera.main.transform.TransformDirection(LocalDirection[0]);
                            Physics.Raycast(Camera.main.transform.position, GazeDirectionWorld, out hit1, LengthOfRay);
                            WriteAndTrans_Pro(new Vector2(hit1.point.x, hit1.point.y), eyetimeStamp[0], eyeTrail[0], 0, 0,0,0,1,0);

                            for (int i = 1; i < indexArray; i++)
                            {
                                //Vector3 GazeDirectionWorld1 = Camera.main.transform.TransformDirection(LocalDirection[i - 1]);
                                Vector3 GazeDirectionWorld2 = Camera.main.transform.TransformDirection(LocalDirection[i]);
                                //Physics.Raycast(Camera.main.transform.position, GazeDirectionWorld1, out hit, LengthOfRay);
                                Physics.Raycast(Camera.main.transform.position, GazeDirectionWorld2, out hit2, LengthOfRay);

                                float distance_X = Mathf.Abs(hit1.point.x - hit2.point.x);
                                float distance_Y = Mathf.Abs(hit1.point.y - hit2.point.y);
                                float distance = Vector2.Distance(new Vector2(hit1.point.x, hit1.point.y), new Vector2(hit2.point.x, hit2.point.y));
                                if(distance >= threshold)
                                {
                                    WriteAndTrans_Pro(new Vector2(hit2.point.x, hit2.point.y), eyetimeStamp[i], eyeTrail[i], distance_X, distance_Y, distance,1,0,i);
                                }else if(distance < threshold)
                                {
                                    WriteAndTrans_Pro(new Vector2(hit2.point.x, hit2.point.y), eyetimeStamp[i], eyeTrail[i], distance_X, distance_Y, distance, 0, 1,i);
                                }
                                hit1 = hit2;
                            }
                        }
                        else
                        {
                            // 就是进行antisaccadeTest
                            Vector3 GazeDirectionWorld = Camera.main.transform.TransformDirection(LocalDirection[0]);
                            Physics.Raycast(Camera.main.transform.position, GazeDirectionWorld, out hit1, LengthOfRay);
                            WriteAndTrans_Anti(new Vector2(hit1.point.x, hit1.point.y), eyetimeStamp[0], eyeTrail[0], 0, 0, 0, 0, 1,0);

                            for (int i = 1; i < indexArray; i++)
                            {
                                //Vector3 GazeDirectionWorld1 = Camera.main.transform.TransformDirection(LocalDirection[i - 1]);
                                Vector3 GazeDirectionWorld2 = Camera.main.transform.TransformDirection(LocalDirection[i]);
                                //Physics.Raycast(Camera.main.transform.position, GazeDirectionWorld1, out hit, LengthOfRay);
                                Physics.Raycast(Camera.main.transform.position, GazeDirectionWorld2, out hit2, LengthOfRay);

                                float distance_X = Mathf.Abs(hit1.point.x - hit2.point.x);
                                float distance_Y = Mathf.Abs(hit1.point.y - hit2.point.y);
                                float distance = Vector2.Distance(new Vector2(hit1.point.x, hit1.point.y), new Vector2(hit2.point.x, hit2.point.y));
                                if (distance >= threshold)
                                {
                                    WriteAndTrans_Anti(new Vector2(hit2.point.x, hit2.point.y), eyetimeStamp[i], eyeTrail[i], distance_X, distance_Y, distance, 1, 0,i);
                                }
                                else if (distance < threshold)
                                {
                                    WriteAndTrans_Anti(new Vector2(hit2.point.x, hit2.point.y), eyetimeStamp[i], eyeTrail[i], distance_X, distance_Y, distance, 0, 1,i);
                                }
                                hit1 = hit2;
                            }

                        }
                        //写完之后，Process为true
                        Process = true;
                        isFinish = false;
                        LocalDirection.Clear();
                        eyetimeStamp.Clear();
                        eyeTrail.Clear();
                        indexArray = 0;
                    }
                }

                private IEnumerator sequence()
                {
                    //Debug.Log("Pro-saccade practice has started.");
                    if (IsPractice)
                    {
                        for(int i = 0;i< pro_n_trial + 5; i++)
                        {
                            yield return StartCoroutine(TargetPrac(pro_trial[i], trial_direct_prc[i]));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < pro_n_trial; i++)
                        {
                            yield return StartCoroutine(TargetAppear(pro_trial[i], pro_trial_direct[i], i));
                        }
                        isFinish = true;
                        isStart = false;
                    }
                }


                private IEnumerator TargetAppear(float fp_time, int direction, int sac_order)
                {
                    //  The central target appears for x seconds between 1 and 3.5 seconds.
                    CenterTarget.SetActive(true);
                    yield return new WaitForSeconds(fp_time);
                    CenterTarget.SetActive(false);

                    /*
                    //  Unix time is recorded when the red target appears.
                    Targettime = DateTime.Now.Ticks;
                    string timingSaccadevalue = Targettime.ToString() + Environment.NewLine;
                    File.AppendAllText("Saccade_Start_Time_" + UserID + ".txt", timingSaccadevalue);
                    */

                    //  Saccade target appears on either the left or right side. (1: right, -1: left from the user's view.)
                    if (direction == 1)
                    {
                        RightTarget.SetActive(true);
                        count = 1;
                        yield return new WaitForSeconds(Targetduration);
                        RightTarget.SetActive(false);
                        count = 0;
                    }

                    if (direction == -1)
                    {
                        LeftTarget.SetActive(true);
                        count = -1;
                        yield return new WaitForSeconds(Targetduration);
                        count = 0;
                        LeftTarget.SetActive(false);
                    }

                    //  Console outputs how many saccade trials have been performed
                    sac_order += 1;
                    //Debug.Log("Saccade trial: " + sac_order);

                }

                private IEnumerator TargetPrac(float fp_time, int direction)
                {
                    //  The central target appears for x seconds between 1 and 3.5 seconds.
                    CenterTarget.SetActive(true);
                    yield return new WaitForSeconds(fp_time);
                    CenterTarget.SetActive(false);

                    //  Saccade target appears on either the left or right side. (1: right, -1: left from the user's view.)
                    if (direction == 1)
                    {
                        RightTarget.SetActive(true);
                        yield return new WaitForSeconds(Targetduration);
                        RightTarget.SetActive(false);
                    }

                    if (direction == -1)
                    {
                        LeftTarget.SetActive(true);
                        yield return new WaitForSeconds(Targetduration);
                        LeftTarget.SetActive(false);
                    }
                }

                void CreateFile()
                {
                    if (ProSaccadeTest)
                    {
                        _writer2 = new StreamWriter(csvFilePath2, false, Encoding.UTF8);
                        _writer2.WriteLine("GazeX,GazeY,index,Group(组别),TimeStamp,Distance_X,Distance_Y,Distance(CurrentAndPrevious),Gaze_Origin_Left_X,Gaze_Origin_Left_Y,Gaze_Origin_Right_X,Gaze_Origin_Right_Y," +
                            "Gaze_Direction_Left_X,Gaze_Direction_Left_Y,Gaze_Direction_Right_X,Gaze_Direction_Right_Y,Pupil_Pos_Left_X,Pupil_Pos_Left_Y,Pupil_Pos_Right_X,Pupil_Pos_Right_Y,Pupil_Size_Left,Pupil_Pos_Right,Eye_Openness_Left,Eye_Openness_Right,IN_SACCADE,IN_FIXATION,TargetAppear,Distance(BetweenGazeAndTarget),isCorrect");
                    }
                    else
                    {
                        _writer2 = new StreamWriter(csvFilePath2, false, Encoding.UTF8);
                        _writer2.WriteLine("GazeX,GazeY,index,Group(组别),TimeStamp,Distance_X,Distance_Y,Distance(CurrentAndPrevious),Gaze_Origin_Left_X,Gaze_Origin_Left_Y,Gaze_Origin_Right_X,Gaze_Origin_Right_Y," +
                            "Gaze_Direction_Left_X,Gaze_Direction_Left_Y,Gaze_Direction_Right_X,Gaze_Direction_Right_Y,Pupil_Pos_Left_X,Pupil_Pos_Left_Y,Pupil_Pos_Right_X,Pupil_Pos_Right_Y,Pupil_Size_Left,Pupil_Pos_Right,Eye_Openness_Left,Eye_Openness_Right,IN_SACCADE,IN_FIXATION,TargetAppear,angle,GazeX,isCorrect");
                    }

                }

                void WriteAndTrans_Pro(Vector2 Gaze, int TIMESTAMP, int Group, float distance_x, float distance_y, float distance, int IN_SACCADE, int IN_FIXATION,int index)
                {
                    float Dis = 0;
                    string Gaze_X = Gaze.x.ToString("F4");
                    string Gaze_Y = Gaze.y.ToString("F4");
                    if (Group == 1)
                    {
                        Dis = Vector2.Distance(new Vector2(RightTarget.transform.position.x, RightTarget.transform.position.y), Gaze);
                        string Dis_4 = Dis.ToString("F4");
                        if(Dis <= 0.05f)
                        {
                            _writer2.WriteLine(Gaze_X + "," + Gaze_Y + "," + Group + "," + GROUP + "," +  TIMESTAMP + "," + distance_x + "," + distance_y + "," + distance + "," +
                                gaze_Origin_Left[index].x + "," + gaze_Origin_Left[index].y + "," + gaze_Origin_Right[index].x + "," + gaze_Origin_Right[index].y + "," +
                                gaze_Direction_Left[index].x + "," + gaze_Direction_Left[index].y + "," + gaze_Direction_Right[index].x + "," + gaze_Direction_Right[index].y + "," +
                                pupil_Pos_Left[index].x + "," + pupil_Pos_Left[index].y + "," + pupil_Pos_Right[index].x + "," + pupil_Pos_Right[index].y + "," + 
                                pupil_Size_Left[index] + "," + pupil_Size_Right[index]+ "," + openness_Left[index] + "," + openness_Right[index] + "," + 
                                IN_SACCADE + "," + IN_FIXATION + "," + "Right"+","+ Dis_4 + ","+"True");
                        }
                        else
                        {
                            _writer2.WriteLine(Gaze_X + "," + Gaze_Y + "," + Group + "," + GROUP + "," + TIMESTAMP + "," + distance_x + "," + distance_y + "," + distance + "," +
                                gaze_Origin_Left[index].x + "," + gaze_Origin_Left[index].y + "," + gaze_Origin_Right[index].x + "," + gaze_Origin_Right[index].y + "," +
                                gaze_Direction_Left[index].x + "," + gaze_Direction_Left[index].y + "," + gaze_Direction_Right[index].x + "," + gaze_Direction_Right[index].y + "," +
                                pupil_Pos_Left[index].x + "," + pupil_Pos_Left[index].y + "," + pupil_Pos_Right[index].x + "," + pupil_Pos_Right[index].y + "," +
                                pupil_Size_Left[index] + "," + pupil_Size_Right[index] + "," + openness_Left[index] + "," + openness_Right[index] + "," +
                                IN_SACCADE + "," + IN_FIXATION + "," + "Right" + "," + Dis_4);
                        }
                    } else if (Group == -1)
                    {
                        Dis = Vector2.Distance(new Vector2(LeftTarget.transform.position.x, LeftTarget.transform.position.y), Gaze);
                        string Dis_4 = Dis.ToString("F4");
                        if (Dis <= 0.05f)
                        {
                            _writer2.WriteLine(Gaze_X + "," + Gaze_Y + "," + Group + "," + GROUP + "," + TIMESTAMP + "," + distance_x + "," + distance_y + "," + distance + "," +
                                gaze_Origin_Left[index].x + "," + gaze_Origin_Left[index].y + "," + gaze_Origin_Right[index].x + "," + gaze_Origin_Right[index].y + "," +
                                gaze_Direction_Left[index].x + "," + gaze_Direction_Left[index].y + "," + gaze_Direction_Right[index].x + "," + gaze_Direction_Right[index].y + "," +
                                pupil_Pos_Left[index].x + "," + pupil_Pos_Left[index].y + "," + pupil_Pos_Right[index].x + "," + pupil_Pos_Right[index].y + "," +
                                pupil_Size_Left[index] + "," + pupil_Size_Right[index] + "," + openness_Left[index] + "," + openness_Right[index] + "," +
                                IN_SACCADE + "," + IN_FIXATION + "," + "Left"+","+ Dis_4 + ","+"True");
                        }
                        else
                        {
                            _writer2.WriteLine(Gaze_X + "," + Gaze_Y + "," + Group + "," + GROUP + "," + TIMESTAMP + "," + distance_x + "," + distance_y + "," + distance + "," +
                                gaze_Origin_Left[index].x + "," + gaze_Origin_Left[index].y + "," + gaze_Origin_Right[index].x + "," + gaze_Origin_Right[index].y + "," +
                                gaze_Direction_Left[index].x + "," + gaze_Direction_Left[index].y + "," + gaze_Direction_Right[index].x + "," + gaze_Direction_Right[index].y + "," +
                                pupil_Pos_Left[index].x + "," + pupil_Pos_Left[index].y + "," + pupil_Pos_Right[index].x + "," + pupil_Pos_Right[index].y + "," +
                                pupil_Size_Left[index] + "," + pupil_Size_Right[index] + "," + openness_Left[index] + "," + openness_Right[index] + "," +
                                IN_SACCADE + "," + IN_FIXATION + "," + "Left" +"," + Dis_4);
                        }                 
                    }
                    else
                    {
                        _writer2.WriteLine(Gaze_X + "," + Gaze_Y + "," + Group + "," + GROUP + "," + TIMESTAMP + "," + distance_x + "," + distance_y + "," + distance + "," +
                                gaze_Origin_Left[index].x + "," + gaze_Origin_Left[index].y + "," + gaze_Origin_Right[index].x + "," + gaze_Origin_Right[index].y + "," +
                                gaze_Direction_Left[index].x + "," + gaze_Direction_Left[index].y + "," + gaze_Direction_Right[index].x + "," + gaze_Direction_Right[index].y + "," +
                                pupil_Pos_Left[index].x + "," + pupil_Pos_Left[index].y + "," + pupil_Pos_Right[index].x + "," + pupil_Pos_Right[index].y + "," +
                                pupil_Size_Left[index] + "," + pupil_Size_Right[index] + "," + openness_Left[index] + "," + openness_Right[index] + "," +
                                IN_SACCADE + "," + IN_FIXATION+","+"center");
                    }

                }

                void WriteAndTrans_Anti(Vector2 Gaze,int TIMESTAMP,int group, float distance_x, float distance_y, float distance, int IN_SACCADE, int IN_FIXATION,int index)
                {
                    float Angle = 0f;
                    string Gaze_X = Gaze.x.ToString("F4");
                    string Gaze_Y = Gaze.y.ToString("F4");
                    //float  Distance = 0;

                    Vector2 Line1 = new Vector2(CenterTarget.transform.position.x,CenterTarget.transform.position.y) - Gaze;
                    Vector2 Line2 = LeftTarget.transform.position - CenterTarget.transform.position;

                    float dotProduct = Vector3.Dot(Line1.normalized, Line2.normalized);

                    Angle = Mathf.Acos(dotProduct);

                    Angle = Angle * Mathf.Rad2Deg;

                    if (group == 1)
                    {
                        if(Angle >=150 && Gaze.x <= LeftTarget.transform.position.x)
                        {
                            _writer2.WriteLine(Gaze_X + "," + Gaze_Y + "," + group + "," + GROUP + "," + TIMESTAMP + "," + distance_x + "," + distance_y + "," + distance + "," +
                            gaze_Origin_Left[index].x + "," + gaze_Origin_Left[index].y + "," + gaze_Origin_Right[index].x + "," + gaze_Origin_Right[index].y + "," +
                            gaze_Direction_Left[index].x + "," + gaze_Direction_Left[index].y + "," + gaze_Direction_Right[index].x + "," + gaze_Direction_Right[index].y + "," +
                            pupil_Pos_Left[index].x + "," + pupil_Pos_Left[index].y + "," + pupil_Pos_Right[index].x + "," + pupil_Pos_Right[index].y + "," +
                            pupil_Size_Left[index] + "," + pupil_Size_Right[index] + "," + openness_Left[index] + "," + openness_Right[index] + "," + 
                            IN_SACCADE + "," + IN_FIXATION + "," + "Right" + "," + Angle + ","+ Gaze_X + "," + "True");
                        }
                        else
                        {
                            _writer2.WriteLine(Gaze_X + "," + Gaze_Y + "," + group + "," + GROUP + "," + TIMESTAMP + "," + distance_x + "," + distance_y + "," + distance + "," +
                                gaze_Origin_Left[index].x + "," + gaze_Origin_Left[index].y + "," + gaze_Origin_Right[index].x + "," + gaze_Origin_Right[index].y + "," +
                                gaze_Direction_Left[index].x + "," + gaze_Direction_Left[index].y + "," + gaze_Direction_Right[index].x + "," + gaze_Direction_Right[index].y + "," +
                                pupil_Pos_Left[index].x + "," + pupil_Pos_Left[index].y + "," + pupil_Pos_Right[index].x + "," + pupil_Pos_Right[index].y + "," +
                                pupil_Size_Left[index] + "," + pupil_Size_Right[index] + "," + openness_Left[index] + "," + openness_Right[index] + "," + 
                                IN_SACCADE + "," + IN_FIXATION + "," + "Right" + "," + Angle + "," + Gaze_X);
                        }
                    }else if(group == -1)
                    {
                        //Distance = Vector2.Distance(new Vector2(RightTarget.transform.position.x, RightTarget.transform.position.y), Gaze);
                        if(Angle <=30 && Gaze.x >= RightTarget.transform.position.x)
                        {
                            _writer2.WriteLine(Gaze_X + "," + Gaze_Y + "," + group + "," + GROUP + "," + TIMESTAMP + "," + distance_x + "," + distance_y + "," + distance + "," +
                                gaze_Origin_Left[index].x + "," + gaze_Origin_Left[index].y + "," + gaze_Origin_Right[index].x + "," + gaze_Origin_Right[index].y + "," +
                                gaze_Direction_Left[index].x + "," + gaze_Direction_Left[index].y + "," + gaze_Direction_Right[index].x + "," + gaze_Direction_Right[index].y + "," +
                                pupil_Pos_Left[index].x + "," + pupil_Pos_Left[index].y + "," + pupil_Pos_Right[index].x + "," + pupil_Pos_Right[index].y + "," +
                                pupil_Size_Left[index] + "," + pupil_Size_Right[index] + "," + openness_Left[index] + "," + openness_Right[index] + "," + 
                                IN_SACCADE + "," + IN_FIXATION + "," + "Left" + "," + Angle + "," + Gaze_X + ","+"True");
                        }
                        else
                        {
                            _writer2.WriteLine(Gaze_X + "," + Gaze_Y + "," + group + "," + GROUP + "," + TIMESTAMP + "," + distance_x + "," + distance_y + "," + distance + "," +
                                gaze_Origin_Left[index].x + "," + gaze_Origin_Left[index].y + "," + gaze_Origin_Right[index].x + "," + gaze_Origin_Right[index].y + "," +
                                gaze_Direction_Left[index].x + "," + gaze_Direction_Left[index].y + "," + gaze_Direction_Right[index].x + "," + gaze_Direction_Right[index].y + "," +
                                pupil_Pos_Left[index].x + "," + pupil_Pos_Left[index].y + "," + pupil_Pos_Right[index].x + "," + pupil_Pos_Right[index].y + "," +
                                pupil_Size_Left[index] + "," + pupil_Size_Right[index] + "," + openness_Left[index] + "," + openness_Right[index] + "," + 
                                IN_SACCADE + "," + IN_FIXATION + "," + "Left" + "," + Angle + "," + Gaze_X);
                        }
                    }
                    else
                    {
                        _writer2.WriteLine(Gaze_X + "," + Gaze_Y + "," + group + "," + GROUP + "," + TIMESTAMP + "," + distance_x + "," + distance_y + "," + distance + "," +
                                gaze_Origin_Left[index].x + "," + gaze_Origin_Left[index].y + "," + gaze_Origin_Right[index].x + "," + gaze_Origin_Right[index].y + "," +
                                gaze_Direction_Left[index].x + "," + gaze_Direction_Left[index].y + "," + gaze_Direction_Right[index].x + "," + gaze_Direction_Right[index].y + "," +
                                pupil_Pos_Left[index].x + "," + pupil_Pos_Left[index].y + "," + pupil_Pos_Right[index].x + "," + pupil_Pos_Right[index].y + "," +
                                pupil_Size_Left[index] + "," + pupil_Size_Right[index] + "," + openness_Left[index] + "," + openness_Right[index] + "," + 
                                IN_SACCADE + "," + IN_FIXATION + "," + "Center");
                    }
                }

                private void Release()
                {
                    if (eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }
                }
                private static void EyeCallback(ref EyeData_v2 eye_data)
                {
                    eyeData = eye_data;
                    Debug.Log(eyeData.verbose_data.left.gaze_direction_normalized);

                    Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;

                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else return;
                    }
                    else
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else return;
                    }

                    if (isStart)
                    {
                        LocalDirection.Add(GazeDirectionCombinedLocal);
                        eyetimeStamp.Add(eyeData.timestamp);
                        eyeTrail.Add(count);
                        gaze_Origin_Left.Add(eyeData.verbose_data.left.gaze_origin_mm);
                        gaze_Origin_Right.Add(eyeData.verbose_data.right.gaze_origin_mm);
                        gaze_Direction_Left.Add(eyeData.verbose_data.left.gaze_direction_normalized);
                        gaze_Direction_Right.Add(eyeData.verbose_data.right.gaze_direction_normalized);
                        pupil_Pos_Left.Add(eyeData.verbose_data.left.pupil_position_in_sensor_area);
                        pupil_Pos_Right.Add(eyeData.verbose_data.right.pupil_position_in_sensor_area);
                        pupil_Size_Left.Add(eyeData.verbose_data.left.pupil_diameter_mm);
                        pupil_Size_Right.Add(eyeData.verbose_data.right.pupil_diameter_mm);
                        openness_Left.Add(eyeData.verbose_data.left.eye_openness);
                        openness_Right.Add(eyeData.verbose_data.right.eye_openness);
                        indexArray++;
                    }
                }

                void Register()
                {
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                         SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

                    if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                    {
                        SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = true;
                    }
                    else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }

                }

                /*
                void EyeData()
                {
                    //以下为新增的部分
                    //pupil diameter 瞳孔的直径
                    pupilDiameterLeft = eyeData.verbose_data.left.pupil_diameter_mm;
                    pupilDiameterRight = eyeData.verbose_data.right.pupil_diameter_mm;

                    //pupil positions 瞳孔位置
                    //pupil_position_in_sensor_area手册里写的是The normalized position of a pupil in [0,1]，给坐标归一化了
                    pupilPositionLeft = eyeData.verbose_data.left.pupil_position_in_sensor_area;
                    pupilPositionRight = eyeData.verbose_data.right.pupil_position_in_sensor_area;

                    //eye open 睁眼
                    //eye_openness手册里写的是A value representing how open the eye is,也就是睁眼程度，从输出来看是在0-1之间，也归一化了
                    eyeOpenLeft = eyeData.verbose_data.left.eye_openness;
                    eyeOpenRight = eyeData.verbose_data.right.eye_openness;

                    // 时间戳
                    timeStamp = eyeData.timestamp;

                    // Gaze_Origin
                    Gaze_Origin_Left = eyeData.verbose_data.left.gaze_origin_mm;
                    Gaze_Origin_Right = eyeData.verbose_data.right.gaze_origin_mm;

                    // Gaze_Direction_Normalized
                    Gaze_Direction_Left = eyeData.verbose_data.left.gaze_direction_normalized;
                    Gaze_Direction_Right = eyeData.verbose_data.right.gaze_direction_normalized;
                }*/

                void ShowOriginDot()
                {
                    OriginDot = Instantiate(OriginDotPre, new Vector3(0, 0, 2.5f), Quaternion.identity);
                }

                void CheckOnPro()
                {
                    if (currentDot != null)
                    {
                        float distance = Vector3.Distance(hit.point, currentDot.transform.position);

                        if (distance < detectionRadius)
                        {
                            SaveProSaccadeDataToCSV(hit.point.x, hit.point.y,count,saccadedetection.IN_SACCADE,saccadedetection.IN_FIXATION,eyeData.timestamp);
                            Destroy(currentDot);
                            currentDot = null;
                            if (count < 10)
                            {
                                TargetPosition();
                            }
                            else
                            {
                                isStart = false;
                                isFinish = true;
                                count = 0;
                            }
                        }
                        else
                        {
                            SaveProSaccadeDataToCSV(hit.point.x, hit.point.y,count,saccadedetection.IN_SACCADE, saccadedetection.IN_FIXATION,eyeData.timestamp);
                        }
                    }
                }

                void EyePosition()
                {
                    // 检查射线是否击中了某个对象
                    /*
                    if (Physics.Raycast(Camera.main.transform.position, GazeDirectionCombined, out hit, LengthOfRay))
                    {
                    gazeHitPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z - 0.01f);

                    if (lastSpawnedPrefab != null)
                    {
                     Destroy(lastSpawnedPrefab);
                    }
                    lastSpawnedPrefab = Instantiate(PrefabToSpawn, gazeHitPosition, Quaternion.identity);
                    }
                    else
                    {
                     //如果射线没有击中任何东西，使用射线的末端位置
                    gazeHitPosition = Camera.main.transform.position + GazeDirectionCombined * LengthOfRay;
                    if (lastSpawnedPrefab != null)
                    {
                    Destroy(lastSpawnedPrefab);
                    }
                    lastSpawnedPrefab = Instantiate(PrefabToSpawn, gazeHitPosition, Quaternion.identity);
                    }*/
                    
                    //Physics.Raycast(Camera.main.transform.position, GazeDirectionCombined, out hit, LengthOfRay);
                    //Debug.Log(hit.point);
                }

                void TargetPosition()
                {
                    /*
                    Vector3 circleCenter = randomPosition;

                    // 计算角度
                    float angle = Random.Range(0, 360f);

                    // 计算下一个随机点的位置

                    float newX = circleCenter.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    float newY = circleCenter.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

                    while (newX < minx || newX > maxx || newY < miny || newY > maxy)
                    {
                        angle = Random.Range(0, 360f);
                        newX = circleCenter.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                        newY = circleCenter.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    }

                    // 生成随机点的位置
                    randomPosition = new Vector3(newX, newY, 2.5f);
                    currentDot = Instantiate(ProSaccadeDotPre, randomPosition, Quaternion.identity);

                    count++;
                    */
                    Vector3 right;      // Position of right target.
                    Vector3 center;     // Position of center target.
                    Vector3 left;       // Position of left target.

                    // ----------------------------------------------------------------------------------------------------------------
                    //  Assigning the game objects (spheres) to the script.
                    // ----------------------------------------------------------------------------------------------------------------
                    RightTarget = GameObject.Find("RightTarget");
                    CenterTarget = GameObject.Find("CenterTarget");
                    LeftTarget = GameObject.Find("LeftTarget");


                    // ----------------------------------------------------------------------------------------------------------------
                    //  Calculating target position.
                    // ----------------------------------------------------------------------------------------------------------------
                    // 1. "Angle" input gets converted from degrees to radians because Unitys' Mathf. works with radians, not degrees.
                    float AngleInRad = Angle * (Mathf.PI / 180);

                    // 2. Center target is placed at desired distance from the user.
                    CenterTarget.transform.position = center = new Vector3(0, 0f, Distance);

                    // 3. Right target is placed at desired angle from the center target.
                    RightTarget.transform.position = right = new Vector3(center.x * Mathf.Cos(AngleInRad) + center.z * Mathf.Sin(AngleInRad), 0f, (-1) * center.x * Mathf.Sin(AngleInRad) + center.z * Mathf.Cos(AngleInRad));

                    // 4. Left target is placed at desired angle from the center target. Either by calculating or by mirroring the right target position (-right.x)
                    LeftTarget.transform.position = left = new Vector3(-right.x, 0f, right.z);


                    // ----------------------------------------------------------------------------------------------------------------
                    //  Change the scale of targets.
                    // ----------------------------------------------------------------------------------------------------------------
                    /*
                    float NewScale = 2 * Distance_camera * Mathf.Tan(target_size / 2 * (Mathf.PI / 180));

                    Vector3 CenterScale = CenterTarget.transform.localScale;
                    Vector3 RightScale = RightTarget.transform.localScale;
                    Vector3 LeftScale = LeftTarget.transform.localScale;

                    CenterScale = new Vector3(NewScale, NewScale, NewScale);
                    RightScale = new Vector3(NewScale, NewScale, NewScale);
                    LeftScale = new Vector3(NewScale, NewScale, NewScale);

                    CenterTarget.transform.localScale = CenterScale;
                    RightTarget.transform.localScale = RightScale;
                    LeftTarget.transform.localScale = LeftScale;
                    */
                    CenterTarget.transform.localScale = new Vector3(0.1f, 0.1f, 1e-06f);
                    RightTarget.transform.localScale = new Vector3(0.1f, 0.1f, 1e-06f);
                    LeftTarget.transform.localScale = new Vector3(0.1f, 0.1f, 1e-06f);

                    Debug.Log("RightPosition" + RightTarget.transform.position + ".........." + "LeftPosition" + LeftTarget.transform.position + "//////////" + "CenterPosition" + CenterTarget.transform.position);
                    // ----------------------------------------------------------------------------------------------------------------
                    //  Hide the targets.
                    // ----------------------------------------------------------------------------------------------------------------
                    if (CenterTarget.activeInHierarchy)
                        CenterTarget.SetActive(false);
                    if (LeftTarget.activeInHierarchy)
                        LeftTarget.SetActive(false);
                    if (RightTarget.activeInHierarchy)
                        RightTarget.SetActive(false);
                }

                void SaveProSaccadeDataToCSV(float GazeX,float GazeY,int count,int IN_SACCADE,int IN_FIXATION,int TimeStamp)
                {
                    //_writer.WriteLine(GazeX + "," + GazeY + ","+count+","+IN_SACCADE+","+IN_FIXATION+","+TimeStamp);
                }
                void OnApplicationQuit()
                {
                    /*
                    if (_writer != null)
                    {
                        _writer.Close();
                    }*/
                    if(_writer2 != null)
                    {
                        _writer2.Close();
                    }
                }
            }
        }
    }
}
