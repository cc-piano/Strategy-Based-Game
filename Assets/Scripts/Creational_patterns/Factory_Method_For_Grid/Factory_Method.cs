using UnityEngine;

namespace Factory_Method
{
    abstract class GridBasement
    {
        public GameObject _girdBasementGO;
    }
    
    class Square : GridBasement
    {
        public Square() : base()
        {
        }
    }

    abstract class Creator
    {
        public abstract GridBasement FactoryMethod(int i, int j, float scale, string prefabPath, Transform parent);
        protected abstract GameObject SpawnPrefab(string path);
    }

    class SquareCreator : Creator
    {
        public override GridBasement FactoryMethod(int i, int j, float scale, string prefabPath, Transform parent)
        {
            Square tempSquare = new Square();
            tempSquare._girdBasementGO = SpawnPrefab(prefabPath);
            SetParent(parent, tempSquare._girdBasementGO);
            SetSizeLocation(i, j, scale, tempSquare._girdBasementGO);
            return tempSquare;
        }

        protected override GameObject SpawnPrefab(string path)
        {
           return GameObject.Instantiate(Resources.Load<GameObject>(path));
        }

        private void SetParent(Transform tr, GameObject _girdBasementGO)
        {
            _girdBasementGO.transform.SetParent(tr);
        }

        private void SetSizeLocation(int i, int j, float scale, GameObject _girdBasementGO)
        {
            _girdBasementGO.transform.localScale *= scale;

            _girdBasementGO.transform.localPosition = new Vector3(
                i * _girdBasementGO.GetComponent<MeshRenderer>().bounds.size.x,
                0,
                j * _girdBasementGO.GetComponent<MeshRenderer>().bounds.size.z);

            _girdBasementGO.name = "Tile:" + i + ":" + j;
        }
    }
}
