using Platsbanken;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillScreenHandler : MonoBehaviour
{
    public GameObject adPanel;
    public Transform activeSnappointsRoot;
    public Transform inactiveSnappointsRoot;
    public Transform orbLevel0StartPos;

    #pragma warning disable
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI activeSkillsText;
    #pragma warning enable

    private void Start()
    {
        activeSkillsText.text = $"Aktiva skills (minst {ApplicationManager.Instance.ActiveSkillsThreshold})";
    }

    public List<GameObject> GetAllActiveSkillMarkerObjects()
    {
        List<GameObject> activeSkillMarkers = new List<GameObject>();
        for(int i = 0; i < activeSnappointsRoot.childCount; i++)
        {
            SnapPointScript snapPointScript = activeSnappointsRoot.GetChild(i).gameObject.GetComponentInChildren<SnapPointScript>();
            if(snapPointScript.currentSkillMarker != null)
            {
               
                activeSkillMarkers.Add(snapPointScript.currentSkillMarker);

            }
        }
        return activeSkillMarkers;
    }

    public List<GameObject> GetAllInactiveSkillMarkerObjects() {
        List<GameObject> inactiveSkillMarkers = new List<GameObject>();
        for (int i = 0; i < inactiveSnappointsRoot.childCount; i++) {
            SnapPointScript snapPointScript = inactiveSnappointsRoot.GetChild(i).gameObject.GetComponentInChildren<SnapPointScript>();
            if (snapPointScript.currentSkillMarker != null) {
                inactiveSkillMarkers.Add(snapPointScript.currentSkillMarker);
            }
        }
        return inactiveSkillMarkers;
    }

    public void OnButtonClose()
    {
        ScreenManager.Instance.CloseScreen();
        audioSource.Play();
    }
}
