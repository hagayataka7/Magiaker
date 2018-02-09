using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Itemtype
{
    HPHeal,
    MPHeal,
}
public class Item : MonoBehaviour
{
    //作成　針ヶ谷天紀
    Rigidbody rb;
    public GameObject Player;
    public Itemtype itemtype;
    public float Heal;
    [SerializeField]
    float speed = 30;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }
    void OnTriggerEnter(Collider other)
    {
        Objectcheck(other);
    }

    //作成　針ヶ谷天紀
    void Objectcheck(Collider other)
    {
        //オブジェクトがEnemy、Sensor、Itemのタグを持つもの以外に当たると（床に落ちる等）PlayerChaseをスタートさせる
		if (/*other.tag == Tags.Wall || other.tag == Tags.Magic*/true)
        {   //地面に入らないようにFreezePositionYをtrueにする
            rb.velocity = Vector3.zero;
            StartCoroutine("PlayerChase", Player);
        }
    }
    //作成　針ヶ谷天紀
    public void Used(GameObject Player)
    {
        switch (itemtype)
        {
            case Itemtype.HPHeal:
				Player.GetComponent<PlayerController>().HPHeal(Heal);
                Debug.Log(itemtype);
                break;
            case Itemtype.MPHeal:
				Player.GetComponent<PlayerController>().MP += Heal;
                Debug.Log(itemtype);
                break;
        }
        Destroy(gameObject);
    }

    //作成　針ヶ谷天紀
    private IEnumerator PlayerChase(GameObject target)
    {
        //Playerに当たれば消えるので終了処理なし
        for (;;)
        {
            //targetに対しての正面方向を取得する
            Quaternion TargetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            //targetに対して正面になるように回転させる
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, 1);
            //オブジェクトの正面に進ませる
            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
            yield return null;
        }
    }
}
