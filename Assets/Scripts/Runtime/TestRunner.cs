using System;
using UnityEngine;
using Shrimp2DWFC.Generics;
namespace Shrimp2DWFC.Runtime
{
    public class TestRunner : MonoBehaviour
    {
        [SerializeField] private MapGenerator _generator;
        [SerializeField] private KeyCode _generateKey;
        public Vector2 _mouseWorldPos;
        public Vector2 _proposedGridPos;
        public float _proposeRadius = 30f;
        public bool _generateOnMouseMove = true;
        private void Update()
        {
            if (Input.GetKeyDown(_generateKey))
            {
                _generator.Generate(); 
            }

            if (_generateOnMouseMove)
            {
                _proposedGridPos = MapGridPosPropose.Instance._proposedGridPos;
                _mouseWorldPos = MouseInputHandler.ScreenPointToWorldPoint();
                MapGridPosPropose.Instance.Proposal(_mouseWorldPos,_proposeRadius,UpdateEntropy);
                try
                {
                    Map.Instance.SetChunkInCell(MapGridPosPropose.Instance._currentCell._gridPos);
                }
                catch
                {
                    Debug.LogWarning("Current cell is null");
                }
            }
        }

        void UpdateEntropy(Cell cell)
        {
            if (cell != null)
            {
                cell.CalculateEntropy();
            }
        }
    }
}