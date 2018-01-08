using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour {

    public int _damage = 1;
    public QuestionType _questionType = QuestionType.ADDITION;
    public Material[] _questionTypeDefenceMaterials;
    public TextMesh   _damageTextMesh;
    public GameObject _defenceParticleSystemGO;
    public Color      _playerDefenceColor;

    private Rigidbody2D _thisDamnRigidBody2D;

    private void Start()
    {
        _thisDamnRigidBody2D = GetComponent<Rigidbody2D>();
        _thisDamnRigidBody2D.gravityScale = 0;
    }

    public void SetupAttack(int damage)
    {
        _damage = damage;
        _damageTextMesh.text = _damage.ToString();
    }

    public void PlayerReached()
    {
        _thisDamnRigidBody2D.gravityScale = 1;
        _damageTextMesh.gameObject.SetActive(true);
        _damageTextMesh.transform.DOLocalMoveY(1.5f, 1.5f);
        StartCoroutine(PlayerDamaged());
    }

    public void PlayerDefended()
    {
        _thisDamnRigidBody2D.gravityScale = 1;
        _thisDamnRigidBody2D.AddForce(new Vector2(100.0f, 0.0f));
        _defenceParticleSystemGO.SetActive(true);
        _defenceParticleSystemGO.GetComponent<ParticleSystemRenderer>().material = _questionTypeDefenceMaterials[(int)_questionType];
        _defenceParticleSystemGO.GetComponent<ParticleSystemRenderer>().material.color = _playerDefenceColor;
        Destroy(gameObject, 1.0f);
    }

    public IEnumerator PlayerDamaged()
    {
        Color color = _damageTextMesh.color;
        yield return new WaitForSeconds(0.8f);
        float lerpRatio = 0.0f;
        while (lerpRatio < 0.7f)
        {
            lerpRatio += Time.deltaTime;
            color.a = Mathf.Lerp(color.a, 0, lerpRatio / 0.7f);
            _damageTextMesh.color = color;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
