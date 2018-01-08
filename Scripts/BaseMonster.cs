using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMonster : MonoBehaviour
{
    public int _maxHealth;
    public int _currentHealth;
    public GameObject _healthIndicator;
    public AudioSource _audioSource;
    public Vector3 _encounterPosition;

    public QuestionDifficulty _questionDiffulty;
    public List<QuestionType> _questionTypes;

    public abstract void Initialise();
    public abstract void Attack();
    public abstract bool TakeDamage(int damageQuantity);
    public abstract void Die();

    protected void UpdateHealthIndicator()
    {
        _healthIndicator.GetComponent<TextMesh>().text = _currentHealth.ToString();
    }
}
