using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shrimp2DWFC.Runtime
{
    public class ModuleData : MonoBehaviour
    {
        [SerializeField] private ModuleData[] _modulesToAvoid;
        public Prototype _prototype;
        public float _weight = 1;
        public string _moduleName;
        public CellInfo _cell;
        private void Awake()
        {
        }

        public bool isModuleMatch(FaceName face,ModuleData module)
        {
            switch (face)
            {
                case FaceName.down:
                    return _prototype.down.isMatch(module._prototype.up._socket);
                case FaceName.left:
                    return _prototype.left.isMatch(module._prototype.right._socket);
                case FaceName.right:
                    return _prototype.right.isMatch(module._prototype.left._socket);
                case FaceName.up:
                    return _prototype.up.isMatch(module._prototype.down._socket);
            }
            return true;
        }

        public void SetCellInfo(Cell cell)
        {
            _cell._logInfo = cell._logInfos.ToArray();
            _cell._possibleModules = cell._possibleModulesName.ToArray();
            _cell.up = cell._up;
            _cell.down = cell._down;
            _cell.right = cell._right;
            _cell.left = cell._left;
            _cell._gridPos = cell._gridPos;
        }
        [Serializable]
        public class Prototype
        {
            public Face up, down, left, right;
        }

        [Serializable]
        public class Face
        {
            public FaceName _faceName;
            public Socket _socket;
            public bool isMatch(Socket anotherSocket)
            {
                if (_socket == Socket.all || anotherSocket == Socket.all) return true;
                if (_socket == Socket.one && anotherSocket != _socket) return false;
                if (_socket == Socket.zero && anotherSocket != Socket.zero_f) return false;
                if (_socket == Socket.zero_f && anotherSocket != Socket.zero) return false;
                return true;
            }
        }

        [Serializable]
        public enum Socket
        {
            one,zero,zero_f,all
        }

        [Serializable]
        public enum FaceName
        {
            up,down,left,right
        }
        
        [Serializable]
        public struct CellInfo
        {
            public Vector2 _gridPos;
            public string[] _logInfo;
            public string[] _possibleModules;

            public Vector2 up, down, left, right;
        }
    }
}