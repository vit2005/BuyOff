using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI soldier;
    [SerializeField] private TextMeshProUGUI police;

    [SerializeField] private Color activeSoldierColor;
    [SerializeField] private Color inactiveSoldierColor;
    [SerializeField] private Color activePoliceColor;
    [SerializeField] private Color inactivePoliceColor;

    private List<string> _lines;
    private int _currentLine = -1;
    private bool _isSoldier = true;

    // Start is called before the first frame update
    void Start()
    {
        Load();
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

    public void ShowNextLine()
    {
        _currentLine++;
        if (_isSoldier) soldier.text = _lines[_currentLine];
        else police.text = _lines[_currentLine];
        SwapColors();
        _isSoldier = !_isSoldier;
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
   
    void Update()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0)) ShowNextLine();
    }
}
