using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopPoint : MonoBehaviour {

    public bool inArea = false;
    [SerializeField]
    private bool NowInArea, OldinArea;
    public float time;

    private void Update()
    {
        if (time - Time.time < -1)
        {
            inArea = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Character>())
        {
            time = Time.time;
            inArea = true;
        }
    }
}
