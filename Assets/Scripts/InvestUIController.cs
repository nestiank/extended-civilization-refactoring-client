﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;

public class InvestUIController : MonoBehaviour {

    public GameObject InvestmentUI;
    public GameObject Tax;
    public GameObject EcoInv;
    public GameObject TechInv;
    public GameObject Logistics;

    private Slider taxSlider;
    private Slider eiSlider;
    private Slider tiSlider;
    private Slider logiSlider;

    private Text taxRateText;
    private Text eiRateText;
    private Text tiRateText;
    private Text logiRateText;
    private static InvestUIController _IVUIController;
    public static InvestUIController I { get { return _IVUIController; } }
    // Use this for initialization
    void Awake()
    {
        // Singleton
        if (_IVUIController != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            _IVUIController = this;
        }
        // Use this when scene changing exists
        // DontDestroyOnLoad(gameObject);
    }
    void Start () {
        taxSlider = Tax.GetComponentInChildren<Slider>();
        eiSlider = EcoInv.GetComponentInChildren<Slider>();
        tiSlider = TechInv.GetComponentInChildren<Slider>();
        logiSlider = Logistics.GetComponentInChildren<Slider>();
        initSlider();
    }
	
	// Update is called once per frame
	void Update ()
    {
        GameManager.I.Game.PlayerInTurn.TaxRate = ((double)((int)(taxSlider.value * 100)))/100f;
        GameManager.I.Game.PlayerInTurn.EconomicInvestmentRatio = ((double)((int)(eiSlider.value * 100))) / 100f;
        GameManager.I.Game.PlayerInTurn.ResearchInvestmentRatio = ((double)((int)(tiSlider.value * 100))) / 100f;
        GameManager.I.Game.PlayerInTurn.RepairInvestmentRatio = ((double)((int)(logiSlider.value * 100))) / 100f;

        taxRateText.text = ((int)(taxSlider.value * 100)).ToString() + "%";
        eiRateText.text = ((int)(eiSlider.value * 100)).ToString() + "%";
        tiRateText.text = ((int)(tiSlider.value * 100)).ToString() + "%";
        logiRateText.text = ((int)(logiSlider.value * 100)).ToString() + "%";
    }

    public void initSlider()
    {
        taxSlider.maxValue = 1f;
        taxSlider.minValue = 0f;

        eiSlider.maxValue = 2f;
        eiSlider.minValue = 0f;

        tiSlider.maxValue = 2f;
        tiSlider.minValue = 0f;

        logiSlider.maxValue = 1f;
        logiSlider.minValue = 0f;

        taxSlider.value = (float)GameManager.I.Game.PlayerInTurn.TaxRate;
        eiSlider.value = (float)GameManager.I.Game.PlayerInTurn.EconomicInvestmentRatio;
        tiSlider.value = (float)GameManager.I.Game.PlayerInTurn.ResearchInvestmentRatio;
        logiSlider.value = (float)GameManager.I.Game.PlayerInTurn.RepairInvestmentRatio;
        Text[] texts = InvestmentUI.GetComponentsInChildren<Text>();
        foreach (Text txt in texts)
        {
            switch (txt.name)
            {
                case "TRate":
                    taxRateText = txt;
                    break;
                case "PIRate":
                    eiRateText = txt;
                    break;
                case "TIRate":
                    tiRateText = txt;
                    break;
                case "LRate":
                    logiRateText = txt;
                    break;
                case "Current PIRate":
                    txt.text = "100%";
                    break;
                case "Current TIRate":
                    txt.text = "100%";
                    break;
                case "Current LRate":
                    txt.text = "50%";
                    break;
            }
        }
    }

    public void ChangeTaxValue(float adden)
    {
        taxSlider.value += adden;
    }
    public void ChangeEIValue(float adden)
    {
        eiSlider.value += adden;
    }
    public void ChangeTIValue(float adden)
    {
        tiSlider.value += adden;
    }
    public void ChangeLogiValue(float adden)
    {
        logiSlider.value += adden;
    }
}