using System.Collections.Generic;
using Builder;
using UnityEngine;

public class DecorationJsonList : ScriptableObject
{
    public List<DecorationJson> Items;

    public static DecorationJsonList CreateInstance()
    {
        return CreateInstance<DecorationJsonList>();
    }

    private DecorationJsonList()
    {
    }
}
