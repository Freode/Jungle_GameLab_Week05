using UnityEngine;
using System.Collections;

/// <summary>
/// 드래그 가능하며, 강에 놓으면 즉시 파괴되는 해골 전용 스크립트입니다.
/// </summary>
public class DraggableSkull : MonoBehaviour
{
    [Header("Deceased Info")]
    [SerializeField] private string deceasedName;
    [SerializeField] private int ageAtDeath;
    [SerializeField] private JobType jobAtDeath;
    [SerializeField] private int loyaltyAtDeath;

    [Header("Despawn Settings")]
    public float despawnDelay = 60f; // 이 시간(초) 뒤에 소멸
    private bool isPreserved = false; // 보존 상태 플래그
    private Coroutine despawnCoroutine;
    // --- 내부 변수 (수정 필요 없음) ---
    private Vector3 offset;
    private bool isDragging = false;
    private bool isOverRiver = false;

    // TODO: 나중에 마우스 툴팁을 추가할 경우 여기에 관련 변수와 로직을 추가합니다.
    // [Tooltip("마우스 오버 시 표시될 툴팁 텍스트")]
    // public string tooltipText = "💀 앗, 해골이다!";
    // ★ PeopleActor로부터 정보를 받는 함수
    public void Initialize(PeopleActor actor)
    {
        deceasedName = actor.DisplayName;
        ageAtDeath = actor.Age;
        jobAtDeath = actor.Job;
        loyaltyAtDeath = actor.Loyalty;
        // 필요하다면 더 많은 정보를 여기에 기록할 수 있습니다.
    }

    void Start()
    {
        despawnCoroutine = StartCoroutine(DespawnCoroutine());
    }
    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(despawnDelay);

        // 시간이 다 되었을 때, '보존' 상태가 아니라면
        if (!isPreserved)
        {
            // 스스로를 파괴하여 소멸
            Destroy(gameObject);
        }
    }
    // ★ 3. 소멸 코루틴의 내용을 단순화합니다.
    private IEnumerator DespawnCoroutine()
    {
        // 정해진 시간만큼 기다렸다가
        yield return new WaitForSeconds(despawnDelay);
        // 소멸시킵니다.
        Destroy(gameObject);
    }
    public void SetPreservation(bool preserve)
    {
        isPreserved = preserve;

        // "보존하라"는 명령을 받았다면
        if (isPreserved)
        {
            // 현재 진행 중인 소멸 절차가 있다면, 즉시 중단(사면)합니다.
            if (despawnCoroutine != null)
            {
                StopCoroutine(despawnCoroutine);
                despawnCoroutine = null;
            }
            //Debug.Log($"{deceasedName}의 유골을 영구히 보존합니다.");
        }
        // "보존을 해제하라"는 명령을 받았다면
        else
        {
            // 혹시 모르니 이전 절차는 중단하고,
            if (despawnCoroutine != null) StopCoroutine(despawnCoroutine);
            
            // '완전히 새로운' 소멸 절차를 처음부터 다시 시작합니다.
            despawnCoroutine = StartCoroutine(DespawnCoroutine());

            //Debug.Log($"{deceasedName}의 유골 보존을 해제합니다. 소멸 절차를 새로 시작합니다.");
        }
    }

    // ★ UI가 정보를 읽어갈 수 있도록 public getter 추가
    public string DeceasedName => deceasedName;
    public int AgeAtDeath => ageAtDeath;
    public JobType JobAtDeath => jobAtDeath;
    public int LoyaltyAtDeath => loyaltyAtDeath;
    public bool IsPreserved => isPreserved;

    void OnMouseDown()
    {
        // 마우스 클릭 시 드래그 시작 준비
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    void OnMouseDrag()
    {
        // 드래그 중 오브젝트 위치 업데이트
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // 강 위에 놓았을 경우 즉시 파괴
        if (isOverRiver)
        {
            //Debug.Log("해골을 강에 놓았습니다. 영원히 사라집니다... 🌊");
            // 오브젝트 즉시 파괴
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // "River" 태그를 가진 콜라이더에 진입
        if (other.CompareTag("River"))
        {
            isOverRiver = true;
        }
        // TODO: 감옥 관련 로직이 해골에도 필요하다면 여기에 추가하세요.
        /*
        else if (other.CompareTag("Jail"))
        {
             Debug.Log("해골이 감옥에 들어갔습니다!");
        }
        */
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // "River" 태그를 가진 콜라이더에서 벗어남
        if (other.CompareTag("River"))
        {
            isOverRiver = false;
        }
        // TODO: 감옥 관련 로직이 해골에도 필요하다면 여기에 추가하세요.
        /*
        else if (other.CompareTag("Jail"))
        {
             Debug.Log("해골이 감옥에서 나왔습니다!");
        }
        */
    }

    /// <summary>
    /// 마우스의 현재 스크린 좌표를 월드 좌표로 변환하여 반환합니다.
    /// </summary>
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        // Z축 깊이를 오브젝트의 현재 Z 깊이로 설정
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        // 스크린 좌표를 월드 좌표로 변환
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}