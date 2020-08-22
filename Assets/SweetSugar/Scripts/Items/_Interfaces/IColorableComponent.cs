using System;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.Level;
using UnityEngine;

[ExecuteAlways]
public class IColorableComponent : MonoBehaviour /* , IPoolable */
{
    public int color;

    public List<SpritePerLevel> Sprites = new List<SpritePerLevel>();

    public Sprite randomEditorSprite;

    public SpriteRenderer directSpriteRenderer;

    private void Awake()
    {
        itemComponent = GetComponent<Item>();
        iColorableComponents = GetComponentsInChildren<IColorableComponent>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        iColorChangables = GetComponentsInChildren<IColorChangable>();
    }

    // public IHighlightableComponent IHighlightableComponent;
    // public IDestroyableComponent IDestroyableComponent;
    // public ItemSound ItemSound;
    private void OnEnable()
    {

        if (itemComponent && !itemComponent.Combinable) color = GetHashCode();
    }

    // [HideInInspector]
    // public bool colorGenerated;
    public bool RandomColorOnAwake = true;
    private Item itemComponent;
    private IColorableComponent[] iColorableComponents;
    private SpriteRenderer spriteRenderer;
    private IColorChangable[] iColorChangables;

    public void SetColor(int _color)
    {
        if (_color < 0 || _color >= GetSprites(LevelManager.THIS.currentLevel).Length) return;
        // colorGenerated = true;
        var component = itemComponent;
        if (component != null && component.currentType != ItemsTypes.MULTICOLOR)
            color = _color;
        if (directSpriteRenderer == null) directSpriteRenderer = spriteRenderer;

        if (GetSprites(LevelManager.THIS.currentLevel).Length > 0 && directSpriteRenderer)
            directSpriteRenderer.sprite = GetSprites(LevelManager.THIS.currentLevel)[_color];
        foreach (var i in iColorChangables) i.OnColorChanged(_color);
    }

    public void RandomizeColor()
    {
        color = ColorGenerator.GenColor(itemComponent.square);
        SetColor(color);
        foreach (var i in iColorableComponents) i.SetColor(color);
    }

    public Sprite[] GetSprites(int level)
    {
        SpritePerLevel spritePerLevel = null;
        if (level == 0) level = 1;
        if (Sprites.Any(i => i.level == level))
            return Sprites.First(i => i.level == level).Sprites;

        return Sprites[0].Sprites;
    }

    public Sprite GetSprite(int level, int color)
    {
        var list = GetSprites(level);
        if (color < list.Length) return list[color];
        else if (list.Any()) return list[0];
        return null;
    }

    public Sprite[] GetSpritesOrAdd(int level)
    {
        if (Sprites.All(i => i.level != level))
        {
            var sprites = Sprites[0].Sprites;
            var other = new Sprite[sprites.Length];
            for (var i = 0; i < sprites.Length; i++) other[i] = sprites[i];

            Sprites.Add(new SpritePerLevel {level = level, Sprites = other});
        }

        return GetSprites(level);
    }
}