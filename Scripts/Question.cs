using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Question : MonoBehaviour
{
    public Font _playerFont;
    public Font _monsterFont;
    public Color _playerColor;
    public Color _monsterColor;
    public bool IsPlayerQuestion { get; set; }
    // Question related variables
    public int Answer { get { return _question.Answer; } }
    [SerializeField]
    private TextMesh            _questionTextMesh;
    private AbstractQuestion    _question;
    private string              _currentAnswer;
    

    // User input related variables
    private Regex regex = new Regex("^[0-9]+$");
    private float deleteDeltaTime;
    private static float DELETE_DELAY = 0.0942f;

    private void Update()
    {
        bool stringChanged = false;
        deleteDeltaTime += Time.deltaTime;

        if (regex.IsMatch(Input.inputString))
        {
            _currentAnswer += Input.inputString;
            stringChanged = true;
        }
    
        if (Input.GetKey(KeyCode.Backspace) && deleteDeltaTime > DELETE_DELAY)
        {
            if (_currentAnswer.Length > 0)
            {
                _currentAnswer = _currentAnswer.Substring(0, _currentAnswer.Length - 1);
                stringChanged = true;
                deleteDeltaTime = 0.0f;
            }
        }

        if (stringChanged)
            _questionTextMesh.text = _question.WrittenQuestion + _currentAnswer;
    }

    public void SetAsPlayerQuestion()
    {
        IsPlayerQuestion = true;
        _questionTextMesh.font = _playerFont;
        _questionTextMesh.characterSize = 7;
        _questionTextMesh.GetComponent<Renderer>().material = _playerFont.material;
        _questionTextMesh.GetComponent<Renderer>().material.color = _playerColor;
    }

    public void SetAsMonsterQuestion()
    {
        IsPlayerQuestion = false;
        _questionTextMesh.font = _monsterFont;
        _questionTextMesh.characterSize = 10;
        _questionTextMesh.GetComponent<Renderer>().material = _monsterFont.material;
        _questionTextMesh.GetComponent<Renderer>().material.color = _monsterColor;
    }

    public void SetQuestionParameters(QuestionType type, QuestionDifficulty difficulty)
    {
        _question = new SimpleArithmeticQuestion();
        _question._questionType = type;
        _question._questionDifficulty = difficulty;
        _question.GenerateQuestion();

        _questionTextMesh.text = _question.WrittenQuestion;
    }

    public bool CheckForEmptyString()
    {
        return (_currentAnswer == null);
    }

    public bool VerifyAnswer()
    {
        return (System.Int32.Parse(_currentAnswer) == _question.Answer);
    }
}
