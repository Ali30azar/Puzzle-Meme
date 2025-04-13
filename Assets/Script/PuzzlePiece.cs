using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Script
{
    public class PuzzlePiece : MonoBehaviour
    {
        public string pieceTag;
        public string pieceStatus = "Movable";

        [SerializeField] private GameObject moveParent;
        [SerializeField] private GameObject stableParent;
        [SerializeField] private GameObject selectedPiece;
        [SerializeField] private GameObject socketPac;
        [SerializeField] private GameObject socketPacHolder;
        [SerializeField] private GameObject neighborPos;
        public int[] neighbors = new int[4];
        public bool isInIsland;
        public int groupIsland;

        private bool isTouchingScreen = false;

        private void Awake()
        {
            gameObject.transform.position = new Vector3(Random.Range(-7f, 7f), Random.Range(-3f, 3f), 0);
        }

        private void Start()
        {
            groupIsland = -1;
            moveParent = GameObject.FindWithTag("Movable");
            stableParent = GameObject.FindWithTag("Stable");

            var transform1 = transform;
            socketPacHolder = Instantiate(socketPac, transform1.position, Quaternion.identity, transform1);
            GroupMovementSystem.Instance.packs.Add(socketPacHolder);
            pieceTag = name.Remove(0, 12);
            FindNeighbors();
        }

        private void FindNeighbors()
        {
            Vector2 size = GetComponent<SpriteRenderer>().bounds.size;
            float length = size.x;
            float height = size.y;
            var everyPieceX = length;
            var everyPieceY = height;

            const int gridWidth = 5;
            const int gridSize = 25;
            const int invalidIndex = 77;

            var thisPiece = int.Parse(pieceTag);

            neighbors[1] = (thisPiece + 1) % gridWidth == 0 ? invalidIndex : thisPiece + 1;
            neighbors[3] = invalidIndex;
            neighbors[2] = (thisPiece + gridWidth < gridSize) ? thisPiece + gridWidth : invalidIndex;
            neighbors[0] = invalidIndex;

            var down = thisPiece + 5;
            if (down <= 24)
            {
                neighbors[2] = down;
            }

            foreach (Transform child in socketPacHolder.transform)
            {
                child.GetComponent<Socket>().SetTag(thisPiece.ToString());

                if (child.CompareTag("Right_Socket"))
                {
                    child.GetComponent<Socket>().SetRoot(gameObject);
                    child.GetComponent<Socket>().SetNeighbor(neighbors[1]);
                    child.GetComponent<Socket>().SetPos(Socket.Position.Right);
                }
                else if (child.CompareTag("Down_Socket"))
                {
                    child.GetComponent<Socket>().SetRoot(gameObject);
                    child.GetComponent<Socket>().SetNeighbor(neighbors[2]);
                    child.GetComponent<Socket>().SetPos(Socket.Position.Down);
                }
                else if (child.CompareTag("Up_Socket"))
                {
                    child.GetComponent<Socket>().SetRoot(gameObject);
                    child.GetComponent<Socket>().SetNeighbor(invalidIndex);
                    child.GetComponent<Socket>().SetPos(Socket.Position.Up);
                }
                else if (child.CompareTag("Left_Socket"))
                {
                    child.GetComponent<Socket>().SetRoot(gameObject);
                    child.GetComponent<Socket>().SetNeighbor(invalidIndex);
                    child.GetComponent<Socket>().SetPos(Socket.Position.Left);
                }
            }
        }

        private void Update()
        {
            if (pieceStatus == "Movable")
            {
                if (IsTouchStart() && !isTouchingScreen)
                {
                    isTouchingScreen = true;
                    selectedPiece = null;

                    if (Camera.main != null)
                    {
                        Vector2 touchPosition = GetTouchPosition();
                        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

                        if (hit.collider != null && hit.collider.gameObject == gameObject)
                        {
                            selectedPiece = gameObject;

                            foreach (Transform child in transform)
                            {
                                if (child.gameObject.CompareTag("Puzzle"))
                                {
                                    child.gameObject.GetComponent<SpriteRenderer>().sortingOrder =
                                        GroupMovementSystem.Instance.SetOrder();
                                }
                            }

                            if (groupIsland >= 0)
                            {
                                foreach (var itsFriends in GroupMovementSystem.Instance.islandsGroups[groupIsland]
                                    .islands)
                                {
                                    foreach (Transform child in itsFriends.transform)
                                    {
                                        if (child.gameObject.CompareTag("Puzzle"))
                                        {
                                            child.gameObject.GetComponent<SpriteRenderer>().sortingOrder =
                                                GroupMovementSystem.Instance.order;
                                        }
                                    }

                                    // itsFriends.GetComponent<SpriteRenderer>().sortingOrder =
                                    //     GroupMovementSystem.Instance.order;
                                    itsFriends.transform.SetParent(gameObject.transform);
                                }
                            }
                        }
                    }

                    foreach (var p in GroupMovementSystem.Instance.packs)
                    {
                        p.gameObject.SetActive(false);
                    }
                    
                    // foreach (Transform child in socketPacHolder.transform)
                    // {
                    //     child.GetComponent<BoxCollider2D>().enabled = false;
                    // }
                }

                if (IsTouching() && selectedPiece != null)
                {
                    if (Camera.main != null)
                    {
                        Vector2 touchPosition = GetTouchPosition();
                        selectedPiece.transform.position = touchPosition;
                    }
                }

                if (IsTouchEnd())
                {
                    isTouchingScreen = false;
                    foreach (var p in GroupMovementSystem.Instance.packs)
                    {
                       p.SetActive(true);
                    }
                    
                    // foreach (Transform child in socketPacHolder.transform)
                    // {
                    //     child.GetComponent<BoxCollider2D>().enabled = true;
                    // }

                    if (groupIsland >= 0)
                    {
                        foreach (var itsFriends in GroupMovementSystem.Instance.islandsGroups[groupIsland].islands)
                        {
                            itsFriends.transform.SetParent(stableParent.transform);
                        }
                    }
                }
            }
        }

        private Vector2 GetTouchPosition()
        {
            if (Application.isEditor)
                return Camera.main.ScreenToWorldPoint(Input.mousePosition);
            else if (Input.touchCount == 1)
                return Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            return Vector2.zero;
        }

        private bool IsTouchStart()
        {
            return Application.isEditor
                ? Input.GetMouseButtonDown(0)
                : (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began); //
        }

        private bool IsTouching()
        {
            return Application.isEditor
                ? Input.GetMouseButton(0)
                : (Input.touchCount == 1 && (Input.GetTouch(0).phase == TouchPhase.Moved ||
                                             Input.GetTouch(0).phase == TouchPhase.Stationary)); // touch == 1;
        }

        private bool IsTouchEnd()
        {
            return Application.isEditor
                ? Input.GetMouseButtonUp(0)
                : (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended); //
        }


        public void ChangeStatus()
        {
            StartCoroutine(MakeMovable());
        }

        IEnumerator MakeMovable()
        {
            yield return new WaitForSeconds(1);
            pieceStatus = "Movable";
        }
    }
}