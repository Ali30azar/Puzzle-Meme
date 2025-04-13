using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class EndGame : MonoBehaviour
    {
        [SerializeField] private GameObject parent;
        [SerializeField] private GameObject particle;
        [SerializeField] private GameObject centerPiece;
        [SerializeField] private GroupMovementSystem instance;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip winSounds;


        private bool _end;

        void Start()
        {
            particle = Resources.Load<GameObject>("EdgeParticle");
            instance = GroupMovementSystem.Instance;
        }

        void Update()
        {
            if (!_end)
                if (instance?.islandsGroups?.Count > 0 && instance.islandsGroups[0].islands?.Count == 25)
                {
                    Endgame();
                    _end = true;
                }
        }

        private void WinSound()
        {
            audioSource.Stop();
            audioSource.PlayOneShot(winSounds);
        }

        private void Endgame()
        {
            foreach (var par in GameObject.FindGameObjectsWithTag("particle"))
            {
                Destroy(par);
            }

            centerPiece = GameObject.FindWithTag("Center");
            var p = Instantiate(parent, centerPiece.transform.position, quaternion.identity);
            foreach (var itsFriends in GroupMovementSystem.Instance.islandsGroups[0].islands)
            {
                itsFriends.transform.SetParent(p.transform);
                itsFriends.GetComponent<BoxCollider2D>().enabled = false;
                itsFriends.GetComponent<PuzzlePiece>().pieceStatus = "Stable";
            }

            WinSound();

            p.transform.position = new Vector3(0, 0, 0);
            Instantiate(particle, new Vector3(0, 0, 0), quaternion.identity);

            print("end"); //End check!!
        }
    }
}