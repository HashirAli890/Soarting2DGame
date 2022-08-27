using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SideTrigger : MonoBehaviour
{
    [FormerlySerializedAs("first")] public bool rightTrigger;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GetComponentInParent<UIDrag>())
        {
            if (GetComponentInParent<UIDrag>().Drag == false && other.gameObject.GetComponentInParent<UIDrag>().Drag == true)
            {
                Debug.Log("BeforeFirst");
                if (rightTrigger)
                {
                    Debug.Log("In IF");
                    GetComponentInParent<UIDrag>().triggerEnteredOnce = true;
                }
                else
                {
                    Debug.Log("In Else");
                    GetComponentInParent<UIDrag>().triggerEnteredTwice = true;
                }

                AssignSwapObject(other.gameObject);
            }
        }
    }

    private void AssignSwapObject(GameObject other)
    {
        if ((GetComponentInParent<UIDrag>().triggerEnteredOnce == true) &&
            (GetComponentInParent<UIDrag>().triggerEnteredTwice == true))
        {
            GameHandler.Instance.SwapableObject = transform.parent.gameObject;
            //GameHandler.Instance.UpdateChildList(other.transform.parent.gameObject, transform.parent.gameObject);
            //StartCoroutine(OnOffOtherCollider(other));
            //GetComponentInParent<UIDrag>().triggerEnteredOnce = false;
            //GetComponentInParent<UIDrag>().triggerEnteredTwice = false;
        }
    }
    public void SwitchApps(GameObject other) 
    {
        Debug.Log("Here");
        GameHandler.Instance.UpdateChildList(other.gameObject, transform.parent.gameObject);
        StartCoroutine(OnOffOtherCollider(other));
        GetComponentInParent<UIDrag>().triggerEnteredOnce = false;
        GetComponentInParent<UIDrag>().triggerEnteredTwice = false;
    }
    private IEnumerator OnOffOtherCollider(GameObject other)
    {
        GameHandler.Instance.DisableTriggers(transform.parent.gameObject, other.transform.parent.gameObject);
        other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.4f);
        other.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        GameHandler.Instance.EnableTriggers();
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (GetComponentInParent<UIDrag>())
        {
            if (!GetComponentInParent<TriggerCheck>().Middle)
            {
                StartCoroutine(wait(other));
            }
        }
    }
    IEnumerator wait(Collider2D other) 
    {
        yield return new WaitForSeconds(0.5f);
        if (GetComponentInParent<UIDrag>().triggerEnteredOnce && other.gameObject.GetComponentInParent<UIDrag>().triggerEnteredTwice)
        {
            GetComponentInParent<UIDrag>().triggerEnteredOnce = false;
            GetComponentInParent<UIDrag>().triggerEnteredTwice = false;
        }
    }

}
