using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargeAttac_NavMesh : Action

{
    [SerializeField]
    GameObject StopPosObj;
    AttackArea Range;
    NavMeshPath path;
    Vector3 targetRot;
    Animator anim;
   
    //突進の時間
    [SerializeField]
    float MoveTime = 3;
    //待機時間
    [SerializeField]
    float WaitTime = 2;
    //移動速度
    [SerializeField]
    float speed = 50;
    [SerializeField]
    float BrakePosDistance = 2;


    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        //CapsuleColl = GetComponent<CapsuleCollider>();
        //EneCon = GetComponent<EnemyController>();
        Range = attackArea.GetComponent<AttackArea>();
        Range.Damage = Damage;
        agent.speed = speed;
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
            //Debug.Log("Start");
            //targetに対しての正面方向をむく
            Turn(target, self);
        }
        //targetに対しての正面方向をむいたら回転をやめChargeActionのコルーチンを開始する
        if (Mathf.Abs(Mathf.Abs(transform.eulerAngles.y) - Mathf.Abs(targetRot.y)) < 0.5 && GetSearchAction())
        {
            SetSearchAction(false);
            StartCoroutine(ChargeAction(target));
            // NavMeshAgent に目的地を設定する
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
    public IEnumerator ChargeAction(GameObject target)
    {
        Vector3 TargetPos = target.transform.position;
        TargetPos.y = transform.position.y;
        Range.CharacterOnTouch = false;
        yield return StartCoroutine(SideMoveCheck(target));

        attackArea.SetActive(true);

        if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            agent.SetDestination(TargetPos);
            // 経路取得用のインスタンス作成
            path = new NavMeshPath();
            // 明示的な経路計算実行
            agent.CalculatePath(TargetPos, path);
            StopPosObj.transform.position = path.corners[path.corners.Length - 1];
            StopPosObj.transform.SetParent(null);
        }
        agent.Resume();
        float AttackTime = 0;
        anim.SetBool("Attack", true);
        StopPosObj.transform.position = path.corners[path.corners.Length - 1] + (transform.forward * BrakePosDistance);
        for (;;)
        {
            AttackTime += Time.deltaTime;
            //Playerのタグを持つものに当たるとWaitTimeの間、待機してfor文を抜ける
            if (Range.CharacterOnTouch)
            {
                agent.SetDestination(TargetPos);
                attackArea.SetActive(false);
                agent.autoBraking = true;
                agent.Stop();
                yield return null;
                break;
            }
            if (MoveTime <= AttackTime)
            {
                agent.Stop();
                yield return null;
                break;
            }
            yield return null;
        }

        agent.autoBraking = false;
        if (Range.TouchChar == Tags.Player)
        {
            agent.velocity *= 0;
        }
        Range.TouchChar = null;
        Range.CharacterOnTouch = false;
        yield return new WaitForSeconds(WaitTime);
        anim.SetBool("Attack", false);
        SetSearchAction(true);
    }
    IEnumerator SideMoveCheck(GameObject Target)
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 1, transform.forward, out hit))
        {
            if (hit.collider.tag == Tags.Enemy)
            {
                StartCoroutine(SideMove(Target));
                yield break;
            }
        }
    }

    IEnumerator SideMove(GameObject Target)
    {
        float time = 1;
        float movetime = 0;
        float Rnd = Random.value;
        float move = 5 * Time.deltaTime;

        while (time >= movetime)
        {
            agent.velocity /= 2;
            movetime += Time.deltaTime;
            Turn(Target, gameObject);
            if (Rnd <= 0.5)
            {
                agent.Move(transform.right * move);
            }
            else
            {
                agent.Move(-transform.right * move);
            }
            yield return null;
        }
        StopCoroutine("ChargeAction");
        agent.Stop();
        agent.velocity *= 0;
        StartCoroutine(ChargeAction(Target));
        yield break;
    }
    private void OnTriggerExit(Collider other)
    {
        if (StopPosObj == other.gameObject)
        {
            agent.Stop();
        }
    }
    private void OnDestroy()
    {
        Destroy(StopPosObj);
    }
}
