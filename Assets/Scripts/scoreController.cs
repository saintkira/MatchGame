using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreController : MonoBehaviour
{
    // Start is called before the first frame update
    public Button playAgainBtn;
    public Button returnToTitleBtn;
    public Text scoreTxt;

    void Start()
    {
        scoreTxt.text = ScoreModel.Instance.Score.ToString();
        playAgainBtn.onClick.AddListener(playAgain);
        returnToTitleBtn.onClick.AddListener(returnToTitle);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void returnToTitle()
    {
        Loader.LoadScene(Loader.SceneList.TitleScene);
    }
    public void playAgain()
    {
        Loader.LoadScene(Loader.SceneList.GamePlay);
    }
}
