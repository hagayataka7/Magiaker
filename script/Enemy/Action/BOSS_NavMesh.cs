using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BOSS_NavMesh : Action
{
    Vector3 targetRot;
    Animator anim;
    [SerializeField]
    List<GameObject> PopPoint = new List<GameObject>();
    [SerializeField]
    GameObject PopEnemy;
    int Rnd;
    [SerializeField]
    int AttackRange = 10;
    [SerializeField]
    float WalkSpeed = 10;       //移動速度
    AttackArea Range;
    bool AttackPlay = false;
    float TargetDistance;
    AnimatorStateInfo nowState;
    [SerializeField]
    int AttackMove = 4;
    [SerializeField]
    int PopMove = 5;
    private void Start()
    {
        RandomSet();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        Range = attackArea.GetComponent<AttackArea>();
        Range.aligment = aligment.enemy;
        Range.element = null;
        Range.Damage = Damage;
        attackArea.SetActive(false);
        agent.speed = WalkSpeed;
    }
    private void Update()
    {
        nowState = anim.GetCurrentAnimatorStateInfo(0);
        if (FlagReset)
        {
            AttackPlay = false;
            SetSearchAction(true);
            attackArea.SetActive(false);
            FlagReset = false;
        }
    }
    public override void ActionEnter(GameObject target, GameObject self)
    {
        base.ActionEnter(target, self);
        if (GetSearchAction() && !AttackPlay)
        {
            //自身とtargetの距離の二乗をTargetDistanceに取る
            TargetDistance = (target.transform.position - self.transform.position).sqrMagnitude;
            //targetに対しての正面を向く
            agent.Resume();
            //TargetDistanceがAttackRangeの二乗より大きければ近づく
            if (TargetDistance >= (AttackRange ^ 2))
            {
                if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
                {
                    Vector3 TargetPos = target.transform.position;
                    agent.SetDestination(TargetPos);
                }
            }
            Turn(target, self);
            if (TargetDistance <= (AttackRange ^ 2))
            {
                if (Mathf.Abs(Mathf.Abs(transform.eulerAngles.y) - Mathf.Abs(targetRot.y)) < 1 && GetSearchAction())
                {
                    SetSearchAction(false);
                    agent.Stop();
                    agent.velocity *= 0;
                    AttackPlay = true;
                    if (Rnd < AttackMove)
                    {
                        StartCoroutine(AttackAction(target));
                    }
                    if (Rnd > 9 - PopMove)
                    {
                        StartCoroutine(PopAction());
                    }
                }
            }
        }
    }
    void RandomSet()
    { Rnd = Random.Range(0, 10); }
    void Turn(GameObject target, GameObject self)
    {
        //targetに対しての正面方向を取得する
        Quaternion TargetRotation = Quaternion.LookRotation(new Vector3(
           target.transform.position.x,
           self.transform.position.y,
           target.transform.position.z) - self.transform.position);
        //targetに対して正面になるように徐々に回転させる
        self.transform.rotation = Quaternion.Slerp(self.transform.rotation, TargetRotation, Time.deltaTime * RotationSpeed);
        targetRot = TargetRotation.eulerAngles;
    }
    
    void EnemyPop()
    {
        int Pointnum = PopPoint.Count - 1;
        for (int i = 0; i <= Pointnum; i++)
        {
            if (!PopPoint[i].GetComponent<PopPoint>().inArea)
            {
                GameObject Enemy = Instantiate(PopEnemy, PopPoint[i].transform.position, Quaternion.identity);
            }
        }
    }
    private IEnumerator PopAction()
    {
        NowCoroutine = PopAction();
        anim.SetBool("Recall", true);
        EnemyPop();
        yield return StartCoroutine(WaitAnimationEnd("Recall"));
        anim.SetBool("Recall", false);

        AttackPlay = false;
        SetSearchAction(true);
        attackArea.SetActive(false);
        RandomSet();
    }

    private IEnumerator AttackAction(GameObject target)
    {
        NowCoroutine = AttackAction(target);
        anim.SetBool("Attack", true);
        yield return StartCoroutine(WaitAnimationEnd("Attack"));
        anim.SetBool("Attack", false);

        AttackPlay = false;
        SetSearchAction(true);
        attackArea.SetActive(false);
        RandomSet();
    }

    private IEnumerator WaitAnimationEnd(string animatorName)
    {
        AnimatorStateInfo nowState = anim.GetCurrentAnimatorStateInfo(0);
        bool finish = false;
        if(animatorName=="Attack")
        {
            yield return new WaitForSeconds(nowState.length / 2);
            attackArea.SetActive(true);
        }

        while (!finish)
        {
            nowState = anim.GetCurrentAnimatorStateInfo(0);
            if (!nowState.IsName(animatorName))
            { anim.Play(animatorName); }
            if (nowState.IsName(animatorName) && nowState.normalizedTime >= 0.9f)
            { finish = true; }
            else
            { yield return new WaitForSeconds(0.1f); }
        }
    }
    private void OnEnable()
    {
        AttackPlay = false;
        SetSearchAction(true);
        attackArea.SetActive(false);
    }
}
