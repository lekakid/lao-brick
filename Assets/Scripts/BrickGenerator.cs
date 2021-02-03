using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BrickGenerator : SerializedMonoBehaviour
{
    [TableList(DrawScrollView = false)]
    public BrickScriptableObject[] DB;
    Queue<BrickScriptableObject> bagQueue = new Queue<BrickScriptableObject>();
    
    public BrickScriptableObject GetRandomBrick() {
        if(bagQueue.Count < 1) {
            List<BrickScriptableObject> TempArr = new List<BrickScriptableObject>(DB);
            while(TempArr.Count > 0) {
                int r = Random.Range(0, TempArr.Count);
                bagQueue.Enqueue(TempArr[r]);
                TempArr.RemoveAt(r);
            }
        }
        return bagQueue.Dequeue();
    }
}
