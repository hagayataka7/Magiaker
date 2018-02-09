using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttack : Action
{
    Rigidbody rb;
    Vector3 targetRot;
    AttackArea Range;
    
    [SerializeField]
    int AttackRange = 125;          //攻撃開始範囲
    [SerializeField]
    float AttackWaitTime = 1;       //攻撃までの待機時間
    [SerializeField]
    float BackWaitTime = 1;         //後退までの待機時間
    [SerializeField]
    float SearchWaitTime = 1;       //索敵までの待機時間
    [SerializeField]
    float MoveSpeed = 10;           //接近時の移動速度
    [SerializeField]
    float AttackStepSpeed = 0.5f;   //攻撃時の移動速度　0.0~1.0の間まで
    [SerializeField]
    float BackStepSpeed = 0.5f;     //後退時の移動速度　0.0~1.0の間まで

    float TargetDistance;

    bool PlayerTouch = false;
    void SetPlayerTouch(bool set) { PlayerTouch = set; }
    bool GetPlayerTouch() { return PlayerTouch; }

    bool CharacterTouch = false;
    void SetCharacterTouch(bool set) { CharacterTouch = set; }
    bool GetCharacterTouch() { return CharacterTouch; }

    bool Attack = false;
    void SetAttack(bool set) { Attack = set; }
    bool GetAttack() { return Attack; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Range = attackArea.GetComponent<AttackArea>();
        Range.aligment = aligment.enemy;
		Range.element = null;
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
        {   //自身とtargetの距離の二乗をTargetDistanceに取る
            TargetDistance = (target.transform.position - self.transform.position).sqrMagnitude;
            //targetに対しての正面を向く
            Turn(target, self);
            //TargetDistanceがAttackRangeの二乗より大きければ近づく
            if (TargetDistance >= (AttackRange ^ 2))
            {
                rb.MovePosition(transform.position + transform.forward * MoveSpeed * Time.deltaTime);
            }
            //targetに対しての正面を向いていればJumpActionのコルーチンを開始する
            else if (Mathf.Abs(Mathf.Abs(transform.eulerAngles.y) - Mathf.Abs(targetRot.y)) < 0.3 && GetSearchAction())
            {
                SetSearchAction(false);
                StartCoroutine("JumpAction", target);
            }
        }

    }
    void Turn(GameObject target, GameObject self)
    {
        //targetに対しての正面方向を取得する
        Quaternion TargetRotation = Quaternion.LookRotation(new Vector3(target.transform.position.x, self.transform.position.y, target.transform.position.z) - self.transform.position);
        //targetに対して正面になるように徐々に回転させる
        self.transform.rotation = Quaternion.Slerp(self.transform.rotation, TargetRotation, Time.deltaTime * RotationSpeed);
        //目的のQuaternionをVector3に変換する
        targetRot = TargetRotation.eulerAngles;
    }

    private IEnumerator JumpAction(GameObject target)
    {
        if (target.GetComponent<Character>().HP <= 0)
        { yield break; }
        Vector3 StartPos = transform.position;
        Vector3 EndPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        yield return new WaitForSeconds(AttackWaitTime);
        attackArea.SetActive(true);
        float movetime=0;
        float StartTime = Time.timeSinceLevelLoad;
        while (!GetCharacterTouch())
        {
            movetime += Time.deltaTime;
            float rate = movetime / AttackStepSpeed;
            transform.position = Vector3.Lerp(StartPos, EndPos, rate);

            yield return null;
            if (transform.position == EndPos || GetCharacterTouch())
            { break; }
        }
        attackArea.SetActive(false); SetCharacterTouch(false);
        yield return new WaitForSeconds(BackWaitTime);
        EndPos = StartPos;
        StartPos = transform.position;
        movetime = 0;
        while (transform.position != EndPos)
        {
            movetime += Time.deltaTime;
            float rate = movetime / BackStepSpeed;
            transform.position = Vector3.Lerp(StartPos, EndPos, rate);
            yield return null;
            if (GetCharacterTouch())
            { break; }
        }
        SetCharacterTouch(false);
        yield return new WaitForSeconds(SearchWaitTime);
        //索敵を開始する
        SetSearchAction(true);
        yield break;
    }
    void OnCollisionEnter(Collision collision)
    {
        Character target = collision.gameObject.GetComponent<Character>();
        if (target)
        {
            SetCharacterTouch(true);
        }
    }
}
