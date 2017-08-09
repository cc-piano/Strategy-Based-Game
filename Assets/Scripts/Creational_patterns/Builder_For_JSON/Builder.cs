using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Builder
{
    [System.Serializable]
    public struct Area
    {
        public int w;
        public int h;

        public Area(int w, int h)
        {
            this.w = w;
            this.h = h;
        }
    }

    [Serializable]
    public class DecorationJson
    {
        public string id;
        public string type;
        public string name;
        public int limit = -1;//-1 is infinity
        public Area area = new Area(1, 1);
    }

    abstract class Parser
    {
        public abstract void ParseJson(string path);

        public virtual void CreateOrReplaceScriptableObj(string pathToSave)
        {
        }

        public abstract bool LoadDataFromScriptableObj();
    }

    class DecorationParser : Parser
    {
        public DecorationJsonList DecorationDescriptionList { get; private set; }
        private GameObject GridConfigGO;
        private GridConfig GridConfig;

        public DecorationParser()
        {
            if (!GridConfigGO)
            {
                GridConfigGO = GameObject.FindObjectOfType<GridConfig>().gameObject;
            }
            if (!GridConfig)
            {
                GridConfig = GameObject.FindObjectOfType<GridConfig>();
            }
        }
#if UNITY_EDITOR
        [MenuItem("Parse/ParseJson")]
        public static void Parse()
        {
            Parser tempParser = new DecorationParser();
            tempParser.ParseJson("game");
            tempParser.CreateOrReplaceScriptableObj("Assets/Resources/JSONScriptableObject.asset");
        }
#endif
        public override bool LoadDataFromScriptableObj()
        {
            DecorationDescriptionList = Resources.Load<DecorationJsonList>("JSONScriptableObject");
            if (!DecorationDescriptionList)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public GameObject MakeOneItem(DecorationJson j)
        {
            if (j.limit != -1)
            {
                if (j.limit > 0)
                {
                    j.limit--;
                }
                else
                {
                    return null;
                }
            }
            GameObject temp = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + j.type + "/" + j.name));
            temp.AddComponent<PlacementInfo>().InfoToDisplay = "ID is: " + j.id + " name is: " + j.name + " type is: " + j.type;
            return temp;
        }

        public override void ParseJson(string path)
        {
            JSONNode parsedJson = JSON.Parse(Resources.Load<TextAsset>("game").text);
            DecorationDescriptionList = DecorationJsonList.CreateInstance();
            DecorationDescriptionList.Items = new List<DecorationJson>();
       
            for (int i = 0; i < parsedJson.Count; i++)
            {
                DecorationJson temp = new DecorationJson();
                temp.id = parsedJson.AsObject.m_Dict.ElementAt(i).Key;
                foreach (KeyValuePair<string, JSONNode> jsonElement in parsedJson[i].AsObject.m_Dict)
                {
                    if (jsonElement.Key == "area")
                    {
                        temp.area.w = jsonElement.Value[0];
                        temp.area.h = jsonElement.Value[1];
                    }
                    else
                    {
                        temp.GetType().GetField(jsonElement.Key).SetValue(
                            temp, 
                            Convert.ChangeType(jsonElement.Value.Value, 
                            temp.GetType().GetField(jsonElement.Key).FieldType));
                    }
                }
                DecorationDescriptionList.Items.Add(temp);
            }
        }

#if UNITY_EDITOR
        public override void CreateOrReplaceScriptableObj(string pathToSave)
        {
#if !UNITY_EDITOR
           Debug.LogError("Creating ScriptableObj only allowed in editor!");
            return;
#endif
            if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(pathToSave))
            {
                if (EditorUtility.DisplayDialog(
                    "Parsed JSON already exists",
                    "Do you want to rewrite it?",
                    "Rewrite",
                    "Do Not Rewrite"))
                {
                    Debug.LogWarning("Rewriting JSON!");
                    AssetDatabase.DeleteAsset(pathToSave);
                    AssetDatabase.CreateAsset(DecorationDescriptionList, pathToSave);
                }
                else
                {
                    Debug.LogWarning("Rewriting cancelled!");
                }
            }
            else
            {
                AssetDatabase.CreateAsset(DecorationDescriptionList, pathToSave);
            }
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
