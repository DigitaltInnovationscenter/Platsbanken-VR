using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class AdCube : MonoBehaviour
{
    public JobAdClasses.Hit JobAd
    {
        get
        {
            return jobAd;
        }
        set
        {
            jobAd = value;
            JobAdChanged();
        }
    }
    private JobAdClasses.Hit jobAd;

    #pragma warning disable
    [SerializeField] private Transform innerCube;
    [SerializeField] private TextMeshPro ungrabbedText;
    [SerializeField] private GameObject ungrabbedObject;
    [SerializeField] private List<GameObject> panels = new List<GameObject>();
    [SerializeField] private float detachTimeout = 2f;
    #pragma warning enable

    private Vector3 ungrabbedOrgScale;

    private List<Vector3> panelsOrgPos = new List<Vector3>();
    private List<Vector3> panelsOrgScale = new List<Vector3>();

    public bool IsGrabbed { get; set; }
    private float detachTime;


    private Camera mainCamera;
    private AdCubeInfo adCubeInfo;

    private Vector3 adCubeOrgPos;

    private Coroutine showPanelsEnum;
    private Coroutine hidePanelsEnum;

    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        adCubeInfo = GetComponent<AdCubeInfo>();
        ungrabbedOrgScale = ungrabbedObject.transform.localScale;
        IsGrabbed = false;
        adCubeOrgPos = transform.localPosition;
        InitPanels();
    }

    // Update is called once per frame
    void Update()
    {
        ungrabbedObject.transform.LookAt(mainCamera.transform.position);
        if(!isOpen)
        {
            innerCube.Rotate(Vector3.up * UnityEngine.Random.Range(0f, 120f) * Time.deltaTime);
        }
    }

    public void UpdateCubeStartPosition(){
        adCubeOrgPos = transform.localPosition;
    }

    public void OnPickUp()
    {
        IsGrabbed = true;
        isOpen = true;
        if (hidePanelsEnum != null)
        {
            StopCoroutine(hidePanelsEnum);
        }
        showPanelsEnum = StartCoroutine(ShowPanelsEnum());
    }

    public void OnDetach()
    {
        IsGrabbed = false;
        if(showPanelsEnum != null)
        {
            StopCoroutine(showPanelsEnum);
        }
        hidePanelsEnum = StartCoroutine(HidePanelsEnum());
    }

    private void JobAdChanged()
    {
        if (adCubeInfo == null) {
            adCubeInfo = GetComponent<AdCubeInfo>();
        }

        adCubeInfo.ResetText();
        adCubeInfo.TextUngrabbedHeadline = JobAd.headline;
        adCubeInfo.TextHeadline = JobAd.headline;
        adCubeInfo.TextDescription = JobAd.description.text;
        adCubeInfo.TextOccupationField = JobAd.occupation_field.label;
        adCubeInfo.TextOccupationGroup = JobAd.occupation_group.label;
        adCubeInfo.TextOccupation = JobAd.occupation.label;
        adCubeInfo.TextEmployer = JobAd.employer.name;
        adCubeInfo.TextAddress = JobAd.workplace_address.municipality;
    }

    private void InitPanels()
    {
        foreach(GameObject panel in panels)
        {
            panelsOrgPos.Add(panel.transform.localPosition);
            panelsOrgScale.Add(panel.transform.localScale);
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = Vector3.zero;
        }
        ToggleAllButtons(false);
    }

    private IEnumerator ShowPanelsEnum()
    {
        adCubeInfo.ResetText();
        int i = 0;
        foreach(GameObject panel in panels)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(panel.transform.DOScale(panelsOrgScale[i], 0.8f).SetEase(Ease.Linear));
            sequence.Insert(0f, panel.transform.DOLocalMove(panelsOrgPos[i], 0.8f).SetEase(Ease.OutBack));
            sequence.Insert(0f, ungrabbedObject.transform.DOScale(Vector3.zero, 0.8f).SetEase(Ease.OutBack));
            yield return new WaitForSeconds(0.05f);
            i++;
        }
        ToggleAllButtons(true);
    }

    private IEnumerator HidePanelsEnum()
    {
        detachTime = Time.time;
        while(Time.time < detachTime + detachTimeout && !IsGrabbed)
        {
            yield return new WaitForEndOfFrame();
        }

        if (!IsGrabbed)
        {
            int i = 0;
            foreach (GameObject panel in panels)
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(panel.transform.DOScale(Vector3.zero, 0.8f).SetEase(Ease.Linear));
                sequence.Insert(0f, panel.transform.DOLocalMove(Vector3.zero, 0.8f).SetEase(Ease.InBack));
                sequence.Insert(0f, ungrabbedObject.transform.DOScale(ungrabbedOrgScale, 0.8f).SetEase(Ease.OutBack));
                yield return new WaitForSeconds(0.05f);
                i++;
            }
        }
        ToggleAllButtons(false);
        isOpen = false;
        yield return new WaitForSeconds(1f);
        Sequence rePlaceCube = DOTween.Sequence();
        rePlaceCube.Append(transform.DOLocalMove(adCubeOrgPos, 1f).SetEase(Ease.OutBack));
    }

    private void ToggleAllButtons(bool active)
    {
        foreach(AdCubeButton adCubeButton in this.gameObject.GetComponentsInChildren<AdCubeButton>(true))
        {
            adCubeButton.gameObject.SetActive(active);
        }
    }

}
