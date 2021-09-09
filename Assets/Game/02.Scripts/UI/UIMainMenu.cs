﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : UIBase
{
    private void Start()
    {
        Init();
        Open();
    }
    public void TestFunc()
    {
        if (SceneChanger.Instance == null)
        {
            Debug.LogWarning("SceneChanger is Null");
        }

        StartCoroutine(SceneChanger.Instance.LoadThisScene_Joke(SceneNames.mainMenu));
    }

    public override void Init()
    {
        base.Init();
        RegisterUIManager();
    }

    protected override void CheckOpen()
    {
        base.CheckOpen();
    }

    public override bool Open()
    {
        StartCoroutine(ProcessOpen());
        return true;
    }

    public override bool Close()
    {
        StartCoroutine(ProcessClose());
        return true;

    }

    public void Button_StartNewGame(UIPopup _uiBase)
    {
        UIManager.Instance.OpenThis(_uiBase);
    }
    public void StartNewGame()
    {
        //상호작용 불가
        Com.canvasGroup.interactable = false;

        DataManager.Instance.currentData_player = new Data_Player();
        StartCoroutine(DataManager.Instance.SaveCurrentData(DataManager.fileName_settings));
        SceneChanger.Instance.LoadThisScene(SceneNames.fieldMap);
    }

    public void Button_ContinueGame(UIPopup _uiBase)
    {
        if (DataManager.Instance.isCreatedNewPlayerData) //데이터가 없었던 상태라면
        {
            UIManager.Instance.OpenThis(_uiBase);
        }
        else
        {
            //상호작용 불가
            Com.canvasGroup.interactable = false;

            SceneChanger.Instance.LoadThisScene(SceneNames.fieldMap);
        }

    }

    public void Button_Continue_OK()
    {
        Com.canvasGroup.interactable = false;
        SceneChanger.Instance.LoadThisScene(SceneNames.fieldMap);
    }

    public void Button_Continue_Close()
    {
        UIManager.Instance.CloseTop();
    }
    public void Button_OpenSettings(UIBase _uiBase)
    {
        UIManager.Instance.OpenThis(_uiBase);
    }

    public void Button_QuitGame(UIPopup _uiBase)
    {
        UIManager.Instance.OpenThis(_uiBase);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    public override void RegisterUIManager()
    {
        UIManager.Instance.RegisterDictThis(this.GetType().Name, this);
        UIManager.Instance.RegisterListThis(this);

    }
}
