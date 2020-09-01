using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.PlayersClasses;
using Assets.Enemies;

public static class GameData
{
    public static List<Color> pcolors = new List<Color>() {
        new Color(33f/255f, 31f/255f, 76f/255f, 0.70f),
        new Color(87f/255f, 31f/255f, 32f/255f, 0.70f),
        new Color(87f/255f, 31f/255f, 79f/255f, 0.70f),
        new Color(32f/255f, 87f/255f, 31f/255f, 0.70f)
    };

    public static List<PlayerClassBase> currentPlayers = new List<PlayerClassBase>() {
                new ShadowTinker("1"),
                //new Barbarian("2"),
            };
    public static List<EnemyBase> nextEnemies;
    public static int floorNumber=1;  // gathered from various places to determine progress (like bandits)
    public static List<EnemyBase> getNextEnemies(int zone)
    {
        // TODO this is where we choose what the players are fighting
        if (zone == 0)
        {
            System.Random rnd = new System.Random();
            int num_enemies = 1;
            if (floorNumber < 4)
            {
                num_enemies = floorNumber;
            }
            else
            {
                num_enemies = 4;
            }
            GameData.nextEnemies = new List<EnemyBase>();

            for (int i = 0; i < num_enemies;i++)
            {
                GameData.nextEnemies.Add(new Bandit($"{i+1}"));
            }
        }

        return GameData.nextEnemies;
    }
}
