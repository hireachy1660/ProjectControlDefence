using UnityEngine;

public class UIManager : MonoBehaviour 
{

    public void AddCardToHaondUI(CardRawData data)
    {
        Debug.Log($"[UI] {data.cardName} 카드를 보관함 UI에 추가하였습니다");
    }
    public void HideSelectionPanel()
    {
        Debug.Log("[UI] 카드 선택 패널을 숨깁니다.");
    }
}
