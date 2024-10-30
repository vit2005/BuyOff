using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum ScenarioPart
{
    GameTitle1 = 0,
    Disclaimer = 1,
    Main = 2,
    GameTitle2 = 3,
    Credits = 4
}

public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI soldier;
    [SerializeField] private TextMeshProUGUI police;
    [SerializeField] private Image blackOverlay;
    [SerializeField] private RectTransform background;

    [SerializeField] private Color activeSoldierColor;
    [SerializeField] private Color inactiveSoldierColor;
    [SerializeField] private Color activePoliceColor;
    [SerializeField] private Color inactivePoliceColor;
    [SerializeField] private GameObject GameTitle;
    [SerializeField] private GameObject Credits;
    [SerializeField] private GameObject Disclaimer;

    private List<string> _lines;
    private int _currentLine = 0;
    private int _currentSentence = 0;
    private bool _isSoldier = false;
    private ScenarioPart _part;

    // Start is called before the first frame update
    void Start()
    {
        Load();
        _part = ScenarioPart.GameTitle1;
    }

    private void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("text");
        _lines = new List<string>(textAsset.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries));
        for (int i = 0; i < _lines.Count; i++)
        {
            _lines[i] = _lines[i].TrimStart('-', ' ');
        }
    }

    public bool ShowNextLine()
    {
        if (_currentLine == _lines.Count)
        {
            soldier.text = string.Empty;
            police.text = string.Empty;
            AnimateLastScene();
            return false;
        }

        List<string> sentences = _lines[_currentLine].Split(". ").ToList();
        string text = string.Empty;
        if (sentences.Count > 1)
        {
            text = sentences[_currentSentence];

            if (_currentSentence == sentences.Count-1)
            {
                _currentLine++;
                _currentSentence = 0;
            }
            else
            {
                if (_currentSentence == 0) _isSoldier = !_isSoldier;
                _currentSentence++;
            }
        }
        else
        {
            text = _lines[_currentLine];
            _currentLine++;
            _isSoldier = !_isSoldier;
            _currentSentence = 0;
        }

        if (_isSoldier) soldier.text = text;
        else police.text = text;
        SwapColors();
        ProceedBlackOverlay();
        return true;
    }

    private void SwapColors()
    {
        if (_isSoldier)
        {
            soldier.color = activeSoldierColor;
            police.color = inactivePoliceColor;
        }
        else
        {
            soldier.color = inactiveSoldierColor;
            police.color = activePoliceColor;
        }
    }

    private void ProceedBlackOverlay()
    {
        Color color = blackOverlay.color;
        float value = 1f - ((float)_currentLine / _lines.Count) * 0.05f;
        color.a = value;
        blackOverlay.color = color;
    }

    private void AnimateLastScene()
    {
        StartCoroutine(MoveImageLeft());
    }

    private IEnumerator MoveImageLeft()
    {
        Vector3 startPosition = background.anchoredPosition;
        float width = background.rect.width;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / 1f;

            // Рух вліво (від стартової позиції до повного виходу за лівий край екрану)
            background.anchoredPosition = new Vector3(
                Mathf.Lerp(startPosition.x, startPosition.x - width/2f, progress),
                startPosition.y
            );

            yield return null;
        }

        // Переконуємось, що картинка повністю зникла
        background.anchoredPosition = new Vector3(startPosition.x - width/2f, startPosition.y);
    }

    void Update()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            switch (_part)
            {
                case ScenarioPart.GameTitle1:
                    GameTitle.SetActive(true);
                    _part = ScenarioPart.Disclaimer;
                    break;
                case ScenarioPart.Disclaimer:
                    GameTitle.SetActive(false);
                    Disclaimer.SetActive(true);
                    _part = ScenarioPart.Main;
                    break;
                case ScenarioPart.Main:
                    Disclaimer.SetActive(false);
                    if (!ShowNextLine()) _part = ScenarioPart.GameTitle2;
                    break;
                case ScenarioPart.GameTitle2:
                    GameTitle.SetActive(true);
                    _part = ScenarioPart.Credits;
                    break;
                case ScenarioPart.Credits:
                    GameTitle.SetActive(false);
                    Credits.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
}
