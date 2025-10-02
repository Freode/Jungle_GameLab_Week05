using UnityEngine;
using System.Collections;

public class Draggable : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("강에 빠진 후 몇 초 뒤에 사라질지 설정합니다.")]
    public float timeToDieInRiver = 5f;

    [Tooltip("오브젝트의 현재 상태를 나타냅니다. (예: 0 for Idle, 1 for Mining)")]
    public int currentState = 0;

    [Header("애니메이터 파라미터 이름")]
    [Tooltip("Hanging 상태를 제외한 모든 행동 상태의 Bool 파라미터 이름을 적어주세요.")]
    public string[] stateParameterNames = { "IsWalking", "IsMining", "IsSwimming" };

    [Header("흔들기 감지")]
    [Tooltip("이 값 이상의 '흔들림 에너지'가 모이면 이벤트가 발생")]
    public float shakeThreshold = 20f;
    [Tooltip("움직임에 따라 에너지가 얼마나 민감하게 쌓일지 결정")]
    public float shakeSensitivity = 0.01f;
    [Tooltip("가만히 있을 때, 에너지가 초당 얼마나 감소할지 결정")]
    public float shakeDecayRate = 20f;
    [Tooltip("흔들기 이벤트 발생 후 다음 감지까지 필요한 대기 시간.")]
    public float shakeCooldown = 3.0f;

    // --- 내부 변수 (수정 필요 없음) ---
    private Vector3 offset;
    private bool isDragging = false;
    private bool isOverRiver = false;
    private Coroutine deathCoroutine;
    private Animator anim;

    // 흔들기 감지용 내부 변수
    private Vector3 lastPosition;
    private float lastVelocityX = 0f;
    private float currentShakeEnergy = 0f;
    private bool isShakeOnCooldown = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;

        // 흔들기 감지 변수 초기화
        lastPosition = transform.position;
        currentShakeEnergy = 0f;
        lastVelocityX = 0f;

        if (deathCoroutine != null)
        {
            StopCoroutine(deathCoroutine);
            deathCoroutine = null;
            Debug.Log("강에서 구출했습니다!");
        }

        if (anim != null)
        {
            ResetAllStateBools();
            anim.SetInteger("PreviousState", currentState);
            anim.SetTrigger("OnHang");
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;

            // 흔들기 감지 로직
            DetectShaking();
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // 드래그가 끝나면 흔들림 에너지 초기화
        currentShakeEnergy = 0f;

        if (anim != null)
        {
            anim.SetTrigger("OnDrop");
        }

        if (isOverRiver)
        {
            if (anim != null)
            {
                anim.SetBool("IsSwimming", true);
            }
            Debug.Log("강에 버려졌습니다! 곧 사라집니다...");
            deathCoroutine = StartCoroutine(DieInRiver());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("River"))
        {
            isOverRiver = true;
        }
        else if (other.CompareTag("Jail"))
        {
            Debug.Log("감옥에 들어갔습니다!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("River"))
        {
            isOverRiver = false;
            
            if (anim != null)
            {
                anim.SetBool("IsSwimming", false);
            }

            if (deathCoroutine != null)
            {
                StopCoroutine(deathCoroutine);
                deathCoroutine = null;
                Debug.Log("강에서 빠져나와 목숨을 건졌습니다!");
            }
        }
        else if (other.CompareTag("Jail"))
        {
            Debug.Log("감옥에서 나왔습니다!");
        }
    }
    
    void ResetAllStateBools()
    {
        foreach (string paramName in stateParameterNames)
        {
            anim.SetBool(paramName, false);
        }
    }
    
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    
    // [핵심 수정] DieInRiver 코루틴 변경
    IEnumerator DieInRiver()
    {
        // 1. 설정된 시간만큼 기다림
        yield return new WaitForSeconds(timeToDieInRiver);

        // 2. 시간이 지난 후에도 여전히 강 위에 있는지 최종 확인
        if (isOverRiver)
        {
            // 3. 죽음이 확정되면 더 이상 드래그할 수 없도록 이 스크립트를 비활성화
            this.enabled = false; 
            
            // 4. 죽는 애니메이션 재생
            if(anim != null)
            {
                anim.SetTrigger("OnSwimDeath");
            }
            
            Debug.Log("첨벙! 죽는 중...");

            // 5. 애니메이션이 끝날 때까지 기다림 (애니메이션 길이를 1초로 가정)
            //    만약 애니메이션 길이가 다르다면 이 숫자를 맞춰주세요.
            yield return new WaitForSeconds(1f); 

            // 6. 애니메이션이 끝난 후 오브젝트 파괴
            Destroy(gameObject);
        }
    }

    //흔들기를 감지하는 핵심 로직
    void DetectShaking()
    {
        // 현재 프레임의 속도 계산
        Vector3 currentVelocity = (transform.position - lastPosition) / Time.deltaTime;

        // X축(좌우) 방향이 이전 프레임과 반대일 때 에너지를 더함 (핵심!)
        if (Mathf.Sign(currentVelocity.x) != Mathf.Sign(lastVelocityX) && lastVelocityX != 0)
        {
            // 속도가 빠를수록 더 많은 에너지를 얻음
            currentShakeEnergy += Mathf.Abs(currentVelocity.x) * shakeSensitivity;
        }

        // 에너지를 서서히 감소시킴
        currentShakeEnergy -= shakeDecayRate * Time.deltaTime;
        currentShakeEnergy = Mathf.Max(0, currentShakeEnergy); // 에너지가 0 밑으로 내려가지 않도록 함

        // 에너지가 임계값을 넘으면 이벤트 발생!
        if (currentShakeEnergy >= shakeThreshold && isShakeOnCooldown == false)
        {
            OnShakeDetected();
            currentShakeEnergy = 0f; // 이벤트 발생 후 에너지 초기화
        }

        // 현재 위치와 속도를 다음 프레임 계산을 위해 저장
        lastPosition = transform.position;
        if (Mathf.Abs(currentVelocity.x) > 0.1f) // 아주 작은 움직임은 무시
        {
            lastVelocityX = currentVelocity.x;
        }
    }

    // [추가] 흔들기가 감지되었을 때 실행될 함수
    void OnShakeDetected()
    {
        isShakeOnCooldown = true;
        StartCoroutine(ShakeCooldownCoroutine());

        Debug.LogWarning("흔들기 감지! 이벤트가 발생했습니다!");
        // 여기에 원하는 이벤트를 추가하세요.
        // 예: 캐릭터의 충성도가 떨어진다거나, 어지러움 상태가 되는 등
        // anim.SetTrigger("OnDizzy");
    }

    // 쿨타임 관리 코루틴
    IEnumerator ShakeCooldownCoroutine()
    {
        // 설정된 쿨타임 시간만큼 기다림
        yield return new WaitForSeconds(shakeCooldown);

        // 쿨타임이 끝나면 플래그를 다시 false로 변경
        isShakeOnCooldown = false;
        Debug.Log("흔들기 쿨타임이 종료되었습니다.");
    }
}