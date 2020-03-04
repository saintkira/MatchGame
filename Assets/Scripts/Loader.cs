using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
   public enum SceneList{
       GamePlay,
       TitleScene,
       Score
   };

   public static void LoadScene(SceneList scene)
   {
       SceneManager.LoadSceneAsync(scene.ToString());
   }

}
