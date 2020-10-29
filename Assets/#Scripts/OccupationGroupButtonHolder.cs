using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupationGroupButtonHolder : MonoBehaviour
{
#pragma warning disable
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Vector3 buttonDistanace;
    [SerializeField] private int maxNumberOfButtons;
    [SerializeField] private Transform buttonRoot;
#pragma warning enable
    private List<GameObject> buttons = new List<GameObject>();


    
    private void Awake()
    {
        
        //StarButtonsTest();
    }

    public void GenerateButtons(List<SkillsToAdsManager.OccupationGroupResult> occupationGroupList)
    {
        ClearButtons();
        for (int i = 0; i < occupationGroupList.Count; i++)
        {
            int max;
            if (occupationGroupList.Count < maxNumberOfButtons)
            {
                max = occupationGroupList.Count;
            }
            else max = maxNumberOfButtons;

            if (i <= maxNumberOfButtons)
            {
              
                InstantiateButton(occupationGroupList[i].preferred_label, occupationGroupList[i].id, i, max);

            }
        }
    }

    public void StarButtonsTest()   //THIS IS FOR TESTING ONLY. REMOVE THIS AND USE THE GENERATEBUTTONS METHOD ABOVE WITH REAL DATA INSTEAD
    {
        for (int i = 0; i < maxNumberOfButtons; i++)
        {
           
            string title = "Yrkesgruppsnamn som kan vara väldigt jobbigt långt med jättelångaordsomsitterihop flerastyckenpåradeftervarandra...";
            string id = "XXXXXXX";
            InstantiateButton(title, id, i, maxNumberOfButtons);

        }
    }

    private void InstantiateButton(string titleText, string id, int i, int numberOfButtons) 
    {
        GameObject button = Instantiate(buttonPrefab, buttonRoot.transform);
        button.transform.position = buttonRoot.position + buttonRoot.transform.right * ((buttonDistanace.x * i) + ((numberOfButtons - 1) * (0.5f * -buttonDistanace.x)));
        OccupationGroupButton OGB = button.GetComponent<OccupationGroupButton>();
        OGB.titleText = titleText;
        OGB.occupationGroupID = id;
        OGB.buttonHolder = this;
        OGB.UpdateTextWithNumberOfAds();
        buttons.Add(button);
    }

    public void SetActiveButton(OccupationGroupButton activeButton)
    {
        foreach (GameObject button in buttons)
        {
            OccupationGroupButton OGB = button.GetComponent<OccupationGroupButton>();
            if (OccupationGroupButton.Equals(OGB, activeButton))
            {
                OGB.ActivateButton();
            }
            else
            {
                OGB.InactivateButton();
            }
        }
    }

    public void ClearButtons()
    {
        if (buttons.Count > 0)  // Destroys previous objects and replace with new ones
        {
            foreach (GameObject obj in buttons)
            {
                Destroy(obj);
            }
            buttons.Clear();
        }
    }
}
