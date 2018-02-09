using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float Damage;
    public bool DestroyCheck = false;
    public aligment aligment;
    //public element element;
	private AbnState ele;
	[SerializeField]
	public AbnState element{ 
		get { return ele ?? null;} 
		set{ ele = value; } }
    public float abnormalStatePersentage = 0;

    public bool CharacterOnTouch;
    public string TouchChar { get; set; }

	/// <summary>
	/// 対象が攻撃対象として正しいか確認し、正しければヒット処理を行う
	/// </summary>
	/// <param name="TargetObj">Target object.</param>
	/// <param name="SelfObj">Self object.</param>
    public void LetDamage(GameObject TargetObj, GameObject SelfObj)
    {
        Character target = TargetObj.GetComponent<Character>();
        if (target)
        {
            CharacterOnTouch = true;
            if (!target.isAligment(aligment))
            {
                target.TakeAttack(Damage, element);
                if (DestroyCheck)
                { Destroy(gameObject); }
            }
        }
  //      else if (SelfObj.tag == Tags.Magic&& TargetObj.tag == Tags.Wall)
  //      {
  //          Debug.Log("magick hit is " + TargetObj.tag);
		//	//MagicDestroy(TargetObj);
		//	if (DestroyCheck)
		//		Destroy (gameObject);
		//}
    }

	/// <summary>
	/// 攻撃元オブジェクトが魔法なら、ヒットした壁を破壊する　
	/// //富澤より 呼び出し元で if(DestroyCheck)Destroy(gameObject);してるのに、この関数内で消しても意味ないような…
	/// </summary>
	/// <param name="TargetObj">Target object.</param>
	/*
    void MagicDestroy(GameObject TargetObj)
    {
        if (TargetObj.tag==Tags.Wall)
        {
            if (DestroyCheck)
            { Destroy(gameObject); }
        }
    }*/
}