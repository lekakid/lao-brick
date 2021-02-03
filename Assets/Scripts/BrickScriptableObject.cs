using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[InlineEditor]
public class BrickScriptableObject : SerializedScriptableObject
{
    [TableColumnWidth(57, false), PreviewField]
    public Sprite Preview;
    public Sprite[] Blocks;
    public Vector2[][] Offsets = new Vector2[][] { new Vector2[4], new Vector2[4], new Vector2[4], new Vector2[4]};
}
