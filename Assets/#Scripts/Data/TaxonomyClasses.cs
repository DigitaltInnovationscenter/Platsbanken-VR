using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxonomyClasses
{
    [Serializable]
    public class OccupationAlls
    {
        public List<OccupationAll> occupationAlls;
    }

    [Serializable]
    public class OccupationAll
    {
        public string OccupationField;
        public int SSYK;
        public string OccupationGroup;
        public string Occupation;
    }

    [Serializable]
    public class OccupationField
    {
        public string occupationField;
        public Dictionary<string, OccupationGroup> occupationGroups = new Dictionary<string, OccupationGroup>();
    }

    [Serializable]
    public class OccupationGroup
    {
        public string occupationGroup;
        public string SSYK;
        public string conceptID;
        public Dictionary<string, Occupation> occupations = new Dictionary<string, Occupation>();
    }

    [Serializable]
    public class Occupation
    {
        public string occupationName;
    }

    [Serializable]
    public class SSYKs
    {
        public List<SSYK> ssyks;
    }

    [Serializable]
    public class SSYK
    {
        public string taxonomy_id; 
        public string taxonomy_definition; 
        public string taxonomy_deprecated_legacy_id; 
        public string taxonomy_ssyk_code_2012; 
        public int taxonomy_quality_level; 
        public string taxonomy_preferred_label; 
        public bool taxonomy_deprecated; 
        public string taxonomy_type; 
    }

    [Serializable]
    public class Broader
    {
        public string preferred_label;
    }

    [Serializable]
    public class Skill
    {
        public string id;
        public string preferred_label;
        public string type;
        public List<Broader> broader;
    }

    [Serializable]
    public class Concept
    {
        public string preferred_label;
        public string id;
        public List<Skill> skills;
    }

    [Serializable]
    public class Data
    {
        public List<Concept> concepts;
    }

    [Serializable]
    public class SkillConcepts
    {
        public Data data;
    }

}
