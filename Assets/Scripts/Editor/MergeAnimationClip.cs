using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
 
public class MergeAnimationClip : MonoBehaviour
{
    [MenuItem("Assets/Merge AnimClips into Controller", false, -1000)]
    static public void Merge() {
        if(Selection.activeObject is AnimatorController) {
            MergeAnimatorController();
            return;
        }
        else if(Selection.activeObject is AnimatorOverrideController) {
            MergeOverrideController();
        }
    }

    static public void MergeAnimatorController() {
        AssetDatabase.SaveAssets();

        AnimatorController targetController = Selection.activeObject as AnimatorController;
        string targetPath = AssetDatabase.GetAssetPath(targetController);
        List<Object> subAssets = new List<Object>(AssetDatabase.LoadAllAssetRepresentationsAtPath(targetPath));

        foreach(AnimatorControllerLayer layer in targetController.layers) {
            foreach(ChildAnimatorState state in layer.stateMachine.states) {
                AnimationClip oldClip = state.state.motion as AnimationClip;
                // State has no clip
                if(oldClip == null) continue;

                if(AssetDatabase.GetAssetPath(oldClip) != targetPath) {
                    AnimationClip newClip = Instantiate(oldClip);
                    newClip.name = oldClip.name;
                    AssetDatabase.AddObjectToAsset(newClip, targetController);
                    AssetDatabase.SetMainObject(targetController, targetPath);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newClip));
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(oldClip));
                    state.state.motion = newClip;
                }

                subAssets.Remove(oldClip);
            }
        }

        foreach(Object o in subAssets) {
            if(o is AnimationClip) {
                DestroyImmediate(o, true);
            }
        }

        AssetDatabase.SaveAssets();
    }

    static public void MergeOverrideController() {
        AssetDatabase.SaveAssets();

        AnimatorOverrideController targetController = Selection.activeObject as AnimatorOverrideController;
        if(targetController.runtimeAnimatorController == null) {
            Debug.Log($"{targetController.name}'s runtimeAnimatorController is null.");
            return;
        }
        List<KeyValuePair<AnimationClip, AnimationClip>> overrideClips;
        overrideClips = new List<KeyValuePair<AnimationClip, AnimationClip>>(targetController.overridesCount);
        targetController.GetOverrides(overrideClips);

        string targetPath = AssetDatabase.GetAssetPath(targetController);
        List<Object> subAssets = new List<Object>(AssetDatabase.LoadAllAssetRepresentationsAtPath(targetPath));

        for(int i = 0; i < overrideClips.Count; i++) {
            AnimationClip oldClip = overrideClips[i].Value;
            if(oldClip != null) {
                if(AssetDatabase.GetAssetPath(oldClip) != targetPath) {
                    AnimationClip newClip = Instantiate(oldClip);
                    newClip.name = oldClip.name;
                    AssetDatabase.AddObjectToAsset(newClip, targetController);
                    AssetDatabase.SetMainObject(targetController, targetPath);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newClip));
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(oldClip));
                    overrideClips[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrideClips[i].Key, newClip);
                }
                subAssets.Remove(oldClip);
            }
        }

        targetController.ApplyOverrides(overrideClips);

        foreach(Object o in subAssets) {
            if(o is AnimationClip) {
                DestroyImmediate(o, true);
            }
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Assets/Merge AnimClips into Controller", true)]
    static public bool Validate() {
        if(Selection.activeObject is AnimatorController) return true;
        if(Selection.activeObject is AnimatorOverrideController) return true;
        return false;
    }
}
 