using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Encounter
{
    public List<BaseMonster> monsters;
    public int monstersTurn;
}

public class MonsterGenerator : MonoBehaviour
{
    public GameObject   _LivingMathPrefab;

    [HideInInspector]
    public Transform    _spawnPoint;

    public BaseMonster GenerateLivingMath(QuestionDifficulty difficulty, List<QuestionType> questionTypes)
    {
        GameObject newLivingMath = Instantiate(_LivingMathPrefab);
        newLivingMath.transform.position = _spawnPoint.position;

        BaseMonster baseMonster = newLivingMath.GetComponent<BaseMonster>();
        baseMonster._questionDiffulty = difficulty;
        baseMonster._questionTypes = questionTypes;
        baseMonster.Initialise();
        return baseMonster;
    }

    /// <summary>
    /// Default function is monster count 1, question difficulty basic, and question types will be set to ADDITION
    /// </summary>
    public Encounter GenerateLivingMathEncounter(int monsterCount = 1, QuestionDifficulty difficulty = QuestionDifficulty.BASIC, List<QuestionType> questionTypes = null)
    {
        Encounter newEncounter = new Encounter();
        newEncounter.monsters = new List<BaseMonster>();

        if(questionTypes == null)
        {
            questionTypes = new List<QuestionType>() { QuestionType.ADDITION };
        }

        for (int monsterIndex = 0; monsterIndex < monsterCount; monsterIndex++)
        {
            newEncounter.monsters.Add(GenerateLivingMath(difficulty, questionTypes));
        }

        return newEncounter;
    }
}
