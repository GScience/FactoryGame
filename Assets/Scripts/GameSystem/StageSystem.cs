using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 阶段系统
/// 任务呀什么什么的
/// </summary>
public class StageSystem : ISystem
{
    private GameStage _currentGameStages;
    private int _currentMissionId;
    private int _missionData;

    private List<BuildingCardBase> _enabledBuildingCard = new List<BuildingCardBase>();

    private IEnumerator _showMissionEnumerator;

    private ObjectiveToast _currentObjectiveToast;

    /// <summary>
    /// 获取当前游戏阶段名称
    /// </summary>
    /// <returns></returns>
    public string GetCurrentGameStageName()
    {
        if (_currentGameStages == null)
            return "???";
        return _currentGameStages.stageName;
    }

    public event Action<BuildingCardBase> OnEnableBuildingCard;

    /// <summary>
    /// 获取当前任务描述
    /// </summary>
    /// <returns></returns>
    public string GetCurrentMissionDesc()
    {
        if (_currentGameStages == null)
            return "";
        var mission = _currentGameStages.missions[_currentMissionId];
        if (mission == null)
            return "";
        return "";
    }

    public void SetGameStage(GameStage stage)
    {
        _currentGameStages = stage;
        _currentMissionId = 0;
        _missionData = 0;
    }

    // 获取所有可以用的建筑卡
    public List<BuildingCardBase> GetAllEnabledBuildingCard()
    {
        return _enabledBuildingCard;
    }

    /// <summary>
    /// 结束当前任务切换到下一个任务
    /// </summary>
    private void FinishCurrentMission()
    {
        var mission = _currentGameStages.missions[_currentMissionId];
        if (mission == null)
            return;

        if (_currentObjectiveToast != null)
        {
            _currentObjectiveToast.CanFadeOut = true;
            if (mission.ShowMissionFinish)
                _currentObjectiveToast.Delay = 0;
            _currentObjectiveToast = null;
        }

        if (mission.ShowMissionFinish)
            GameManager.ShowToastMessage("任务小助手", "任务已完成", 2);

        // 处理奖励
        foreach (var card in mission.cards)
        {
            OnEnableBuildingCard?.Invoke(card);
            _enabledBuildingCard.Add(card);
        }

        ++_currentMissionId;
        if (_currentGameStages.missions.Length <= _currentMissionId)
        {
            _currentGameStages = _currentGameStages.nextStage;
            _currentMissionId = 0;
        }

        ShowCurrentMission(2);
    }

    public void Load(BinaryReader reader)
    {
        _currentGameStages = SaveHelper.ReadScriptable<GameStage>(reader);
        _currentMissionId = reader.ReadInt32();
        _missionData = reader.ReadInt32();

        var count = reader.ReadInt32();

        for (var i = 0; i < count; ++i)
        {
            var keyHash = reader.ReadInt32();
            var card = ResourcesManager.GetCard(keyHash);
            _enabledBuildingCard.Add(card);
            OnEnableBuildingCard?.Invoke(card);
        }
    }

    public void Save(BinaryWriter writer)
    {
        SaveHelper.Write(writer, _currentGameStages);
        writer.Write(_currentMissionId);
        writer.Write(_missionData);

        writer.Write(_enabledBuildingCard.Count);
        foreach (var card in _enabledBuildingCard)
        {
            var keyHash = ResourcesManager.GetCardKeyHash(card);
            writer.Write(keyHash);
        }
    }

    public void Init()
    {
        var allStage = ResourcesManager.GetAllStage();
        SetGameStage(allStage.FirstOrDefault((stage) => stage.isFirstStage));

        ShowCurrentMission(0);
    }

    public void Update()
    {
        // 刷新消息
        if (_showMissionEnumerator != null)
        {
            if (!_showMissionEnumerator.MoveNext())
                _showMissionEnumerator = null;
            else
                return;
        }

        if (_currentGameStages == null)
            return;
        var mission = _currentGameStages.missions[_currentMissionId];
        if (_currentObjectiveToast != null)
            _currentObjectiveToast.UpdateCounterText(mission.MissionState);

        if (mission == null)
            return;
        if (mission.Check())
            FinishCurrentMission();
    }

    private void ShowCurrentMission(float delay)
    {
        _showMissionEnumerator = ShowCurrentMissionDelay(delay);
    }
    IEnumerator ShowCurrentMissionDelay(float delay)
    {
        var waitTime = 0f;
        while (waitTime < delay)
        { 
            waitTime += Time.deltaTime;
            yield return 0;
        }

        if (_currentGameStages == null)
            yield break;
        var mission = _currentGameStages.missions[_currentMissionId];
        if (mission == null)
            yield break;
        _currentObjectiveToast = GameManager.ShowToastMessage("任务小助手", mission.Prefix + mission.desc, 15, false);
    }
}
