using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Sets the position and parent of the Orb panel
/// Sets the offset Transform (see the OrbTextPrefab prefab for offset Transforms)
/// Sets active rotation axis (FaceTowardsCamera objects)
/// </summary>
public class OrbPanelHandler : MonoBehaviour
{
#pragma warning disable
    [SerializeField] private GameObject pivotPoint;
    [SerializeField] private Transform innerOffset;
    [SerializeField] private Transform outerOffset;
    [SerializeField] private FaceTowardsCamera rootAxis;
    [SerializeField] private FaceTowardsCamera panelAxis;
   
    [SerializeField] private TextMeshPro panelText;

    [SerializeField] public Canvas canvas;
#pragma warning enable



    public enum OrbPanelPosition { Inner, Outer }
    private OrbPanelPosition panelPos;
    public OrbPanelPosition PanelPosition
    {
        get
        {
            return panelPos;
        }
        set
        {
            panelPos = value;
            switch (panelPos)
            {
                case OrbPanelPosition.Inner:
                    SetPanelPosAndParent(innerOffset);
                    rootAxis.enabled = true;
                    
                    panelAxis.enabled = false;
                    
                    break;
                case OrbPanelPosition.Outer:
                    SetPanelPosAndParent(outerOffset);
                    rootAxis.StopRotating();
                    rootAxis.enabled = false;
                    panelAxis.enabled = true;
                   
                    GetComponentInParent<LookAtParent>()?.RotateTowardsParent();
                    

                    break;
                default:
                    break;
            }
        }
    }



    public void UpdatePanelPos(float time)
    {
        StartCoroutine(WaitAndCheckHierarchy(time));
    }


    private IEnumerator WaitAndCheckHierarchy(float time)
    {
        yield return new WaitForSeconds(time);
        if (this.transform.parent.GetComponent<OrbBase>().IsGrabbed == true)
        {
            PanelPosition = OrbPanelPosition.Inner;
        }
        else
        {
            PanelPosition = OrbPanelPosition.Outer;

        }
    }
    
    private void SetPanelPosAndParent(Transform tr)
    {
        if (tr != null)
        {
            pivotPoint.transform.position = tr.position;
            pivotPoint.transform.SetParent(tr);
        }
        else Debug.Log("<b>[OrbPanelHandler]</b> No panel offset Transform set!");
    }

    public void SetTransparency(float val)
    {
        panelText.color = new Color(panelText.color.r, panelText.color.g, panelText.color.b, val);
    }
}
