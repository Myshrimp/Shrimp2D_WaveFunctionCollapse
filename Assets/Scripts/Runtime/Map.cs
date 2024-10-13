using System.Collections.Generic;
using Shrimp2DWFC.Generics;
using UnityEngine;
using FaceName = Shrimp2DWFC.Runtime.ModuleData.FaceName;
using Random = UnityEngine.Random;

namespace Shrimp2DWFC.Runtime
{
    public class Map : Singleton<Map>
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private GameObject _testSprite;
        public static float _gridSize = 0;

        //Map that contains all cells and cellindices
        public Dictionary<CellIndex, Cell> _mapInfo;
        
        //Map that only contains cells that are not set yet
        public Dictionary<CellIndex, Cell> _uncollapsedMap;
        
        //Dict that stores all cell_indices
        public Dictionary<Vector2Int, List<CellIndex>> _CellIndices;
        
        //for test
        public List<Vector2> _allPos;

        #region Map size
        public int _mapScale_x = 5;
        public int _mapScale_y = 5;
        #endregion

        //return this once we can't find existing key with vector
        private static CellIndex _invalidIndex;
        protected override void Awake()
        {
            base.Awake();
            _allPos = new List<Vector2>();
            _mapInfo = new Dictionary<CellIndex, Cell>();
            _gridSize = Mathf.Abs(_sprite.bounds.size.x);
            _CellIndices = new Dictionary<Vector2Int, List<CellIndex>>();
            _invalidIndex = new CellIndex(new Vector2(), false);
        }

        private void Start()
        {
            InitMap();
        }

        public static Vector2 VectorToGridPos(Vector2 vec)
        {
            int x = Mathf.RoundToInt(vec.x / _gridSize);
            int y = Mathf.RoundToInt(vec.y / _gridSize);
            Vector2 result = new Vector2(x * _gridSize, y * _gridSize);
            return result;
        }

        /// <summary>
        /// Try to get value from uncollapsed_cell dictionary
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Cell TryGetValue(Vector2 pos)
        {
            CellIndex index = PosToIndex(pos);
            if (_uncollapsedMap.ContainsKey(index))
            {
                return _uncollapsedMap[index];
            }
            return null;
        }
        
        /// <summary>
        /// Initialize the map by generating grids and setting up cells
        /// </summary>
        void InitMap()
        {
            for (int i = 0; i < _mapScale_x; i++)
            {
                for (int j = 0; j < _mapScale_y; j++)
                {
                    Vector2 pos = new Vector2(i * _gridSize, j * _gridSize);
                    Vector2Int posInt = VectorToVectorInt(pos);
                    CellIndex index = new CellIndex(pos);

                    Vector2 down, left;
                    down = new Vector2(i * _gridSize, (j-1) * _gridSize);
                    left = new Vector2((i - 1) * _gridSize, j * _gridSize);
                    CellIndex downIndex = PosToIndex(down);
                    CellIndex leftIndex = PosToIndex(left);
                    if (_CellIndices.ContainsKey(posInt))
                    {
                        _CellIndices[posInt].Add(index);
                    }
                    else
                    {
                        _CellIndices.Add(posInt,new List<CellIndex>(){index});
                    }
                    _allPos.Add(pos);
                    Cell cell = new Cell(pos,_gridSize);
                    GameObject grid = new GameObject(pos.ToString());
                    grid.transform.position = pos;
                    cell._grid = grid;
                    if (leftIndex._valid)
                    {
                        cell._leftCell = _mapInfo[leftIndex];
                        _mapInfo[leftIndex]._rightCell = cell;
                        cell._logInfos.Add("left index valid");
                    }

                    if (downIndex._valid)
                    {
                        cell._downCell = _mapInfo[downIndex];
                        _mapInfo[downIndex]._upCell = cell;
                        cell._logInfos.Add("down index valid");
                    }
                    _mapInfo.Add(index,cell);
                }
            }

            _uncollapsedMap = new Dictionary<CellIndex, Cell>(_mapInfo);
        }

        public void SetChunkInCell(Vector2 pos)
        {
            CellIndex index = PosToIndex(pos); //首先获取距离位置pos最近的cell的索引CellIndex
            if (!index._valid) return;
            if (!_uncollapsedMap.ContainsKey(index)) return;
            Cell cell = _uncollapsedMap[index];
            try
            {
                if (cell.UpdateCell())
                {
                    cell.TrySetChunk(Instantiate(_uncollapsedMap[index]._chunkToSet,
                        _uncollapsedMap[index]._grid.transform));
                }
            }
            catch
            {
                Debug.Log("Uncollapsed map null:"+cell);
                if (cell._currentModuleData == null)
                {
                    Debug.Log("module data null");
                }
            }
            _uncollapsedMap.Remove(index);
        }
        
        
        public void TryCollapse(Cell from,Cell target,FaceName face,ModuleData data)
        {
            Cell cell=target;
            if (cell == null)
            {
                from._logInfos.Add(face.ToString()+" cell is not found");
                return;
            }

            from._logInfos.Add("Collapsing " + face.ToString() + " in " + target._gridPos);
            cell.Collapse(face,data);
        }

        public static Vector2Int VectorToVectorInt(Vector2 vec)
        {
            return new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
        }

