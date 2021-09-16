using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pixy : MonoBehaviour
{
    public Transform pixyModel;

    public Vector3 firePos;
    public Vector3 crouchFirePos;
    public Vector3 pixyPos;
    public Vector3 pixyCounterPos;

    public float pixyMoveTime = 0.2f;
    public float counterRange = 100f;
    public float counterSpeed = 10f;

    private void Awake()
    {
        firePos = transform.localPosition;
        pixyModel.localPosition = pixyPos;
    }

    public void ReadyToCounter()
    {
        pixyModel.DOLocalMove(pixyCounterPos, pixyMoveTime).SetEase(Ease.Unset);
    }

    public IEnumerator Counter()
    {
        var counter = CustomPoolManager.Instance.counterPool.SpawnThis(transform.position, transform.eulerAngles, null);
        counter.isActive = true;
        Vector3 curPosition = transform.position;

        while (counter.isActive)
        {
            if ((curPosition - counter.transform.position).sqrMagnitude < counterRange)
            {
                counter.transform.Translate(Vector3.forward * counterSpeed * Time.fixedDeltaTime);

                yield return null;
            }
            else
            {
                counter.isActive = false;
                CustomPoolManager.Instance.ReleaseThis(counter);
            }
        }
    }

    public void EndCounter()
    {
        pixyModel.DOLocalMove(pixyPos, pixyMoveTime).SetEase(Ease.Unset);
    }
}
