﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타이틀 화면을 담당한다-!! 
/// </summary>
public class UITitleScreen : UIBase
{

    [Space(10)]
    [Tooltip("현재 load 중 뭘 하고 있는지 표시하는 텍스트")]
    public Text text_load;

    [Tooltip("아무 키나 누르세요 텍스트")]
    public GameObject text_pressKey;
    public override void Init()
    {
        base.Init();
    }

    public void SetText_Load(string _text)
    {
        text_load.text = _text;
    }
    public void SetText_pressKey(bool _isActive)
    {
        text_pressKey.gameObject.SetActive(_isActive);
    }

    public IEnumerator ProcessWaitPressAnyKey()
    {
        while (true)
        {
            //아무 키나 눌렀을 경우
            if (Input.anyKeyDown)
            {
                break;
            }

            yield return null;
        }

        yield break;
    }

    public override bool Open()
    {
        fadeDuration = 0.5f;
        StartCoroutine(ProcessOpen());
        return true;
    }
    public override bool Close()
    {
        fadeDuration = 1f;
        StartCoroutine(ProcessClose());
        return true;
    }

    //protected override IEnumerator ProcessOpen()
    //{
    //    return base.ProcessOpen();
    //}
    protected override IEnumerator ProcessClose()
    {
        return base.ProcessClose();
    }
}
