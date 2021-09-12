using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveBullet : MonoBehaviour
{
    public Vector3 target;
    public float firingAngle = 45.0f;
    public float gravity = 9.8f;

    public Transform projectile;
    public Transform myTransform;

    private void Awake()
    {
        projectile = transform;
        myTransform = transform;
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (Vector3.Distance(gameObject.transform.position, target) < 0.05f)
            gameObject.SetActive(false);
    }

    public IEnumerator ParabolaShoot()
    {
        projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);

        float targetDistance = Vector3.Distance(projectile.position, target);

        float projectileVelocity = targetDistance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        float Vx = Mathf.Sqrt(projectileVelocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectileVelocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        float flightDuration = targetDistance / Vx;

        projectile.rotation = Quaternion.LookRotation(target - projectile.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            gameObject.SetActive(false);
    }
}
