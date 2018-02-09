using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : Attack
{
	protected Character character;
	protected void Start()
    {
        if (gameObject.tag == Tags.Magic) 
			gameObject.layer = Layers.Magic;

		//Characterを探索する
		if (!character)
			character = GetComponent<Character> ();
		if (!character && transform.parent && GetComponentInParent<Character> ())
			character = GetComponentInParent<Character> ();

		//無事にCharacterが見つかったら、Characterから設定を読み込む
		if (character)
			DestroyCheck = character.DamageObjDestroy;
		else
			Debug.Log (gameObject.name + "が自身の親Characterを発見出来ませんでした。");
    }

	protected void OnTriggerEnter(Collider other)
    {
        Character target = other.gameObject.GetComponent<Character>();
        if (target)
        {
            CharacterOnTouch = true;
            TouchChar = target.tag;
        }
        LetDamage(other.gameObject, gameObject);
    }

	private void OnTriggerStay(Collider other)
    {
        Character target = other.gameObject.GetComponent<Character>();
        if (target)
        {
            CharacterOnTouch = true;
            TouchChar = target.tag;
        }
    }
}
