﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI를 작동/관리하는 매니저 클래스
/// </summary>

public class UIManager : MonoBehaviour
{

    #region Instance
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }
    #endregion

    [SerializeField]
    private List<UIBase> uiList = new List<UIBase>();

    [SerializeField]
    private Stack<UIBase> uiStack = new Stack<UIBase>();

    [Tooltip("가장 최근에 접근 시도했던 UIBase")]
    private UIBase latelyUI;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;

            // DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Debug.Log("이미 instance가 존재합니다." + this);
        }
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTop();
        }
    }

    public void OpenThis(UIBase _uiBase)
    {


        if (_uiBase.isOpen)
        {
            Debug.Log("이미 열려있어!");
            return;
        }

        latelyUI = _uiBase;

        if (latelyUI.Open()) //성공적으로 열렸으면
        {
            if (uiStack.Count != 0)
            {
                uiStack.Peek().Com.canvasGroup.interactable = false;
            }

            uiStack.Push(latelyUI);

            latelyUI.Com.canvasGroup.interactable = true;
            latelyUI.Com.canvas.sortingOrder = uiStack.Count;
        }
    }

    /// <summary>
    /// Stack 맨 위에 있는 UI를 닫습니다.
    /// </summary>
    public void CloseTop()
    {
        if (uiStack.Count == 0)
        {
            Debug.Log("닫을 수 있는 UI가 없어!");
            return;
        }

        latelyUI = uiStack.Peek();

        if (latelyUI.Close()) //성공적으로 닫혔으면
        {
            uiStack.Pop();

            if (uiStack.Count != 0)
            {
                uiStack.Peek().Com.canvasGroup.interactable = true;
            }
        }

    }


    /// <summary>
    /// uiList에 해당 uiBase를 Add합니다.
    /// </summary>
    public void RegisterThis(UIBase _uiBase)
    {
        uiList.Add(_uiBase);
        Debug.Log("uiList에 " + _uiBase.name + " 추가!");
    }
}
