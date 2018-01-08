using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleArithmeticQuestion : AbstractQuestion
{
    private int     _memberCount;
    private int[]   _members;
    private int     _memberMinRange;
    private int     _memberMaxRange;

    public override void GenerateQuestion()
    {
        _answer = 0;
        _writtenQuestion = string.Empty;

        // Decided not to put the new memberCount here for difficulty ajusting reasons
        // Since 3 member addition are much easier than 3 member multiplication

        switch (_questionType)
        {
            case QuestionType.ADDITION:
                GenerateAddition();
                break;
            case QuestionType.SUBSTRACTION:
                GenerateSubstraction();
                break;
            case QuestionType.MULTIPLICATION:
                GenerateMultiplication();
                break;
            case QuestionType.DIVISION:
                break;
            default:
                Debug.LogWarning("Wrong question type set for simple arithmethicc equation.");
                GenerateAddition();
                break;
        }
        Debug.Log(_answer);
    }

    private void GenerateAddition()
    {
        switch(_questionDifficulty)
        {
            case QuestionDifficulty.BASIC:
                _memberCount = 2;
                _memberMinRange = 1;
                _memberMaxRange = 15;
                break;
            case QuestionDifficulty.SIMPLE:
                _memberCount = 2;
                _memberMinRange = 1;
                _memberMaxRange = 30;
                break;
            case QuestionDifficulty.INTERMEDIATE:
                _memberCount = 3;
                _memberMinRange = 1;
                _memberMaxRange = 50;
                break;
            case QuestionDifficulty.ADVANCED:
                _memberCount = 3;
                _memberMinRange = 1;
                _memberMaxRange = 100;
                break;
            case QuestionDifficulty.TOUGHCOOKIE:
                _memberCount = 3;
                _memberMinRange = 1;
                _memberMaxRange = 1000;
                break;
            default:
                break;
        }
        
        _members = new int[_memberCount];

        for (int i = 0; i < _memberCount; i++)
        {
            int member = Random.Range(_memberMinRange, _memberMaxRange);
            _members[i] = member;
            _answer += member;
            _writtenQuestion += member.ToString();
            if (i < _memberCount - 1)
                _writtenQuestion += " + ";
        }
        _writtenQuestion += " = ";
    }

    // Figure out if the difficulty for substractions
    private void GenerateSubstraction()
    {
        for (int i = 0; i < _memberCount; i++)
        {
            // TODO: Member value should take into account the question difficulty
            int member = Random.Range(1, 100);
            _members[i] = member;
            if (i == 0)
                _answer += member;
            else
                _answer -= member;
            _writtenQuestion += member.ToString();
            if (i < _memberCount - 1)
                _writtenQuestion += " - ";
        }
        _writtenQuestion += " = ";
    }

    private void GenerateMultiplication()
    {
        for (int i = 0; i < _memberCount; i++)
        {
            // TODO: Member value should take into account the question difficulty
            int member = Random.Range(1, 30);
            _members[i] = member;
            if(i == 0)
                _answer += member;
            else
                _answer *= member;
            _writtenQuestion += member.ToString();
            if (i < _memberCount - 1)
                _writtenQuestion += " x ";
        }
        _writtenQuestion += " = ";
    }
}

public class MixedArithmeticQuestion : AbstractQuestion
{
    // TODO: include paratheses somehow
    private int _memberCount;
    private int[] _members;

    public override void GenerateQuestion()
    {
    }
}