using UnityEngine;

[DisallowMultipleComponent]
public class PeopleActor : MonoBehaviour
{
    [Header("Runtime Values")]
    [SerializeField] private int id;               // ★ 세션 내 고유 ID
    [SerializeField] private int age;
    [SerializeField, Range(0, 100)] private int loyalty;
    [SerializeField] private string displayName;
    [SerializeField] private JobType job;

    public int Id => id;
    public int Age => age;
    public int Loyalty => loyalty;
    public string DisplayName => displayName;
    public JobType Job => job;

    void OnEnable()
    {
        // 스폰될 때마다 새 ID 부여
        id = RuntimeIdGenerator.Next();
    }
     
    public void Apply(PeopleValue v)
    {
        if (v == null) return;
        age = Mathf.Max(0, v.age);
        loyalty = Mathf.Clamp(v.loyalty, 0, 100);
        displayName = string.IsNullOrWhiteSpace(v.name) ? "NPC" : v.name;
        job = v.job;
    }

    void OnDisable()
    {
        // 다음 스폰 시 새 ID를 받게 하려면 0으로 리셋
        id = 0;

        // 선택: 나머지 런타임 상태 리셋
        age = 0;
        loyalty = 0;
        displayName = null;
        job = JobType.None;
    }
}
