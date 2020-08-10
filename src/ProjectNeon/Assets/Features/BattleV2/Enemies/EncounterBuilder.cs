﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Generates an anemy encounter based on the difficulty set.
 * 
 * The script will add a random enemy up to the difficult level set and
 * will add more enemies until the desired difficulty is reached, or the
 * number of enemies is 7. it will also allow duplicate enemy types.
 */

[CreateAssetMenu]
public class EncounterBuilder : ScriptableObject
{
    [SerializeField] private Enemy[] possible;

    public void Init(IEnumerable<Enemy> possibleEnemies)
    {
        possible = possibleEnemies.ToArray();
    }
    
    public List<Enemy> Generate(int difficulty)
    {
        BattleLog.Write($"Started generating encounter of difficulty {difficulty}");
        /**
         * @todo #52:30min Evolve Encounter Generation after playtesting. 
         */

        var currentDifficulty = 0;
        var enemies = new List<Enemy>();

        while (currentDifficulty < difficulty && enemies.Count < 7)
        {
            var maximum = difficulty - currentDifficulty;
            var nextEnemy = possible.Where(
                enemy => enemy.PowerLevel <= maximum
            ).Random();
            enemies.Add(nextEnemy);
            BattleLog.Write($"Added {nextEnemy.Name} to Encounter");
            currentDifficulty = currentDifficulty + Math.Max(nextEnemy.PowerLevel, 1);
        }
        
        BattleLog.Write("Finished generating encounter");
        return enemies;
    }
}
