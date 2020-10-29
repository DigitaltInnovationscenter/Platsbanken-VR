using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTowardsCamera : MonoBehaviour
{
    [SerializeField][Tooltip("If no target is specified, Camera.main is used.")] private Transform target;
    private enum TypeOfRotation { LookAtCamera, InvertCameraRotation }

#pragma warning disable
    [SerializeField] private TypeOfRotation typeofRotation;
#pragma warning enable

    [SerializeField][Range(0.0f, 0.5f)] private float waitBetweenUpdates = 0.01f;

    private Coroutine rotating;

    private void Awake()
    {
        if (target == null)
        {
            target = Camera.main.transform;
            if (target == null)
            {
                Debug.LogError("<b>[FaceTowardsCamera]</b> no target set and no main camera found!");
            }
        }
    }

    private void Start()
    {
        StartRotating();
    }

 


    private IEnumerator Rotate()
    {
        while (true)
        {
            switch (typeofRotation)
            {
                case TypeOfRotation.LookAtCamera:
                    this.transform.rotation = Quaternion.LookRotation(this.transform.position - target.position);
                    break;
                case TypeOfRotation.InvertCameraRotation:
                    this.transform.rotation = target.rotation;
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(waitBetweenUpdates);
        }
    }


    public void StartRotating()
    {
        if (rotating == null)
        {
            rotating = StartCoroutine(Rotate());
            
        }
    }

    public void StopRotating()
    {
        if (rotating != null)
        {
            StopCoroutine(rotating);
            
        }
    }

}
