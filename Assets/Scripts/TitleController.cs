using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;


public class TitleController : MonoBehaviour
{
    // Start is called before the first frame update public Button startBtn;
    [SerializeField] Text playText;
    [SerializeField] Image logo;

    [SerializeField] Vector3 logoBegin;
    [SerializeField] Vector3 logoEnd;
    [SerializeField] Vector3 logoRotate;
    void Start()
    {

        playText.DOFade(0, 2f).SetLoops(-1, LoopType.Yoyo);
        logo.rectTransform.anchoredPosition = logoBegin;
        logo.rectTransform.DOAnchorPos(logoEnd, 0.5f).SetDelay(0.5f).SetEase(Ease.InOutElastic);
        logo.transform.DORotate(logoRotate,1f).SetLoops(-1, LoopType.Incremental);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Loader.LoadScene(Loader.SceneList.GamePlay);
        }
    }


}
