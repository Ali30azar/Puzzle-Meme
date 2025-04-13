using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class GroupMovementSystem : MonoBehaviour
    {
        public static GroupMovementSystem Instance { get; private set; }

        [SerializeField] public List<Island> islandsGroups = new List<Island>();

        [SerializeField] public List<GameObject> packs = new List<GameObject>();

        [SerializeField] public int order;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            order = 1;
        }

        public int SetOrder()
        {
            ResetOrder();
            order = order + 1;
            return order;
        }

        private void ResetOrder()
        {
            if (order > 25)
            {
                order = 15;
                foreach (var piece in GameObject.FindGameObjectsWithTag("Puzzle"))
                {
                    var p = piece.GetComponent<SpriteRenderer>().sortingOrder;
                    if (p >= 2)
                        piece.GetComponent<SpriteRenderer>().sortingOrder = p / 2;
                }
            }
        }
    }
}