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
    public NavMeshAgent navAgent;
    public Collider2D coll;


    //public enum EnemyState { Generating, Active, Vulnerable, Fleeing };
    //public EnemyState currentState;
    //public enum EnemySubState { Scattering, Chasing, CloseChasing };
    //public EnemySubState currentSubState;



    public GhostStateBase currentState;

    public GhostState_Generating stateGenerating;
    public GhostState_Active stateActive;
    public GhostState_Vulnerable stateVulnerable;
    public GhostState_Fleeing stateFleeing;








    public enum GhostAIStyle { Shadow, Bashful, Speedy, Pokey};
    public GhostAIStyle style;

    public float speed;

    public float vulnerabilityTime;
    Coroutine vulCoroutine;

    public float generatingTime;

    public GameObject player;

    public Vector2 homeBaseTarget;
    public Vector2 scatterTarget;
    public Vector2 vulnerableTarget;
    public float vulTimeSwitcher;


    void Awake()
    {
        transform = base.transform;
        rb = GetComponent<Rigidbody2D>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;
        navAgent.speed = speed;
        CreateStateObjects();
        stateGenerating.EnterState();
    }

    void CreateStateObjects()
    {
        stateGenerating = new GhostState_Generating(this);
        stateActive = new GhostState_Active(this);
        stateVulnerable = new GhostState_Vulnerable(this);
        stateFleeing = new GhostState_Fleeing(this);
    }





    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        PlayerCharacterScript player = collision.GetComponent<PlayerCharacterScript>();
        if(player != null)
        {
            if (currentState == stateActive) LevelManagerScript.instance.Death();
            else if (currentState == stateVulnerable)
            {
                vulCoroutine.StopAuto();
                vulCoroutine = null;
                stateFleeing.EnterState();
            }

        }
    }

    public void MakeVulnerable()
    {
        if (currentState != stateActive) return;
        stateVulnerable.EnterState();
    }


    IEnumerator VulnerableCoroutine(float time)
    {
        yield return WaitFor.Seconds(time);
        yield return null;
        stateActive.EnterState();
        vulCoroutine = null;
    }




    private void Update()
    {
        if (currentState == null)
        {
            CreateStateObjects();
            stateGenerating.EnterState();
        }
        currentState.StateUpate();
    }
    







    public abstract class GhostStateBase
    {
        public EnemyEntityScript owner;

        public GhostStateBase(EnemyEntityScript owner) => this.owner = owner;


        public void EnterState()
        {

            OnEnterState();
        }

        protected abstract void OnEnterState();

        public abstract void StateUpate();
    }

    public class GhostState_Generating : GhostStateBase
    {
        public GhostState_Generating(EnemyEntityScript owner) : base(owner) => this.owner = owner;

        float generationTimeLeft;



        protected override void OnEnterState()
        {
            generationTimeLeft = owner.generatingTime;
        }

        public override void StateUpate()
        {
            if (generationTimeLeft > 0)
            {
                owner.transform.position = Vector3.zero + Vector3.right * Mathf.Lerp(-1, 1, Mathf.Sin(Time.time));
                generationTimeLeft -= Time.deltaTime;
            }
            else owner.stateActive.EnterState();
        }
    }

    public class GhostState_Active : GhostStateBase
    {
        public GhostState_Active(EnemyEntityScript owner) : base(owner) => this.owner = owner;

        protected override void OnEnterState()
        {

        }

        public override void StateUpate()
        {
            owner.navAgent.destination = owner.player.transform.position;
        }
    }

    public class GhostState_Vulnerable : GhostStateBase
    {
        public GhostState_Vulnerable(EnemyEntityScript owner) : base(owner) => this.owner = owner;

        float vulnerableTimeLeft;
        float switchTimeLeft;
        bool switchVerticle;

        protected override void OnEnterState()
        {
            owner.navAgent.destination = owner.vulnerableTarget;
            vulnerableTimeLeft = owner.vulnerabilityTime;
            switchTimeLeft = owner.vulTimeSwitcher * 3;
        }

        public override void StateUpate()
        {
            if (vulnerableTimeLeft > 0) vulnerableTimeLeft -= Time.deltaTime;
            else owner.stateActive.EnterState();

            if (switchTimeLeft > 0) switchTimeLeft -= Time.deltaTime;
            else
            {
                Vector2 switching = switchVerticle ? -Vector2.up : -Vector2.right;
                owner.vulnerableTarget *= switching;
                owner.navAgent.destination = owner.vulnerableTarget;
            }
        }
    }

    public class GhostState_Fleeing : GhostStateBase
    {
        public GhostState_Fleeing(EnemyEntityScript owner) : base(owner) => this.owner = owner;

        protected override void OnEnterState()
        {
            owner.navAgent.destination = owner.homeBaseTarget;
            owner.coll.enabled = false;
        }

        public override void StateUpate()
        {
            if ((Vector2)owner.transform.position == owner.homeBaseTarget) owner.stateGenerating.EnterState();
        }
    }






    bool isPaused;
    Vector2 lastAgentVelocity;
    NavMeshPath lastAgentPath;
    public void SetPause(bool pause = true)
    {
        isPaused = pause;

        if (pause)
        {
            lastAgentVelocity = navAgent.velocity;
            Debug.Log(lastAgentVelocity);
            lastAgentPath = navAgent.path;
            navAgent.velocity = Vector3.zero;
            navAgent.ResetPath();
        }
        else
        {
            navAgent.velocity = lastAgentVelocity;
            navAgent.SetPath(lastAgentPath);
        }
        navAgent.isStopped = pause;

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
