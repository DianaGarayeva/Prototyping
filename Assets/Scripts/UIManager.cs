using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // handle to text
    [SerializeField]
    private Text _score;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Text _gameOver;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private Image _thrustersController;
    [SerializeField]
    private Text _chargeText;
    private float maxWidth;
    [SerializeField]
    private Image _shieldController;
    [SerializeField]
    private Text _shieldText;
    private float maxShieldWidth;
    [SerializeField]
    private Text _ammoText;
    void Start()
    {
        maxWidth = _thrustersController.rectTransform.sizeDelta.x; 
        _score.text = "Score: " + 0;
        _gameOver.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("game manager is NULL");
        }
        _thrustersController.color = Color.green;
        _thrustersController.rectTransform.sizeDelta = new Vector2(0f, _thrustersController.rectTransform.sizeDelta.y);

        maxShieldWidth = _shieldController.rectTransform.sizeDelta.x;
        _shieldController.rectTransform.sizeDelta = new Vector2(0f, _shieldController.rectTransform.sizeDelta.y);
        _shieldText.text = "";

        _ammoText.text = "Shots: 15";
    }

    public void UpdateAmmo(int ammo)
    {
        if (ammo == 0)
        {
            _ammoText.text = "No shots!";
        }else
        {
            _ammoText.text = "Shots: " + ammo;
        }

    }

    public void UpdateShield(float shieldPercent)
    {
        if (shieldPercent == 0)
        {
            _shieldText.text = "";
            _shieldController.rectTransform.sizeDelta = new Vector2(maxShieldWidth * shieldPercent, _shieldController.rectTransform.sizeDelta.y);
        }
        else
        {
            _shieldController.rectTransform.sizeDelta = new Vector2(maxShieldWidth * shieldPercent, _shieldController.rectTransform.sizeDelta.y);
            _shieldText.text = Math.Round(shieldPercent * 100).ToString() + "%";
        }
    }

    public void UpdateCharge(float chargePercent)
    {
        _thrustersController.rectTransform.sizeDelta = new Vector2(maxWidth * chargePercent, _thrustersController.rectTransform.sizeDelta.y);
        _chargeText.text = Math.Round(chargePercent*100).ToString() + "%";
    }


    public void UpdateScore(int playerScore)
    {
        _score.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < 0)
        {
            currentLives = 0;
        }

        _livesImg.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }


    void GameOverSequence()
    {
        _gameOver.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        _gameManager.GameOver();
        StartCoroutine(GameOverFlickerRoutine());
    }


    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOver.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOver.text = " ";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
