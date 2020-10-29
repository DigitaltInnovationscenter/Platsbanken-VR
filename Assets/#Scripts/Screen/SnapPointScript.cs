using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using DG.Tweening;

public class SnapPointScript : MonoBehaviour
{
    public bool spotTaken;
    public GameObject currentSkillMarker;
    private SpriteRenderer rend;
    public SpriteRenderer startMat;

    [SerializeField] private Color blinkColor;
    private Color orgColor;
    private Sequence sequence;

    void Start()
    {
        spotTaken = false;
        rend = GetComponentInChildren<SpriteRenderer>();
        orgColor = rend.material.color;
    }



    public void SnapSkillMarker(GameObject skillMarker)
    {
        currentSkillMarker = skillMarker;
        skillMarker.transform.position = this.gameObject.transform.position;
        skillMarker.transform.rotation = this.gameObject.transform.rotation;
        rend.material.color = orgColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!spotTaken)
        {

            if (other.gameObject.CompareTag("SkillMarker"))
            {
                if (other.gameObject.GetComponent<Interactable>().attachedToHand)
                {
                    rend.material.color = Color.green;
                }
                
                print("Spot is not taken");
               
                currentSkillMarker = other.gameObject;
                spotTaken = true;
            }

            if (other.gameObject.CompareTag("JobOrb")) {

                if (other.gameObject.GetComponent<Interactable>()?.attachedToHand)
                {
                    rend.material.color = Color.green;
                }

            }
        }
    }

    private void OnTriggerExit(Collider other) {

        if (GameObject.Equals(other.gameObject,currentSkillMarker))
        {
            currentSkillMarker = null;
            spotTaken = false;
        }
    }

    public void Blink(bool enabled)
    {
        sequence.Kill();

        if (enabled)
        {
            rend.material.color = orgColor;
            sequence = DOTween.Sequence();
            sequence.Append(rend.material.DOFade(0f, 0.3f));
            sequence.Append(rend.material.DOFade(1f, 0.3f));
            //sequence.Append(rend.material.DOColor(blinkColor, 0.3f));
            //sequence.Append(rend.material.DOColor(orgColor, 0.3f));
            sequence.SetLoops(-1);
        }
        else
        {
            rend.material.DOFade(1f, 0.2f);
            //rend.material.DOColor(orgColor, 0.2f);
        }
    }

    private IEnumerator SetColorEnum(Color color)
    {
        yield return new WaitForSeconds(0.1f);
        
        rend.material.color = color;
        
    }
}
