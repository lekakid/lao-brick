using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BrickGenerator : SerializedMonoBehaviour
{
    [TableList(DrawScrollView = false)]
    public BrickScriptableObject[] DB;
    Queue<BrickScriptableObject> bagQueue = new Queue<BrickScriptableObject>();

    void GenerateBag() {
        List<BrickScriptableObject> TempArr = new List<BrickScriptableObject>(DB);
        while(TempArr.Count > 0) {
            int r = Random.Range(0, TempArr.Count);
            bagQueue.Enqueue(TempArr[r]);
            TempArr.RemoveAt(r);
        }
    }
    
    public BrickScriptableObject GetRandomBrick() {
        if(bagQueue.Count < 1) GenerateBag();
        return bagQueue.Dequeue();
    }
}
