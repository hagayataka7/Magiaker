using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 作成　針ヶ谷天紀
public class ChargeAttack : Action
{

    Rigidbody rb;
    Vector3 targetRot;

    
    //突進の時間
    [SerializeField]
    float MoveTime = 3;
    //待機時間
    [SerializeField]
    float WaitTime = 2;
    //移動速度
    [SerializeField]
    float speed = 10;
    AttackArea Range;


    /* bool PlayerTouch = false;
     void SetPlayerTouch(bool set) { PlayerTouch = set; }
     bool GetPlayerTouch() { return PlayerTouch; }

     bool EnemyTouch = false;
     void SetEnemyTouch(bool set) { EnemyTouch = set; }
     bool GetEnemyTouch() { return EnemyTouch; }*/

    void Start()
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
        if(FlagReset)
        {
            SetSearchAction(true);
            attackArea.SetActive(false);
            FlagReset = false;
        }

        if (GetSearchAction())
        {
            //targetに対しての正面方向をむく
            Turn(target, self);
        }
        //targetに対しての正面方向をむいたら回転をやめChargeActionのコルーチンを開始する
        if (Mathf.Abs(Mathf.Abs(transform.eulerAngles.y) - Mathf.Abs(targetRot.y)) < 0.5 && GetSearchAction())
        {
            SetSearchAction(false);
            StartCoroutine("ChargeAction", target);
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
        if (target.GetComponent<Character>().HP <= 0)
        { yield break; }
        attackArea.SetActive(true);
        float AttackTime = 0;
        Range.CharacterOnTouch = false;
        //AttackTimeの間、動いたらwhile文を抜ける
        while ((MoveTime >= AttackTime))
        { //フレームレートで移動時間をとる
            AttackTime += Time.deltaTime;
            //Playerのタグを持つものに当たるとWaitTimeの間、待機してfor文を抜ける
            if (Range.CharacterOnTouch)
            {
                Range.CharacterOnTouch = false;
                if (Range.TouchChar == Tags.Player)
                {
                    rb.AddForce(-(transform.forward * 2), ForceMode.Impulse);
                    yield return new WaitForSeconds(WaitTime);                    
                }
                yield return new WaitForSeconds(WaitTime / 3);                
                attackArea.SetActive(false);
                break;
            }
           /* float movedistance = (speed * Time.deltaTime);
            RaycastHit hit;
            //自身の位置から移動方向に自身の半径+移動距離分の長さのRayを飛ばす
            if (Physics.Raycast(transform.position, transform.forward, out hit, movedistance + radius))
            {
                Debug.Log(hit.point);
                //移動距離をClampして移動距離を制限する
                movedistance = Mathf.Clamp(movedistance, 0, hit.distance - radius > 0 ? hit.distance - radius : 0);
            }
            //オブジェクトの正面に進ませる
            // rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);*/
            transform.position += transform.forward * speed * Time.deltaTime;
            //一度中断させ反映させる
            yield return null;
        }

        //索敵を開始し移動時間を初期化する
        SetSearchAction(true); attackArea.SetActive(false);
        yield break;
    }
}
