using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private Button play;

        [SerializeField] private Button exit;

        [SerializeField] private Button levels;

        [SerializeField] private Button options;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] menuSounds;

        private int _i;


        void Start()
        {
            play.onClick.AddListener(Play);
            exit.onClick.AddListener(ExitG);
            levels.onClick.AddListener(Level);
            options.onClick.AddListener(Option);
            // if (_i == menuSounds.Length) _i = 0;
            // audioSource.PlayOneShot(menuSounds[_i]);
            // _i++;
        }

        private void Play()
        {
            audioSource.PlayOneShot(menuSounds[0]);
            Invoke(nameof(PlayRecentLevel), 0.3f);
        }

        private void PlayRecentLevel()
        {
            SceneManager.LoadScene("Level1Scene");
        }

        private void Option()
        {
            audioSource.PlayOneShot(menuSounds[0]);
            Invoke(nameof(Options), 0.2f);
        }

        private void Options()
        {
            print("Options");
        }

        private void Level()
        {
            audioSource.PlayOneShot(menuSounds[2]);
            Invoke(nameof(SelectLevel), 0.2f);
        }

        private void SelectLevel()
        {
            SceneManager.LoadScene("LevelScene");
        }

        private void ExitG()
        {
            audioSource.PlayOneShot(menuSounds[1]);
            Invoke(nameof(ExitGame), 0.2f);
        }

        private void ExitGame()
        {
            Application.Quit();
            print("Quit");
        }
    }
}