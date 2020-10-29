using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Threading.Tasks;

public class OrbsManager : MonoBehaviour
{
    private static OrbsManager _instance;
    public static OrbsManager Instance { get { return _instance; } }

    private readonly int childOrbsCap = 666;

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
    [SerializeField] bool displayParentAsChild = true;
    [SerializeField] Vector3 ungrabbedSize;
    [SerializeField] Vector3 grabbedSize;
    [SerializeField] Vector3 orgRootPosition = new Vector3(0f, 1.2f, 0.5f);
    [SerializeField] float childDistance = 0.4f;
    [SerializeField] float childDistanceStep = 50f;
    [SerializeField] GameObject loadingOrbPrefab;
    #pragma warning enable

    public List<GameObject> orbPrefabs = new List<GameObject>();

    public Transform orbsRoot;
    [HideInInspector] public GameObject orbLevel0GameObject;

    private List<GameObject> grabbedObjects = new List<GameObject>(); 

    public void Init()
    {
        InitOrbLevelO();
    }

    private void InitOrbLevelO()
    {
        Vector3 pos;
        if(ScreenManager.Instance.screen == null)
        {
            pos = orgRootPosition;
        } else
        {
            pos = ScreenManager.Instance.screen.GetComponent<SkillScreenHandler>().orbLevel0StartPos.position;
        }

        orbLevel0GameObject = Instantiate(GetPrefab(typeof(OrbLevel0).FullName), orbsRoot);
        orbLevel0GameObject.transform.localScale = ungrabbedSize;
        orbLevel0GameObject.transform.position = pos;
        OrbBase orbBase = orbLevel0GameObject.GetComponent<OrbBase>();
        orbBase.Text = "PLATSBANKEN";
    }

    public void ActivateOrb(GameObject grabbedOrbObject)
    {
        grabbedObjects.Add(grabbedOrbObject);

        string grabbedObjectType = grabbedOrbObject.GetComponent<OrbBase>().GetType().FullName;
        switch (grabbedObjectType)
        {
            case "OrbLevel0":
                grabbedOrbObject.transform.DOScale(grabbedSize, 0.3f);
                CreateChildsLevel1(grabbedOrbObject.GetComponent<OrbLevel0>());
                break;
            case "OrbLevel1":
                grabbedOrbObject.transform.DOScale(grabbedSize, 0.3f);
                CreateChildsLevel2(grabbedOrbObject.GetComponent<OrbLevel1>());
                break;
            case "OrbLevel2":
                grabbedOrbObject.transform.DOScale(grabbedSize, 0.3f);
                CreateChildsLevel3(grabbedOrbObject.GetComponent<OrbLevel2>());
                break;
            case "OrbLevel3":
                grabbedOrbObject.transform.DOScale(grabbedSize, 0.3f);
                CreateChildsLevel4(grabbedOrbObject.GetComponent<OrbLevel3>());
                break;
            case "OrbLevel4":
                
                break;
        }
    }

    private void CreateChildsLevel1(OrbLevel0 orbGrabbed)
    {
        var numberOfChildren = ClampNumberOfChildren(TaxonomyManager.Instance.occupationFields.Values.Count);
        List<Vector3> points = Placement.GetPointsOnSphere(numberOfChildren, 1f);
        points = Placement.SortPointsBasedOnDistance(points, Camera.main.transform.position);
        float childCountDistance = numberOfChildren / childDistanceStep;
        
        int i = 0;
        foreach (TaxonomyClasses.OccupationField occupationField in TaxonomyManager.Instance.occupationFields.Values.OrderBy(d => d.occupationField))
        {
            if(i < numberOfChildren) {
                GameObject newOrb = Instantiate(GetPrefab("OrbLevel1"), orbGrabbed.childRoot);
                newOrb.name = "OrbLevel1" + " " + occupationField.occupationField;
                newOrb.transform.localPosition = Vector3.zero;
                newOrb.transform.localScale = ungrabbedSize + grabbedSize;
                newOrb.transform.DOKill();
                newOrb.transform.DOLocalMove(points[i] * (childDistance + childCountDistance), 0.5f).SetEase(Ease.OutBack);
                OrbLevel1 orbLevel1 = newOrb.GetComponent<OrbLevel1>();
                orbLevel1.ID = occupationField.occupationField;
                orbLevel1.Text = AdjustText(occupationField.occupationField);
                orbLevel1.OccupationField = occupationField;
                orbLevel1.parentOrbObject = orbGrabbed.gameObject;
                orbLevel1.startLocalPos = points[i] * (childDistance + childCountDistance);
                i++;
            }
        }
    }

