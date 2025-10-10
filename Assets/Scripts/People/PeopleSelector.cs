// 파일 이름: PeopleSelector.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class PeopleSelector : MonoBehaviour
{
    // 인스펙터에서 방송을 보낼 채널을 연결할 슬롯
    public PeopleActorEventChannelSO OnPeopleSelectedChannel;
    public VoidEventChannelSO OnDeselectedChannel;
    public DraggableSkullEventChannelSO OnSkullSelectedChannel;

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
           // ★ 선택 로직 확장
            if (hit.collider != null)
            {
                // 1. 살아있는 백성을 먼저 확인
                PeopleActor actor = hit.collider.GetComponent<PeopleActor>();
                if (actor != null)
                {
                    OnPeopleSelectedChannel.RaiseEvent(actor);
                    return; // 보고했으니 임무 종료
                }

                // 2. 백성이 아니라면, 유골인지 확인
                DraggableSkull skull = hit.collider.GetComponent<DraggableSkull>();
                if (skull != null)
                {
                    OnSkullSelectedChannel.RaiseEvent(skull);
                    return; // 보고했으니 임무 종료
                }
            }

            // 3. 아무것도 해당되지 않으면 선택 해제 방송
            OnDeselectedChannel.RaiseEvent();
        }
    }

}