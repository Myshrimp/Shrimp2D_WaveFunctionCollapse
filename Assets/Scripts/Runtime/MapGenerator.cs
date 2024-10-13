using UnityEngine;

namespace Shrimp2DWFC.Runtime
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private Map _map;
        public Vector2 pos;
        private CellIndex _index;
        public void Generate()
        {
            if (_map._uncollapsedMap.Count <= 0) return;
            while (!_map._uncollapsedMap.ContainsKey(_index))
            {
                int index = Random.Range(0, _map._allPos.Count);
                pos = _map._allPos[index];
                _index = Map.Instance.PosToIndex(pos);
            }
            _map.SetChunkInCell(pos);
        }
    }
}