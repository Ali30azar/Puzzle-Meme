using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script
{
    public class SceneButtons : MonoBehaviour
    {
        public void BackToMenu()
        {
            SceneManager.LoadScene("MenuScene");
        }

        public void LevelOne()
        {
            SceneManager.LoadScene("Level1Scene");
        }

        public void LevelTwo()
        {
            SceneManager.LoadScene("Level2Scene");
        }
        
        public void LevelThree()
        {
            Debug.Log("Action Three executed!");
        }
    }
}
