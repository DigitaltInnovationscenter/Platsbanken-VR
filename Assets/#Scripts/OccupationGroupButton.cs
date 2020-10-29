using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;


public class OccupationGroupButton : MonoBehaviour
{

    public string titleText { get { return titleTextField.text; } set { titleTextField.text = value; } }
    [HideInInspector] public bool isPressed;
    [HideInInspector] public OccupationGroupButtonHolder buttonHolder;
    public string occupationGroupID { get; set; }

    #pragma warning disable
    [SerializeField] private TextMeshProUGUI titleTextField;
    [SerializeField] private Sprite buttonDefaultSprite;
    [SerializeField] private Sprite buttonActiveSprite;
    [SerializeField] private Image buttonImage;
    [SerializeField] private AudioSource audioSource;
    #pragma warning enable

    public void PressButton()
    {
        isPressed = !isPressed;
        if (isPressed)
        {
            buttonHolder.SetActiveButton(this);
            Platsbanken.ApplicationManager.Instance.DisplayAdPanel(occupationGroupID);
            ScreenManager.Instance.HighlightSkills(SkillsToAdsManager.Instance.GetSkillIDs(occupationGroupID));
           
        }
        else
        {
            InactivateButton();
           
        }
        audioSource.Play();
       
    }

    public async void UpdateTextWithNumberOfAds() {
        var num = await SkillsToAdsManager.Instance.GetNumberOfJobAdsForOccupationGroup(occupationGroupID);
        titleText = titleText + " (" + num + ")";
    }

    public void InactivateButton()
    {
        isPressed = false;
        buttonImage.sprite = buttonDefaultSprite;
    }

    public void ActivateButton()
    {
        isPressed = true;
        buttonImage.sprite = buttonActiveSprite;
    }
}

