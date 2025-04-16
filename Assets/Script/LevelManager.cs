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
    }
}
