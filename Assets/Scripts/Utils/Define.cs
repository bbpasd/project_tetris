using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour
{
    #region Tetris Block Data

    public static readonly int BlockAmount = 7;

    public enum ColorEnum
    {
        Grey,
        Yellow,
        Purple,
        Green,
        Pink,
        Orange,
        Blue,
    }

    public static readonly Color[] Colors = {
        new Color(152 / 255.0f, 152 / 255.0f, 152 / 255.0f),
        Color.yellow,
        new Color(128 / 255.0f, 0, 128 / 255.0f),
        Color.green,
        new Color(1, 105 / 255.0f, 180 / 255.0f),
        new Color(1, 165 / 255.0f, 0),
        Color.blue
    };

    #endregion

    #region Tetris Board Data

    public static readonly int TBoardWidth = 10;
    public static readonly int TBoardHeight = 20;
    public static readonly float TBoardBlockSize = 5.0f;
    public static readonly float TBoardBlockInterval = 0.5f;

    #endregion

    #region AniPang Board Data

    public static readonly int ABoardWidth = 7;
    public static readonly int ABoardHeight = 7;
    public static readonly float ABoardBlockSize = 10.0f;
    public static readonly float ABoardBlockInterval = 0.5f;

    #endregion
}