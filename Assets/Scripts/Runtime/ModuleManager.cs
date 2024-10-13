using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shrimp2DWFC.Runtime
{
    public class ModuleManager : MonoBehaviour
    {
        public static ModuleManager Instance;
        public Transform _prototypes;
        public List<ModuleData> _allModules;
        public List<GameObject> _allModuleObjects;
        public Dictionary<string, ModuleData> _allModulesDict;
        private void Awake()
        {
            Instance = this;
            _allModules = new List<ModuleData>();
            _allModuleObjects = new List<GameObject>();
            _allModulesDict = new Dictionary<string, ModuleData>();
            foreach (Transform tf in _prototypes)
            {
                var item = tf.GetComponent<ModuleData>();
                _allModules.Add(item);
                _allModuleObjects.Add(tf.gameObject);
                _allModulesDict.TryAdd(tf.gameObject.name,item);
            }
        }
    }
}