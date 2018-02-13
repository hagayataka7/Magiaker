using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Action : MonoBehaviour
{
    public bool FlagReset;
    bool SearchAction = true;
    public void SetSearchAction(bool set) { SearchAction = set; }
    public bool GetSearchAction() { return SearchAction; }
    public int Damage = 10;
    public float RotationSpeed = 10f;  //回転速度
    public GameObject attackArea;
    public NavMeshAgent agent;
    public IEnumerator NowCoroutine;
    //継承する
    public virtual void ActionEnter(GameObject target, GameObject self)
    {

    }
    public virtual void StopCoroutines()
    {
        StopAllCoroutines();
       // FlagReset = true;
    }
    public void PauseEnd()
    {
        if (NowCoroutine != null)
        { StartCoroutine(NowCoroutine); }
        else
        { FlagReset = true; }
    }
    public virtual void Death()
    {
        FlagReset = true;
        SetSearchAction(false);
        StopAllCoroutines();
        if (attackArea)
        { attackArea.SetActive(false); }
    }
    public void PlayerDeathStop()
    {
        SetSearchAction(false);
        StopAllCoroutines();
        if (agent != null)
        {
            agent.velocity *= 0;
            agent.Stop();
        }
    }
    public void SetCoroutineReset()
    {
        NowCoroutine = null;
    }
}
