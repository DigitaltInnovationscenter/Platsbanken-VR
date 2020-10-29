using UnityEngine;

public class AdPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject panelPrefab;

    private static AdPanelManager _instance;
    public static AdPanelManager Instance => _instance;

    private AdPanel adPanel;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void Init()
    {
        if (ScreenManager.Instance.screen == null) return;

    }

    public async void LoadAdsForOccupationGroup(string occupationGroupID) {
        adPanel = ScreenManager.Instance.screen.GetComponent<SkillScreenHandler>().adPanel.GetComponent<AdPanel>();
        adPanel.Show();
       
        await adPanel.LoadAdsForOccupationGroup(occupationGroupID);
        
    }
}