using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator
{
    [CreateAssetMenu(fileName = "New Database", menuName = "BattleSimulator/Database")]
    public class ScriptableObjectDatabase : ScriptableObject
    {
        [SerializeField] private ScriptableObjectWithGuid[] _records = null;

        private Dictionary<Guid, ScriptableObjectWithGuid> _recordsByGuid;

        private Dictionary<Guid, ScriptableObjectWithGuid> recordsByGuid {
            get {
                if (_recordsByGuid == null)
                {
                    _recordsByGuid = new Dictionary<Guid, ScriptableObjectWithGuid>();
                    if (_records != null)
                        foreach (var record in _records)
                            _recordsByGuid.Add(record.guid, record);
                }

                return _recordsByGuid;
            }
        }

        public T GetRecord<T>(int index) where T : ScriptableObjectWithGuid => _records[index] as T;

        public T GetRecord<T>(Guid id) where T : ScriptableObjectWithGuid =>
            recordsByGuid.TryGetValue(id, out var record) ? record as T : null;
    }
}
