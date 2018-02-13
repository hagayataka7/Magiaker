using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//作成　針ヶ谷天紀
public class ShotAttack : Action
{
    Vector3 targetRot;
    [SerializeField]
    GameObject Bullet;
    [SerializeField]
    GameObject PlayerSensor;
    PlayerSensor Sensor;
    Character character;

    //待機時間
    [SerializeField]
    float WaitTime = 1;
    //弾の初速
    [SerializeField]
    float ShotSpeed = 30;

    //弾を発射しているか判断用のbool
    bool Shooting = false;
    void onShooting() { Shooting = true; }
    void offShooting() { Shooting = false; }
    bool GetShooting() { return Shooting; }
    
    void Start()
    {
        character = GetComponent<Character>();
    }
    private void Update()
    {
        if (FlagReset)
        {
            offShooting();
            FlagReset = false;
        }
    }
    public override void ActionEnter(GameObject target, GameObject self)
    {
        base.ActionEnter(target, self);
       
        //targetに対しての正面方向をむく
        Turn(target, self);
        //targetを正面にしてShootingがfalseのときtreuにしてShotActionのコルーチンを開始させる
        if (Mathf.Abs(Mathf.Abs(transform.eulerAngles.y) - Mathf.Abs(targetRot.y)) < 0.5 && !GetShooting())
        { onShooting(); StartCoroutine("ShotAction",target); }
    }
    void Turn(GameObject target, GameObject self)
    {
        //targetに対しての正面方向をとる
        Quaternion TargetRotation = Quaternion.LookRotation(target.transform.position - self.transform.position);
        //targetに対して正面になるように徐々に回転させる
        self.transform.rotation = Quaternion.Slerp(self.transform.rotation, TargetRotation, Time.deltaTime * RotationSpeed);
        targetRot = TargetRotation.eulerAngles;
    }
    private IEnumerator ShotAction(GameObject target)
    {
        NowCoroutine = ShotAction(target);
        Sensor = PlayerSensor.GetComponent<PlayerSensor>();
        //targetが感知範囲から出た時にShootingがのtreuときfalseにしてwhile抜ける
        while (Sensor.GetPL_Search() && GetShooting())
        {
           // if (target.GetComponent<Character>().HP <= 0)
            //{ break; }
            //弾を生成する
            GameObject Bullets = Instantiate(Bullet, transform.position, Quaternion.identity);
            //弾に初速を与える
            Bullets.GetComponent<Rigidbody>().AddForce(transform.forward * ShotSpeed, ForceMode.Impulse);
            Bullets.AddComponent<AttackArea>();
            Bullets.GetComponent<AttackArea>().aligment = aligment.enemy;
            Bullets.GetComponent<AttackArea>().DestroyCheck = character.DamageObjDestroy;
            Bullets.GetComponent<AttackArea>().Damage = Damage;
            //判別用のタグをつける
            Bullets.tag = Tags.Magic;
            //WaitTimeの間、待って再度実行する
            yield return new WaitForSeconds(WaitTime);
        }
        SetCoroutineReset();
        offShooting();
    }
}
