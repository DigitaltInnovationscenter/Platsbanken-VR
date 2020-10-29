using DG.Tweening;
using Platsbanken;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ScreenManager : MonoBehaviour
{
    private static ScreenManager _instance;
    public static ScreenManager Instance { get { return _instance; } }

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
    [SerializeField] private Vector3 startPosOffeset = new Vector3(0f, -2f, 0f);
    [SerializeField] private float startPosOffsetDistance = 0.4f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private GameObject screenPrefab;
    [SerializeField] private Transform screenRoot;
    [SerializeField] private Transform snappedSkillsRoot;
    [SerializeField] private bool rotateScreenWithPlayer = true;
    [SerializeField] private GameObject skillMarkerPrefab;
    [SerializeField] private Player player;
    [SerializeField] private int maxOccupationGroupButtons = 5;
    #pragma warning enable

    [HideInInspector] public GameObject screen;
    

    private bool IsScreenClosing { get; set; }

    private float yOrg;

    private void Update()
    {
        UpdateScreenPos();
    }

    public void InitScreen()
    {
        if(screen == null)
        {
            IsScreenClosing = false;

            Transform cameraTransform = Camera.main.transform;
            screen = Instantiate(screenPrefab, screenRoot);

            Vector3 camForward = cameraTransform.forward;
           
            camForward.y = 0f;
            Vector3 pos = cameraTransform.position + (camForward * startPosOffsetDistance) + startPosOffeset;
            screen.transform.position = pos;

            
            Vector3 rot = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up);

            screen.transform.LookAt(rot);

        

            yOrg = screen.transform.position.y;
        }
    }

    public void TurnOffAllSkillHighlights() {
        var skillScreenHandler = screen.GetComponent<SkillScreenHandler>();
        foreach (GameObject skillMarkerObject in skillScreenHandler.GetAllActiveSkillMarkerObjects()) {
            SkillMarker skillMarker = skillMarkerObject.GetComponent<SkillMarker>();
            skillMarker.HighlightOff();
        }
    }

    public void HighlightSkills(List<string> skillIDs) {
        var skillScreenHandler = screen.GetComponent<SkillScreenHandler>();
        foreach (GameObject skillMarkerObject in skillScreenHandler.GetAllActiveSkillMarkerObjects()) {
            SkillMarker skillMarker = skillMarkerObject.GetComponent<SkillMarker>();
            if (skillIDs.Contains(skillMarker.Skill.id)) {
                skillMarker.HighlightOn();
            } else {
                skillMarker.HighlightOff();
            }
        }
    }

    public void CloseScreen()
    {
        if(!IsScreenClosing)
        {
            IsScreenClosing = true;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(screen.transform.DOScale(Vector3.zero, 0.8f)).OnComplete(() =>
            {
                Destroy(screen);
                screen = null;
            });
            CleanSkills();
        }
    }

    private void UpdateScreenPos()
    {
        if (screen == null || !rotateScreenWithPlayer) return;

        Transform cameraTransform = Camera.main.transform;
        float step = moveSpeed * Time.deltaTime;
        Vector3 pos = Vector3.MoveTowards(screen.transform.position, cameraTransform.position + (cameraTransform.forward * startPosOffsetDistance) + startPosOffeset, step);
        pos.y = yOrg;
        screen.transform.position = pos;
        screen.transform.LookAt(2 * screen.transform.position - cameraTransform.position);
    }

    public void SnapOrbToScreen(TaxonomyClasses.Skill skill, GameObject snapPoint)
    {
        
        
        GameObject newSkillObject = Instantiate(skillMarkerPrefab, snappedSkillsRoot);
        newSkillObject.transform.position = snapPoint.transform.position;
        newSkillObject.transform.rotation = snapPoint.transform.rotation;
        
        snapPoint.GetComponent<SnapPointScript>().currentSkillMarker = newSkillObject;
        SkillMarker skillMarker = newSkillObject.GetComponent<SkillMarker>();
        skillMarker.Skill = skill;
        skillMarker.OccupationGroup = ApplicationManager.Instance.CurrentOccupationGroup;

        snapPoint.GetComponent<SnapPointScript>().SnapSkillMarker(newSkillObject);
        Platsbanken.ApplicationManager.Instance.OnSkillPlacedOnScreen();

    }

    public void RemoveOrbFromScreen(TaxonomyClasses.Skill skill, GameObject snapPoint)
    {
        SnapPointScript snapPointScript = snapPoint.GetComponent<SnapPointScript>();
        snapPointScript.spotTaken = false;

        
    }

    public List<string> GetAllActiveSkills()
    {
        List<string> activeSkillIDs = new List<string>();
        SkillScreenHandler skillScreenHandler = screen.GetComponent<SkillScreenHandler>();
        foreach(GameObject skillMarkerObject in skillScreenHandler.GetAllActiveSkillMarkerObjects())
        {
            SkillMarker skillMarker = skillMarkerObject.GetComponent<SkillMarker>();
            activeSkillIDs.Add(skillMarker.Skill.id);
        }
        return activeSkillIDs;
    }

    public List<string> GetAllActiveSkillsOccupationGroups() {
        List<string> occupationGroupIDs = new List<string>();
        SkillScreenHandler skillScreenHandler = screen.GetComponent<SkillScreenHandler>();
        foreach (GameObject skillMarkerObject in skillScreenHandler.GetAllActiveSkillMarkerObjects()) {
            SkillMarker skillMarker = skillMarkerObject.GetComponent<SkillMarker>();
            occupationGroupIDs.Add(skillMarker.OccupationGroup.conceptID);
        }
        return occupationGroupIDs;
    }

    public List<string> GetAllInactiveSkills() {
        List<string> inactiveSkillIDs = new List<string>();
        SkillScreenHandler skillScreenHandler = screen.GetComponent<SkillScreenHandler>();
        foreach (GameObject skillMarkerObject in skillScreenHandler.GetAllInactiveSkillMarkerObjects()) {
            SkillMarker skillMarker = skillMarkerObject.GetComponent<SkillMarker>();
            inactiveSkillIDs.Add(skillMarker.Skill.id);
            
        }
        return inactiveSkillIDs;
    }

    public void CleanSkills()
    {
        SkillScreenHandler skillScreenHandler = screen.GetComponent<SkillScreenHandler>();
        foreach (GameObject skillMarkerObject in skillScreenHandler.GetAllActiveSkillMarkerObjects())
        {
            Destroy(skillMarkerObject);
        }
        foreach (GameObject skillMarkerObject in skillScreenHandler.GetAllInactiveSkillMarkerObjects())
        {
            Destroy(skillMarkerObject);
        }
    }

    public void DisplayOccupationGroups()
    {

        List<string> activeSkillIDs = GetAllActiveSkills();
        List<string> activeSkillOccupationGroupsIDs = GetAllActiveSkillsOccupationGroups();

        List<SkillsToAdsManager.OccupationGroupResult> occupationGroupResults = SkillsToAdsManager.Instance.GetOccupationGroups(activeSkillIDs);

        if(Platsbanken.ApplicationManager.Instance.ShowUnexpectedAds == true) {
            occupationGroupResults.RemoveAll(x => activeSkillOccupationGroupsIDs.Contains(x.id));
        }

        while (occupationGroupResults.Count > maxOccupationGroupButtons) {
            occupationGroupResults.RemoveAt(occupationGroupResults.Count - 1);
        }

        OccupationGroupButtonHolder occupationGroupButtonHolder = screen.GetComponentInChildren<OccupationGroupButtonHolder>();
        occupationGroupButtonHolder.GenerateButtons(occupationGroupResults);

        TurnOffAllSkillHighlights();

    }

    public void ClearOccupationGroups()
    {
        OccupationGroupButtonHolder occupationGroupButtonHolder = screen.GetComponentInChildren<OccupationGroupButtonHolder>();
        occupationGroupButtonHolder.ClearButtons();
    }

    public void BlinkFreeSnapPoints(bool enabled)
    {
        SkillScreenHandler skillScreenHandler = screen.GetComponent<SkillScreenHandler>();
        foreach (SnapPointScript snapPointScript in screen.GetComponentsInChildren<SnapPointScript>())
        {
            if(!snapPointScript.spotTaken)
            {
                snapPointScript.Blink(enabled);
            }
        }
    }

}