    private int ClampNumberOfChildren(int num) {
        if(num > childOrbsCap) {
            
        }
        return (num <= childOrbsCap) ? num : childOrbsCap;
    }

    private void CreateChildsLevel2(OrbLevel1 orbGrabbed)
    {
        TaxonomyClasses.OccupationField occupationField = orbGrabbed.OccupationField;
        var numberOfChildren = ClampNumberOfChildren(occupationField.occupationGroups.Values.Count);
        List <Vector3> points = Placement.GetPointsOnSphere(numberOfChildren, 1f);
        points = Placement.SortPointsBasedOnDistance(points, Camera.main.transform.position);
        float childCountDistance = numberOfChildren / childDistanceStep;
        int i = 0;
        foreach (TaxonomyClasses.OccupationGroup occupationGroup in occupationField.occupationGroups.Values.OrderBy(d => d.occupationGroup))
        {
            if(i < numberOfChildren){
                GameObject newOrb = Instantiate(GetPrefab("OrbLevel2"), orbGrabbed.childRoot);
                newOrb.name = "OrbLevel2" + " " + occupationGroup.occupationGroup;
                newOrb.transform.localPosition = Vector3.zero;
                newOrb.transform.localScale = ungrabbedSize + grabbedSize;
                newOrb.transform.DOKill();
                newOrb.transform.DOLocalMove(points[i] * (childDistance + childCountDistance), 0.5f).SetEase(Ease.OutBack);
                OrbLevel2 orbLevel2 = newOrb.GetComponent<OrbLevel2>();
                orbLevel2.ID = occupationGroup.occupationGroup;
                orbLevel2.Text = AdjustText(occupationGroup.occupationGroup);
                orbLevel2.OccupationGroup = occupationGroup;
                orbLevel2.parentOrbObject = orbGrabbed.gameObject;
                orbLevel2.startLocalPos = points[i] * (childDistance + childCountDistance);
                i++;
            }
        }
    }

    private void CreateChildsLevel3(OrbLevel2 orbGrabbed)
    {
        TaxonomyClasses.OccupationGroup occupationGroup = orbGrabbed.OccupationGroup;
        var numberOfChildren = ClampNumberOfChildren(occupationGroup.occupations.Values.Count);
        if(numberOfChildren == 0)
        {
            numberOfChildren = 1;
        }
        List<Vector3> points = Placement.GetPointsOnSphere(numberOfChildren, 1f);
        points = Placement.SortPointsBasedOnDistance(points, Camera.main.transform.position);
        float childCountDistance = numberOfChildren / childDistanceStep;
        int i = 0;
        foreach (TaxonomyClasses.Occupation occupation in occupationGroup.occupations.Values.OrderBy(d => d.occupationName))
        {
            if (i < numberOfChildren) {
                GameObject newOrb = Instantiate(GetPrefab("OrbLevel3"), orbGrabbed.childRoot);
                newOrb.name = "OrbLevel3" + " " + occupation.occupationName;
                newOrb.transform.localPosition = Vector3.zero;
                newOrb.transform.localScale = ungrabbedSize + grabbedSize;
                newOrb.transform.DOKill();
                newOrb.transform.DOLocalMove(points[i] * (childDistance + childCountDistance), 0.5f).SetEase(Ease.OutBack);
                OrbLevel3 orbLevel3 = newOrb.GetComponent<OrbLevel3>();
                orbLevel3.ID = occupation.occupationName;
                orbLevel3.Text = AdjustText(occupation.occupationName);
                orbLevel3.Occupation = occupation;
                orbLevel3.occupationGroupParent = occupationGroup;
                orbLevel3.parentOrbObject = orbGrabbed.gameObject;
                orbLevel3.startLocalPos = points[i] * (childDistance + childCountDistance);
                i++;
            }
        }

        //Check if there are no childs (Occupations) then create a default one
        if(i == 0)
        {
            GameObject newOrb = Instantiate(GetPrefab("OrbLevel3"), orbGrabbed.childRoot);
            newOrb.name = "OrbLevel3" + " " + occupationGroup.occupationGroup;
            newOrb.transform.localPosition = Vector3.zero;
            newOrb.transform.localScale = ungrabbedSize + grabbedSize;
            newOrb.transform.DOKill();
            newOrb.transform.DOLocalMove(points[i] * (childDistance + childCountDistance), 0.5f).SetEase(Ease.OutBack);
            OrbLevel3 orbLevel3 = newOrb.GetComponent<OrbLevel3>();
            orbLevel3.ID = occupationGroup.occupationGroup;
            orbLevel3.Text = AdjustText(occupationGroup.occupationGroup);
            orbLevel3.Occupation = TaxonomyManager.Instance.CreateOccupation(occupationGroup.occupationGroup);
            orbLevel3.occupationGroupParent = occupationGroup;
            orbLevel3.parentOrbObject = orbGrabbed.gameObject;
            orbLevel3.startLocalPos = points[i] * (childDistance + childCountDistance);
        }
    }

