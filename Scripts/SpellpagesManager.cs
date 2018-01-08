using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellpagesManager : MonoBehaviour {

    public List<GameObject> _allPages;
    public List<GameObject> _allQuestionMarkPower;

    public int CurrentSelectedPage { get; set; }
    public QuestionDifficulty CurrentDifficulty { get; set; }
    public Sprite _emptyQuestionMark;
    public Sprite _fullQuestionMark;

    [SerializeField]
    private ScrollRect 	_spellpagesScrollview;
    [SerializeField]
    private GameObject 	_spellpageScrollviewContent;
	private Transform 	_canvasTransform;
    private Vector3     _basePosition;
    private float       _lerpToPagePercentage;

    private void Start()
    {
		_canvasTransform = _spellpagesScrollview.GetComponentInParent<Canvas>().transform;
        _basePosition = _spellpageScrollviewContent.transform.position;
    }

    private void OnEnable()
    {
        CurrentSelectedPage = 0;
    }

    public void OpenBook()
    {
        _spellpagesScrollview.gameObject.SetActive(true);
    }

    public void CloseBook()
    {
        _spellpagesScrollview.gameObject.SetActive(false);
    }

    // Should also return the difficulty
    public QuestionType SelectCurrentPage()
    {
        return QuestionType.ADDITION;
    }

    public QuestionType GetDifficulty()
    {
        return QuestionType.ADDITION;
    }

    public void ScrollRight()
    {
		if (CurrentSelectedPage + 1 < _allPages.Count) {
			CurrentSelectedPage++;
            _lerpToPagePercentage = 0;
        }
    }

    public void ScrollLeft()
    {
		if (CurrentSelectedPage - 1 >= 0) {
			CurrentSelectedPage--;
            _lerpToPagePercentage = 0;
        }
    }

    public void IncrementSpellDifficulty()
    {
        if ((int)CurrentDifficulty < System.Enum.GetNames(typeof(QuestionDifficulty)).Length)
        {
            CurrentDifficulty = (QuestionDifficulty)((int)CurrentDifficulty + 1);
            UpdateDifficultyQuestionMarks();
        }
    }

    public void DecrementSpellDifficulty()
    {
        if ((int)CurrentDifficulty > 0)
        {
            CurrentDifficulty = (QuestionDifficulty)((int)CurrentDifficulty - 1);
            UpdateDifficultyQuestionMarks();
        }
    }

    private void UpdateDifficultyQuestionMarks()
    {
        int index = 0;
        foreach(GameObject questionMarks in _allQuestionMarkPower)
        {
            if(index <= (int)CurrentDifficulty)
                questionMarks.GetComponent<Image>().sprite = _fullQuestionMark;
            else
                questionMarks.GetComponent<Image>().sprite = _emptyQuestionMark;
            index++;
        }
    }

    private void Update()
    {
        if (_lerpToPagePercentage < 1.0f)
        {
            _lerpToPagePercentage += Time.deltaTime * (0.3f) + Mathf.Pow(_lerpToPagePercentage, 2);
            if (_lerpToPagePercentage > 1.0f)
                _lerpToPagePercentage = 1.0f;
            _spellpagesScrollview.horizontalNormalizedPosition = Mathf.Lerp(_spellpagesScrollview.horizontalNormalizedPosition, ((float)CurrentSelectedPage / (float)(_allPages.Count - 1)), _lerpToPagePercentage);
        }
    }
}
