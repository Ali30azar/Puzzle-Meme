using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Script
{
    public class Web : MonoBehaviour
    {
        [SerializeField] private List<GameObject> lockedPiece;
        // private float _timer = 2;

        void Start()
        {
            lockedPiece = new List<GameObject>();
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                // if (touch.phase == TouchPhase.Began)
                // {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                       OnWebDelete();
                }
                // }
            }
        }
        
        private void OnWebDelete()
        {
            Destroy(gameObject);
        }
        // public void OnPointerClick(PointerEventData eventData)
        // {
        //     OnWebDelete();
        // }
    }
}