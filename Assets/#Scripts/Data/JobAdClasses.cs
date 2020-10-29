using System;
using System.Collections.Generic;

public class JobAdClasses
{

    

    [Serializable]
    public class Total
    {
        public int value;
    }

    [Serializable]
    public class FreetextConcepts
    {
    }

    [Serializable]
    public class Description
    {
        public string text;
        public string text_formatted;
        public object company_information;
        public object needs;
        public object requirements;
        public string conditions;
    }

    [Serializable]
    public class EmploymentType
    {
        public string concept_id;
        public string label;
        public string legacy_ams_taxonomy_id;
    }

    [Serializable]
    public class SalaryType
    {
        public string concept_id;
        public string label;
        public string legacy_ams_taxonomy_id;
    }

    [Serializable]
    public class Duration
    {
        public string concept_id;
        public string label;
        public string legacy_ams_taxonomy_id;
    }

    [Serializable]
    public class WorkingHoursType
    {
        public string concept_id;
        public string label;
        public string legacy_ams_taxonomy_id;
    }

    [Serializable]
    public class ScopeOfWork
    {
        public object min;
        public object max;
    }

    [Serializable]
    public class Employer
    {
        public object phone_number;
        public object email;
        public string url;
        public string organization_number;
        public string name;
        public string workplace;
    }

    [Serializable]
    public class ApplicationDetails
    {
        public object information;
        public string reference;
        public object email;
        public bool via_af;
        public string url;
        public object other;
    }

    [Serializable]
    public class DrivingLicense
    {
        public string concept_id;
        public string label;
        public string legacy_ams_taxonomy_id;
    }

    [Serializable]
    public class Occupation
    {
        public string concept_id;
        public string label;
        public string legacy_ams_taxonomy_id;
    }

    [Serializable]
    public class OccupationGroup
    {
        public string concept_id;
        public string label;
        public string legacy_ams_taxonomy_id;
    }

    [Serializable]
    public class OccupationField
    {
        public string concept_id;
        public string label;
        public string legacy_ams_taxonomy_id;
    }

    [Serializable]
    public class WorkplaceAddress
    {
        public string municipality;
        public string municipality_code;
        public string municipality_concept_id;
        public string region;
        public string region_code;
        public string region_concept_id;
        public string country;
        public string country_code;
        public string country_concept_id;
        public string street_address;
        public string postcode;
        public string city;
        public List<double> coordinates;
    }

    [Serializable]
    public class WorkExperience
    {
        public int weight;
        public string concept_id;
        public string label;
        public string legacy_ams_taxonomy_id;
    }

    [Serializable]
    public class MustHave
    {
        public List<object> skills;
        public List<object> languages;
        public List<WorkExperience> work_experiences;
    }

    [Serializable]
    public class Skill
    {
        public int weight;
        public string concept_id;
        public string label;
        public string legacy_ams_taxonomy_id;
    }

    [Serializable]
    public class NiceToHave
    {
        public List<Skill> skills;
        public List<object> languages;
        public List<object> work_experiences;
    }

    [Serializable]
    public class Hit
    {
        public int relevance;
        public string id;
        public string external_id;
        public string webpage_url;
        public string logo_url;
        public string headline;
        public DateTime application_deadline;
        public int number_of_vacancies;
        public Description description;
        public EmploymentType employment_type;
        public SalaryType salary_type;
        public string salary_description;
        public Duration duration;
        public WorkingHoursType working_hours_type;
        public ScopeOfWork scope_of_work;
        public object access;
        public Employer employer;
        public ApplicationDetails application_details;
        public bool experience_required;
        public bool access_to_own_car;
        public bool driving_license_required;
        public List<DrivingLicense> driving_license;
        public Occupation occupation;
        public OccupationGroup occupation_group;
        public OccupationField occupation_field;
        public WorkplaceAddress workplace_address;
        public MustHave must_have;
        public NiceToHave nice_to_have;
        public DateTime publication_date;
        public DateTime last_publication_date;
        public bool removed;
        public object removed_date;
        public string source_type;
        public object timestamp;
    }

    [Serializable]
    public class Ads
    {
        public Total total;
        public int positions;
        public int query_time_in_millis;
        public int result_time_in_millis;
        public List<object> stats;
        public FreetextConcepts freetext_concepts;
        public List<Hit> hits;
    }

}
