using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spellpage : MonoBehaviour {

    public QuestionType _spellQuestionType;
    public QuestionDifficulty _spellQuestionDifficulty;
    public bool _locked;

    private Image spellImage;

    [SerializeField]
    private Texture2D lockedImage;
    [SerializeField]
    private Texture2D partialUnlockedImage;
    [SerializeField]
    private Texture2D unlockedImage;
}