    private async void CreateChildsLevel4(OrbLevel3 orbGrabbed)
    {
        TaxonomyClasses.Occupation occupation = orbGrabbed.Occupation;
        TaxonomyClasses.OccupationGroup occupationGroup = orbGrabbed.occupationGroupParent;
        var loadingOrb = ShowLoading(orbGrabbed.gameObject);
        TaxonomyClasses.SkillConcepts skillConcepts = await TaxonomyManager.Instance.GetCompetensWords(occupationGroup.conceptID);
        await Task.Delay(700);
        HideLoading(loadingOrb, 0.3f);
        await Task.Delay(100);
        if (skillConcepts == null) {
            return;
        }
        if (skillConcepts.data.concepts.Count == 0) {
            return;
        }
        var numberOfChildren = ClampNumberOfChildren(skillConcepts.data.concepts[0].skills.Count);
        List<Vector3> points = Placement.GetPointsOnSphere(numberOfChildren, 1f);
        points = Placement.SortPointsBasedOnDistance(points, Camera.main.transform.position);
        float childCountDistance = numberOfChildren / childDistanceStep;
        int i = 0;
        foreach (TaxonomyClasses.Skill skill in skillConcepts.data.concepts[0].skills.OrderBy(d => d.preferred_label))
        {
            if(i < numberOfChildren) {
                GameObject newOrb = Instantiate(GetPrefab("OrbLevel4"), orbGrabbed.childRoot);
                newOrb.name = "OrbLevel4" + " " + skill.preferred_label;
                newOrb.transform.localPosition = Vector3.zero;
                newOrb.transform.localScale = ungrabbedSize + grabbedSize;
                newOrb.transform.DOKill();
                newOrb.transform.DOLocalMove(points[i] * (childDistance + childCountDistance), 0.5f).SetEase(Ease.OutBack);
                OrbLevel4 orbLevel4 = newOrb.GetComponent<OrbLevel4>();
                orbLevel4.ID = skill.id;
                orbLevel4.Text = AdjustText(skill.preferred_label);
                orbLevel4.parentOrbObject = orbGrabbed.gameObject;
                orbLevel4.startLocalPos = points[i] * (childDistance + childCountDistance);
                orbLevel4.Skill = skill;
                i++;
            }
        }
    }

    private string AdjustText(string text) {
        text = text.Replace("/", "/ ");
        var words = text.Split(null);
        for (int i = 0; i < words.Length; i++) {
            if(words[i].Length >= 18) {
                if (!words[i].Contains("-")) {
                    words[i] = words[i].Insert(14, "-");
                }
            }
        }
        var result = String.Join(" ", words);
        return result;
    }

    private GameObject ShowLoading(GameObject gameObject) {
        var loadingOrb = Instantiate(loadingOrbPrefab, gameObject.transform);
        loadingOrb.transform.localScale = Vector3.one * 1.3f;
        loadingOrb.transform.DORotate(new Vector3(0, 0, 3600), 5f).SetLoops(-1);
        return loadingOrb;
    }

