﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using System;
using System.Linq;

public class AlarmManager : MonoBehaviour
{

    public static AlarmManager Instance = null;

    public GameObject alarmTab;
    // Alarm ViewPort
    public GameObject alarmViewPort;
    // Alarm Content Prefab
    public GameObject alarmContent;
    private List<GameObject> alarmQueue;

    public AudioClip alarmSound;
    AudioSource alarmAudio;

    void Awake()
    {
        // Singleton
        if(Instance == null)
            Instance = this;
    }
    // Use this for initialization
    void Start ()
    {
        alarmQueue = new List<GameObject>();
        alarmAudio = GetComponent<AudioSource>();
        ActiveAlarm();
    }
    
    // Update is called once per frame
    void Update ()
    {
        
    }

    // Hide Alarm Window if alarm does not exist.
    void ActiveAlarm()
    {
        if (alarmViewPort.transform.childCount <= 0)
        {
            alarmTab.SetActive(false);
        }
        else
        {
            alarmTab.SetActive(true);
            PlayAlarmSound();
        }
    }

    // Add new AlarmModel in Alarm Queue.
    public void AddAlarm(Sprite alarmImage, String alarmText, Action action, int leftTurn)
    {
        AddAlarm(alarmImage, alarmText, action, leftTurn, false);
    }

    // Add new AlarmModel in Alarm Queue.
    public void AddAlarm(Sprite alarmImage, String alarmText, Action action, int leftTurn, bool isDied)
    {
        GameObject alarm = (GameObject)Instantiate(alarmContent);
        alarm.AddComponent<AlarmModel>();

        alarm.GetComponent<AlarmModel>().SetProperties(alarmImage, alarmText, action, leftTurn);

        if (alarm.GetComponent<AlarmModel>().leftTurn == 0)
        {
            ShowAlarm(alarm, isDied);
        }
        else
        {
            alarmQueue.Add(alarm);
        }
    }

    // Update leftTurn of alarms and Show alarms whose leftTurn is 0.
    // Called After a Turn Starts.
    public void updateAlarmQueue()
    {   
        for (int x = alarmViewPort.transform.childCount - 1; x >= 0; x--)
        {
            DeleteAlarm(alarmViewPort.transform.GetChild(x).gameObject);
        }

        List<GameObject> alarmToRemove = new List<GameObject>();

        foreach(GameObject alarm in alarmQueue)
        {
            AlarmModel alarmModel = alarm.GetComponent<AlarmModel>();
            --alarmModel.leftTurn;
            if(alarmModel.leftTurn == 0)
            {
                ShowAlarm(alarm);
                alarmToRemove.Add(alarm);
            }
        }
        ActiveAlarm();

        //Remove Alarms
        foreach (GameObject alarm in alarmToRemove)
        {
            alarmQueue.Remove(alarm);
        }
        alarmToRemove.Clear();
    }

    public void DeleteAlarm(GameObject alarm)
    {
        alarmQueue.Remove(alarm);
        Destroy(alarm);
    }

    void ShowAlarm(GameObject alarm)
    {
        ShowAlarm(alarm, false);
    }

    void ShowAlarm(GameObject alarm, bool isDied)
    {
        alarm.transform.SetParent(alarmViewPort.transform, false);
        alarm.GetComponent<AlarmModel>().DispAlarmData(isDied);

        //alarmViewPort.GetComponent<GridLayoutGroup>().constraintCount = ++rowCount;
        //alarmViewPort.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 20);
        ActiveAlarm();
    }

    public void PlayAlarmSound()
    {
        if (alarmAudio.isPlaying) return;
        alarmAudio.PlayOneShot(alarmSound);
    }


}
