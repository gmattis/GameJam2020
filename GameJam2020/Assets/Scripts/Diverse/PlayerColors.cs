using System;
using UnityEngine;

public enum ColorList
{
    Rouge,
    Orange,
    Jaune,
    Violet,
    Bleu,
    Vert
}

public class GameColors
{
    public static Color Rouge { get; } = new Color(1f, 0f, 0.43f);
    public static Color Orange { get; } = new Color(1f, 0.41f, 0f);
    public static Color Jaune { get; } = new Color(1f, 0.84f, 0f);
    public static Color Violet { get; } = new Color(0.28f, 0f, 1f);
    public static Color Bleu { get; } = new Color(0f, 1f, 1f);
    public static Color Vert { get; } = new Color(0.3f, 1f, 0f);

    public static ColorList[] Player1Colors = new ColorList[] { ColorList.Rouge, ColorList.Orange, ColorList.Jaune };
    public static ColorList[] Player2Colors = new ColorList[] { ColorList.Violet, ColorList.Bleu, ColorList.Vert };

    public static int DamageMultiplier(ColorList colorAttack, ColorList colorReceiver)
    {
        if (colorAttack == colorReceiver) { return 4; }
        else if ((Array.Exists(Player1Colors, el => el == colorAttack) && Array.Exists(Player1Colors, el => el == colorReceiver))
            || (Array.Exists(Player2Colors, el => el == colorAttack) && Array.Exists(Player2Colors, el => el == colorReceiver)))
        { return 2; }
        else { return 1; }
    }

    public static Color ToColor(ColorList color)
    {
        if (color == ColorList.Rouge) { return Rouge; }
        else if (color == ColorList.Orange) { return Orange; }
        else if (color == ColorList.Jaune) { return Jaune; }
        else if (color == ColorList.Violet) { return Violet; }
        else if (color == ColorList.Bleu) { return Bleu; }
        else if (color == ColorList.Vert) { return Vert; }
        else { return Rouge; }
    }
}