    private void HideLoading(GameObject loadingOrb, float duration) {
        var color = loadingOrb.GetComponent<Renderer>().material.color;
        color.a = 0f;
        if (loadingOrb != null) {
            loadingOrb.GetComponent<MeshRenderer>().material
                .DOColor(color, duration)
                .OnComplete(() => {
                    DOTween.Kill(loadingOrb.transform);
                    Destroy(loadingOrb);
                });
        }
    }

    public void DeactivateOrb(GameObject unGrabbedOrbObject)
    {
        OrbBase orbBaseUnGrabbed = unGrabbedOrbObject.GetComponent<OrbBase>();
        OrbLevel4 orbLevel4 = unGrabbedOrbObject.GetComponent<OrbLevel4>();

        bool isSnappedToScreen = false;
        if(orbLevel4 != null)
        {
            isSnappedToScreen = orbLevel4.IsSnappedToScreen;
        }


        for (int i = 0; i < orbBaseUnGrabbed.childRoot.childCount; i++)
        {
            GameObject child = orbBaseUnGrabbed.childRoot.GetChild(i).gameObject;
            OrbBase childOrbBase = child.GetComponent<OrbBase>();
            childOrbBase.IsDestroying = true;
            child.transform.DOKill();
            child.transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() => Destroy(child));
        }

        GameObject grabbedChild = unGrabbedOrbObject.GetComponent<OrbBase>().grabbedChild;
        if(grabbedChild != null && displayParentAsChild)
        {
            unGrabbedOrbObject.transform.SetParent(grabbedChild.transform);
            unGrabbedOrbObject.GetComponent<OrbBase>().grabbedChild = null;
            
            unGrabbedOrbObject.transform.DOKill();
            unGrabbedOrbObject.transform.DOMove(orbBaseUnGrabbed.startLocalPos, 0.3f).SetEase(Ease.OutBack);
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(unGrabbedOrbObject.transform.DOLocalMove(grabbedChild.transform.position + Vector3.one, 0.5f).SetEase(Ease.OutBack))
              .Insert(0, unGrabbedOrbObject.transform.DOScale(ungrabbedSize + grabbedSize, 0.5f).SetEase(Ease.OutBack));
        }
        else
        {
            if (grabbedObjects.Count > 1 && !isSnappedToScreen)
            {
                if (!GameObject.Equals(grabbedObjects[0], unGrabbedOrbObject))
                {
                    unGrabbedOrbObject.transform.DOKill();
                    unGrabbedOrbObject.transform.DOMove(orbBaseUnGrabbed.startLocalPos, 0.3f).SetEase(Ease.OutBack);
                    Sequence mySequence = DOTween.Sequence();
                    mySequence.Append(unGrabbedOrbObject.transform.DOLocalMove(orbBaseUnGrabbed.startLocalPos, 0.5f).SetEase(Ease.OutBack))
                      .Insert(0, unGrabbedOrbObject.transform.DOScale(ungrabbedSize + grabbedSize, 0.5f).SetEase(Ease.OutBack));
                }
                else
                {

                    unGrabbedOrbObject.transform.DOKill();
                    unGrabbedOrbObject.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => {
                        Destroy(unGrabbedOrbObject);
                        if (grabbedObjects.Count == 0)
                        {
                            InitOrbLevelO();
                        }
                    });
                }
            }
            else
            {
                unGrabbedOrbObject.transform.DOKill();
                unGrabbedOrbObject.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => {
                    Destroy(unGrabbedOrbObject);
                    if (grabbedObjects.Count == 0)
                    {
                        InitOrbLevelO();
                    }
                });
            }
        }



        grabbedObjects.Remove(unGrabbedOrbObject);
    }

    public GameObject GetPrefab(string type)
    {
        switch (type)
        {
            case "OrbLevel0":
                return OrbsManager.Instance.orbPrefabs[0];
            case "OrbLevel1":
                return OrbsManager.Instance.orbPrefabs[1];
            case "OrbLevel2":
                return OrbsManager.Instance.orbPrefabs[2];
            case "OrbLevel3":
                return OrbsManager.Instance.orbPrefabs[3];
            case "OrbLevel4":
                return OrbsManager.Instance.orbPrefabs[4];
        }
        return null;
    }

    public bool IsOrbLevel4Grabbed()
    {
        foreach(GameObject grabbedObject in grabbedObjects)
        {
            if (grabbedObject.GetComponent<OrbLevel4>() != null) return true;
        }

        return false;
    }

}
