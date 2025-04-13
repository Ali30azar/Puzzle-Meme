using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Script
{
    public class Spider : MonoBehaviour
    {
        [SerializeField] private GameObject web;
        [SerializeField] private GameObject point;
        [SerializeField] private bool hasPoint;
        [SerializeField] private bool isOnTheStage;
        [SerializeField] private float speed;
        [SerializeField] private float defaultRestTime;
        public int hitCount;
        private bool _cLipIsPlaying;
        private int _counter;
        private Vector3 _target;
        private Vector3 _direction;
        private float _timer;
        bool _rageSound;


        private enum State
        {
            Idle,
            Movement,
            Rage,
            GettingDamage,
            ThrowWeb,
            Sleep
        }

        [SerializeField] private State currentState;
        private Action _currentStateAction;

        [SerializeField] private Animator animator;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] spiderSounds;

        private readonly int _beaten = Animator.StringToHash("Beaten");
        private readonly int _sleep = Animator.StringToHash("Sleep");
        private readonly int _angry = Animator.StringToHash("Angry");
        private readonly int _idle = Animator.StringToHash("Idle");
        private readonly int _downMove = Animator.StringToHash("DownMove");
        private readonly int _castingThreads = Animator.StringToHash("Cast silk");
        private readonly int _upMove = Animator.StringToHash("UpMove");
        private readonly int _wakeUp = Animator.StringToHash("WakeUp");
        private readonly int _goToSleep = Animator.StringToHash("GoToSleep");
        private List<GameObject> _points = new List<GameObject>();

        private void Start()
        {
            SetState(State.Idle);
            SetWebCounter(1);
            SetTimer(3);
        }

        private void Update()
        {
            _currentStateAction?.Invoke();

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        gameObject.GetComponent<CircleCollider2D>().enabled = false;
                        Beaten();
                    }
                }
            }
        }

        private void SetState(State newState)
        {
            currentState = newState;
            switch (currentState)
            {
                case State.Idle:
                    _currentStateAction = Idle;
                    break;
                case State.Movement:
                    _currentStateAction = Movement;
                    break;
                case State.Rage:
                    _currentStateAction = Rage;
                    break;
                case State.GettingDamage:
                    _currentStateAction = GettingDamage;
                    break;
                case State.ThrowWeb:
                    _currentStateAction = CastingThreads;
                    break;
                case State.Sleep:
                    _currentStateAction = Sleep;
                    break;
            }
        }

        private void Idle()
        {
            animator.Play(_idle);
            if (!gameObject.GetComponent<CircleCollider2D>().enabled)
                gameObject.GetComponent<CircleCollider2D>().enabled = true;
            _timer -= Time.deltaTime;
            if (!(_timer < 0)) return;
            SetState(State.Movement);
            SetTimer(Random.Range(3, 7));
        }

        private void MakingDecision()
        {
            if (hitCount < 2)
            {
                var r = Random.Range(1, 5);
                if (r > 2)
                {
                    SetState(State.Sleep);
                    return;
                }
            }

            SetState(State.ThrowWeb);
        }

        private void Movement()
        {
            if (!isOnTheStage)
            {
                StepIn();
            }
            else
            {
                StepOut();
            }
        }

        private void StepIn()
        {
            if (!_cLipIsPlaying)
                audioSource.PlayOneShot(spiderSounds[1]);
            _cLipIsPlaying = true;

            animator.Play(_downMove);

            if (!hasPoint)
            {
                MakePathPoint();
            }

            if (Vector3.Distance(_target, transform.position) < 0.1f)
            {
                hasPoint = false;
                isOnTheStage = true;
                _cLipIsPlaying = false;
                audioSource.Stop(); //

                MakingDecision();
                return;
            }

            transform.Translate(_direction.normalized * (speed * Time.deltaTime), Space.World);
        }

        private void StepOut()
        {
            if (!_cLipIsPlaying)
                audioSource.PlayOneShot(spiderSounds[1]);
            _cLipIsPlaying = true;

            animator.Play(_upMove);

            if (Vector3.Distance(new Vector3(transform.position.x, 7.5f, transform.position.z), transform.position) <
                0.1f)
            {
                hasPoint = false;
                isOnTheStage = false;
                _cLipIsPlaying = false;
                audioSource.Stop(); //

                SetState(State.Idle);
                if (_counter <= 1)
                    SetWebCounter(1);
                return;
            }

            transform.Translate(Vector3.up * ((speed * 3 / 2) * Time.deltaTime), Space.World);
        }

        private void MakePathPoint()
        {
            _target = new Vector3(Random.Range(-8, 9), Random.Range(-4, 5), 0);
            if (_points.Count > 0)
            {
                foreach (var s in _points)
                {
                    Destroy(s);
                }
            }

            var p = Instantiate(point, _target, Quaternion.identity);
            _points.Add(p);
            var transform1 = transform;
            var position1 = p.transform.position;
            var t = new Vector3(position1.x, 7.5f, 0);
            var position = t;
            transform1.position = position;
            _direction = position1 - position;
            hasPoint = true;
        }

        private void CastingThreads()
        {
            if (!_cLipIsPlaying)
                audioSource.PlayOneShot(spiderSounds[2]);
            _cLipIsPlaying = true;
            animator.Play(_castingThreads);
        }

        public void OnSpinning()
        {
            if (_counter <= 0) return;

            var i = new[] {0, -1, 1};

            var one = i[Random.Range(0, 3)];
            var two = i[Random.Range(0, 3)];
            // print(one + "   " + two);
            Instantiate(web, transform.position + new Vector3(one, two, 0), Quaternion.identity);
            _counter--;
        }

        public void AfterCastingThreads()
        {
            if (_counter <= 0)
                SetState(State.Movement);
            hitCount = 0;
            _cLipIsPlaying = false;
        }

        private void Rage()
        {
            if (!_rageSound)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(spiderSounds[Random.Range(3, 7)]);
                _rageSound = true;
            }

            animator.Play(_angry);
        }

        private void OnRageEnd()
        {
            _rageSound = false;
            SetState(State.Movement);
            if (hitCount < 3) return;
            _counter = 3;
        }

        private void GettingDamage()
        {
            animator.Play(_beaten);
        }

        private void Beaten()
        {
            audioSource.Stop();
            audioSource.PlayOneShot(spiderSounds[0]);
            hitCount++;
            SetState(State.GettingDamage);
            print("beaten");
        }

        public void OnDamageEnd()
        {
            _cLipIsPlaying = false;

            isOnTheStage = true;
            if (hitCount >= 3)
            {
                SetState(State.Rage);
                return;
            }

            SetState(State.Movement);
        }

        // private void OnMouseDown()
        // {
        //     Beaten();
        // }

        private void Sleep()
        {
            animator.Play(_sleep);
        }

        private void WakeUp()
        {
            SetState(State.Movement);
        }

        private void SetTimer(int timer)
        {
            _timer = timer;
        }

        private void SetWebCounter(int num)
        {
            _counter = num;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("DownEnd") || other.gameObject.CompareTag("UpEnd"))
            {
                isOnTheStage = other.gameObject.CompareTag("DownEnd");

                hasPoint = false;
                _cLipIsPlaying = false;
                audioSource.Stop();
                SetState(State.Idle);
            }
        }
    }
}