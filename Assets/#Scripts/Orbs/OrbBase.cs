using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using Valve.VR.InteractionSystem;
using System.Linq;
using UnityEngine.UI;

public class OrbBase : MonoBehaviour
{
#pragma warning disable
    [SerializeField] private LineRenderer lineRenderer;
   
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private Renderer rendOrb;
    [SerializeField] private float grabTransparency = -1.2f;
    [SerializeField] private OrbPanelHandler panelHandler;
    [SerializeField] private AudioSource audioSource;
#pragma warning enable

    private int minNumberOfItemsToShow = 7;
    private float orbsVisibleRatio = 0.22f;

    private List<TransformDistance> transformDistances = new List<TransformDistance>();

    
    private Transform cam;
    private Coroutine panelVisibilityUpdater;
    private float closestPanelDistance;
    private float furthestPanelDistance;

    public Transform childRoot;
    [HideInInspector] public GameObject parentOrbObject;
    [HideInInspector] public Vector3 startLocalPos;

    public string ID { get; set; }
    public string Text { get; set; }


    /* Peter testar */
    [HideInInspector] public GameObject grabbedChild;
    /* ------ */

    private bool IsGrabbing;

    public bool IsDestroying {
    get
        {
            return isDestroying;
        }
        set
        {
            if(isDestroying != value)
            {
                isDestroying = value;
                IsDestoryingChanged();
            }
        }
    }
    private bool isDestroying;

    public bool IsGrabbed
    {
        get
        {
            return isGrabbed;
        }
        set
        {
            isGrabbed = value;
            IsGrabbedChanged();
            panelHandler.UpdatePanelPos(0.02f);
            
        }
    }
    private bool isGrabbed;

    private LineRenderer line;

    private void Awake()
    {
        cam = Camera.main.transform;
    }
    public virtual void OnGrab()
    {
        IsGrabbing = true;
        if (!IsDestroying && !IsGrabbed)
        {
            Platsbanken.ApplicationManager.Instance.OnOrbGrabbed(this);
            IsGrabbed = true;
            OrbsManager.Instance.ActivateOrb(this.gameObject);
            panelHandler.SetTransparency(1f);
            panelVisibilityUpdater = StartCoroutine(VisibilityUpdater());
            audioSource.Play();
            if(parentOrbObject != null)
            {
                parentOrbObject.GetComponent<OrbBase>().grabbedChild = this.gameObject; // Peter testar
            }
        }
    }

    public virtual void OnDetach()
    {
        if (!IsDestroying)
        {
            StartCoroutine(OnDetachEnum());
        }
    }

    private IEnumerator OnDetachEnum()
    {
        IsGrabbing = false;
        yield return new WaitForSeconds(0.1f);
        if(!IsGrabbing)
        {
            IsGrabbed = false;
            //Only destory if parent is destoyed
            //If parent still is alive - zoom back to org pos
            if (parentOrbObject == null)
            {
                IsDestroying = true;
            }
            OrbsManager.Instance.DeactivateOrb(this.gameObject);
            if (panelVisibilityUpdater != null)
            {
                StopCoroutine(panelVisibilityUpdater);
            }
        }
    }

    void Start() {
        IsDestroying = false;
        IsGrabbed = false;

        if (lineRenderer != null) {
            line = Instantiate(lineRenderer, this.transform);
        }
        textMesh.text = Text;
        panelHandler.UpdatePanelPos(0.02f);

    }

    protected virtual void Update() {

        if (lineRenderer != null && this.transform.parent != null) {
            if(this.transform.gameObject.GetComponentInParent<OrbBase>() != null)
            {
                line.SetPosition(0, this.transform.parent.position);
                line.SetPosition(1, this.transform.position);
            }
        }
    }

    protected virtual void IsGrabbedChanged()
    {
        if(IsGrabbed)
        {
            rendOrb.material.SetFloat("Vector1_32125ABA", grabTransparency);
        } else
        {
            rendOrb.material.SetFloat("Vector1_32125ABA", 0f);
        }
    }

    private void IsDestoryingChanged()
    {
        

        if (IsDestroying)
        {
            GetComponent<Throwable>().enabled = false;
            GetComponent<Interactable>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }
    }




    //Panel visibility stuff below
    #region PanelVisibility

    private void AddPanelsToList()
    {
        foreach (OrbPanelHandler oPH in childRoot.GetComponentsInChildren<OrbPanelHandler>())
        {
            TransformDistance tD = new TransformDistance(oPH, CalculateDistance(oPH.canvas.transform));
            transformDistances.Add(tD);
        }
    }

    private void ClearTransformDistancesList()
    {
        //transformDistances.RemoveAll(item => item.trans == null);
        transformDistances.Clear();
    }

    private void UpdateDistances()
    {
        foreach (TransformDistance tD in transformDistances)
        {
            tD.distance = CalculateDistance(tD.trans.canvas.transform);
        }
    }

    private void SortByDistance()
    {
        transformDistances = transformDistances.OrderBy(e => e.distance).ToList();
        if (transformDistances.Count >= 1)
        {
            closestPanelDistance = transformDistances[0].distance; 
            furthestPanelDistance = transformDistances[transformDistances.Count() - 1].distance;
        }
    }


    private float CalculateDistance(Transform trs)
    {
        return (trs.position - cam.position).sqrMagnitude;
    }

    private void ShowClosestItems()
    {
        
        var numberOfItemsToShow = (int)Mathf.Max(transformDistances.Count * orbsVisibleRatio, minNumberOfItemsToShow);
        //float transVal;
        //float currentDistT;
        for (int i = 0; i < transformDistances.Count; i++)
        {
            var transformDistanceItem = transformDistances[i];
            //currentDistT = Mathf.InverseLerp(furthestPanelDistance, closestPanelDistance, transformDistances[i].distance);
            //transVal = Mathf.Lerp(0, 0.1f, currentDistT);

            if (i + 1 <= numberOfItemsToShow)
            {
                //transformDistances[i].trans.GetComponentInChildren<Canvas>().enabled = true;
                transformDistanceItem.trans?.SetTransparency(1f);
            }
            else
            {
                transformDistanceItem.trans?.SetTransparency(0.1f);
                //transformDistances[i].trans.GetComponentInChildren<Canvas>().enabled = false;
            }
        }
    }

    private IEnumerator VisibilityUpdater()
    {
        while (true)
        {
            ClearTransformDistancesList();
            AddPanelsToList();
            UpdateDistances();
            SortByDistance();
            ShowClosestItems();
            //yield return new WaitForSeconds(0.02f);
            yield return new WaitForEndOfFrame();
        }
    }

    #endregion PanelVisibility
}
