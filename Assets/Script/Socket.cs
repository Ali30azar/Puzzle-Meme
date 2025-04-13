using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class Socket : MonoBehaviour
    {
        public string pieceTag;
        public int neighbor;
        public Position position;
        public GameObject root;
        [SerializeField] private GameObject particle;
        [SerializeField] private GameObject stack;

        private void Awake()
        {
            stack = GameObject.FindGameObjectWithTag("ParticleStak");
            particle = Resources.Load<GameObject>("Particles");
        }

        public enum Position
        {
            Right,
            Left,
            Down,
            Up
        }

        public void SetRoot(GameObject r)
        {
            root = r;
        }

        public void SetPos(Position p)
        {
            position = p;
        }

        public void SetTag(string sTag)
        {
            pieceTag = sTag;
        }

        public void SetNeighbor(int n)
        {
            neighbor = n;
        }

        public string GetTag()
        {
            return pieceTag;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Socket>())
                if (other.GetComponent<Socket>().pieceTag == neighbor.ToString())
                {
                    if (position == Position.Right && other.GetComponent<Socket>().position == Position.Left)
                    {
                        MakeIsland(other);
                        Instantiate(particle, transform.position, quaternion.identity, stack.transform);
                        // gameObject.GetComponent<BoxCollider2D>().enabled = false;
                        Destroy(gameObject);
                    }

                    if (position == Position.Down && other.GetComponent<Socket>().position == Position.Up)
                    {
                        MakeIsland(other);
                        Instantiate(particle, transform.position, quaternion.identity, stack.transform);
                        // gameObject.GetComponent<BoxCollider2D>().enabled = false;
                        Destroy(gameObject);
                    }
                }
        }

        private void MakeIsland(Component other)
        {
            Island islandA = null;
            Island islandB = null;
            int groupA = -1, groupB = -1;

            PuzzlePiece pieceA = root.GetComponent<PuzzlePiece>();
            PuzzlePiece pieceB = other.GetComponent<Socket>().root.GetComponent<PuzzlePiece>();

            Transform socketA = transform; // The current socket triggering the connection
            Transform socketB = other.transform; // The other socket being triggered

            // Calculate the correct offset so the sockets align
            Vector3 offset = socketA.position - socketB.position;


            if (pieceB.isInIsland)
            {
                // Move all pieces in the island so they stay connected
                islandB = GroupMovementSystem.Instance.islandsGroups[pieceB.groupIsland];
                foreach (GameObject obj in islandB.islands)
                {
                    obj.transform.position += offset;
                }
            }
            else
            {
                // Move only pieceB
                pieceB.transform.position += offset;
            }


//           Find existing islands
            if (pieceA.isInIsland)
            {
                groupA = pieceA.groupIsland;
                islandA = GroupMovementSystem.Instance.islandsGroups[groupA];
            }

            if (pieceB.isInIsland)
            {
                groupB = pieceB.groupIsland;
                islandB = GroupMovementSystem.Instance.islandsGroups[groupB];
            }


//           Case 1: Neither piece is in an island
            if (!pieceA.isInIsland && !pieceB.isInIsland)
            {
                Island newIsland = new Island();
                newIsland.islands.Add(root);
                newIsland.islands.Add(other.GetComponent<Socket>().root);

                pieceA.isInIsland = true;
                pieceB.isInIsland = true;

                GroupMovementSystem.Instance.islandsGroups.Add(newIsland);
                int newIndex = GroupMovementSystem.Instance.islandsGroups.Count - 1;
                pieceA.groupIsland = newIndex;
                pieceB.groupIsland = newIndex;
            }

//           Case 2: One piece is in an island, the other is not
            else if (islandA != null && islandB == null)
            {
                islandA.islands.Add(other.GetComponent<Socket>().root);
                pieceB.isInIsland = true;
                pieceB.groupIsland = groupA;
            }
            else if (islandB != null && islandA == null)
            {
                islandB.islands.Add(root);
                pieceA.isInIsland = true;
                pieceA.groupIsland = groupB;
            }

//           Case 3: Both pieces are in different islands, merge them
            else if (groupA != groupB)
            {
                if (groupA < groupB)
                {
                    islandA?.islands.AddRange(islandB.islands);
                    // if (islandB != null)
                    //     foreach (GameObject p in islandB.islands)
                    //         p.GetComponent<PuzzlePiece>().groupIsland = groupA;
                    GroupMovementSystem.Instance.islandsGroups.RemoveAt(groupB);
                }
                else
                {
                    islandB?.islands.AddRange(islandA.islands);
                    // if (islandA != null)
                    //     foreach (GameObject p in islandA.islands)
                    //         p.GetComponent<PuzzlePiece>().groupIsland = groupB;
                    GroupMovementSystem.Instance.islandsGroups.RemoveAt(groupA);
                }

                ReOrderIslands();
            }

            int i = 0;
            foreach (var island in GroupMovementSystem.Instance.islandsGroups)
            {
                foreach (var p in GroupMovementSystem.Instance.islandsGroups[i].islands)
                {
                    p.GetComponent<SpriteRenderer>().sortingOrder = GroupMovementSystem.Instance.order;
                }

                i++;
            }
        }

        private void ReOrderIslands()
        {
            for (int k = 0; k < GroupMovementSystem.Instance.islandsGroups.Count; k++)
            {
                foreach (var part in GroupMovementSystem.Instance.islandsGroups[k].islands)
                {
                    part.GetComponent<PuzzlePiece>().groupIsland = k;
                    // print(k);
                }
            }
        }
    }
}