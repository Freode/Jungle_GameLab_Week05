using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AcquireGoldAmountUI : MonoBehaviour
{
    public Image imageGold;
    public TextMeshProUGUI textAmount;

    public Sprite criticalGoldImage;
    public Sprite normalGoldImage;

    [SerializeField] float maxTime = 0.5f;

    // 금 획득량 표기 시작
    public void AcquireGold(string amount, Vector3 startPos, Vector3 endPos, Color color)
    {
        textAmount.color = color;
        textAmount.text = "+" + amount;

        // 금 이미지 조정
        if (color == Color.red)
            imageGold.sprite = criticalGoldImage;
        else
            imageGold.sprite = normalGoldImage;

        StartCoroutine(AnimGold(startPos, endPos));
    }

    // 금 획득량 애님 업데이트
    IEnumerator AnimGold(Vector3 startPos, Vector3 endPos)
    {
        Vector3 curPos = startPos;
        float curTime = 0f;

        // 위치 변경
        while (Vector3.Distance(curPos, endPos) > 0.05f)
        {
            curPos = startPos + (endPos - startPos) * (1f - (maxTime - curTime) / maxTime);
            gameObject.transform.position = curPos;
            curTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        CompleteAnimGold();
    }

    private void CompleteAnimGold()
    {
        ObjectPooler.Instance.ReturnObject(gameObject);
    }
}
