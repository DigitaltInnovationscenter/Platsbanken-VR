using Platsbanken;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

// Put this script on a GameObject in the active secene so it can parse the local SkillsWithOccupationGroups.json
// Pass a List<String> of skill ids to GetOccupationGroups() to get occupation groups (sorted by relevance)
// Send those occupation-group IDs into GetJobAdsForOccupationGroup() to fetch relevant ads

public class SkillsToAdsManager : MonoBehaviour
{
    [SerializeField] private int numberOfRandomSkills = 10;

    private static SkillsToAdsManager _instance;
    public static SkillsToAdsManager Instance => _instance;

    private SkillsData skillsData;

    public class OccupationGroupResult
    {
        public string preferred_label;
        public string id;
        public int count;
    }

    private void Awake() 
    {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void Init()
    {
        ImportSkillsData();
        if (numberOfRandomSkills > 0) {
            TestSkillData();
        }
    }

    private async void TestSkillData() {
        var skills = GetRandomSkillIDs(numberOfRandomSkills);
        var results = GetOccupationGroups(skills, 12);
     
        foreach (var result in results) {
            print(result.id + " : " + result.preferred_label + " : count = " + result.count);
            await GetJobAdsForOccupationGroup(result.id);
        }
        

        FindObjectOfType<OccupationGroupButtonHolder>()?.GenerateButtons(results); //Markus Test 1/1

    }

    private void ImportSkillsData() {
        StreamReader reader = new StreamReader("Assets/#Data/SkillsWithOccupationGroups.json");
        var json = reader.ReadToEnd();
        skillsData = JsonUtility.FromJson<SkillsData>(json);
       
    }

    public List<String> GetSkillIDs(string occupationGroupID) {
        var result = new List<string>();
        foreach (var skill in skillsData.data.concepts) {
            foreach (var occGroup in skill.occupation_groups) {
                if (occGroup.id.Equals(occupationGroupID)) {
                    result.Add(skill.id);
                }
            }
        }
        return result;
    }

    private List<String> GetRandomSkillIDs(int number)
    {
        var list = new List<string>();
        var num = skillsData.data.concepts.Count;
        for (int i = 0; i < number; i++) {
            var randomSkill = skillsData.data.concepts[UnityEngine.Random.Range(0, num)];
            list.Add(randomSkill.id);
        }
        return list;
    }

    private Concept GetSkill(string id)
    {
        foreach (var concept in skillsData.data.concepts) {
            if(concept.id.Equals(id)) {
                return concept;
            }
        }
        return null;
    }

    public string GetOccupationTitle(string id) {
        foreach (var skill in skillsData.data.concepts) {
            foreach (var occupation in skill.occupation_groups) {
                if(occupation.id.Equals(id)) {
                    return occupation.preferred_label;
                }
            }
        }
        return "";
    }

    public List<OccupationGroupResult> GetOccupationGroups(List<String> skillIDs, int numberOfResults = 9999)
    {
        var results = new List<OccupationGroupResult>();
        foreach (var skillID in skillIDs) {
            var skillObj = GetSkill(skillID);
            if(skillObj != null) {
                foreach (var group in skillObj.occupation_groups) {
                    var index = results.FindIndex(r => r.id == group.id);
                    if (index >= 0) {
                        results[index].count++;
                    } else {
                        var newResult = new OccupationGroupResult() {
                            preferred_label = group.preferred_label,
                            id = group.id,
                            count = 1
                        };
                        results.Add(newResult);
                    }
                }
            }
        }
        var sortedList = results.OrderByDescending(r => r.count).ToList();
        while(sortedList.Count > numberOfResults) {
            sortedList.RemoveAt(sortedList.Count - 1);
        }
        return sortedList;
    }

    public async Task<List<JobAdClasses.Hit>> GetJobAdsForOccupationGroup(string occGroupID, int numberOfAdsToLoad = 99) {

        var apiUrl = "https://jobsearch.api.jobtechdev.se/search";
        var minutesToSearch = 60 * 24 * ApplicationManager.Instance.SearchSpanInDays;
        var urlRequest = apiUrl
            + "?published-after=" + minutesToSearch
            + "&occupation-group=" + occGroupID
            + "&resdet=" + "full" // use "brief" here to get the ads without body text
            + "&offset=0"
            + "&limit=" + numberOfAdsToLoad;

        HttpWebRequest request = HttpWebRequest.CreateHttp(urlRequest);
        request.Headers.Add("api-key:" + ApplicationManager.Instance.jobTechAPIKey);
        try {
            WebResponse response = await request.GetResponseAsync();
            Stream responseStream = response.GetResponseStream();
            string json = new StreamReader(responseStream).ReadToEnd();
            JobAdClasses.Ads data = JsonUtility.FromJson<JobAdClasses.Ads>(json);
            
            return data.hits;
        } catch (WebException e) {
            Debug.Log("Error when trying to load ads: " + e.Message);
        }
        return new List<JobAdClasses.Hit>();
    }

    public async Task<int> GetNumberOfJobAdsForOccupationGroup(string occGroupID) {
        // Return ONLY the total number of ads for this timespan
        var apiUrl = "https://jobsearch.api.jobtechdev.se/search";
        var minutesToSearch = 60 * 24 * ApplicationManager.Instance.SearchSpanInDays;
        var urlRequest = apiUrl
            + "?published-after=" + minutesToSearch
            + "&occupation-group=" + occGroupID
            + "&resdet=" + "brief" // use "brief" here to get the ads without body text
            + "&offset=0"
            + "&limit=0";

        HttpWebRequest request = HttpWebRequest.CreateHttp(urlRequest);
        request.Headers.Add("api-key:" + ApplicationManager.Instance.jobTechAPIKey);
        try {
            WebResponse response = await request.GetResponseAsync();
            Stream responseStream = response.GetResponseStream();
            string json = new StreamReader(responseStream).ReadToEnd();
            JobAdClasses.Ads data = JsonUtility.FromJson<JobAdClasses.Ads>(json);
            return data.total.value;
        } catch (WebException e) {
            Debug.Log("Error when trying to count ads: " + e.Message);
        }
        return 0;
    }

}

[Serializable]
public class OccupationGroups
{
    public string id;
    public string preferred_label;
    public string type;
}

[Serializable]
public class Concept
{
    public string preferred_label;
    public string id;
    public List<OccupationGroups> occupation_groups;
}

[Serializable]
public class Data
{
    public List<Concept> concepts;
}

[Serializable]
public class SkillsData
{
    public Data data;
}

