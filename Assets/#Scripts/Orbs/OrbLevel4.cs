using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class OrbLevel4 : OrbBase
{
    public TaxonomyClasses.Skill Skill { get; set; }
    [HideInInspector] public bool IsSnappedToScreen { get; set; }

    private GameObject currentSnapPoint;

    protected override void Update()
    {
        base.Update();
        
    }

    private bool SkillAlreadyCollected() {
        var activeSkills = ScreenManager.Instance.GetAllActiveSkills();
        var inactiveSkills = ScreenManager.Instance.GetAllInactiveSkills();
        if(activeSkills.Contains(Skill.id) || inactiveSkills.Contains(Skill.id)) {
            return true;
        }
        return false;
    }

    public override void OnGrab()
    {
        base.OnGrab();
        ScreenManager.Instance.BlinkFreeSnapPoints(true);
    }

    public override void OnDetach()
    {
        if (currentSnapPoint != null)
        {
            SnapPointScript snapPointScript = currentSnapPoint.GetComponent<SnapPointScript>();
            if(!snapPointScript.spotTaken && !SkillAlreadyCollected())
            {
                ScreenManager.Instance.SnapOrbToScreen(Skill, currentSnapPoint);
                IsSnappedToScreen = true;
                base.OnDetach();
            } else
            {
                IsSnappedToScreen = false;
                base.OnDetach();
            }
        } else
        {
            IsSnappedToScreen = false;
            base.OnDetach();
        }
        ScreenManager.Instance.BlinkFreeSnapPoints(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SnappointActive") || (other.gameObject.CompareTag("SnappointInactive")))
        {
            currentSnapPoint = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SnappointActive") || (other.gameObject.CompareTag("SnappointInactive")))
        {
            currentSnapPoint = null;
        }
    }

    protected override void IsGrabbedChanged()
    {

    }
}
