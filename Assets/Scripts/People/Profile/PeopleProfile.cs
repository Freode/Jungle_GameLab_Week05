using UnityEngine;

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
    public bool randomName = true;
    public string[] namePool = new string[] { "Alex", "Blake", "Casey" };
    public string fixedName = "NPC";

    // 프로필에서 새 인스턴스 값 생성
    public PeopleValue Generate()
    {
        var v = new PeopleValue();
        v.age = randomAge ? Random.Range(minAge, maxAge + 1) : fixedAge;
        v.loyalty = randomLoyalty ? Random.Range(minLoyalty, maxLoyalty + 1) : fixedLoyalty;
        v.name = randomName && namePool != null && namePool.Length > 0
            ? namePool[Random.Range(0, namePool.Length)]
            : fixedName;
        return v;
    }
}

// 런타임으로 넘겨줄 값 DTO(구조체/클래스 아무거나)
[System.Serializable]
public class PeopleValue
{
    public int age;
    public int loyalty; // 0~100
    public string name;
}
