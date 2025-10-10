

// 파일 이름: PeopleInfoUI.cs
using UnityEngine;
using TMPro; // TextMeshPro를 사용하므로 추가
// PeopleInfoUI.cs 파일 상단 (다른 using 선언 아래)
using UnityEngine.Video; // VideoPlayer를 사용하므로 추가
using UnityEngine.UI;

// 이 클래스는 인스펙터에 노출시키기 위한 데이터 묶음입니다.
[System.Serializable]
public class JobVisual
{
    public JobType job;
    public Sprite portrait; // 초상화 (None 직업용)
    public VideoClip video;  // 직업 영상
}

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

    public RawImage portraitOrVideoImage; // 이미지를 표시할 RawImage
    public VideoPlayer videoPlayer;       // 영상을 재생할 VideoPlayer
    public RenderTexture videoRenderTexture; 

    [Header("Job Visuals Data")]
    // ★ 2단계에서 만든 JobVisual 데이터 묶음을 배열로 선언
    public JobVisual[] jobVisuals;

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
        OnDeselectedChannel.OnEventRaised -= HideUI;
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
    
        UpdateVisuals(selectedActor.Job);
    }
    private void UpdateVisuals(JobType job)
    {
        // jobVisuals 배열에서 현재 직업과 일치하는 데이터를 찾습니다.
        JobVisual visualToShow = null;
        foreach (var visual in jobVisuals)
        {
            if (visual.job == job)
            {
                visualToShow = visual;
                break;
            }
        }
        
        // 일치하는 데이터를 찾지 못했다면 아무것도 하지 않고 함수 종료
        if (visualToShow == null)
        {
            portraitOrVideoImage.enabled = false;
            videoPlayer.enabled = false;
            return;
        }

        // --- 데이터 종류에 따라 다르게 처리 ---

        // 1. 재생할 비디오가 지정되어 있다면 (Miner, Carver 등)
        if (visualToShow.video != null)
        {
            portraitOrVideoImage.texture = videoRenderTexture; 
            portraitOrVideoImage.enabled = true; // RawImage를 켜고
            videoPlayer.enabled = true;          // VideoPlayer도 켭니다.
            videoPlayer.clip = visualToShow.video; // 재생할 클립을 지정하고
            videoPlayer.isLooping = true;        // 반복 재생 설정
            videoPlayer.Play();                  // 재생!
        }
        // 2. 비디오는 없지만 초상화(Sprite)가 지정되어 있다면 (None 직업)
        else if (visualToShow.portrait != null)
        {
            videoPlayer.enabled = false;         // 비디오는 확실히 끄고
            portraitOrVideoImage.enabled = true; // RawImage는 켠 다음
            // RawImage는 Texture를 받으므로, Sprite에서 texture를 추출해서 넣어줍니다.
            portraitOrVideoImage.texture = visualToShow.portrait.texture;
        }
        // 3. 둘 다 지정되지 않았다면 (Worker 직업 등)
        else
        {
            // 둘 다 꺼서 아무것도 보이지 않게 합니다.
            portraitOrVideoImage.enabled = false;
            videoPlayer.enabled = false;
        }
    }
    private void HideUI()
    {
        infoPanel.SetActive(false);
    }
}