using System.Diagnostics;
using System.Collections.Specialized;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Avatars.Components;
using EUI = FZTools.EditorUtils.UI;
using ELayout = FZTools.EditorUtils.Layout;
using static FZTools.AvatarUtils;

using Anatawa12.AvatarOptimizer;
using nadena.dev.modular_avatar.core;
using nadena.dev.modular_avatar.core.editor;

namespace FZTools
{
    public class OneClickSetUpTools : EditorWindow
    {
        [SerializeField] GameObject targetAvatar;

        bool? isInstalledMA;
        bool IsInstalledMA
        {
            get
            {
                if (isInstalledMA == null)
                {
                    isInstalledMA = ExternalToolUtils.IsInstalledMA();
                }
                return (bool)isInstalledMA;
            }
        }

        bool? isInstalledAAO;
        bool IsInstalledAAO
        {
            get
            {
                if (isInstalledAAO == null)
                {
                    isInstalledAAO = ExternalToolUtils.IsInstalledAAO();
                }
                return (bool)isInstalledAAO;
            }
        }

        bool? isInstalledLilycal;
        bool IsInstalledLilycal
        {
            get
            {
                if (isInstalledLilycal == null)
                {
                    isInstalledLilycal = ExternalToolUtils.IsInstalledLilycal();
                }
                return (bool)isInstalledLilycal;
            }
        }

        [MenuItem("FZTools/OneClickSetUpTools(β)")]
        private static void OpenWindow()
        {
            var window = GetWindow<OneClickSetUpTools>();
            window.titleContent = new GUIContent("OneClickSetUpTool(β)");
        }

        private void OnGUI()
        {
            ELayout.Horizontal(() =>
            {
                EUI.Space();
                ELayout.Vertical(() =>
                {
                    EUI.Label("Target Avatar");
                            EUI.ChangeCheck(
                                () => EUI.ObjectField<GameObject>(ref targetAvatar),
                                () =>
                                {
                                });
                            EUI.Space();
                    if (IsInstalledAAO)
                    {
                        EUI.Button("AAO: Trace and Optimize を設定", AttatchTraceAndOptimize);
                    }
                    if (IsInstalledMA)
                    {
                        EUI.Button("MA Floor Adjustor を設定", AttatchFloorAdjustor);
                        EUI.Button("Setup Outfit していない衣装をSetupOutfitする", AttatchSetupOutfit);
                    }
                    if (IsInstalledLilycal)
                    {
                        EUI.Button("FixLighting prefab でライティング統一", AttatchFixLightingPrefab);
                    }
                });
                EUI.Space();
            });
        }

        private void AttatchTraceAndOptimize()
        {
            targetAvatar.AddComponent<TraceAndOptimize>();
        }

        private void AttatchFloorAdjustor()
        {
            // アバター直下にEmpty Objectを作成
            // 名前をFloorAdjustorに変更
            // ModularAvatarFloorAdjusterをアタッチ

            var item = new GameObject("FloorAdjustor");
            item.transform.SetParent(targetAvatar.transform);
            item.AddComponent<ModularAvatarFloorAdjuster>();
        }

        private void AttatchSetupOutfit()
        {
            targetAvatar.GetComponentsInChildren<Transform>()
                        .Where(c => GetArmature(c.gameObject) != null)
                        .Where(c => !c.name.ToLower().Contains("armature"))
                        .Where(c => c.GetComponent<VRCAvatarDescriptor>() == null)
                        .ToList()
                        .ForEach(c => SetupOutfit.SetupOutfitUI(c.gameObject));

            GameObject GetArmature(GameObject avatar)
            {
                Animator animator = avatar.GetComponent<Animator>();
                var armature = AvatarUtils.GetArmature(animator);
                if (armature == null)
                {
                    avatar.GetComponentsInChildren<Transform>().ToList().ForEach(t =>
                    {
                        if (t.name.ToLower().Contains("armature"))
                        {
                            armature = t.gameObject;
                            return;
                        }
                    });
                }
                return armature;
            }
        }

        private void AttatchFixLightingPrefab()
        {
            GameObject flPrefab = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Packages/jp.lilxyzw.lilycalinventory/Prefabs/[lilToon] Fix Lighitng.prefab")) as GameObject;
            flPrefab.transform.SetParent(targetAvatar.transform);
        }
    }
}