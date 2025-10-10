// 파일 이름: PeopleSelector.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class PeopleSelector : MonoBehaviour
{
    // 인스펙터에서 방송을 보낼 채널을 연결할 슬롯
    public PeopleActorEventChannelSO OnPeopleSelectedChannel;
    public VoidEventChannelSO OnDeselectedChannel;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // 만약 UI 위에 있다면, 아무것도 하지 않고 함수를 즉시 종료.
                return;
            }
            RaycastHit2D hit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Input.mousePosition));

            PeopleActor actor = null;
            if (hit.collider != null)
            {
                // 부딪힌 오브젝트에서 PeopleActor 컴포넌트를 일단 찾아봅니다.
                actor = hit.collider.GetComponent<PeopleActor>();
            }

            // ★ 로직 변경 ★
            // 만약 actor를 성공적으로 찾았다면 (사람을 클릭했다면)
            if (actor != null)
            {
                // '선택됨' 채널에 방송
                OnPeopleSelectedChannel.RaiseEvent(actor);
            }
            // 그게 아니라면 (땅, 빈 곳, 사람이 아닌 것을 클릭했다면)
            else
            {
                // '선택 해제됨' 채널에 방송
                OnDeselectedChannel.RaiseEvent();
            }
        }
    }
}