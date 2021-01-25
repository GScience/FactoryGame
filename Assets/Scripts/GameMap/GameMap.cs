using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GameMap : MonoBehaviour
{
    public static InstanceHelper<GameMap> GlobalMap;

    void Awake()
    {
        GlobalMap = new InstanceHelper<GameMap>(this);
    }
}
