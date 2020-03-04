using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MessagePopup : MonoBehaviour
{
    public static MessagePopup Instance;
    public Image messagePopup;
    public Button okBtn;

    private Action handler;
    private void Awake()
    {
        Instance = this;
        Hide();
    }
    public void Show(Action handler)
    {
        this.handler = handler;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        okBtn.onClick.AddListener(OnClickButtonHandler);
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        okBtn.onClick.RemoveListener(OnClickButtonHandler);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        this.handler = null;
    }

    private void OnClickButtonHandler()
    {
        handler?.Invoke();
    }
}
