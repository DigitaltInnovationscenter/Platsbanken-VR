using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InstructionsManager : MonoBehaviour
{
    private static InstructionsManager _instance;
    public static InstructionsManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #pragma warning disable
    [SerializeField] GameObject textInfoPrefab;
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;
#pragma warning enable

    private GameObject textInfoObject;

    private bool isInstructionsDisplayed;
    private Camera mainCamera;

    private void Start()
    {
        isInstructionsDisplayed = false;
    }

    private void Update()
    {
        if(textInfoObject != null)
        {
            if (mainCamera == null) mainCamera = Camera.main;

            textInfoObject.transform.LookAt(2 * textInfoObject.transform.position - Camera.main.transform.position);
        }
    }

    public void ToggleDisplayInstructions(SteamVR_Behaviour_Boolean steamVR_Behaviour_Boolean)
    {
        Debug.Log(steamVR_Behaviour_Boolean.booleanAction.GetActiveDevice(steamVR_Behaviour_Boolean.inputSource).ToString());

        isInstructionsDisplayed = !isInstructionsDisplayed;

        if (isInstructionsDisplayed)
        {
            DisplayInstructions(steamVR_Behaviour_Boolean.booleanAction.GetActiveDevice(steamVR_Behaviour_Boolean.inputSource).ToString());
        } else
        {
            Destroy(textInfoObject);
            textInfoObject = null;
        }
    }

    private void DisplayInstructions(string triggerHand)
    {
        textInfoObject = Instantiate(textInfoPrefab);

        if (triggerHand.ToLower() == "lefthand")
        {
            textInfoObject.transform.SetParent(leftHand);
        } else if(triggerHand.ToLower() == "righthand")
        {
            textInfoObject.transform.SetParent(rightHand);
        }

        textInfoObject.transform.localPosition = Vector3.zero;
    }

}
