using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingMath : BaseMonster {

    public AudioClip hurtSound;
    public AudioClip deathSound;

    public override void Initialise()
    {
        switch (_questionDiffulty)
        {
            case QuestionDifficulty.BASIC:
                _maxHealth = 40;
                break;
            case QuestionDifficulty.SIMPLE:
                _maxHealth = 80;
                break;
            case QuestionDifficulty.INTERMEDIATE:
                _maxHealth = 200;
                break;
            case QuestionDifficulty.ADVANCED:
                _maxHealth = 400;
                break;
            case QuestionDifficulty.TOUGHCOOKIE:
                _maxHealth = 1000;
                break;
            default:
                break;
        }
        _currentHealth = _maxHealth;
        UpdateHealthIndicator();
    }

    public override void Attack() { }

    public override bool TakeDamage(int damageQuantity)
    {
        _currentHealth -= damageQuantity;
        UpdateHealthIndicator();
        if (_currentHealth <= 0)
        {
            _audioSource.PlayOneShot(deathSound);
            Die();
            return true;
        }
        else
        {
            _audioSource.PlayOneShot(hurtSound);
        }
        return false;
    }

    public override void Die()
    {
        StartCoroutine(LivingMathDyingCoroutine());
    }

    private IEnumerator LivingMathDyingCoroutine()
    {
        float alphaValue = 0.0f;
        while(alphaValue < 1.0f)
        {
            GetComponent<Renderer>().material.SetFloat("_AlphaMask", alphaValue);
            alphaValue += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
