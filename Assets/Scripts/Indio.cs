using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indio : MonoBehaviour
{

    private Transform target;

    [Header("Attributes")]

    public float range = 15f;
    public float fireRate = 1f; //one shoot per second
    private float fireCountdown = 0f;

    [Header("Unity Setup Fields")]

    public string enemyTag = "Enemy";

    public Transform partToRotate;
    public float turnSpeed = 10f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public Animator anim;
    public float animSpeed;
    public float animDelay;
    public GameObject lance;
    public AudioSource throwBulletAudio;

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        anim.speed = animSpeed;
    }

    void UpdateTarget()
    {
        GameObject[] enimies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enimies)
        {
            float distanceEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceEnemy < shortestDistance)
            {
                shortestDistance = distanceEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            fireCountdown -= Time.deltaTime;
            if (fireCountdown < 0)
                fireCountdown = 0;
            return;
        }

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = lookRotation.eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if (fireCountdown <= 0f)
        {
            AnimAttackStart();
            Invoke("Shoot", animDelay);
            //Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
        
    }

    void Shoot()
    {
        throwBulletAudio.Play();
        lance.gameObject.SetActive(false);
        Invoke("ShowLance", 0.4f);
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(target);
        }
    }
    
    void AnimAttackStart()
    {
        anim.Play("attack");
        Invoke("AnimAttackStop", 0.1f);
    }

    void AnimAttackStop()
    {
        anim.SetBool("attack", false);
    }

    void ShowLance()
    {
        lance.gameObject.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
