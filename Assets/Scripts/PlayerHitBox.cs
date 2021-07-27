using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ParryingBullet"))
        {
            Debug.Log("CanParrying");
            Destroy(other.gameObject);
            StartCoroutine(GameManager.instance.playerController.Parrying());
        }
    }
}
