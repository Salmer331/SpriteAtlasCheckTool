using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

class UnboundSpritesTool
{
    Sprite[] unboundSprites = null;
    bool dropperState = false;

    public void DrawButton()
    {
        if (GUILayout.Button("Find Unbound Sprites"))
        {
            unboundSprites = GetSpritesNotBoundToAnyAtlasInBuild(SpritesInSceneTool.GetSpritesGeneric()).ToArray();
        }
    }

    public void DrawSpritesFoldout()
    {
        SpritesInSceneTool.DropdownSpites(unboundSprites, "Unbound Sprites: ", ref dropperState);
    }

    static IEnumerable<Sprite> GetSpritesNotBoundToAnyAtlasInBuild(IEnumerable<Sprite> spritesToCheck)
    {
        var atlases = FindAssetsByType<SpriteAtlas>();

        foreach (var sprite in spritesToCheck)
        {
            if (atlases.Any(x => x.CanBindTo(sprite)) == false)
            {
                yield return sprite;
            }
        }
    }

    static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets;
    }
}

