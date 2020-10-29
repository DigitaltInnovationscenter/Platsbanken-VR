using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AdCubeInfo : MonoBehaviour
{
    #pragma warning disable
    [SerializeField] private TextMeshPro textMeshProUngrabbedHeadline;
    [SerializeField] private TextMeshProUGUI textMeshProHeadline;
    [SerializeField] private TextMeshProUGUI textMeshProDescription;

    [SerializeField] private TextMeshProUGUI textMeshProOccupationField;
    [SerializeField] private TextMeshProUGUI textMeshProOccupationGroup;
    [SerializeField] private TextMeshProUGUI textMeshProOccupation;

    [SerializeField] private TextMeshProUGUI textMeshProEmployer;
    [SerializeField] private TextMeshProUGUI textMeshProAddress;
    #pragma warning enable

    private Camera mainCamera;
    private AdCube adCube;

    public string TextUngrabbedHeadline
    {
        get
        {
            return textMeshProUngrabbedHeadline.text;
        }
        set
        {
            textMeshProUngrabbedHeadline.text = value;
        }
    }
    public string TextHeadline
    {
        get
        {
            return textMeshProHeadline.text;
        }
        set
        {
            textMeshProHeadline.text = value;
        }
    }
    public string TextDescription
    {
        get
        {
            return textMeshProDescription.text;
        }
        set
        {
            textMeshProDescription.text = value;
        }
    }

    public string TextOccupationField
    {
        get
        {
            return textMeshProOccupationField.text;
        }
        set
        {
            textMeshProOccupationField.text = value;
        }
    }

    public string TextOccupationGroup
    {
        get
        {
            return textMeshProOccupationGroup.text;
        }
        set
        {
            textMeshProOccupationGroup.text = value;
        }
    }

    public string TextOccupation
    {
        get
        {
            return textMeshProOccupation.text;
        }
        set
        {
            textMeshProOccupation.text = value;
        }
    }

    public string TextEmployer
    {
        get
        {
            return textMeshProEmployer.text;
        }
        set
        {
            textMeshProEmployer.text = value;
        }
    }

    public string TextAddress
    {
        get
        {
            return textMeshProAddress.text;
        }
        set
        {
            textMeshProAddress.text = value;
        }
    }

    private void Awake()
    {
        adCube = GetComponent<AdCube>();
    }

    public void ResetText()
    {
        textMeshProDescription.pageToDisplay = 0;
    }

    public void Description_OnButtonNext()
    {
        if(textMeshProDescription.pageToDisplay < textMeshProDescription.textInfo.pageCount)
        {
            textMeshProDescription.pageToDisplay++;
        }
    }

    public void Description_OnButtonPrev()
    {
        if(textMeshProDescription.pageToDisplay > 1)
        {
            textMeshProDescription.pageToDisplay--;
        }
    }

    private void Update()
    {
        if (!adCube.IsGrabbed) return;

        if (mainCamera == null) mainCamera = Camera.main;

        foreach(TextMeshProUGUI text in GetComponentsInChildren<TextMeshProUGUI>())
        {
            float angle = Vector3.Dot(mainCamera.transform.forward, text.gameObject.transform.forward);
            if (angle > 0.5f)
            {
                Color newColor = text.color;
                newColor.a = 1f;
                text.color = newColor;
             
                //text.color = Color.white;
            } else
            {
                Color newColor = text.color;
                newColor.a = (angle + 1f) / 1.5f;
                text.color = newColor;
                
                //text.color = new Color(0.3f, 0.3f, 0.4f);
            }

        }

    }
}
