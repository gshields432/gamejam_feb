using UnityEngine;

public static class UnityExtensions
{
    //for use in NightmarePlatform & NeutralPlatform
    public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}