using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogoManager : MonoBehaviour
{
    private static LogoManager _instance;
    public static LogoManager Instance { get { return _instance; } }

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
    [SerializeField] private TextMeshPro textMeshScreen;
    [SerializeField] private List<string> screenTexts = new List<string>();
#pragma warning enable

    private int screenTextCurrent = 0;

    // Start is called before the first frame update
    public void Init()
    {
        StartCoroutine(ScreenTextEnum());
    }

    private IEnumerator ScreenTextEnum()
    {
        while(true)
        {
            string text = screenTexts[screenTextCurrent];
            string textDisplaying = "";
            for(int i = 0; i < text.Length; i++)
            {
                textDisplaying = textDisplaying + text.Substring(i, 1);
                textMeshScreen.text = textDisplaying;
                yield return new WaitForSeconds(0.08f);
            }
            screenTextCurrent++;
            if (screenTextCurrent == screenTexts.Count) screenTextCurrent = 0;
            yield return new WaitForSeconds(3f);
        }
    }

}
