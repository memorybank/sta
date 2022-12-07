using Playa.Avatars;
using Playa.Avatars.IK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IKRigUI : MonoBehaviour
{
    private string _ActiveAnimancerName;
    private Dictionary<string, Transform> _AnimancerComponentDict;
    [SerializeField] private TMP_Dropdown _AvatarPrefabDropdown;
    [SerializeField] private Transform _Reference;

    // Start is called before the first frame update
    void Awake()
    {
        _AnimancerComponentDict = new Dictionary<string, Transform>();
        var animancerNames = new List<TMP_Dropdown.OptionData>();
        var target2 = GameObject.Find("Target2");
        foreach (Transform child in target2.transform)
        {
            _AnimancerComponentDict.Add(child.name, child);
            animancerNames.Add(new TMP_Dropdown.OptionData(child.name));
        }

        _AvatarPrefabDropdown.options = animancerNames;
        _ActiveAnimancerName = animancerNames[0].text;

        _AvatarPrefabDropdown.onValueChanged.AddListener(index =>
        {
            var ikrigDecoder = target2.GetComponent<IKRigDecoder>();
            var optionText = _AvatarPrefabDropdown.options[index].text;
            var selectedPrefab = _AnimancerComponentDict[optionText];
            ikrigDecoder._Playback = _AnimancerComponentDict[optionText];
            _AnimancerComponentDict[_ActiveAnimancerName].gameObject.SetActive(false);
            selectedPrefab.gameObject.SetActive(true);
            _ActiveAnimancerName = optionText;
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Playing back");
            var ikrig = GameObject.Find("Intermediate");
            var ikrigDecoder = ikrig.GetComponent<IKRigDecoder>();
            var frames = new List<KeyFrame>();

            ikrig = GameObject.Find("Target");
            ikrigDecoder = ikrig.GetComponent<IKRigDecoder>();
            ikrigDecoder.Play();
            if (ikrigDecoder._Deformer == null)
            {
                var m = AvatarMeasurements.Get(ikrigDecoder._Playback);
                frames.Clear();
                frames.Add(new KeyFrame(0f, ArmatureUtils.FindPartString(ikrigDecoder._Playback, "Spine2").position +
                    ArmatureUtils.FindPartString(ikrigDecoder._Playback, "Spine2").forward * 0.3f -
                    ArmatureUtils.FindPartString(ikrigDecoder._Playback, "LeftArm").position,
                    Vector3.up, Vector3.up));
                frames.Add(new KeyFrame(2.5f, ArmatureUtils.FindPartString(ikrigDecoder._Playback, "Head").position +
                    ArmatureUtils.FindPartString(ikrigDecoder._Playback, "Head").forward * m.HeadSize -
                    ArmatureUtils.FindPartString(ikrigDecoder._Playback, "LeftArm").position,
                    Vector3.back, Vector3.back));
                ikrigDecoder._Deformer = new AffineDeformer(ikrigDecoder._Playback, frames);
            }

            ikrig = GameObject.Find("Target2");
            ikrigDecoder = ikrig.GetComponent<IKRigDecoder>();
            ikrigDecoder.Play();
            if (ikrigDecoder._Deformer == null)
            {
                var measurement = AvatarMeasurements.Get(ikrigDecoder._Playback);
                var headSize = measurement.HeadSize + 0.1f;
                Debug.Log("Head size " + ikrigDecoder._Playback + " " + measurement.HeadSize);
                frames.Clear();
                frames.Add(new KeyFrame(0f, ArmatureUtils.FindPartString(ikrigDecoder._Playback, "Spine2").position +
                    ArmatureUtils.FindPartString(ikrigDecoder._Playback, "Spine2").forward * 0.3f -
                    ArmatureUtils.FindPartString(ikrigDecoder._Playback, "LeftArm").position,
                    Vector3.up, Vector3.up));
                frames.Add(new KeyFrame(2.5f, ArmatureUtils.FindPartString(ikrigDecoder._Playback, "Head").position +
                    ArmatureUtils.FindPartString(ikrigDecoder._Playback, "Head").forward * headSize -
                    ArmatureUtils.FindPartString(ikrigDecoder._Playback, "LeftArm").position,
                    Vector3.back, Vector3.back));
                ikrigDecoder._Deformer = new AffineDeformer(ikrigDecoder._Playback, frames);
            }

            ikrig = GameObject.Find("Target3");
            ikrigDecoder = ikrig.GetComponent<IKRigDecoder>();
            ikrigDecoder.Play();
            if (ikrigDecoder._Deformer == null)
            {
                var measurement = AvatarMeasurements.Get(ikrigDecoder._Playback);
                var headSize = measurement.HeadSize + 0.1f;
                ikrigDecoder._Deformer = new InteractionTargetDeformer(
                    ikrigDecoder._Playback,
                    new InteractionTarget(ArmatureUtils.FindPartString(ikrigDecoder._Playback, "Head"),
                    new Vector3(0f, 0.1f, 0.3f), 0.65f));
            }
        }
    }
}
