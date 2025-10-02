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

    // --- 내부 변수 (수정 필요 없음) ---
    private Vector3 offset;
    private bool isDragging = false;
    private bool isOverRiver = false;
    private Coroutine deathCoroutine;
    private Animator anim;
    private Mover spriteMover;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteMover = GetComponent<Mover>();
    }

    void OnMouseDown()
    {
        if (isOverRiver) return;

        offset = transform.position - GetMouseWorldPos();
        isDragging = true;

        if (deathCoroutine != null)
        {
            StopCoroutine(deathCoroutine);
            deathCoroutine = null;
        }

        if (anim != null)
        {
            ResetAllStateBools();
            anim.SetTrigger("OnHang");
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

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
            deathCoroutine = StartCoroutine(DieInRiver());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("River"))
        {
            isOverRiver = true;
            spriteMover.moveSpeed = 0.5f;
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
            spriteMover.moveSpeed = 3f;

            if (anim != null)
            {
                anim.SetBool("IsSwimming", false);
            }

            if (deathCoroutine != null)
            {
                StopCoroutine(deathCoroutine);
                deathCoroutine = null;
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
            
            

            // 5. 애니메이션이 끝날 때까지 기다림 (애니메이션 길이를 1초로 가정)
            //    만약 애니메이션 길이가 다르다면 이 숫자를 맞춰주세요.
            yield return new WaitForSeconds(1f);

            // 6. 애니메이션이 끝난 후 오브젝트 파괴
            PeopleManager.Instance.DespawnPerson(this.gameObject);
        }
    }
}