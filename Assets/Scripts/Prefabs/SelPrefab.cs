﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;
using System.Linq;

public class SelPrefab : MonoBehaviour
{
    private static ManagementUIController uicontroller;
    private Text[] textarguments;
    private Image unitPrt;
    private Button[] buttons;
    // Use this for initialization

    void Awake()
    {
        Debug.Log("call SelPre");
        textarguments = gameObject.GetComponentsInChildren<Text>();
        foreach (Image unt in gameObject.GetComponentsInChildren<Image>())
        {
            if (unt.name == "Portrait")
            {
                unitPrt = unt;
            }
        }
        buttons = gameObject.GetComponentsInChildren<Button>();
    }
    void Start ()
    {
        uicontroller = ManagementUIController.GetManagementUIController();
    }
    public GameObject MakeItem(IProductionFactory fact)
    {
        Debug.Log("Selection Queue Item Made");
        string nameofFactory = ProductionFactoryTraits.GetFactoryName(fact);
        unitPrt.sprite = Resources.Load<Sprite>("Unit_portrait/" + nameofFactory + "_portrait");
        foreach (Text txt in textarguments)
        {
            switch (txt.name)
            {
                case "UnitName":
                    txt.text = nameofFactory;
                    break;
                case "NumberOfUnits":
                    txt.text = "X 1";
                    break;
            }
        }
        return this.gameObject;
    }

    public GameObject MakeItem()
    {
        unitPrt.enabled = false;
        Debug.Log("NULL Selection Queue");
        foreach (Text txt in textarguments)
        {
            switch (txt.name)
            {
                case "UnitName":
                    txt.text = "생산 가능 유닛 없음.";
                    txt.fontSize = Screen.height / 40;
                    break;
                case "NumberOfUnits":
                    txt.text = "";
                    break;
            }
        }
        foreach (Button but in buttons)
        {
            but.gameObject.SetActive(false);
        }
        return this.gameObject;
    }
    public void SetButton(int i)
    {
        foreach (Button but in buttons)
        {
            switch (but.name)
            {
                case "Deploy":
                    but.onClick.AddListener(delegate () { ProduceItem(i); });
                    break;
            }
        }
    }

    private void ProduceItem(int i)
    {
        IProductionFactory factory = GameManager.I.Game.PlayerInTurn.AvailableProduction.ToList()[i];
        GameManager.I.Game.PlayerInTurn.Production.AddLast(factory.Create(GameManager.I.Game.PlayerInTurn));
        
        Debug.Log(i + " inputed");
        uicontroller.MakeProductionQ();
        uicontroller.MakeDeploymentQ();
    }
    // Update is called once per frame
    void Update () {
		
	}
}