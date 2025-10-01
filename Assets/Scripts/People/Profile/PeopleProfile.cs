using UnityEngine;

public enum NameMode { Fixed, Pool, FirstLast }

[CreateAssetMenu(menuName = "People/People Profile", fileName = "PeopleProfile_")]
public class PeopleProfile : ScriptableObject
{
    [Header("Age")]
    public bool randomAge = true;
    [Min(0)] public int minAge = 18;
    [Min(0)] public int maxAge = 60;
    public int fixedAge = 20;

    [Header("Loyalty (0~100)")]
    public bool randomLoyalty = true;
    [Range(0, 100)] public int minLoyalty = 10;
    [Range(0, 100)] public int maxLoyalty = 90;
    [Range(0, 100)] public int fixedLoyalty = 50;

    [Header("Name")]
    public NameMode nameMode = NameMode.Pool;

    // Pool 모드
    public string[] namePool = new string[] { "Alex", "Blake", "Casey" };

    // FirstLast 모드
    public string[] firstNames = new string[]
    {
        "Alex","Blake","Casey","Drew","Evan","Finn",
        "Jamie","Jordan","Taylor","Morgan","Quinn","Riley","Avery","Cameron","Hayden","Parker",
        "Reese","Rowan","Skyler","Sage","Harper","Logan","Kendall","Peyton","Remy","Charlie",
        "Emerson","Dakota","Phoenix","Rory","Micah","Sidney"
    };

    public string[] lastNames = new string[]
    {
        "Smith","Johnson","Williams","Brown","Jones","Miller","Davis","Garcia","Rodriguez","Martinez",
        "Wilson","Anderson","Taylor","Thomas","Moore","Jackson","Martin","Thompson","White","Harris",
        "Clark","Lewis","Robinson","Walker","Young","Allen"
    };


    // Fixed 모드
    public string fixedName = "NPC";

    // 프로필에서 새 인스턴스 값 생성
    public PeopleValue Generate()
    {
        var v = new PeopleValue();

        // Age
        v.age = randomAge ? RandomIntInclusive(minAge, maxAge)
                          : Mathf.Max(0, fixedAge);

        // Loyalty
        v.loyalty = randomLoyalty ? RandomIntInclusive(minLoyalty, maxLoyalty)
                                  : Mathf.Clamp(fixedLoyalty, 0, 100);

        // Name
        v.name = GenerateName();

        return v;
    }

    public string GenerateName()
    {
        switch (nameMode)
        {
            case NameMode.Fixed:
                return string.IsNullOrWhiteSpace(fixedName) ? "NPC" : fixedName;

            case NameMode.Pool:
                if (HasAny(namePool)) return Pick(namePool);
                return string.IsNullOrWhiteSpace(fixedName) ? "NPC" : fixedName;

            case NameMode.FirstLast:
                if (HasAny(firstNames) && HasAny(lastNames))
                    return $"{Pick(firstNames)} {Pick(lastNames)}";
                if (HasAny(namePool)) return Pick(namePool);
                return string.IsNullOrWhiteSpace(fixedName) ? "NPC" : fixedName;
        }
        return "NPC";
    }

    // --- 유틸 ---

    private static bool HasAny(string[] arr) => arr != null && arr.Length > 0;

    private static string Pick(string[] arr) => arr[Random.Range(0, arr.Length)];

    // Unity Random.Range(int,int)는 상한 미포함 → 포함 범위 도우미
    private static int RandomIntInclusive(int min, int max)
    {
        if (min > max) (min, max) = (max, min);
        return Random.Range(min, max + 1);
    }

    private void OnValidate()
    {
        if (minAge > maxAge) (minAge, maxAge) = (maxAge, minAge);
        if (minLoyalty > maxLoyalty) (minLoyalty, maxLoyalty) = (maxLoyalty, minLoyalty);

        fixedAge = Mathf.Max(0, fixedAge);
        fixedLoyalty = Mathf.Clamp(fixedLoyalty, 0, 100);
        minLoyalty = Mathf.Clamp(minLoyalty, 0, 100);
        maxLoyalty = Mathf.Clamp(maxLoyalty, 0, 100);
    }
}

[System.Serializable]
public class PeopleValue
{
    public int age;
    public int loyalty; // 0~100
    public string name;
}
