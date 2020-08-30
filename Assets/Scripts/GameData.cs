using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.PlayersClasses;
using Assets.Enemies;

public static class GameData
{
    public static List<PlayerClassBase> currentPlayers;
    public static List<EnemyBase> nextEnemies;
    public static void getNextEnemies(int zone)
    {
        if (zone == 0)
        {
            GameData.nextEnemies = new List<EnemyBase>() {
                new Bandit("1"),
                new Bandit("2")
            };
        }
    }
}
