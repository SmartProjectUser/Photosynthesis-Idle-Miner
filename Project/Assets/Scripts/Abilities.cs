using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Abilities : MonoBehaviour {
    public Transform End;
    public Transform Section;
    public TextMeshPro DamageNumberPrefab;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI LevelText;

    public Transform CameraTarget;
    public Transform CameraHomePosition;
    public Transform CameraEndPosition;
    public CanvasGroup BotCanvasGroup;

    public Image HPBar;
    public TextMeshProUGUI HPBarText;

    [Header("Abilities")]
    public ParticleSystem LaserFX;
    public ParticleSystem BoomFX;
    public ParticleSystem DestroyFX;

    public Button LaserButton;
    public Image LaserButtonCd;

    public GameObject BuildButton;
    public TextMeshProUGUI LaserUpgradeCost;
    public TextMeshProUGUI LaserBonusCost;


    int _score;
    int _passiveIncome;
    float _currentEndHP;
    float _maxEndHP = 3;
    float _endHPMultiplier = 1.2f;
    float _endOffset = -10;

    float _currentLaserDamage = 1;
    float _laserDamageMultiplier = 1.1f;
    float _laserUpgradeBaseCost = 100;
    float _laserUpgradeLevel = 1;

    int _level = 1;

    public void CameraMoveHome() {
        CameraTarget.position = CameraHomePosition.position;
        BotCanvasGroup.interactable = false;
        BotCanvasGroup.DOFade(0, 0.2f);
    }

    public void CameraMoveEnd() {
        CameraTarget.position = CameraEndPosition.position;
        BotCanvasGroup.interactable = true;
        BotCanvasGroup.DOFade(1, 0.2f);
    }

    public void DamageEnd(float damage) {
        _currentEndHP = Mathf.Clamp(_currentEndHP - (int)damage, 0, _maxEndHP);
        UpdateHP();
        if (_currentEndHP <= 0 && _level < 5) {
            DOVirtual.DelayedCall(1, DestroyEnd);
        }

        UpdateScore(Random.Range(1 + _level, 5 + _level));

        TextMeshPro damageText = Instantiate(DamageNumberPrefab, End.position + Vector3.up, Quaternion.identity);
        damageText.text = ((int)damage).ToString();
        damageText.transform.DOScale(0, 1).SetEase(Ease.InBack);
        damageText.transform.DOMove(damageText.transform.position + Vector3.up * 3, 1).SetEase(Ease.OutBack).OnComplete(() => Destroy(damageText.gameObject));
    }

    void DestroyEnd() {
        _level++;
        LevelText.text = _level.ToString();

        if(_level == 3) {
            BuildButton.SetActive(true);
        }

        _maxEndHP = _maxEndHP * (Mathf.Pow(_endHPMultiplier, _level));
        _currentEndHP = _maxEndHP;
        End.position += new Vector3(0, _endOffset, 0);
        CameraMoveEnd();
        UpdateHP();
    }

    void UpdateHP() {
        HPBar.fillAmount = _currentEndHP / _maxEndHP;
        HPBarText.text = (int)_currentEndHP + "/" + (int)_maxEndHP;
    }

    public void LaserShoot() {
        LaserButton.interactable = false;
        DamageEnd(_currentLaserDamage);
        LaserButtonCd.fillAmount = 1;
        LaserButtonCd.DOFillAmount(0, 2).OnComplete(() => LaserButton.interactable = true);
        LaserFX.Play();
    }

    private void Start() {
        _currentEndHP = _maxEndHP;
        UpdateHP();
    }

    public void BuyBuilding(int cost) {
        UpdateScore(-cost);
        _passiveIncome = 1;
        InvokeRepeating("PassiveIncome", 1, 1);
    }

    void PassiveIncome() {
        UpdateScore(_passiveIncome);
    }

    void UpdateScore(int amount) {
        _score += amount;
        ScoreText.text = _score.ToString();
        ScoreText.transform.localScale = Vector3.one;
        ScoreText.transform.DOPunchScale(new Vector3(0.5f, 0.5f), 0.2f, 4).OnComplete(() => ScoreText.transform.localScale = Vector3.one);
    }

    public void UpgradeLaser() {
        UpdateScore(-(int)(_laserUpgradeBaseCost * _laserUpgradeLevel));

        _laserUpgradeLevel++;
        _currentLaserDamage = _currentLaserDamage * (Mathf.Pow(_laserDamageMultiplier, _laserUpgradeLevel));

        LaserUpgradeCost.text = (_laserUpgradeBaseCost * _laserUpgradeLevel).ToString();
        LaserBonusCost.text = "+ " + (_currentLaserDamage * (Mathf.Pow(_laserDamageMultiplier, _laserUpgradeLevel + 1)) - _currentLaserDamage).ToString();
    }
}
