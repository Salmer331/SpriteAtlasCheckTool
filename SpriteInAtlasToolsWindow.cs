using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

#if UNITY_EDITOR
class SpriteInAtlasToolsWindow : SpriteHelperTools
{
    [MenuItem("Tools/Sprite Tools/Atlas Check")]
    public static void ShowWindow()
    {
        var wind = EditorWindow.GetWindow(typeof(SpriteInAtlasToolsWindow));
        wind.Show();
    }

    bool getAtlasslessSprites = false;
    bool showDropdownSprites = false;
    bool showDropdownFolders = false;
    string statusSpritesDropdown = "Unpacked Sprites: ";
    string statusFoldersDropdown = "Folders: ";
    [SerializeField] Sprite[] arrayOfAtlaslessSprites = null;
    [SerializeField] List<DefaultAsset> folders = new List<DefaultAsset>(1);


    Vector2 scrollFoldersPosition = default;

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("Find all sprites not included in the atlas", MessageType.Info);
        GUILayout.Label("Find Sprites without atlas", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("Folders To Search In:", EditorStyles.label);

        scrollFoldersPosition = EditorGUILayout.BeginScrollView(scrollFoldersPosition, GUILayout.MinWidth(300), GUILayout.MinHeight(100), GUILayout.MaxHeight(800), GUILayout.MaxWidth(600));

        FoldersDropDown();
        GUILayout.Space(20);
        getAtlasslessSprites = GUILayout.Button("Find");

        if (getAtlasslessSprites)
        {
            arrayOfAtlaslessSprites = GetUnpackedSprites().ToArray();
            showDropdownSprites = true;
        }

        DropdownSpites();
        EditorGUILayout.EndScrollView();
    }

    void FoldersDropDown()
    {
        showDropdownFolders = EditorGUILayout.Foldout(showDropdownFolders, statusFoldersDropdown);
        if (showDropdownFolders)
        {
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size: ", folders.Count));
            while (newCount < folders.Count)
                folders.RemoveAt(folders.Count - 1);
            while (newCount > folders.Count)
                folders.Add(null);

            for (int i = 0; i < folders.Count; i++)
            {
                folders[i] = (DefaultAsset)EditorGUILayout.ObjectField(folders[i], typeof(DefaultAsset), false);
            }
        }
    }

    void DropdownSpites()
    {
        showDropdownSprites = EditorGUILayout.Foldout(showDropdownSprites, statusSpritesDropdown);
        if (showDropdownSprites)
        {
            if (arrayOfAtlaslessSprites == null) return;
            foreach (var s in arrayOfAtlaslessSprites)
            {
                EditorGUILayout.ObjectField(s, Type.GetType("Sprite"), true);
            }
        }
    }

    public List<Sprite> GetUnpackedSprites()
    {
        List<Sprite> sp = new List<Sprite>();
        var sprites = GetAllSprites(GetFolderPaths(folders));
        var atlasGUIDs = GetAtlasGUIDs();
        foreach (var s in sprites)
        {
            if (IsAtlasPacked(s, atlasGUIDs)) continue;
            sp.Add(s);
        }
        return sp;
    }
}
#endif
