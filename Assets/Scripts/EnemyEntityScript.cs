using AStarSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class EnemyEntityScript : MonoBehaviour
{



    new Transform transform;
    Rigidbody2D rb;
    NavMeshAgent agent;


    public enum EnemyState { Generating, Active, Vulnerable, Fleeing };
    public EnemyState enemyState;

    public float speed;

    public float vulnerabilityTime;
    Coroutine vulCoroutine;

    public GameObject player;


    void Awake()
    {
        transform = base.transform;
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
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



    private void Update()
    {
        agent.destination = player.transform.position;
    }











    // A* Attempt. (Honestly seems like the tutorial guy just doesn't know what he's doing.)
    //https://youtu.be/mZfyt03LDH4?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&t=779

    /*

    private void Update()
    {
        gridPos = grid.NodeFromWorldPoint(transform.position).worldPosition;
        if (chaseCheckTime >= 0) chaseCheckTime -= Time.deltaTime;
        else if (chaseCheckTime < 0)
        {
            chaseCheckTime = 5f;

            bool successfulPath = grid.FindPath(out currentPath, transform.position, player.transform.position);
            if (successfulPath)
            {

                //if (followingCo!=null) if(followingCo.running) followingCo.StopAuto();
                followingCo = new Coroutine(FollowPath(currentPath), this);
            }
        }

    }

    public AGrid grid;
    Vector2[] currentPath;
    int targetIndex;

    public bool testChase;
    public float chaseCheckTime = 5f;
    public Vector2 gridPos;

    Coroutine followingCo;
    IEnumerator FollowPath(Vector2[] path)
    {
        if (path.Length < 1) yield break;
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;

        }
    }

        public void OnDrawGizmos()
    {
        if (currentPath != null)
        {
            for (int i = targetIndex; i < currentPath.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(currentPath[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, currentPath[i]);
                }
                else
                {
                    Gizmos.DrawLine(currentPath[i - 1], currentPath[i]);
                }
            }
        }
    }


     */


}
