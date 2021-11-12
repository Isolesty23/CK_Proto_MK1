﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : UIBase
{

    public UIMovieScreen movieScreen;

    private void Start()
    {
        Open();
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
    public void Button_StartNewGame()
    {
        UIManager.Instance.OpenPopup(eUIText.StartNewGame,
            StartNewGame, UIManager.Instance.CloseTop);
    }
    private void StartNewGame()
    {
        //상호작용 불가
        Com.canvasGroup.interactable = false;

        Debug.Log("Start New Game...");
        //DataManager.Instance.currentData_player = new Data_Player();
        //StartCoroutine(DataManager.Instance.SaveCurrentData(DataManager.DataName.settings));
        //SceneChanger.Instance.LoadThisScene(SceneNames.fieldMap);
        movieScreen.OnMovieEnded += OnMovieEnded;
        StartCoroutine(movieScreen.playingCoroutine);
    }

    private void OnMovieEnded()
    {
        movieScreen.OnMovieEnded -= OnMovieEnded;
        DataManager.Instance.currentData_player = new Data_Player();
        StartCoroutine(DataManager.Instance.SaveCurrentData(DataName.settings));
        SceneChanger.Instance.LoadThisScene(SceneNames.fieldMap);
    }

    private IEnumerator ProcessStartNewGame()
    {
        //Com.canvasGroup.interactable = false;

        //DataManager.Instance.currentData_player = new Data_Player();
        //yield return StartCoroutine(DataManager.Instance.SaveCurrentData(DataManager.DataName.settings));
        //StartCoroutine(movieScreen.playingCoroutine);
        //SceneChanger.Instance.LoadThisScene(SceneNames.fieldMap);

        yield break;
    }
    public void Button_ContinueGame()
    {
        if (DataManager.Instance.isCreatedNewPlayerData) //데이터가 없었던 상태라면
        {
            UIManager.Instance.OpenPopup(eUIText.NoPlayerData,
                Button_Continue_OK,
                Button_Continue_Close);
        }
        else
        {
            //상호작용 불가
            Com.canvasGroup.interactable = false;

            SceneChanger.Instance.LoadThisScene(SceneNames.fieldMap);
        }

    }

    private void Button_Continue_OK()
    {
        Com.canvasGroup.interactable = false;
        SceneChanger.Instance.LoadThisScene(SceneNames.fieldMap);
    }

    private void Button_Continue_Close()
    {
        UIManager.Instance.CloseTop();
    }
    public void Button_OpenSettings()
    {
        UIManager.Instance.OpenThis(UIName.UIOption_Main);
    }

    public void Button_QuitGame()
    {
        UIManager.Instance.OpenQuitPopup();
    }
}
