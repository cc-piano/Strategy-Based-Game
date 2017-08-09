using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridConfig))]
public class GridCreatInspector : Editor
{
    private GridConfig _gridConfig;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _gridConfig = (GridConfig) target;
    
        if (GUILayout.Button("Clear and create Grid"))
        {
            ClearGrid();
            CreateGrid();
        }
    }

    private void ClearGrid()
    {
        for (int i = _gridConfig.transform.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(_gridConfig.transform.GetChild(i).gameObject);
        }
    }

    private void CreateGrid()
    {
        GreateGridSquire();
    }

    private void GreateGridSquire()
    {
        for (int i = 0; i < _gridConfig.Rows; ++i)
        {
            for (int j = 0; j < _gridConfig.Columns; ++j)
            {
                _gridConfig.CreateTile(i, j);
            }
        }
    
        _gridConfig.UpdateCollider(
            new Vector3((_gridConfig.Rows - 1) * 5 * _gridConfig.TileSize, 0, (_gridConfig.Columns - 1) * 5 * _gridConfig.TileSize),
            new Vector3(_gridConfig.Rows * 10 * _gridConfig.TileSize, 0, _gridConfig.Columns * 10 * _gridConfig.TileSize));
    }
}
