using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestionType { ADDITION, SUBSTRACTION, MULTIPLICATION, DIVISION, MIXED_ARITHMETIC, ALGEBRA_QUESTION }
public enum QuestionDifficulty { BASIC, SIMPLE, INTERMEDIATE, ADVANCED, TOUGHCOOKIE }

public abstract class AbstractQuestion
{
    // Basic Question Variables //
    public QuestionType         _questionType;
    public QuestionDifficulty   _questionDifficulty;
    protected int                 _answer;
    protected string              _writtenQuestion;

    // Accessors //
    public int Answer { get { return _answer; } }
    public string WrittenQuestion { get { return _writtenQuestion; } }

    // Abstract Question Function //
    public abstract void GenerateQuestion();
}
