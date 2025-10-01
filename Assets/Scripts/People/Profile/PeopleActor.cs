using UnityEngine;

[DisallowMultipleComponent]
public class PeopleActor : MonoBehaviour
{
    [Header("Runtime Values")]
    [SerializeField] private int age;
    [SerializeField, Range(0, 100)] private int loyalty;
    [SerializeField] private string displayName;
    [SerializeField] private JobType job;

    // 읽기 전용 프로퍼티
    public int Age => age;
    public int Loyalty => loyalty;
    public string DisplayName => displayName;
    public JobType Job => job;

    // 스포너가 호출: 값 주입
    public void Apply(PeopleValue v)
    {
        if (v == null) return;
        age = Mathf.Max(0, v.age);
        loyalty = Mathf.Clamp(v.loyalty, 0, 100);
        displayName = string.IsNullOrWhiteSpace(v.name) ? "NPC" : v.name;
        job = v.job;

        // TODO: UI 갱신, 애니메이션 파라미터, AI 초기화 등 여기서 수행
    }

    // 풀로 돌려질 때 상태 초기화가 필요하면
    void OnDisable()
    {
        // 가벼운 기본화(선택)
        age = 0;
        loyalty = 0;
        displayName = null;
        job = JobType.None;
    }
}
