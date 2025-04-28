using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip levelSounds;
        public string url1 = "https://t.me/Ali30azar";
        public string url2 = "https://www.linkedin.com/in/seyed-ali-mozaffari-5b35452ab?utm_source=share&utm_campaign=share_via&utm_content=profile&utm_medium=android_app";
        
        public async void LevelOne()
        {
            audioSource.PlayOneShot(levelSounds);
            await Task.Delay(100);
            SceneManager.LoadScene("Level1Scene");
        }

        public async void LevelTwo()
        {
            audioSource.PlayOneShot(levelSounds);
            await Task.Delay(100);
            SceneManager.LoadScene("Level2Scene");
        }

        public async void LevelThree()
        {
            audioSource.PlayOneShot(levelSounds);
            await Task.Delay(100);
            SceneManager.LoadScene("Level3Scene");
        }

        public async void OpenMyUrl()
        {
            audioSource.PlayOneShot(levelSounds);
            await Task.Delay(100);
            Application.OpenURL(url1);
        } 
        public async void OpenAliUrl()
        {
            audioSource.PlayOneShot(levelSounds);
            await Task.Delay(100);
            Application.OpenURL(url2);
        }
    }
}
