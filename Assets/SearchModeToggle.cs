using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchModeToggle : MonoBehaviour
{
    #pragma warning disable
    [SerializeField] private TextMeshProUGUI titleTextField;
    [HideInInspector] public bool isPressed;

    [SerializeField] private Sprite buttonDefaultSprite;
    [SerializeField] private Sprite buttonActiveSprite;
    [SerializeField] private Image buttonImage;
    [SerializeField] private AudioSource audioSource;
    #pragma warning enable

    void Start() {
        if(Platsbanken.ApplicationManager.Instance.ShowUnexpectedAds == true) {
            SetPressed();
        } else {
            SetUnpressed();
        }
    }

    public void Toggle() {
        if (!isPressed) {
            SetPressed();
        } else {
            SetUnpressed();
        }
        Platsbanken.ApplicationManager.Instance.ShowUnexpectedAds = isPressed;
        audioSource.Play();
    }

    private void SetPressed() {
        buttonImage.sprite = buttonActiveSprite;
        isPressed = true;
    }

    private void SetUnpressed() {
        isPressed = false;
        buttonImage.sprite = buttonDefaultSprite;
    }

}

