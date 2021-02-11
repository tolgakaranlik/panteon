using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIController : MonoBehaviour
{
    public TextMeshProUGUI TxtCountDown;
    public TextMeshProUGUI PercentPainted;
    public TextMeshProUGUI TxtOrder;
    public Button BtnRetry;
    public Image ImgLife1;
    public Image ImgLife2;
    public Image ImgLife3;
    public Slider PrgLevel;

    // Start is called before the first frame update
    void Start()
    {
        ClearCountDown();

        BtnRetry.gameObject.SetActive(false);
        PercentPainted.gameObject.SetActive(false);
    }

    public void StartCountDown()
    {
        TxtCountDown.gameObject.SetActive(true);
        StartCoroutine(CountNumber("3", 0));
        StartCoroutine(CountNumber("2", 1));
        StartCoroutine(CountNumber("1", 2));
        StartCoroutine(CountNumber("GO!", 3));
    }

    public void FailUI()
    {
        TxtCountDown.text = "";
        TxtCountDown.gameObject.SetActive(true);
        StartCoroutine(CountNumber("FAILED", 0));

        BtnRetry.gameObject.SetActive(true);
    }

    public void SuccessUI()
    {
        TxtCountDown.text = "";
        TxtCountDown.gameObject.SetActive(true);
        StartCoroutine(CountNumber("HURAY!", 0));
        StartCoroutine(StartPainting());
    }

    public void ClearCountDown()
    {
        TxtCountDown.gameObject.SetActive(true);
        TxtCountDown.text = "";
    }

    public void UpdateLives(int lives)
    {
        ImgLife1.gameObject.SetActive(lives >= 1);
        ImgLife2.gameObject.SetActive(lives >= 2);
        ImgLife3.gameObject.SetActive(lives >= 3);
    }

    public void UpdateProgress(float z)
    {
        PrgLevel.value = z;
    }

    public void OnRetry()
    {
        ClearCountDown();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetTexturePainted(float percent)
    {
        PercentPainted.gameObject.SetActive(true);
        PercentPainted.text = "Painted percent: " + percent.ToString("N2");
    }

    public void DisplayOrder(int order)
    {
        switch(order)
        {
            case 1:
                TxtOrder.text = order + "st";
                break;
            case 2:
                TxtOrder.text = order + "nd";
                break;
            case 3:
                TxtOrder.text = order + "rd";
                break;
            default:
                TxtOrder.text = order + "th";
                break;
        }
    }

    IEnumerator CountNumber(string title, float delay)
    {
        yield return new WaitForSeconds(delay);

        TxtCountDown.text = title;
        TxtCountDown.gameObject.transform.localScale = Vector3.zero;
        TxtCountDown.gameObject.transform.DOScale(Vector3.one, 0.3f);
    }

    IEnumerator StartPainting()
    {
        yield return new WaitForSeconds(3);

        StartCoroutine(CountNumber("", 0));
    }
}
