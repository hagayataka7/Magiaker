using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BOSS : EnemyController {
	public override void Death ()
	{
		GotoResult.InitGotoResult ();
		base.Death ();
	}

    protected override void ItemDrop()
    {
        //ボスはアイテムを落とさないので空
    }

    public override void TakeAbnormalState(AbnState value)
    {
        //ボスは状態異常を無効化するので空
    }
}
