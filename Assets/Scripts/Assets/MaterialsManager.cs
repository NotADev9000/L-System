using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsManager : MonoBehaviour
{
    [SerializeField] private Material[] _materials;
    public Material[] Materials => _materials;

    public static MaterialsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<string> GetMaterialNames()
    {
        List<string> materialNames = new List<string>();

        foreach (Material material in _materials)
        {
            materialNames.Add(material.name);
        }

        return materialNames;
    }
}
