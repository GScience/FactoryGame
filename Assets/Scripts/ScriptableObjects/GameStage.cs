using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 游戏阶段
/// </summary>
[CreateAssetMenu(fileName = "new Stage", menuName = "Game/Stage")]
public class GameStage : NamedScriptableObject
{
    public string stageName;

    public Mission[] missions;

    public bool isFirstStage = false;

    public GameStage nextStage;
}
