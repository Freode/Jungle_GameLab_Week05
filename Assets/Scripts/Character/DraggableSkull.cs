using UnityEngine;

/// <summary>
/// 드래그 가능하며, 강에 놓으면 즉시 파괴되는 해골 전용 스크립트입니다.
/// </summary>
public class DraggableSkull : MonoBehaviour
{
    // --- 내부 변수 (수정 필요 없음) ---
    private Vector3 offset;
    private bool isDragging = false;
    private bool isOverRiver = false;

    // TODO: 나중에 마우스 툴팁을 추가할 경우 여기에 관련 변수와 로직을 추가합니다.
    // [Tooltip("마우스 오버 시 표시될 툴팁 텍스트")]
    // public string tooltipText = "💀 앗, 해골이다!";


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
            Debug.Log("해골을 강에 놓았습니다. 영원히 사라집니다... 🌊");
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