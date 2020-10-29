using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;


public class PanelVisibilityToggler : MonoBehaviour
{
    #pragma warning disable
    [SerializeField] private int numberOfItemsToShow;
    #pragma warning enable

    private List<TransformDistance> transformDistances = new List<TransformDistance>();
    private Transform cam;


    private static PanelVisibilityToggler _instance;
    public static PanelVisibilityToggler Instance { get { return _instance; } }

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
        cam = Camera.main.transform;
    }

    private void Start()
    {
        StartCoroutine(RunVisibilityRoutine());
    }

    private IEnumerator RunVisibilityRoutine()
    {
        while (true)
        {
            ClearEmptyItems();
            UpdateDistances();
            SortByDistance();
            ShowClosestItems();
            
            yield return new WaitForSeconds(0.2f);
        }

    }


    public void AddPanelHandler(OrbPanelHandler panHand)
    {
        TransformDistance transDist = new TransformDistance(panHand, CalculateDistance(panHand.transform)); ;
        transformDistances.Add(transDist);
    }
    private void RemovePanelHandler(OrbPanelHandler panHand)
    {

        foreach (TransformDistance tD in transformDistances)
        {
            if (tD.trans == panHand.transform)
            {
                transformDistances.Remove(tD);
                break;
            }
        }
        
    }

    private void ClearEmptyItems()
    {
        transformDistances.RemoveAll(item => item.trans == null);
       
    }

    private void UpdateDistances()
    {
        foreach (TransformDistance tD in transformDistances)
        {
            tD.distance = CalculateDistance(tD.trans.canvas.transform); ;
        }
    }    
    
    private void SortByDistance()
    {
        transformDistances.OrderBy(e => e.distance);
    }

    



    private float CalculateDistance(Transform trs)
    {
        return (trs.position - cam.position).sqrMagnitude;
    }

    private void ShowClosestItems()
    {
        for (int i = 0; i < transformDistances.Count; i++)
        {
            if (i +1 <= numberOfItemsToShow)
            {
                transformDistances[i].trans.GetComponentInChildren<Canvas>().enabled = true;
            }
            else
            {
                transformDistances[i].trans.GetComponentInChildren<Canvas>().enabled = false;
            }
        }
    }
}
