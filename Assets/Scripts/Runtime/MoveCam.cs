using System;
using UnityEngine;

namespace Shrimp2DWFC.Runtime
{
    public class MoveCam : MonoBehaviour
    {
        private Camera _cam;
        public float _moveSpeed = 5f;
        private void Awake()
        {
            _cam = GetComponent<Camera>();
        }

        private void Update()
        {
            float x_move = Input.GetAxisRaw("Horizontal");
            float y_move = Input.GetAxisRaw("Vertical");
            _cam.transform.position += new Vector3(x_move, y_move, 0) * Time.deltaTime * _moveSpeed;
            Vector2 mouseScroll = Input.mouseScrollDelta;
            Debug.Log(mouseScroll);
            _cam.orthographicSize += mouseScroll.y;
        }
    }
}