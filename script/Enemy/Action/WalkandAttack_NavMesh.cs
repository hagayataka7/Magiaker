using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class WalkandAttack_NavMesh : Action
{
    Vector3 targetRot;
    Animator anim;

    [SerializeField]
    int AttackRange = 10;
    [SerializeField]
    float WalkSpeed = 10;       //移動速度
    AttackArea Range;
    bool asd = false;
    float TargetDistance;
    AnimatorStateInfo nowState;
    private void Start()
    {
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
    }
    public override void ActionEnter(GameObject target, GameObject self)
    {
        base.ActionEnter(target, self);
        if (FlagReset)
        {
            SetSearchAction(true);
            attackArea.SetActive(false);
            FlagReset = false;
        }
        if (GetSearchAction() && !asd)
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
                    asd = true;
                    StartCoroutine(AttackAction(target));
                }
            }
        }
    }
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
    private IEnumerator AttackAction(GameObject target)
    {
        anim.SetBool("Attack", true);


        yield return StartCoroutine(WaitAnimationEnd("Attack"));
        anim.SetBool("Attack", false);

        asd = false;
        SetSearchAction(true);
        attackArea.SetActive(false);
    }

    private IEnumerator WaitAnimationEnd(string animatorName)
    {
        AnimatorStateInfo nowState = anim.GetCurrentAnimatorStateInfo(0);
        bool finish = false;
        yield return new WaitForSeconds(nowState.length / 2);
        attackArea.SetActive(true);
        while (!finish)
        {
            nowState = anim.GetCurrentAnimatorStateInfo(0);
            if (nowState.IsName(animatorName) && nowState.normalizedTime >= 0.9f)
            { finish = true; }
            else
            { yield return new WaitForSeconds(0.1f); }
        }
    }
}
