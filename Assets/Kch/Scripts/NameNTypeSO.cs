using System;
using System.Collections.Generic;
using UnityEngine;

public class NameNTypeSO : ScriptableObject
{
    [Serializable]
    public class NameNType
    {
        public string name;
        public TowerType type;
    }
    public enum TowerType
    { NON, ARCHER, CANON, MORTAR}

    [SerializeField]
    private GameObject newPrefab = null;

    [SerializeField]
    private List<NameNType> tagNType = new List<NameNType>();

    private void OnValidate()
    {
        

        if (newPrefab == null) return; 
        NameNType newclass = new NameNType();
        newclass.name = newPrefab.name;
        newclass.type = TowerType.NON;

        if (!tagNType.Exists(data => data.name == newPrefab.name))
            tagNType.Add(newclass);

        newPrefab = null;
    }
}
