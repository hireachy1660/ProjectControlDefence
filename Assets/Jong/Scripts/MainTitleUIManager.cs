using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainTitleUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject panelTitle;
    [SerializeField]
    private GameObject panelStage;
    [SerializeField]
    private Button btnStart;
    [SerializeField]
    private Button btnStage;
    [SerializeField]
    private Button btnBack;

    private void Awake()
    {
        btnStart.onClick.AddListener(() => ChangePanelStage());
        btnStage.onClick.AddListener(() => LoadScene());
        btnBack.onClick.AddListener(() => ChangePanelTitle());
    }

    private void ChangePanelStage()
    {
        panelTitle.SetActive(false);
        panelStage.SetActive(true);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void ChangePanelTitle()
    {
        panelStage.SetActive(false);
        panelTitle.SetActive(true);
    }
}
