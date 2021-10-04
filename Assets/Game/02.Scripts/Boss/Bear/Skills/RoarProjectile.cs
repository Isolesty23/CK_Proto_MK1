﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarProjectile : BearProjectile
{
    public void Init(Vector3 _start, Vector3 _end)
    {
        startPos = _start;
        endPos = _end;

        SetParryMode(true);
        moveEnumerator = ProcessMove();
        playerController = GameManager.instance.playerController;
        parryEnumerator = playerController.Parrying();
    }

    public override void Move()
    {
        StartCoroutine(moveEnumerator);
    }


    protected override IEnumerator ProcessDespawn()
    {
        StopCoroutine(moveEnumerator);
        yield return null;
        CustomPoolManager.Instance.ReleaseThis(this);
    }

    protected override IEnumerator ProcessMove()
    {
        float progress = 0f;
        float timer = 0;
        while (progress < 1f)
        {
            timer += Time.deltaTime;

            progress = timer / moveTime;
            myTransform.position = Vector3.Lerp(startPos, endPos, progress);
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }

        myTransform.position = endPos;
        Despawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            OnTrigger();
            //Despawn만 불러도 됨
        }
    }

}
