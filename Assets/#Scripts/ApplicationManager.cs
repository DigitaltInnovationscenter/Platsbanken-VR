using UnityEngine;

namespace Platsbanken
{
    public class ApplicationManager : MonoBehaviour
    {
        private static ApplicationManager _instance;
        public static ApplicationManager Instance { get { return _instance; } }

        private bool showUnexpectedAds = false;
        public bool ShowUnexpectedAds {
            get {
                return showUnexpectedAds;
            }
            set {
                showUnexpectedAds = value;
                UpdateOccupationGroupButtons();
            }
        }

        public TaxonomyClasses.OccupationGroup CurrentOccupationGroup;

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

        public string jobTechAPIKey;
        [SerializeField] private int activeSkillsThreshold = 5;
        public int ActiveSkillsThreshold => activeSkillsThreshold;

        [SerializeField] private int searchSpanInDays = 14;
        public int SearchSpanInDays => searchSpanInDays;

        void Start()
        {
            TaxonomyManager.Instance.Init();
            SkillsToAdsManager.Instance.Init();
            OrbsManager.Instance.Init();
            LogoManager.Instance.Init();
        }

        public void OnOrbGrabbed(OrbBase orb)
        {
            switch(orb.GetType().FullName)
            {
                case "OrbLevel0":
                    ScreenManager.Instance.InitScreen();
                    CurrentOccupationGroup = null;
                    break;
                case "OrbLevel1":
                    CurrentOccupationGroup = null;
                    break;
                case "OrbLevel2":
                    CurrentOccupationGroup = ((OrbLevel2)orb).OccupationGroup;
                    break;
                case "OrbLevel3":
                    break;

            }
        }

        private void UpdateOccupationGroupButtons()
        {
            int c = ScreenManager.Instance.GetAllActiveSkills().Count;
            if (c >= activeSkillsThreshold) {
                ScreenManager.Instance.DisplayOccupationGroups();
            } else {
                //Clear buttons if there are any
                ScreenManager.Instance.ClearOccupationGroups();
            }
        }

        public void OnSkillPlacedOnScreen()
        {
            UpdateOccupationGroupButtons();
        }

        public void DisplayAdPanel(string occupationGroupID)
        {
            AdPanelManager.Instance.LoadAdsForOccupationGroup(occupationGroupID);
        }
    }
}