        /// <summary>
        /// Generate CellIndex based on vector2
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public CellIndex PosToIndex(Vector2 pos)
        {
            Vector2Int posInt = VectorToVectorInt(pos);
            if (!_CellIndices.ContainsKey(posInt)) return _invalidIndex;
            foreach (var item in _CellIndices[posInt])
            {
                if (Vector2.Distance(item._pos, pos) < 1f)
                {
                    return item;
                }
            }

            return _invalidIndex;
        }
    }

    public struct CellIndex
    {
        public bool _valid;
        public Vector2 _pos;

        public CellIndex(Vector2 pos=new Vector2(),bool isValid=true)
        {
            _pos = pos;
            _valid = isValid;
        }
    }
    
    //Cell is the child of a grid,we generate the chunk of a module in a grid based on Cell's property
    //"module" has a ModuleData property
    //"chunk" is the instance of a module
    public class Cell
    {
        public bool _collapsed=false;
        public float _entropy=0;
        public Vector2 _gridPos;
        public GameObject _grid;
        public GameObject _currentChunk;
        public GameObject _chunkToSet;
        public ModuleData _currentModuleData;
        public List<string> _possibleModulesName;
        public List<string> _logInfos;
        public Vector2 _up,_down,_left,_right;

        public Cell _upCell;
        public Cell _downCell;
        public Cell _rightCell;
        public Cell _leftCell;
        public void TrySetChunk(GameObject chunk)
        {
            _currentChunk = chunk;
            _currentChunk.transform.localPosition = Vector3.zero;
            _collapsed = true;
            _currentModuleData =_currentChunk.GetComponent<ModuleData>();
            CollapseAround();
            _currentModuleData.SetCellInfo(this);
        }

        /// <summary>
        /// return true if there's any possible module to instantiate
        /// </summary>
        /// <returns></returns>
        public bool UpdateCell()
        {
            return UpdateChunkToSet();
        }

        /// <summary>
        /// generate index of one possible chunk using shuffle and set the chunk to spawn in this cell
        /// </summary>
        /// <returns></returns>
        public bool UpdateChunkToSet()
        {
            if (_collapsed) return false;
            float[] possibilities = GetDistribution();
            float total = Random.Range(0f,1f);
            for (int i = 0; i < possibilities.Length; i++)
            {
                if (total < possibilities[i])
                {
                    _chunkToSet = ModuleManager.Instance._allModulesDict[_possibleModulesName[i]].gameObject;
                    if (_currentModuleData != null)
                    {
                        Debug.Log("Current module data is set");
                    }
                    else
                    {
                        Debug.Log("Current module data is not set");
                    }
                    return true;
                }
                total -= possibilities[i];
            }
            return true;
        }

        public Cell(Vector2 gridPos,float gridSize)
        {
            _gridPos = gridPos;
            _possibleModulesName = new List<string>();
            _logInfos = new List<string>();
            
            //Initialize the list of possible modules in this cell
            foreach (var item in ModuleManager.Instance._allModulesDict)
            {
                _possibleModulesName.Add(item.Key);
            }
            Debug.Log("Possible module names:"+_possibleModulesName.Count);
            
            //visualize the positions of surrounding cells,CAN BE REMOVED
            _right.x = gridPos.x+gridSize;
            _right.y = gridPos.y;
            
            _left.y = gridPos.y;
            _left.x = gridPos.x-gridSize;
            
            _up.x = gridPos.x;
            _up.y =gridPos.y+ gridSize;
            
            _down.x = gridPos.x;
            _down.y =gridPos.y - gridSize;
            //CAN BE REMOVED
        }

        /// <summary>
        /// Exclude modules that don't follow our rule in surrounding cells once this cell has its
        /// chunk spawned
        /// </summary>
        public void CollapseAround()
        {
            Map.Instance.TryCollapse(this,_upCell,FaceName.down,_currentModuleData);
            Map.Instance.TryCollapse(this,_downCell,FaceName.up,_currentModuleData);
            Map.Instance.TryCollapse(this,_leftCell,FaceName.right,_currentModuleData);
            Map.Instance.TryCollapse(this,_rightCell,FaceName.left,_currentModuleData);
        }

        /// <summary>
        /// Exclude modules that don't fit
        /// </summary>
        /// <param name="face"></param>
        /// <param name="data"></param>
        public void Collapse(FaceName face,ModuleData data)
        {
            _logInfos.Add("Collapsed by :"+data.gameObject.name+" in "+data._cell._gridPos);
            List<string> moduleNames = new List<string>(_possibleModulesName);
            foreach(var name in moduleNames)
            {
                var item = ModuleManager.Instance._allModulesDict[name];
                if (!item.isModuleMatch(face, data))
                {
                    _possibleModulesName.Remove(name);
                    _logInfos.Add("removed:"+name+" in face:"+face.ToString()+ " by "+data.gameObject.name+
                                  " in "+data._cell._gridPos);
                }
            }
        }

        /// <summary>
        /// Get probabilities of every possible module
        /// </summary>
        /// <returns></returns>
        private float[] GetDistribution()
        {
            float[] array = new float[_possibleModulesName.Count];
            float sum = 0;
            foreach (var item in _possibleModulesName)
            {
                sum += ModuleManager.Instance._allModulesDict[item]._weight;
            }

            for (int i = 0; i < _possibleModulesName.Count; i++)
            {
                array[i]=ModuleManager.Instance._allModulesDict[_possibleModulesName[i]]._weight/sum;
            }

            return array;
        }

        /// <summary>
        /// Calculate entropy with formula:Sigma(-p * log(p))
        /// </summary>
        public void CalculateEntropy()
        {
            float[] distribution = GetDistribution();
            _entropy = 0;
            for (int i = 0; i < distribution.Length; i++)
            {
                _entropy += -distribution[i] * Mathf.Log(distribution[i]);
            }
        }
    }
}


