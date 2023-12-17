using System.Collections.Generic;
using UnityEditor;

static class SerializedPropertyExtensions
{
    public static IEnumerable<SerializedProperty> GetVisibleChildren(this SerializedProperty serializedProperty)
    {
        SerializedProperty currentProperty = serializedProperty.Copy();
        SerializedProperty nextSiblingProperty = serializedProperty.Copy();
        {
            nextSiblingProperty.NextVisible(false);
        }

        if (currentProperty.NextVisible(true))
        {
            do
            {
                if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                    break;

                yield return currentProperty;
            }
            while (currentProperty.NextVisible(false));
        }
    }

    public static IEnumerable<SerializedProperty> GetVisibleAllChildren(this SerializedProperty serializedProperty)
    {
        var current = serializedProperty.Copy();
        while (current.NextVisible(true))
        {
            yield return current;
        }
    }
}

