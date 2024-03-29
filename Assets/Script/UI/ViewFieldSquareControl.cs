﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ViewFieldSquareControl : MonoBehaviour
{
    Vector3 Scaler;
    private int _zoom_counter;
    public int zoom_counter { get { return _zoom_counter; } }
    private static ViewFieldSquareControl _view = null;
    public static ViewFieldSquareControl ViewInstance { get { return _view; } }

    private void Awake()
    {
        _view = this;
    }
    // Use this for initialization
    void Start()
    {
        Scaler = new Vector3(0.32f, 0.32f, 0.32f);
        _zoom_counter = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && _zoom_counter < 2)
            {
                GetComponent<Transform>().localScale += Scaler;
                transform.Translate(0, 0, 3);
                _zoom_counter++;
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && _zoom_counter > 0)
            {
                GetComponent<Transform>().localScale -= Scaler;
                transform.Translate(0, 0, -3);
                _zoom_counter--;
            }

        }
    }
}
