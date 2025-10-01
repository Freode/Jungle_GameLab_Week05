using UnityEngine;
using System.Collections.Generic;

public enum MoveState { Returning, Wandering, Dwelling }

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Mover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float dwellTimeMin = 1f;
    [SerializeField] private float dwellTimeMax = 3f;
    [SerializeField] private float arrivalDistance = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private MoveState currentState = MoveState.Dwelling; // 초기에는 대기 상태로 시작
    private AreaZone currentArea;
    public AreaZone lockedArea; // public으로 변경하여 외부에서 직접 설정 가능
    private Vector2 targetPosition;
    private float dwellTimer;
    private Rigidbody2D rb;
    private Vector2 lastValidPosition;
    private bool wasInsideArea = false;
    private bool isInitialized = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        lastValidPosition = transform.position;
        targetPosition = transform.position; // 초기 목표를 현재 위치로 설정
    }

    private void Start()
    {
        // Start에서 초기화 - lockedArea가 외부에서 설정된 후
        Initialize();
    }

    private void Initialize()
    {
        if (isInitialized) return;

        AreaZone targetArea = lockedArea != null ? lockedArea : currentArea;
        if (targetArea != null)
        {
            wasInsideArea = targetArea.IsPointInside(transform.position);

            // 영역 내부에 있으면 바로 배회 시작
            if (wasInsideArea)
            {
                StartWandering();
            }
            else
            {
                // 영역 외부에 있으면 복귀
                ReturnToArea(targetArea);
            }
        }
        else
        {
            // 영역이 없으면 현재 위치에서 대기
            targetPosition = transform.position;
            currentState = MoveState.Dwelling;
            dwellTimer = Random.Range(dwellTimeMin, dwellTimeMax);
        }

        isInitialized = true;
    }

    private void Update()
    {
        // 초기화가 안 됐으면 다시 시도
        if (!isInitialized)
        {
            Initialize();
        }

        // 영역 이탈 체크 (드래그나 임의 위치 변경 감지)
        CheckIfOutsideArea();

        // Dwelling 상태의 타이머만 Update에서 처리
        if (currentState == MoveState.Dwelling)
        {
            dwellTimer -= Time.deltaTime;
            if (dwellTimer <= 0)
            {
                StartWandering();
            }
        }
    }

    private void FixedUpdate()
    {
        // 물리 기반 이동은 FixedUpdate에서 처리
        switch (currentState)
        {
            case MoveState.Returning:
            case MoveState.Wandering:
                MoveToTarget();
                break;
        }
    }

    private void CheckIfOutsideArea()
    {
        AreaZone targetArea = lockedArea != null ? lockedArea : currentArea;

        if (targetArea != null)
        {
            bool isInside = targetArea.IsPointInside(transform.position);

            // 영역 내부에서 외부로 나간 경우
            if (wasInsideArea && !isInside)
            {
                ReturnToArea(targetArea);
            }

            wasInsideArea = isInside;
        }
    }

    private void MoveToTarget()
    {
        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPosition - currentPos).normalized;
        float distance = Vector2.Distance(currentPos, targetPosition);

        if (distance > arrivalDistance)
        {
            // FixedUpdate에서는 Time.fixedDeltaTime 사용
            Vector2 newPosition = currentPos + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            // 영역 내부에 있을 때만 유효한 위치로 저장
            if (currentArea != null && currentArea.IsPointInside(newPosition))
            {
                lastValidPosition = newPosition;
            }
        }
        else
        {
            // 목표 도착
            if (currentState == MoveState.Returning)
            {
                // 복귀 완료 후 배회 시작
                StartWandering();
            }
            else if (currentState == MoveState.Wandering)
            {
                // 배회 중 목표 도착 시 대기
                StartDwelling();
            }
        }
    }

    private void ReturnToArea(AreaZone area)
    {
        if (area == null) return;

        currentState = MoveState.Returning;
        targetPosition = area.GetRandomPointInside();
        wasInsideArea = false;
    }

    private void StartWandering()
    {
        AreaZone targetArea = lockedArea != null ? lockedArea : currentArea;

        if (targetArea != null)
        {
            currentState = MoveState.Wandering;
            targetPosition = targetArea.GetRandomPointInside();
        }
    }

    private void StartDwelling()
    {
        currentState = MoveState.Dwelling;
        dwellTimer = Random.Range(dwellTimeMin, dwellTimeMax);
    }

    // 영역 고정/해제
    public void LockToArea(AreaZone area)
    {
        lockedArea = area;
        if (area != null)
        {
            isInitialized = false; // 재초기화 필요
            ReturnToArea(area);
        }
    }

    public void UnlockArea()
    {
        lockedArea = null;
    }

    // 외부에서 호출하여 즉시 초기화 (스폰 직후 등)
    public void ForceInitialize()
    {
        isInitialized = false;
        Initialize();
    }

    public bool IsAreaLocked()
    {
        return lockedArea != null;
    }

    public AreaZone GetLockedArea()
    {
        return lockedArea;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        AreaZone area = other.GetComponent<AreaZone>();
        if (area != null)
        {
            // 고정된 영역이 있으면 무시
            if (lockedArea != null) return;

            currentArea = area;
            wasInsideArea = true;

            // 처음 진입 시 배회 시작
            if (currentState != MoveState.Returning)
            {
                StartWandering();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        AreaZone area = other.GetComponent<AreaZone>();
        if (area != null && area == currentArea)
        {
            wasInsideArea = false;
            // 고정된 영역이 없을 때만 currentArea 해제
            if (lockedArea == null)
            {
                currentArea = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        // 현재 상태에 따라 색상 변경
        switch (currentState)
        {
            case MoveState.Returning:
                Gizmos.color = Color.red;
                break;
            case MoveState.Wandering:
                Gizmos.color = Color.green;
                break;
            case MoveState.Dwelling:
                Gizmos.color = Color.yellow;
                break;
        }

        Gizmos.DrawWireSphere(transform.position, 0.3f);

        // 목표 지점 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, targetPosition);
        Gizmos.DrawWireSphere(targetPosition, 0.2f);
    }

    


    // Public getters
    public MoveState GetCurrentState() => currentState;
    public AreaZone GetCurrentArea() => currentArea;
}