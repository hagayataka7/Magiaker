using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkandAttack : Action
{

    Vector3 targetRot;
    Animator anim;

    [SerializeField]
    int AttackRange = 10;
    [SerializeField]
    float MoveSpeed = 10;       //移動速度
    AttackArea Range;
    float TargetDistance;
    float radius = 2;

    private void Start()
    {
        anim = GetComponent<Animator>();
        Range = attackArea.GetComponent<AttackArea>();
        Range.aligment = aligment.enemy;
        Range.element = null;
        Range.Damage = Damage;
        attackArea.SetActive(false);
    }

    public override void ActionEnter(GameObject target, GameObject self)
    {
        base.ActionEnter(target, self);
        if (GetSearchAction())
        {
            AnimatorStateInfo nowState = anim.GetCurrentAnimatorStateInfo(0);
            //自身とtargetの距離の二乗をTargetDistanceに取る
            TargetDistance = (target.transform.position - self.transform.position).sqrMagnitude;
            //targetに対しての正面を向く
            Turn(target, self);

            //TargetDistanceがAttackRangeの二乗より大きければ近づく
            if (TargetDistance >= (AttackRange ^ 2))
            {
                if (nowState.IsName("Attack") || nowState.IsName("Wait"))
                { anim.Play("Walk"); }
                if (nowState.IsName("Walk"))
                {
                    Range.gameObject.SetActive(false);
                    //rb.MovePosition(transform.position + transform.forward * MoveSpeed * Time.deltaTime);
                    float movedistance = MoveSpeed * Time.deltaTime;
                    RaycastHit hit;
                    //自身の位置から移動方向に自身の半径+移動距離分の長さのRayを飛ばす
                    if (Physics.Raycast(transform.position, transform.forward, out hit, movedistance + radius))
                    {
                        if (hit.collider.gameObject != gameObject)
                        {
                            //移動距離をClampして移動距離を制限する
                            movedistance = Mathf.Clamp(movedistance, 0, hit.distance - radius > 0 ? hit.distance - radius : 0);
                        }
                    }
                    //transform.positionを変更して移動する
                    transform.position += (transform.forward * movedistance);
                }
            }
            //targetに対しての正面を向いていればJumpActionのコルーチンを開始する
            else if (Mathf.Abs(Mathf.Abs(transform.eulerAngles.y) - Mathf.Abs(targetRot.y)) < 1 && GetSearchAction())
            {
                SetSearchAction(false);
                StartCoroutine(AttackAction(target));
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

        anim.Play("Attack");
        yield return StartCoroutine(WaitAnimationEnd("Attack"));
        SetSearchAction(true);

        yield break;
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
