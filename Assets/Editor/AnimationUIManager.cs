using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using Animancer;

class AnimationClipDropdown : AdvancedDropdown
{
    private AnimationUIManager UIManager;
    private int layer;
    public AnimationClipDropdown(AdvancedDropdownState state, AnimationUIManager uiManager, int layer) : base(state)
    {
        this.UIManager = uiManager;
        this.layer = layer;
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("Animations");

        foreach (var clip in UIManager.dataManager.AnimationLayers[layer].Clips)
        {
            var dropdownItem = new AdvancedDropdownItem(clip.name);
            root.AddChild(dropdownItem);
        }
        root.AddChild(new AdvancedDropdownItem("None"));

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        if (item.name == "None")
        {
            UIManager.rin.Clear(layer);
            UIManager.rinCom.Clear(layer);
            return;
        }

        UIManager.rin.Play(layer, item.name);
        UIManager.rinCom.Play(layer, item.name);
    }
}

public class AnimationUIManager : MonoBehaviour
{
    public Rin rin;
    public Rin rinCom;
    public AnimationDataManager dataManager;
    public AnimationControlManager controlManager;
    public AnimationControlManager controlManagerCom;
    
    public Rin[] models;
    public Rin[] comModels;

    enum Models
    {
        kiki,
        picola,
        rin,
        wolf
    }

    private void Start()
    {
        LoadModel(Models.kiki);
    }

    private void OnGUI()
    {
        GUI.skin.button.fontSize = 25;
        GUI.skin.label.fontSize = 25;
        GUI.skin.toggle.fontSize = 25;
        GUI.skin.horizontalSlider.fontSize = 25;
        EditorStyles.toolbarButton.fontSize = 25;
        GUILayout.BeginArea(new Rect(0, 0, 300, 1000));
        {
            GUILayout.BeginVertical();
            {
                var rect0 = GUILayoutUtility.GetRect(new GUIContent("Stroke"), GUI.skin.button);
                if (GUI.Button(rect0, new GUIContent("Stroke"), GUI.skin.button))
                {
                    var dropdown = new AnimationClipDropdown(new AdvancedDropdownState(), this, 0);
                    dropdown.Show(rect0);
                }

                var rect1 = GUILayoutUtility.GetRect(new GUIContent("Legs"), GUI.skin.button);
                if (GUI.Button(rect1, new GUIContent("Legs"), GUI.skin.button))
                {
                    var dropdown = new AnimationClipDropdown(new AdvancedDropdownState(), this, 1);
                    dropdown.Show(rect1);
                }
                var rect2 = GUILayoutUtility.GetRect(new GUIContent("Left arm"), GUI.skin.button);
                if (GUI.Button(rect2, new GUIContent("Left arm"), GUI.skin.button))
                {
                    var dropdown = new AnimationClipDropdown(new AdvancedDropdownState(), this, 2);
                    dropdown.Show(rect2);
                }
                var rect3 = GUILayoutUtility.GetRect(new GUIContent("Right arm"), GUI.skin.button);
                if (GUI.Button(rect3, new GUIContent("Right arm"), GUI.skin.button))
                {
                    var dropdown = new AnimationClipDropdown(new AdvancedDropdownState(), this, 3);
                    dropdown.Show(rect3);
                }

                if (GUILayout.Button("0"))
                {
                    Debug.Log("_Sequence Has Pushed 0");
                    rin.Sequence.Enqueue(0);
                    rinCom.Sequence.Enqueue(0);
                }
                if (GUILayout.Button("1"))
                {
                    Debug.Log("_Sequence Has Pushed 1");
                    rin.Sequence.Enqueue(1);
                    rinCom.Sequence.Enqueue(1);
                }

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Fade Time Value");
                    controlManagerCom.FadeDuration = GUILayout.HorizontalSlider(controlManagerCom.FadeDuration, 0.0f, 1.0f);
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("Value : " + controlManagerCom.FadeDuration.ToString());
                controlManagerCom.IsRinComUsingFromNormalizedStart =
                    GUILayout.Toggle(controlManagerCom.IsRinComUsingFromNormalizedStart ,"FromNormalizedStart");
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("kiki"))
                    {
                        LoadModel(Models.kiki);
                    }
                    if (GUILayout.Button("picola"))
                    {
                        LoadModel(Models.picola);
                    }
                    if (GUILayout.Button("rin"))
                    {
                        LoadModel(Models.rin);
                    }
                    if (GUILayout.Button("wolf"))
                    {
                        LoadModel(Models.wolf);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(700, 200, 600, 100));
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("RIN");
                GUILayout.FlexibleSpace();
                GUILayout.Label("RIN COMPARISON");
            }
            GUILayout.EndHorizontal();

        }
        GUILayout.EndArea();
    }

    private void LoadModel(Models m)
    {
        if (rin != null && rin.gameObject.activeInHierarchy)
        {
            Destroy(rin.gameObject);
        }
        if (rinCom != null && rinCom.gameObject.activeInHierarchy)
        {
            Destroy(rinCom.gameObject);
        }
        rin = Instantiate<Rin>(models[(int)m]);
        rin.transform.position = new Vector3(-0.66f, 0.0f, -8f);
        rin.DataManager = this.dataManager;
        rin.ControlManager = this.controlManager;
        rinCom = Instantiate<Rin>(comModels[(int)m]);
        rinCom.transform.position = new Vector3(0.75f, 0.0f, -8f);
        rinCom.DataManager = this.dataManager;
        rinCom.ControlManager = this.controlManagerCom;
    }

}
