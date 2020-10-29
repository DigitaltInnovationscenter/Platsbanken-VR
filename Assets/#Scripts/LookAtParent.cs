using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtParent : MonoBehaviour
{
    private Transform parentGO;
    private Transform grandParentGO;


    private void Awake()
    {
        parentGO = this.transform.parent;
        grandParentGO = this.transform.parent.parent;


    }

    private void Start()
    {
        StartCoroutine(WaitAndSetRotation());
    }

    private IEnumerator WaitAndSetRotation()
    {
        
        yield return new WaitForSeconds(0.1f);
        this.transform.LookAt(this.transform.parent);
        

    }

    public void RotateTowardsParent()
    {
        StartCoroutine(WaitAndSetRotation());

    }
}
