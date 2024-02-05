using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class EnemyEntityScript : MonoBehaviour
{



    new Transform transform;
    Rigidbody2D rb;
    [HideInInspector] public NavMeshAgent navAgent;
    [HideInInspector] new public Collider2D collider;
    [HideInInspector] public SpriteRenderer sprite;
    [HideInInspector] public Animator anim;










    public enum GhostAIStyle { Shadow, Bashful, Speedy, Pokey};
    public GhostAIStyle style;

    public float speed;

    public float vulnerabilityTime;
    Coroutine vulCoroutine;

    public float generatingTimeStart;
    public Vector2 generatingTimeRange;

    public PlayerCharacterScript player;
        
        
    public Vector2 homeBaseTarget;
    public Vector2 scatterTarget;
    public Vector2 vulnerableTarget;
    public float vulTimeSwitcher;

    public float hotPursuitRadius;
    public float pokeyScareRadius;
    public Transform bashfulFollowee;

    //[SerializeField] string seeStateName;
    //[SerializeField] string seeSubStateName;
    public LayerMask playerAndCollMask;

    public Sprite[] ghostSprites = new Sprite[4];



    void Awake()
    {
        transform = base.transform;
        rb = GetComponent<Rigidbody2D>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;
        navAgent.speed = speed;
        collider = GetComponent<Collider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        CreateStateObjects();
        Begin();
    }

    void CreateStateObjects()
    {
        stateGenerating = new GhostState_Generating(this);
        stateVulnerable = new GhostState_Vulnerable(this);
        stateFleeing = new GhostState_Fleeing(this);
        stateActive = new GhostState_Active(this);
        stateActive.style = style;
    }

    public void Begin()
    {
        stateGenerating.EnterState();
        stateGenerating.generationTimeLeft = generatingTimeStart;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        PlayerCharacterScript player = collision.GetComponent<PlayerCharacterScript>();
        if(player != null)
        {
            if (currentStateObject == stateActive) LevelManagerScript.instance.Death();
            else if (currentStateObject == stateVulnerable) stateFleeing.EnterState();

        }
    }


    private void Update()
    {
        if (isPaused) return; 
        if (currentStateObject == null)
        {
            CreateStateObjects();
            stateGenerating.EnterState();
        }
        currentStateObject.StateUpate();

        //seeStateName = currentStateObject.ToString();
        //seeSubStateName = stateActive.subState.ToString();
        UpdateSprite();
    }





    #region States

    #region StateDefinitions

    public enum GhostState {Generating, Active, Vulnerable, Fleeing}
    public abstract class GhostStateBase
    {
        public EnemyEntityScript owner;

        public GhostStateBase(EnemyEntityScript owner) => this.owner = owner;


        public void EnterState()
        {
            owner.currentStateObject = this;
            OnEnterState();
        }

        protected abstract void OnEnterState();

        public abstract void StateUpate();
    }

    GhostState_Generating stateGenerating;
    public class GhostState_Generating : GhostStateBase
    {
        public GhostState_Generating(EnemyEntityScript owner) : base(owner) => this.owner = owner;

        public float generationTimeLeft;
        float randomOffset;


        protected override void OnEnterState()
        {
            generationTimeLeft = Random.Range(owner.generatingTimeRange.x, owner.generatingTimeRange.y);
            owner.sprite.color = Color.white;
            randomOffset = Random.Range(10f, -10f);
            owner.collider.enabled = true;
            owner.navAgent.enabled = false;
            owner.ChangeBlueState(0);
        }

        public override void StateUpate()
        {

            if (generationTimeLeft > 0)
            {
                generationTimeLeft -= Time.deltaTime;
                owner.transform.position = owner.homeBaseTarget + Vector2.right * TriangleWave(generationTimeLeft + randomOffset, 3);
            }
            else owner.stateActive.EnterState();
        }

        //Courtesy of Wikipedia.
        float TriangleWave(float x, float p)
        {
            float f1 = Mathf.Floor((x / p) + 0.5f);
            float f2 = (x / p) - f1;
            float f3 = Mathf.Abs(2 * f2);
            float f4 = 2 * f3 - 1;
            return f4;

        }

    }

    GhostState_Active stateActive;
    public class GhostState_Active : GhostStateBase
    {
        public GhostState_Active(EnemyEntityScript owner) : base(owner) => this.owner = owner;

        protected override void OnEnterState()
        {
            owner.sprite.color = Color.white;
            owner.navAgent.enabled = true;
            style = owner.style;
            subState = GhostSubState.Scatter;
            owner.ChangeBlueState(0);
        }

        public override void StateUpate()
        {
            playerPos = owner.player.transform.position;
            enemyPos = owner.transform.position;
            distance = Vector2.Distance(playerPos, enemyPos);

            if (distance <= owner.hotPursuitRadius && subState != GhostSubState.HotPursuit)
            {
                backupSubState = subState;
                subState = GhostSubState.HotPursuit;
                HotUpdate();
                return;
            }
            else if (distance > owner.hotPursuitRadius && subState == GhostSubState.HotPursuit && backupSubState != GhostSubState.HotPursuit)
            {
                subState = backupSubState;
            }

            switch (subState)
            {
                case GhostSubState.Scatter:     ScatterUpdate();    break;
                case GhostSubState.Chase:       ChaseUpdate();      break;
                case GhostSubState.HotPursuit:  HotUpdate();        break;
                default:                        ScatterUpdate();    break;
            }
        }

        public enum GhostSubState { Scatter, Chase, HotPursuit }
        public GhostSubState subState = GhostSubState.Scatter;
        GhostSubState backupSubState = GhostSubState.HotPursuit;
        public GhostAIStyle style;
        Vector2 playerPos;
        Vector2 enemyPos;
        float distance;
        bool isScattering;

        void ScatterUpdate()
        {
            if (!isScattering)
            {
                owner.navAgent.destination = owner.scatterTarget;
                isScattering = true;
            }
        }
        void ChaseUpdate()
        {
            isScattering = false;
            switch (style)
            {
                case GhostAIStyle.Shadow:
                default:
                    {
                        owner.navAgent.destination = owner.player.transform.position;
                    }
                    break;
                case GhostAIStyle.Bashful:
                    {
                        Vector2 pacmanPos = owner.player.transform.position;
                        Vector2 leaderPos = owner.bashfulFollowee.position;
                        owner.navAgent.destination = ((pacmanPos - leaderPos) * 2) + leaderPos;
                    }
                    break;
                case GhostAIStyle.Speedy:
                    {
                        owner.navAgent.destination = (Vector2)owner.player.transform.position + (owner.player.currentDirection * 3);
                    }
                    break;
                case GhostAIStyle.Pokey:
                    {
                        owner.navAgent.destination = playerPos;
                        if (distance > owner.pokeyScareRadius || owner.player.currentDirection == Vector2.zero) return;
                        Vector2 direction = (playerPos - enemyPos).normalized;
                        RaycastHit2D hit = Physics2D.Raycast(enemyPos, direction, distance, owner.playerAndCollMask);
                        if (hit.transform != owner.player.transform) return;
                        float angle = Vector2.Angle(-direction, owner.player.currentDirection);
                        if (angle < 60)
                        {
                            Debug.Log("Clyde: AAAAAAAAAAAAAAAAAAA");
                            subState = GhostSubState.Scatter;
                        }
                    }
                    break;
            }
        }
        void HotUpdate()
        {
            isScattering = false;
            owner.navAgent.destination = owner.player.transform.position;
        }

    }

    GhostState_Vulnerable stateVulnerable;
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
            //owner.sprite.color = Color.blue;
            owner.ChangeBlueState(1);
        }

        public override void StateUpate()
        {
            if (vulnerableTimeLeft > 0) vulnerableTimeLeft -= Time.deltaTime;
            else owner.stateActive.EnterState();

            if (vulnerableTimeLeft < owner.vulnerabilityTime * .3f) owner.ChangeBlueState(2);

            if (switchTimeLeft > 0) switchTimeLeft -= Time.deltaTime;
            else
            {
                Vector2 switching = switchVerticle ? -Vector2.up : -Vector2.right;
                owner.vulnerableTarget *= switching;
                owner.navAgent.destination = owner.vulnerableTarget;
            }
        }
    }

    GhostState_Fleeing stateFleeing;
    public class GhostState_Fleeing : GhostStateBase
    {
        public GhostState_Fleeing(EnemyEntityScript owner) : base(owner) => this.owner = owner;

        protected override void OnEnterState()
        {
            owner.navAgent.destination = owner.homeBaseTarget;
            owner.collider.enabled = false;
            //owner.sprite.color = Color.white * 0.5f;
            owner.ChangeBlueState(3);
        }

        public override void StateUpate()
        {
            if ((Vector2)owner.transform.position == owner.homeBaseTarget) owner.stateGenerating.EnterState();
        }
    }

    #endregion StateDefinitions

    #region StateChangers

    GhostStateBase currentStateObject;
    /*
    GhostState currentState
    {
        get
        {
            return currentStateObject switch
            {
                GhostStateBase o when o == stateGenerating => GhostState.Generating,
                GhostStateBase o when o == stateActive     => GhostState.Active,
                GhostStateBase o when o == stateVulnerable => GhostState.Vulnerable,
                GhostStateBase o when o == stateFleeing    => GhostState.Fleeing,
                _ => GhostState.Generating
            };
        }
    }
    public void SetState(GhostState state)
    {
        GhostStateBase result =
        state switch
        {
            GhostState.Generating => stateGenerating,
            GhostState.Active => stateActive,
            GhostState.Vulnerable => stateVulnerable,
            GhostState.Fleeing => stateFleeing,
            _ => stateGenerating
        };
        result.EnterState();
    }
     */

    public void MakeVulnerable()
    {
        if (currentStateObject != stateActive) return;
        stateVulnerable.EnterState();
    }


    public void ScatterChaseSwitch(bool makeChase)
    {
        if (currentStateObject != stateActive) return;
        if (stateActive.subState == GhostState_Active.GhostSubState.HotPursuit) return;
        stateActive.subState = (makeChase) ? GhostState_Active.GhostSubState.Chase : GhostState_Active.GhostSubState.Scatter;
    }

    #endregion StateChangers

    #endregion States


    #region Pausing

    bool isPaused;
    Vector2 lastAgentVelocity;
    NavMeshPath lastAgentPath;
    public void SetPause(bool pause = true)
    {
        isPaused = pause;

        if (!navAgent.enabled) return;

        if (pause)
        {
            lastAgentVelocity = navAgent.velocity;
            //Debug.Log(lastAgentVelocity);
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

    #endregion Pausing




    void UpdateSprite()
    {
        Vector2 direction = (Vector2)navAgent.desiredVelocity.normalized;

        float up = direction.y;
        float down = -direction.y;
        float left = -direction.x;
        float right = direction.x;

        float choose = down;
        if (down > choose) choose = up;
        if (left > choose) choose = left;
        if (right > choose) choose = right;

        if(choose == down) sprite.sprite = ghostSprites[0];
        else if (choose == up) sprite.sprite = ghostSprites[1];
        else if (choose == left) sprite.sprite = ghostSprites[2];
        else if (choose == right) sprite.sprite = ghostSprites[3];
        
    }

    void ChangeBlueState(int phase)
    {
        // 0 Is non Blue
        // 1 is Blue
        // 2 is Flashing Blue
        // 3 is Dead

        anim.Play("Blue." + animNames[phase]);
    }

    string[] animNames = new string[4] { "Default", "Blue", "Flashing", "Dead" } ;
}
