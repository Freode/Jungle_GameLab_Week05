using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class PeopleActor : MonoBehaviour
{
    [Header("Death Settings")]
    public GameObject skullPrefab; // 죽었을 때 생성할 해골 프리팹
    public PeopleActorEventChannelSO OnActorDiedChannel; // 죽음을 알릴 방송 채널
    private bool isDying = false;

    [Header("Runtime Values")]
    [SerializeField] private int id;               // ★ 세션 내 고유 ID
    [SerializeField] private int age;
    [SerializeField, Range(0, 100)] private int loyalty;
    [SerializeField] private string displayName;
    [SerializeField] private JobType job;
    [SerializeField] private CarrierItem carrierItem;

    public int Id => id;
    public int Age => age;
    public int Loyalty => loyalty;
    public string DisplayName => displayName;
    public JobType Job => job;
    public CarrierItem CarrierItem => carrierItem;


    // ★ '죽음'을 명하는 함수
    public void Die()
    {
        if (isDying) return; // 이미 죽음이 예고되었다면 무시

        isDying = true;
        float delay = Random.Range(0f, 10f); // 0~10초 사이의 랜덤한 시간
        StartCoroutine(DieAfterDelay(delay));
    }

    // ★ 지정된 시간 뒤에 죽음을 실행하는 코루틴
    private IEnumerator DieAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 1. 해골 생성 및 정보 전달
        if (skullPrefab != null)
        {
            GameObject skullObj = Instantiate(skullPrefab, transform.position, Quaternion.identity);
            DraggableSkull skull = skullObj.GetComponent<DraggableSkull>();
            if (skull != null)
            {
                skull.Initialize(this); // 해골에게 자신의 정보를 넘겨줌
            }
        }

        // 2. "내가 죽었노라!" 라고 방송
        if (OnActorDiedChannel != null)
        {
            OnActorDiedChannel.RaiseEvent(this);
        }
    }
    
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
        carrierItem = v.carrier;
    }

    public void ApplyJop(JobType _job)
    {
        job = _job;
        return;
    }

    public void SetCarrierItem(CarrierItem _carrierItem)
    {
        carrierItem = _carrierItem;
    }

    public void ChangeName(string newName)
    {
        // 이름이 비어있거나 공백뿐인 경우는 무시하고, 아니라면 displayName을 변경
        if (!string.IsNullOrWhiteSpace(newName))
        {
            displayName = newName;
        }
    }
    public void AddAge(int amount)
    {
        age += amount;
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
        carrierItem = CarrierItem.None;
    }
}
