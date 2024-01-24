using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntityScript : MonoBehaviour
{



    new Transform transform;
    Rigidbody2D rb;


    public enum EnemyState { Generating, Active, Vulnerable, Fleeing };
    public EnemyState enemyState;

    public float vulnerabilityTime;
    Coroutine vulCoroutine;


    void Awake()
    {
        transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }






    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCharacterScript player = collision.GetComponent<PlayerCharacterScript>();
        if(player != null)
        {
            if (enemyState == EnemyState.Active) player.Kill();
            else if (enemyState == EnemyState.Vulnerable) Chomp();

        }
    }

    public void MakeVulnerable()
    {
        enemyState = EnemyState.Vulnerable;
        vulCoroutine = new Coroutine(VulnerableCoroutine(vulnerabilityTime), this);
    }


    IEnumerator VulnerableCoroutine(float time)
    {
        yield return WaitFor.Seconds(time);
        yield return null;
        enemyState = EnemyState.Active;
    }


    private void Chomp()
    {
        vulCoroutine.StopAuto();
        vulCoroutine = null;
    }

}
