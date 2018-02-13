using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JumpAttack_NavMesh : Action
{

    [SerializeField]
    GameObject StopPosObj;
    AttackArea Range;
    Animator anim;
    NavMeshPath path;
    Vector3 targetRot;
    [SerializeField]
    int AttackRange = 125;
    [SerializeField]
    float AttackWaitTime = 1;     //攻撃までの待機時間
    [SerializeField]
    float BackWaitTime = 1;       //攻撃までの待機時間
    [SerializeField]
    float WalkSpeed = 30;         //接近時の移動速度
    [SerializeField]
    float AttackSpeed = 120;
    public bool StopFlag = false;
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        Range = attackArea.GetComponent<AttackArea>();
        Range.Damage = Damage;
        attackArea.SetActive(false);
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
        if (GetSearchAction())
        {
            if (agent.velocity == Vector3.zero)
            { anim.SetBool("Walk", false); }

            float TargetDistance = 0;
            //自身とtargetの距離の二乗をTargetDistanceに取る
            TargetDistance = (target.transform.position - self.transform.position).sqrMagnitude;
            //targetに対しての正面を向く
            Turn(target, self);
            //TargetDistanceがAttackRangeの二乗より大きければ近づく
            if (TargetDistance >= (AttackRange ^ 2))
            {
                anim.SetBool("Walk", true);
                agent.SetDestination(target.transform.position);
                // 経路取得用のインスタンス作成
                path = new NavMeshPath();
                // 明示的な経路計算実行
                agent.CalculatePath(target.transform.position, path);
                agent.Resume();
            }
            //targetに対しての正面を向いていればJumpActionのコルーチンを開始する
            else if (Mathf.Abs(Mathf.Abs(transform.eulerAngles.y) - Mathf.Abs(targetRot.y)) < 0.3 && GetSearchAction())
            {
                anim.SetBool("Walk", false);
                SetSearchAction(false);
                Range.CharacterOnTouch = false;
                Vector3 StartPos = transform.position;
                Vector3 EndPos = target.transform.position;
                EndPos.y = transform.position.y;
                agent.speed = AttackSpeed;
                agent.Stop();
                agent.velocity *= 0;
                StartCoroutine(JumpAction(StartPos, EndPos));
            }
        }
    }
    void Turn(GameObject target, GameObject self)
    {
        Vector3 targetPos = target.transform.position;
        targetPos.y = self.transform.position.y;
        //targetに対しての正面方向を取得する
        Quaternion TargetRotation = Quaternion.LookRotation(targetPos - self.transform.position);
        //targetに対して正面になるように徐々に回転させる
        self.transform.rotation = Quaternion.Slerp(self.transform.rotation, TargetRotation, Time.deltaTime * RotationSpeed);
        targetRot = TargetRotation.eulerAngles;
    }
    void PosSet(GameObject target)
    {
       
    }
    public IEnumerator JumpAction(Vector3 StartPos,Vector3 EndPos)
    {
        NowCoroutine = JumpAction(StartPos, EndPos);
        if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            agent.SetDestination(EndPos);
            StopPosObj.transform.position = EndPos;
            StopPosObj.transform.SetParent(null);
        }
        yield return new WaitForSeconds(AttackWaitTime);
        anim.SetBool("Attack", true);
        attackArea.SetActive(true);
        agent.Resume();
        StopFlag = false;
        while (!StopFlag)
        {
            if (Range.CharacterOnTouch)
            {
                agent.SetDestination(EndPos);
                attackArea.SetActive(false);
                StopFlag = true;
            }
            if (transform.position == EndPos)
            { StopFlag = true; }
            yield return null;

        }
        StopFlag = false;
        agent.Stop();
        agent.velocity *= 0;
        Debug.Log(132);
        StartCoroutine(Backmove(StartPos));

    }
    public IEnumerator Backmove(Vector3 StartPos)
    {
        NowCoroutine = Backmove(StartPos);
        if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            agent.SetDestination(StartPos);
            StopPosObj.transform.position = StartPos;
        }
        yield return new WaitForSeconds(BackWaitTime);
        attackArea.SetActive(false);
        anim.SetBool("Return", true);
        agent.Resume();
        StopFlag = false;
        agent.angularSpeed = 2000;
        while (!StopFlag)
        {
            if (Range.CharacterOnTouch)
            {
                agent.SetDestination(StartPos);
                attackArea.SetActive(false);
                StopFlag = true;
            }
            if (transform.position == StartPos)
            {
                StopFlag = true;
            }
            yield return null;
        }
        agent.angularSpeed = 0;
        anim.SetBool("Return", false);
        agent.Stop();
        agent.velocity *= 0;
        if (Range.TouchChar == Tags.Player)
        {
            agent.velocity *= 0;
        }
        Range.TouchChar = null;
        Range.CharacterOnTouch = false;
        agent.ResetPath();
        anim.SetBool("Attack", false);
        agent.speed = WalkSpeed;
        SetSearchAction(true);
        SetCoroutineReset();
        yield break;
    }
    private void OnTriggerExit(Collider other)
    {
        if (StopPosObj == other.gameObject)
        {
            StopFlag = true;
        }
    }
    private void OnDestroy()
    {
        Destroy(StopPosObj);
    }
}
//void OnDrawGizmos()
//{
//    RaycastHit hit;
//    var scale = transform.lossyScale.x * 0.5f;
//    var isHit = Physics.BoxCast(transform.position, Vector3.one * 2, transform.forward, out hit, transform.rotation);
//    if (isHit)
//    {
//        Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
//        Gizmos.DrawWireSphere(transform.position + transform.forward * hit.distance, 1);
//    }
//    else
//    {
//        Gizmos.DrawRay(transform.position, transform.forward * 100);
//    }
//}