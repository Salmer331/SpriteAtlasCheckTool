using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;
using UnityEditor;

    [Serializable]
    internal class AtlasSpriteRef
    {
        public SpriteAtlas atlas;
        public List<Sprite> sprites;

        public AtlasSpriteRef()
        {
            sprites = new List<Sprite>();
        }
    }


public class SpriteAtlasReferenceTool
{

    private AtlasSpriteRef[] unboundSprites = new AtlasSpriteRef[0];
    private bool[] dropperStates = new bool[0];

    public void DrawButton()
    {
        if (GUILayout.Button("Get Sprite Atlas References"))
        {
            FillAtlasSpriteReferences(SpritesInSceneTool.GetSpritesGeneric());
        }
    }

    public void DrawSpritesFoldout()
    {
        for (int i = 0; i < unboundSprites.Length; i++)
        {
            EditorGUILayout.ObjectField(unboundSprites[i].atlas, typeof(SpriteAtlas), true);
            SpritesInSceneTool.DropdownSpites(unboundSprites[i].sprites.ToArray(), "Inclusions: ",
                ref dropperStates[i]);
        }
    }

    void FillAtlasSpriteReferences(IEnumerable<Sprite> spritesToCheck)
    {
        var atlases = FindAssetsByType<SpriteAtlas>();
        List<AtlasSpriteRef> refs = new List<AtlasSpriteRef>();
        //SpriteAtlas lastAtlas = null;

        foreach (var sprite in spritesToCheck)
        {
            foreach (var atlas in atlases)
            {
                if (atlas.CanBindTo(sprite))
                {
                    var found = false;
                    foreach (var spRef in refs)
                    {
                        if (spRef.atlas == atlas)
                        {
                            spRef.sprites.Add(sprite);
                            found = true;
                            break;
                        }
                    }

                    if (found) continue;
                    var nspRef = new AtlasSpriteRef {atlas = atlas};
                    nspRef.sprites.Add(sprite);
                    refs.Add(nspRef);
                }
            }
        }

        unboundSprites = refs.ToArray();
        dropperStates = new bool[unboundSprites.Length];
        for (int i = 0; i < unboundSprites.Length; i++)
        {
            dropperStates[i] = false;
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
