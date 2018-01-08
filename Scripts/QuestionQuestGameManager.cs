using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuestionQuestGameManager : MonoBehaviour {

    public enum EncounterStep { START, PLAYER_GENERAL, PLAYER_TARGET_SELECT, PLAYER_SPELLCHOICE, PLAYER_CASTING, ENEMY_TURN, ENEMY_ATTACK, ENEMY_ATTACK_RESOLVING, END, GAME_OVER }

    public GameObject _playerGameObject;
    public TextMesh   _playerHealthTextMesh;
    public int        _playerHealth;
    public GameObject _gameOverPanel;
    public GameObject _selectionArrow;
    public EncounterStep CurrentStep { get; set; }

    public Transform _playerEntryPoint;
    public Transform _playerExitPoint;
    public Transform _playerEncounterPoint;

    public Transform[]  _monsterPositions;
    public Transform    _monsterEntryPoint;

    [SerializeField]
    private GameObject  _QuestionPrefab;
    [SerializeField]
    private GameObject  _LivingMathAttackPrefab;

    private Question    _currentQuestion;
    private GameObject  _currentMonsterAttack;
    private Encounter   _currentEncounter;
    private int         _encounterCount;
    private int         _currentTarget;
    private int         _lastTarget;
    private bool        _attackReached;
    private bool        _attackDefended;

    private MonsterGenerator    _monsterGenerator;
    private SpellpagesManager   _spellpageManager;

    void Awake()
    {
        DOTween.Init();
    }

    void Start()
    {
        _monsterGenerator = GetComponent<MonsterGenerator>();
        _spellpageManager = GetComponent<SpellpagesManager>();

        _monsterGenerator._spawnPoint = _monsterEntryPoint;
        _currentEncounter = _monsterGenerator.GenerateLivingMathEncounter(1);

        _playerHealthTextMesh.text = _playerHealth.ToString();

        CurrentStep = EncounterStep.START;
        StartCoroutine(BasicEntrance());
    }

    void Update()
    {
        switch(CurrentStep)
        {
            case EncounterStep.START:
                break;
            case EncounterStep.PLAYER_TARGET_SELECT:
                HandleTargetChoice();
                break;
            case EncounterStep.PLAYER_SPELLCHOICE:
                HandleSpellChoice();
                break;
            case EncounterStep.PLAYER_CASTING:
                HandlePlayerCasting();
                break;
            case EncounterStep.ENEMY_TURN:
                HandleMonsterTurn();
                break;
            case EncounterStep.ENEMY_ATTACK:
                HandleMonsterAttack();
                break;
            case EncounterStep.ENEMY_ATTACK_RESOLVING:
                ResolveEnemyAttack();
                break;
            default:
                break;
        }
    }

    private void HandleTargetChoice()
    {
        bool targetChanged = false;

        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            CurrentStep = EncounterStep.PLAYER_SPELLCHOICE;
            _spellpageManager.OpenBook();
            _selectionArrow.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _currentTarget++;
            _currentTarget = _currentTarget % _currentEncounter.monsters.Count;
            targetChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _currentTarget--;
            if(_currentTarget < 0)
                _currentTarget = _currentTarget + _currentEncounter.monsters.Count;
            targetChanged = true;
        }

        if (targetChanged)
            SetArrowUnderTarget(_currentTarget);
    }

    private void HandleSpellChoice()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            GenerateQuestion();
            _currentQuestion.SetAsPlayerQuestion();
            _currentQuestion.SetQuestionParameters(_spellpageManager.SelectCurrentPage(), _spellpageManager.CurrentDifficulty);
            _spellpageManager.CloseBook();
            CurrentStep = EncounterStep.PLAYER_CASTING;
            return;
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            CurrentStep = EncounterStep.PLAYER_TARGET_SELECT;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _spellpageManager.ScrollRight();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _spellpageManager.ScrollLeft();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _spellpageManager.IncrementSpellDifficulty();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _spellpageManager.DecrementSpellDifficulty();
        }
    }

    private void HandlePlayerCasting()
    {
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            if (!_currentQuestion.CheckForEmptyString())
            {
                if (_currentQuestion.VerifyAnswer())
                {
                    if (_currentEncounter.monsters[_currentTarget].TakeDamage(_currentQuestion.Answer))
                    {
                        Destroy(_currentEncounter.monsters[_currentTarget].gameObject, 1.0f);
                        _currentEncounter.monsters.RemoveAt(_currentTarget);
                        _currentTarget = 0; // Don't want to point to nothing...
                    }

                    if (_currentEncounter.monsters.Count == 0)
                    {
                        NextEncounter();
                    }
                    else
                        CurrentStep = EncounterStep.ENEMY_TURN;
                    Destroy(_currentQuestion.gameObject);
                }
                else
                {
                    CurrentStep = EncounterStep.ENEMY_TURN;
                    Destroy(_currentQuestion.gameObject);
                }
            }
        }
    }

    private void HandleMonsterTurn()
    {
        ResetMonsterPosition();
        if (_currentEncounter.monstersTurn < _currentEncounter.monsters.Count)
        {
            GenerateQuestion();
            _currentQuestion.SetAsMonsterQuestion();
            _currentQuestion.SetQuestionParameters(QuestionType.ADDITION, QuestionDifficulty.BASIC);

            Transform currentMonsterTransform = _currentEncounter.monsters[_currentEncounter.monstersTurn].transform;
            currentMonsterTransform.DOJump(currentMonsterTransform.position, 0.55f, 1, 0.3f).SetLoops(-1);

            // MONSTER ATTACKS!!
            _attackDefended = false;
            _attackReached = false;
            _currentMonsterAttack = Instantiate(_LivingMathAttackPrefab);
            _currentMonsterAttack.GetComponent<MonsterAttack>().SetupAttack(_currentQuestion.Answer);
            _currentMonsterAttack.transform.position = currentMonsterTransform.position + new Vector3(0,6.0f,0);
            _currentMonsterAttack.transform.DOMove(_playerEncounterPoint.position + new Vector3(0, 3.0f, 0), 4.0f).SetEase(Ease.InSine).OnComplete(AttackReachedPlayer);

            CurrentStep = EncounterStep.ENEMY_ATTACK;
        }
        else
        {
            _currentEncounter.monstersTurn = 0;
            CurrentStep = EncounterStep.PLAYER_TARGET_SELECT;
            SetArrowUnderTarget(_currentTarget);
            _selectionArrow.SetActive(true);
        }
    }

    private void HandleMonsterAttack()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (!_currentQuestion.CheckForEmptyString())
            {
                if (_currentQuestion.VerifyAnswer())
                {
                    _attackDefended = true;
                    CurrentStep = EncounterStep.ENEMY_ATTACK_RESOLVING;
                }
                else
                {
                    _attackDefended = false;
                    CurrentStep = EncounterStep.ENEMY_ATTACK_RESOLVING;
                }
            }
        }
    }

    private void ResolveEnemyAttack()
    {
        if (_attackDefended)
        {
            _currentMonsterAttack.transform.DOKill();
            _currentMonsterAttack.GetComponent<MonsterAttack>().PlayerDefended();

            DOTween.Kill(_currentEncounter.monsters[_currentEncounter.monstersTurn].transform);
            _currentEncounter.monstersTurn++;
            CurrentStep = EncounterStep.ENEMY_TURN;

            // Old question now, destroying
            Destroy(_currentQuestion.gameObject);
        }
        else if(_attackReached)
        {
            _currentMonsterAttack.GetComponent<MonsterAttack>().PlayerReached();

            // Update player health
            _playerHealth -= _currentQuestion.Answer;
            _playerHealthTextMesh.text = _playerHealth.ToString();

            DOTween.Kill(_currentEncounter.monsters[_currentEncounter.monstersTurn].transform);
            _currentEncounter.monstersTurn++;

            if (_playerHealth <= 0)
            {
                CurrentStep = EncounterStep.GAME_OVER;
                _gameOverPanel.SetActive(true);
            }
            else
                CurrentStep = EncounterStep.ENEMY_TURN;

            // Old question now, destroying
            Destroy(_currentQuestion.gameObject);
        }
    }

    private void AttackReachedPlayer()
    {
        CurrentStep = EncounterStep.ENEMY_ATTACK_RESOLVING;
        _attackReached = true;
    }

    private IEnumerator BasicEntrance()
    {
        _playerGameObject.transform.DOJump(_playerEncounterPoint.position, 1.0f, 5, 1.0f);
        yield return new WaitForSeconds(0.4f);
        int monsterIndex = 0;
        foreach (BaseMonster monster in _currentEncounter.monsters)
        {
            monster.transform.DOJump(_monsterPositions[monsterIndex].position, 1.0f, 5, 1.0f);
            monster._encounterPosition = _monsterPositions[monsterIndex].position;
            monsterIndex++;
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.8f);
        CurrentStep = EncounterStep.PLAYER_TARGET_SELECT;
        _selectionArrow.SetActive(true);
        _currentTarget = 0;
        SetArrowUnderTarget(0);
    }

    private IEnumerator MonsterOnlyEntrance()
    {
        int monsterIndex = 0;
        foreach (BaseMonster monster in _currentEncounter.monsters)
        {
            monster.transform.DOJump(_monsterPositions[monsterIndex].position, 1.0f, 5, 1.0f);
            monster._encounterPosition = _monsterPositions[monsterIndex].position;
            monsterIndex++;
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.8f);
        CurrentStep = EncounterStep.PLAYER_TARGET_SELECT;
        _selectionArrow.SetActive(true);
        _currentTarget = 0;
        SetArrowUnderTarget(0);
    }

    private void GenerateQuestion()
    {
        if (_currentQuestion != null)
            Destroy(_currentQuestion.gameObject);
        GameObject newQuestion = Instantiate(_QuestionPrefab);
        _currentQuestion = newQuestion.GetComponent<Question>();
    }
    
    private void SetArrowUnderTarget(int targetIndex)
    {
        Vector3 selectionArrowPosition = _currentEncounter.monsters[targetIndex]._encounterPosition;//_monsterPositions[targetIndex].position;
        selectionArrowPosition.y -= 1.25f;
        selectionArrowPosition.x *= (Screen.width / Screen.height) / (512 / 288);
        selectionArrowPosition.y *= (Screen.width / Screen.height) / (512 / 288);
        _selectionArrow.GetComponent<RectTransform>().position = selectionArrowPosition;
    }

    private void ResetMonsterPosition()
    {
        int monsterIndex = 0;
        foreach (BaseMonster monster in _currentEncounter.monsters)
        {
            monster.transform.position = monster._encounterPosition;
            monsterIndex++;
        }
    }

    private void NextEncounter()
    {
        
        switch(_encounterCount)
        {
            case 0:
                _currentEncounter = _monsterGenerator.GenerateLivingMathEncounter(2);
                break;
            case 1:
                _currentEncounter = _monsterGenerator.GenerateLivingMathEncounter(2,QuestionDifficulty.SIMPLE);
                break;
            case 2:
                _currentEncounter = _monsterGenerator.GenerateLivingMathEncounter(4, QuestionDifficulty.SIMPLE);
                break;
            case 3:
                _currentEncounter = _monsterGenerator.GenerateLivingMathEncounter(1, QuestionDifficulty.INTERMEDIATE);
                break;
            case 4:
                _currentEncounter = _monsterGenerator.GenerateLivingMathEncounter(2, QuestionDifficulty.INTERMEDIATE);
                break;
            case 5:
                _currentEncounter = _monsterGenerator.GenerateLivingMathEncounter(4, QuestionDifficulty.INTERMEDIATE);
                break;
            case 6:
                _currentEncounter = _monsterGenerator.GenerateLivingMathEncounter(2, QuestionDifficulty.ADVANCED);
                break;
            case 7:
                _currentEncounter = _monsterGenerator.GenerateLivingMathEncounter(4, QuestionDifficulty.ADVANCED);
                break;
            case 8:
                _currentEncounter = _monsterGenerator.GenerateLivingMathEncounter(2, QuestionDifficulty.TOUGHCOOKIE);
                break;
            default:
                _currentEncounter = _monsterGenerator.GenerateLivingMathEncounter(4, QuestionDifficulty.TOUGHCOOKIE);
                break;
        }
        CurrentStep = EncounterStep.START;
        _encounterCount++;
        StartCoroutine(MonsterOnlyEntrance());
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
