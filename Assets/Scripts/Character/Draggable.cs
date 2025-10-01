using UnityEngine;
using System.Collections;

public class Draggable : MonoBehaviour
{
    [Tooltip("강에 빠진 후 몇 초 뒤에 사라질지 설정합니다.")]
    public float timeToDieInRiver = 5f;

    [Tooltip("오브젝트의 현재 상태를 나타냅니다. (예: 0 for Idle, 1 for Mining)")]
    // 이 값은 이 오브젝트의 상태를 제어하는 다른 스크립트에서 업데이트해주어야 합니다.
    public int currentState = 0;

    private Vector3 offset;
    private bool isDragging = false;
    private bool isOverRiver = false;
    private Coroutine deathCoroutine;
    private Animator anim;

    // 스크립트가 활성화될 때 Animator 컴포넌트를 미리 찾아둡니다.
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // 오브젝트를 마우스로 클릭했을 때 호출됩니다.
    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;

        // 만약 강 위에서 죽어가고 있었다면, 구출 처리
        if (deathCoroutine != null)
        {
            StopCoroutine(deathCoroutine);
            deathCoroutine = null;
            Debug.Log("강에서 구출했습니다!");
        }

        // 애니메이터가 설정되어 있다면
        if (anim != null)
        {
            // 집기 직전의 상태(Idle인지 Mining인지)를 애니메이터에 알려줍니다.
            anim.SetInteger("PreviousState", currentState);
            
            // OnHang 트리거를 발동시켜 Any State -> Hanging 전환을 실행합니다.
            anim.SetTrigger("OnHang");
        }
    }

    // 마우스를 드래그하는 동안 계속 호출됩니다.
    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    // 마우스 클릭을 놓았을 때 호출됩니다.
    void OnMouseUp()
    {
        isDragging = false;

        // 애니메이터가 설정되어 있다면
        if (anim != null)
        {
            // OnDrop 트리거를 발동시켜 Hanging -> 이전 상태로의 전환을 실행합니다.
            anim.SetTrigger("OnDrop");
        }

        // 만약 강 위에 놓았다면, 사망 코루틴 시작
        if (isOverRiver)
        {
            Debug.Log("강에 버려졌습니다! 곧 사라집니다...");
            deathCoroutine = StartCoroutine(DieInRiver());
        }
    }

    // 마우스 위치를 게임 월드 좌표로 변환합니다.
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    // 트리거 영역에 들어갔을 때 호출됩니다.
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

    // 트리거 영역에서 빠져나왔을 때 호출됩니다.
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("River"))
        {
            isOverRiver = false;

            // 강에서 빠져나왔을 때, 죽음 카운트다운을 멈춥니다.
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

    // 지정된 시간 후 강에서 오브젝트를 파괴하는 코루틴입니다.
    IEnumerator DieInRiver()
    {
        yield return new WaitForSeconds(timeToDieInRiver);

        // 시간이 지난 후에도 여전히 강 위에 있다면 파괴
        if (isOverRiver)
        {
            Debug.Log("첨벙! 사라졌습니다.");
            Destroy(gameObject);
        }
    }
}