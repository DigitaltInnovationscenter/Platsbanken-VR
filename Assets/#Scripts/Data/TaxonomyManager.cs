using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static TaxonomyClasses;
using UnityEditor;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Net;
using System.Security.Policy;
using Platsbanken;

public class TaxonomyManager : MonoBehaviour
{
    private static TaxonomyManager _instance;
    public static TaxonomyManager Instance { get { return _instance; } }

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

    //public GameObject prefabOccupationField;
    //public GameObject prefabOccupationGroup;
    //public GameObject prefabOccupation;
    //public GameObject prefabCompetenceConcept;

    public Dictionary<string, OccupationField> occupationFields = new Dictionary<string, OccupationField>();
    public Dictionary<string, SSYK> ssykIndexed = new Dictionary<string, SSYK>();

    public List<GameObject> competenceConceptObjects = new List<GameObject>();

    public void Init()
    {
        ImportSSYK();
        ImportOccupationsFields();
    }

    private void ImportSSYK()
    {
        StreamReader reader = new StreamReader("Assets/#Data/SSYKTaxonomy.json");
        string json = reader.ReadToEnd().Replace("/", "_").Replace("-", "_");
        json = "{\"ssyks\":" + json + "}";
        TaxonomyClasses.SSYKs sSYKs = JsonUtility.FromJson<TaxonomyClasses.SSYKs>(json);

        foreach(SSYK ssyk in sSYKs.ssyks)
        {
            ssykIndexed.Add(ssyk.taxonomy_ssyk_code_2012, ssyk);
        }

       
    }

    private void ImportOccupationsFields()
    {
        StreamReader reader = new StreamReader("Assets/#Data/occupations.json");
        string json = reader.ReadToEnd();
        json = "{\"occupationAlls\":" + json + "}";
        TaxonomyClasses.OccupationAlls occupationAlls = JsonUtility.FromJson<TaxonomyClasses.OccupationAlls>(json);

        foreach (OccupationAll occupationAll in occupationAlls.occupationAlls)
        {
            OccupationField occupationField = new OccupationField();
            if(!occupationFields.ContainsKey(occupationAll.OccupationField))
            {
                occupationField.occupationField = occupationAll.OccupationField;
                occupationFields.Add(occupationAll.OccupationField, occupationField);
            } else
            {
                occupationField = occupationFields[occupationAll.OccupationField];
                OccupationGroup occupationGroup = new OccupationGroup();
                if(!occupationField.occupationGroups.ContainsKey(occupationAll.OccupationGroup))
                {
                    occupationGroup.occupationGroup = occupationAll.OccupationGroup;
                    occupationGroup.SSYK = occupationAll.SSYK.ToString();
                    if(ssykIndexed.ContainsKey(occupationGroup.SSYK))
                        occupationGroup.conceptID = ssykIndexed[occupationGroup.SSYK].taxonomy_id;
                    occupationField.occupationGroups.Add(occupationAll.OccupationGroup, occupationGroup);
                } else
                {
                    occupationGroup = occupationField.occupationGroups[occupationAll.OccupationGroup];
                    Occupation occupation = new Occupation();
                    if(!occupationGroup.occupations.ContainsKey(occupationAll.Occupation))
                    {
                        occupation.occupationName = occupationAll.Occupation;
                        occupationGroup.occupations.Add(occupationAll.Occupation, occupation);
                    }
                }
            }
        }
    }

    public async Task<TaxonomyClasses.SkillConcepts> GetCompetensWords(string conceptID)
    {
        string query = $"{{  concepts(type: \"ssyk-level-4\", id: \"{ conceptID }\") {{    preferred_label    id    skills: related {{      id      preferred_label      type      broader(type: \"skill-headline\") {{        preferred_label      }}    }}  }}}}";
        

        string requestURL = "https://taxonomy.api.jobtechdev.se/v1/taxonomy/graphql?query=" + UnityWebRequest.EscapeURL(query, System.Text.Encoding.UTF8);
       

        HttpWebRequest request = HttpWebRequest.CreateHttp(requestURL);
        request.Headers.Add("api-key:" + ApplicationManager.Instance.jobTechAPIKey);
        WebResponse response = await request.GetResponseAsync();


        Stream responseStream = response.GetResponseStream();
        string json = new StreamReader(responseStream).ReadToEnd();

       

        TaxonomyClasses.SkillConcepts skillConcepts = JsonUtility.FromJson<TaxonomyClasses.SkillConcepts>(json);

        return skillConcepts;
    }

    public Occupation CreateOccupation(string occupationName)
    {
        return new Occupation
        {
            occupationName = occupationName
        };
    }
}
