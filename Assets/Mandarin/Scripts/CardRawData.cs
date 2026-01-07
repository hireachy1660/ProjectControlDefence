using UnityEngine;

public enum ECardType
{
    Unit,   // 유닛
    Tower   // 타워
}

[System.Serializable]
public class CardRawData   // 태스트용 데이터 클래스
{
    public string cardName;
    public int cost;
    public string prefabPath;   // 리소스폴더 내의 실제 파일 경로
    public Sprite cardIcon;

    public ECardType cardType;
}