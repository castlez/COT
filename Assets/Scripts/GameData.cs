using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.PlayersClasses;
using Assets.Enemies;

public static class GameData
{
    public static List<PlayerClassBase> currentPlayers;
    public static List<EnemyBase> nextEnemies;
    public static int floorNumber=1;  // gathered from various places to determine progress (like bandits)
    public static List<EnemyBase> getNextEnemies(int zone)
    {
        // TODO this is where we choose what the players are fighting
        // for now just return 1-4 bandits
        if (zone == 0)
        {
            System.Random rnd = new System.Random();
            int num_enemies = rnd.Next(1,5);
            GameData.nextEnemies = new List<EnemyBase>();

            for (int i = 0; i < num_enemies;i++)
            {
                GameData.nextEnemies.Add(new Bandit($"{i+1}"));
            }
        }

        return GameData.nextEnemies;
    }
}
