// 파일 이름: PeopleInfoUI.cs
using UnityEngine;
using TMPro; // TextMeshPro를 사용하므로 추가

public class PeopleInfoUI : MonoBehaviour
{
    // 인스펙터에서 구독할 방송 채널을 연결할 슬롯
    public PeopleActorEventChannelSO OnPeopleSelectedChannel;
    public VoidEventChannelSO OnDeselectedChannel;

    [Header("UI Elements")]
    public GameObject infoPanel; // UI 패널 전체
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI ageText;
    public TextMeshProUGUI jobText;
    public TextMeshProUGUI loyaltyText;

    // 이 컴포넌트(스크립트)가 활성화될 때 자동으로 호출
    private void OnEnable()
    {
        // 방송 채널의 구독자 명단에 'UpdateUI' 함수를 등록(+=)합니다.
        OnPeopleSelectedChannel.OnEventRaised += UpdateUI;
        OnDeselectedChannel.OnEventRaised += HideUI;
    }

    // 이 컴포넌트가 비활성화될 때 자동으로 호출
    private void OnDisable()
    {
        // 구독자 명단에서 'UpdateUI' 함수를 제거(-=)합니다.
        OnPeopleSelectedChannel.OnEventRaised -= UpdateUI;
    }

    // 방송이 오면 채널이 이 함수를 호출해 줍니다.
    private void UpdateUI(PeopleActor selectedActor)
    {
        // 우선 패널을 켜서 보이게 합니다.
        infoPanel.SetActive(true);

        // 전달받은 selectedActor의 정보로 각 텍스트를 업데이트합니다.
        nameText.text = $"{selectedActor.DisplayName}";
        ageText.text = $"나이: {selectedActor.Age.ToString()}";
        jobText.text = $"직업: {selectedActor.Job.ToString()}";
        loyaltyText.text = $"충성도: {selectedActor.Loyalty.ToString()}";
    }

    private void HideUI()
    {
        infoPanel.SetActive(false);
    }
}