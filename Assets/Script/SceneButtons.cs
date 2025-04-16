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
    }
}
