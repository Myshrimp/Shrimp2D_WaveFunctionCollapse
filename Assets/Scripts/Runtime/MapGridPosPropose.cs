using System;
using System.Collections.Generic;
using Shrimp2DWFC.Generics;
using UnityEngine;

namespace Shrimp2DWFC.Runtime
{
    public class MapGridPosPropose : Singleton<MapGridPosPropose>
    {
        private float _gridSize;
        public Vector2 _proposedGridPos;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            _gridSize = Map._gridSize;
        }

        public void Proposal(Vector2 pos, float radius,Action<Cell> action)
        {
            int x = (int)(pos.x / _gridSize);
            int y = (int)(pos.y / _gridSize);
            float proposed_x =x * _gridSize;
            float proposed_y =y * _gridSize;
            _proposedGridPos = Map.VectorToGridPos(pos);
            ApplyFunctionOnCell(action, _proposedGridPos);

            int max_x = (int)
            (
                radius /
                (
                    (Mathf.Sqrt(2) * _gridSize)
                    )
                );

            for (int i = 0; i < max_x; i++)
            {
                for (int j = 0; j < max_x; j++)
                {
                    float positive_proposeX =proposed_x + i * _gridSize;
                    float negative_proposeX =proposed_x - i * _gridSize;
                    float positive_proposeY = proposed_y + i * _gridSize;
                    float negative_proposeY =proposed_y - i * _gridSize;

                    Vector2 up_right = new Vector2(positive_proposeX, positive_proposeY);
                    Vector2 up_left =new Vector2(negative_proposeX, positive_proposeY);
                    Vector2 down_right =new Vector2(positive_proposeX, negative_proposeY);
                    Vector2 down_left =new Vector2(negative_proposeX, negative_proposeY);
                    
                    ApplyFunctionOnCell(action,up_right);
                    ApplyFunctionOnCell(action,up_left);
                    ApplyFunctionOnCell(action,down_right);
                    ApplyFunctionOnCell(action,down_left);
                }
            }
        }
        public Cell _currentCell;

        private  void ApplyFunctionOnCell(Action<Cell> action, Vector2 proposedPos)
        {
            Cell cell = Map.Instance.TryGetValue(proposedPos);
            if (cell != null)
            {
                action?.Invoke(cell);
                if (_currentCell==null ||_currentCell._entropy > cell._entropy || _currentCell._collapsed)
                {
                    _currentCell = cell;
                }
            }
        }
    }
}