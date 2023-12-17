using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class SpritesInSceneTool : SpriteHelperTools
{
    [MenuItem("Tools/Sprite Tools/Get On Scene Sprites")]
    public static void ShowWindow()
    {
        var wind = EditorWindow.GetWindow(typeof(SpritesInSceneTool));
        wind.Show();
    }

    [SerializeField] List<DefaultAsset> folders = new List<DefaultAsset>(1);
    [SerializeField] Sprite[] spritesInScene = null;
    [SerializeField] Sprite[] spriteArray = null;
    private UnboundSpritesTool unboundSprites = new UnboundSpritesTool();
    private SpriteAtlasReferenceTool spRefs = new SpriteAtlasReferenceTool();

    string statusFoldersDropdown = "Folders To Check: ";
    bool showDropdownFolders = false;

    bool initiateSearch = false;
    bool findUnusedInFolders = false;


    string statusSpritesDropdown = "Sprites: ";
    string statusSpritesExlusionDropdown = "Unused Sprites: ";
    bool showOnSceneList = false;
    bool showExclusionList = false;

    Vector2 scrollPos = default;

    private void OnGUI()
    {

        initiateSearch = GUILayout.Button("Initiate Searching");
        if (initiateSearch)
        {
            spritesInScene = GetSpritesGeneric();
        }

        findUnusedInFolders = GUILayout.Button("Get Unused Sprites In Folders");
        if (findUnusedInFolders)
        {
            var spritesInScene = GetSpritesGeneric();

            var spritesInFolders = GetAllSprites(GetFolderPaths(folders));

            Debug.LogFormat("sprite in Folders Count : {0} \n sprite on Scene Count: {1}",
                spritesInFolders.Count, spritesInScene.Length);

            spritesInFolders.ExceptWith(spritesInScene);
            spriteArray = new Sprite[spritesInFolders.Count];
            spritesInFolders.CopyTo(spriteArray);
        }
        unboundSprites.DrawButton();
        spRefs.DrawButton();

        GUILayout.Space(10);
        //Start scroll area
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MinWidth(300), GUILayout.MinHeight(100), GUILayout.MaxHeight(800), GUILayout.MaxWidth(600));
        FoldersDropDown();
        GUILayout.Space(10);

        DropdownSpites(spritesInScene, statusSpritesDropdown, ref showOnSceneList);
        DropdownSpites(spriteArray, statusSpritesExlusionDropdown, ref showExclusionList);
        unboundSprites.DrawSpritesFoldout();
        GUILayout.Space(10);
        spRefs.DrawSpritesFoldout();
        //Finish scroll area
        EditorGUILayout.EndScrollView();
    }

    public static Sprite[] GetSpritesGeneric()
    {
        List<GameObject> sceneRoots = new List<GameObject>();

        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            Scene scene = EditorSceneManager.GetSceneAt(i);
            sceneRoots.AddRange(scene.GetRootGameObjects());
        }

        var result = GetAllSpritesFromChildren(sceneRoots.ToArray());
        return result;
    }

    public static Sprite[] GetAllSpritesFromChildren(GameObject[] roots)
    {
        var res = new HashSet<Sprite>();

        foreach (var root in roots)
        {
            foreach (var c in root.GetComponentsInChildren<Component>(true))
            {
                if (c == null)
                    continue;

                foreach (var prop in new SerializedObject(c).GetIterator().GetVisibleAllChildren())
                {
                    if (prop.propertyType == SerializedPropertyType.ObjectReference &&
                        prop.objectReferenceValue is Sprite s)
                    {
                        res.Add(s);
                    }
                }
            }
        }

        return res.ToArray();
    }

    public static void DropdownSpites(Sprite[] spArray, string status, ref bool dropper)
    {
        dropper = EditorGUILayout.Foldout(dropper, status);
        if (dropper)
        {
            if (spArray == null) return;
            foreach (var s in spArray)
            {
                EditorGUILayout.ObjectField(s, Type.GetType("Sprite"), true);
            }
        }
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
}

