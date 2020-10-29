using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class SkillMarker : MonoBehaviour
{
    #pragma warning disable
    [SerializeField] private TextMeshPro textMeshPro;
    #pragma warning enable

    [HideInInspector] public GameObject CurrentSnapPoint { get; set; }
    [HideInInspector] public TaxonomyClasses.Skill Skill { get; set; }
    [HideInInspector] public TaxonomyClasses.OccupationGroup OccupationGroup { get; set; }

    private Vector3 startScale;

    private Coroutine coroutineDestory;

    private void Awake() {
        startScale = this.transform.localScale;
    }

    private void Start()
    {
        textMeshPro.text = Skill.preferred_label;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("SnappointActive") || other.gameObject.CompareTag("SnappointInactive"))
        {
            CurrentSnapPoint = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SnappointActive") || other.gameObject.CompareTag("SnappointInactive"))
        {
            CurrentSnapPoint = other.gameObject;
        }
    }

    public void OnDetach()
    {
        if(CurrentSnapPoint != null)
        {
            SnapPointScript snapPointScript = CurrentSnapPoint.GetComponent<SnapPointScript>();
            snapPointScript.SnapSkillMarker(this.gameObject);
        } else
        {
            //Kill it - it was outside a snap point!
            coroutineDestory = StartCoroutine(DestoryAnimation(this.gameObject));
        }
        Platsbanken.ApplicationManager.Instance.OnSkillPlacedOnScreen();
        //ScreenManager.Instance.BlinkFreeSnapPoints(false);
    }

    public void OnPickup()
    {
        if(coroutineDestory != null)
        {
            StopCoroutine(coroutineDestory);
            this.transform.DOKill();
        }
        
        //ScreenManager.Instance.BlinkFreeSnapPoints(true);
    }

    public void HighlightOn() {
        this.transform.DOKill();
        transform.localScale = startScale;
        var targetScale = startScale * 1.1f;
        this.transform.DOScale(targetScale, 0.25f)
            .SetDelay(1.2f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void HighlightOff() {
        this.transform.DOKill();
        transform.localScale = startScale;
    }

    private IEnumerator DestoryAnimation(GameObject gameObject)
    {
        yield return new WaitForSeconds(2f);
        gameObject.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
