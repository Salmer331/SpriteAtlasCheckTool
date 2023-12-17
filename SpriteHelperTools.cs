using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

#if UNITY_EDITOR
class SpriteHelperTools : EditorWindow
{
    protected string[] GetFolderPaths(List<DefaultAsset> folders)
    {
        List<string> paths = new List<string>();
        foreach (var f in folders)
        {
            paths.Add(AssetDatabase.GetAssetPath(f));
        }
        return paths.ToArray();
    }

    protected HashSet<Sprite> GetAllSprites(string[] folders)
    {
        HashSet<Sprite> sprites = new HashSet<Sprite>();
        var spritesGUID = folders.Length > 0 ? AssetDatabase.FindAssets("t: sprite", folders) : AssetDatabase.FindAssets("t: sprite");

        foreach (var spriteGUID in spritesGUID)
        {
            Sprite item = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(spriteGUID));
            sprites.Add(item);
        }
        return sprites;
    }

    protected string[] GetAtlasGUIDs()
    {
        return AssetDatabase.FindAssets("t:spriteatlas");
    }

    protected bool IsAtlasPacked(Sprite sprite, string[] atlasesGUID)
    {
        foreach (var atlasGUID in atlasesGUID)
        {
            SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AssetDatabase.GUIDToAssetPath(atlasGUID));

            if (atlas.CanBindTo(sprite))
                return true;
        }
        return false;
    }

    protected static HashSet<Sprite> GetSpritesOnSceneIn<T>(bool includeInactive) where T : UnityEngine.Object
    {
        var objects = FindObjectsOfType<T>(includeInactive);
        Type genericType = typeof(T);
        HashSet<Sprite> res = new HashSet<Sprite>();
        if (genericType.FullName == "UnityEngine.UI.Image")
        {
            res.UnionWith(FindImages(objects as Image[]));
        }
        if (genericType.FullName == "UnityEngine.SpriteRenderer")
        {
            res.UnionWith(FindImages(objects as SpriteRenderer[]));
        }
        return res;
    }

    private static HashSet<Sprite> FindImages(Image[] objects)
    {
        HashSet<Sprite> result = new HashSet<Sprite>();
        foreach (var obj in objects)
        {
            try
            {
                if (obj.sprite != null)
                {
                    result.Add(obj.sprite);
                    Debug.Log("sprite has been added");
                }
            }
            catch
            {
                Debug.LogFormat("catched an exeption with {0}", obj);
            }
        }
        return result;
    }
    private static HashSet<Sprite> FindImages(SpriteRenderer[] objects)
    {
        HashSet<Sprite> result = new HashSet<Sprite>();
        foreach (var obj in objects)
        {
            try
            {
                if (obj.sprite != null)
                {
                    result.Add(obj.sprite);
                    Debug.Log("sprite has been added");
                }
            }
            catch
            {
                Debug.LogFormat("catched an exeption with {0}", obj);
            }
        }
        return result;
    }

}
#endif
