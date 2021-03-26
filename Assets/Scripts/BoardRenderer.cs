using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardRenderer : MonoBehaviour
{
    [Header("Render Item")]
    public Transform ItemContainer;
    public GameObject ItemPrefab;

    [Header("ScriptableObjects")]
    public GameDataSO GameData;

    Vector2 prevBrickPivot;
    Vector2 prevShadowPivot;
    Vector2[] prevOffset;

    private void Awake() {
        GameData.BoardItems = new BoardItem[10, 20];
        GameData.LineCounts = new int[20];
        for(int y = 0; y < 20; y++) {
            for(int x = 0; x < 10; x++) {
                GameObject item = Instantiate(ItemPrefab, ItemContainer.localPosition + new Vector3(x, y, 0), Quaternion.identity);
                item.transform.SetParent(ItemContainer);
                GameData.BoardItems[x, y] = item.GetComponent<BoardItem>();
            }
        }
    }

    public void SetActiveContainer(bool active) {
        ItemContainer.gameObject.SetActive(active);
    }

    public void RenderBrick() {
        Vector2 pivot = GameData.BrickPivot;
        int rotation = GameData.BrickRotation;
        Vector2[] offsets = GameData.BrickData.Offsets[rotation];

        for(int i = 0; i < 4; i++) {
            int x = (int)(pivot.x + offsets[i].x);
            int y = (int)(pivot.y + offsets[i].y);

            if(x < 10 && y < 20) {
                GameData.BoardItems[x, y].Render(GameData.BrickData.Blocks[i], rotation);
            }
        }

        prevBrickPivot = pivot;
        prevOffset = offsets;
    }

    public void RenderBrick(bool Erase) {
        if(Erase) EraseBrick();
        RenderBrick();
    }

    public void EraseBrick() {
        if(prevOffset == null) return;

        Vector2 pivot = prevBrickPivot;
        Vector2[] offsets = prevOffset;

        for(int i = 0; i < 4; i++) {
            int x = (int)(pivot.x + offsets[i].x);
            int y = (int)(pivot.y + offsets[i].y);

            if(x < 10 && y < 20) {
                GameData.BoardItems[x, y].Erase();
            }
        }
    }

    public void RenderShadow() {
        Vector2 pivot = GameData.BrickDropPos;
        int rotation = GameData.BrickRotation;
        Vector2[] offsets = GameData.BrickData.Offsets[rotation];

        for(int i = 0; i < 4; i++) {
            int x = (int)(pivot.x + offsets[i].x);
            int y = (int)(pivot.y + offsets[i].y);

            if(x < 10 && y < 20) {
                GameData.BoardItems[x, y].RenderShadow(GameData.BrickData.Blocks[i], rotation);
            }
        }

        prevShadowPivot = pivot;
    }

    public void RenderShadow(bool Erase) {
        if(Erase) EraseShadow();
        RenderShadow();
    }

    public void EraseShadow() {
        if(prevOffset == null) return;

        Vector2 pivot = prevShadowPivot;
        Vector2[] offsets = prevOffset;

        for(int i = 0; i < 4; i++) {
            int x = (int)(pivot.x + offsets[i].x);
            int y = (int)(pivot.y + offsets[i].y);

            if(x < 10 && y < 20) {
                GameData.BoardItems[x, y].Erase();
            }
        }
    }
}
