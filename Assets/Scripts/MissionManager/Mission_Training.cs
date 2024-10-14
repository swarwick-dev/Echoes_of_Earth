using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Training - Mission", menuName = "Missions/Training - Mission")]

public class Mission_Training : Mission
{
    public int amountToKill = 12;
    public EnemyType enemyType;

    private int killsToGo;

    public override void StartMission()
    {
        killsToGo = amountToKill;
        UpdateMissionUI();

        MissionObject_Training.OnTargetKilled += EliminateTarget;

        List<Enemy> validEnemies = new List<Enemy>();

        if (enemyType == EnemyType.Random)
            validEnemies = LevelGenerator.instance.GetEnemyList();
        else
        {
            foreach (Enemy enemy in LevelGenerator.instance.GetEnemyList())
            {
                if (enemy.enemyType == enemyType)
                    validEnemies.Add(enemy);
            }
        }

        for (int i = 0; i < amountToKill; i++)
        {
            if (validEnemies.Count <= 0)
                return;

            int randomIndex = Random.Range(0, validEnemies.Count);
            validEnemies[randomIndex].AddComponent<MissionObject_Training>();
            validEnemies.RemoveAt(randomIndex);
        }
    }

    public override bool MissionCompleted()
    {
        return killsToGo <= 0;
    }

    private void EliminateTarget()
    {
        killsToGo--;
        UpdateMissionUI();

        if (killsToGo <= 0)
        {
            MissionObject_Training.OnTargetKilled -= EliminateTarget;
        }
    }

    private void UpdateMissionUI()
    {
        string missionText = "Eliminate " + amountToKill + " enemies with signal disruptor.";
        string missionDetaiils = "Targets left: " + killsToGo;

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetaiils);
    }

}
