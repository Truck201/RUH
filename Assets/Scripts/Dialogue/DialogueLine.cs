using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 4)]
    public string text;
    public Sprite npcFace;
    public Transform cameraFocus; // punto opcional donde mirar durante esta línea

    [Header("Tutorial Objects")]
    public bool haveGameObjects;
    public bool activeInGame;
    public GameObject[] tutorialObjects; // los objetos a mostrar en esta línea
}