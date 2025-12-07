using UnityEngine;
using System;
using System.Collections.Generic;

namespace Spriter2UnityDX {
	[DisallowMultipleComponent, ExecuteInEditMode, AddComponentMenu("")]
	public class EntityRenderer : MonoBehaviour {
		private SpriteRenderer[] renderers = new SpriteRenderer [0];
		private SortingOrderUpdater[] updaters = new SortingOrderUpdater [0];
		private SpriteRenderer _first;
		private SpriteRenderer first {
			get {
				if (_first == null && renderers.Length > 0)
					_first = renderers [0];
				return _first;
			}
		}
		public Color Color {
			get { return (first != null) ? first.color : default(Color); }
			set { DoForAll (x => x.color = value); }
		}

		public Material Material {
			get { return (first != null) ? first.sharedMaterial : null; }
			set { DoForAll (x => x.sharedMaterial = value); }
		}

		public int SortingLayerID {
			get { return (first != null) ? first.sortingLayerID : 0; }
			set { DoForAll (x => x.sortingLayerID = value); }
		}

		public string SortingLayerName {
			get { return (first != null) ? first.sortingLayerName : null; }
			set { DoForAll (x => x.sortingLayerName = value); }
		}

		[SerializeField, HideInInspector] private int sortingOrder = 0;
		public int SortingOrder {
			get { return sortingOrder; }
			set { 
				sortingOrder = value;
				if (applySpriterZOrder)
					for (var i = 0; i < updaters.Length; i++)
						updaters [i].SortingOrder = value;
				else DoForAll (x => x.sortingOrder = value);
			}
		}

		[SerializeField, HideInInspector] private bool applySpriterZOrder = false;
		public bool ApplySpriterZOrder {
			get { return applySpriterZOrder; }
			set { 
				applySpriterZOrder = value;
				if (applySpriterZOrder) {
					var list = new List<SortingOrderUpdater> ();
					var spriteCount = renderers.Length;
					foreach (var renderer in renderers) {
						var updater = renderer.GetComponent<SortingOrderUpdater> ();
						if (updater == null) updater = renderer.gameObject.AddComponent<SortingOrderUpdater> ();
						updater.SortingOrder = sortingOrder;
						updater.SpriteCount = spriteCount;
						list.Add (updater);
					}
					updaters = list.ToArray ();
				}
				else {
					for (var i = 0; i < updaters.Length; i++) {
						if (Application.isPlaying) Destroy (updaters [i]);
						else DestroyImmediate (updaters [i]);
					}
					updaters = new SortingOrderUpdater [0];
					DoForAll (x => x.sortingOrder = sortingOrder);
				}
			}
		}
		
		[Header("Animations (Cached)")]
		private const string IS_WALKING_PARAM = "IsWalking";
		private const string IS_HURT_PARAM = "HurtTrigger";
		private const string IS_IDLE_PARAM = "IsIdle";
		private const string IS_DYING_PARAM = "DyingTrigger";
		private const string IS_SLASHING_PARAM = "SlashTrigger";
		[Header("Part Renderers (Cached)")]
        [SerializeField] private SpriteRenderer headRenderer;
        [SerializeField] private SpriteRenderer faceRenderer; 
        [SerializeField] private SpriteRenderer rightHandRenderer;
        [SerializeField] private SpriteRenderer leftHandRenderer;
        [SerializeField] private SpriteRenderer rightArmRenderer;
        [SerializeField] private SpriteRenderer leftArmRenderer;
        [SerializeField] private SpriteRenderer rightLegRenderer;
        [SerializeField] private SpriteRenderer leftLegRenderer;
        [SerializeField] private SpriteRenderer bodyRenderer;
        [SerializeField] private SpriteRenderer swordRenderer;
        [SerializeField] private SpriteRenderer fxRenderer;
        
        private Animator animator;

		private void Awake () {
			RefreshRenders();
			CachePartRenderers();
			animator = GetComponent<Animator>();
		}

		private void CachePartRenderers()
        {
            Transform boneRoot = transform; 

            Transform bone007 = boneRoot.Find("bone_006/bone_007");
            if (bone007 != null)
            {
                headRenderer = bone007.Find("Head")?.GetComponent<SpriteRenderer>();
                faceRenderer = bone007.Find("Face 01")?.GetComponent<SpriteRenderer>();
            }

            Transform bone001 = boneRoot.Find("bone_006/bone_000/bone_001");
            if (bone001 != null)
            {
                rightHandRenderer = bone001.Find("Right Hand")?.GetComponent<SpriteRenderer>();
            }
            rightArmRenderer = boneRoot.Find("bone_006/bone_000/Right Arm")?.GetComponent<SpriteRenderer>();

            Transform bone003 = boneRoot.Find("bone_006/bone_002/bone_003");
            if (bone003 != null)
            {
                swordRenderer = bone003.Find("Sword")?.GetComponent<SpriteRenderer>();
                leftHandRenderer = bone003.Find("Left Hand")?.GetComponent<SpriteRenderer>();
            }
            leftArmRenderer = boneRoot.Find("bone_006/bone_002/Left Arm")?.GetComponent<SpriteRenderer>();

            rightLegRenderer = boneRoot.Find("bone_005/Right Leg")?.GetComponent<SpriteRenderer>();
            leftLegRenderer = boneRoot.Find("bone_004/Left Leg")?.GetComponent<SpriteRenderer>();
            bodyRenderer = boneRoot.Find("bone_006/Body")?.GetComponent<SpriteRenderer>();
            fxRenderer = boneRoot.Find("SlashFX")?.GetComponent<SpriteRenderer>();
        }

		private void OnEnable () {
			DoForAll (x => x.enabled = true);
		}

		private void OnDisable () {
			DoForAll (x => x.enabled = false);
		}
		
		private void DoForAll (Action<SpriteRenderer> action) {
			for (var i = 0; i < renderers.Length; i++) action (renderers [i]);
		}
		private void ChangeAnimation(string animationName)
		{
			Debug.Log($"Changing animation to uiiii {animationName}");

			if (animator != null)
			{
				Debug.Log($"Changing animation to {animationName}");
				StopAnimation();
				if (animationName == IS_WALKING_PARAM || animationName == IS_IDLE_PARAM )
                {
					animator.SetBool(animationName, true);
                } else
                {
                    animator.SetTrigger(animationName);
                }
			}
		}

		public void RefreshRenders () {
			renderers = GetComponentsInChildren<SpriteRenderer> (true);
			updaters = GetComponentsInChildren<SortingOrderUpdater> (true);
			var length = updaters.Length;
			for (var i = 0; i < length; i++) updaters [i].SpriteCount = length;
			_first = null;
		}
		public void StopAnimation()
		{
			if (animator != null && animator.enabled)
			{
				animator.SetBool(IS_IDLE_PARAM, false);
				animator.SetBool(IS_WALKING_PARAM, false);
			}
		}
		public void StartAnimation()
		{
			if (animator != null)
			{
				animator.enabled = true;
				animator.SetBool(IS_IDLE_PARAM, true);
			}
		}
		public void PlayWalkingAnimation()
        {
			ChangeAnimation(IS_WALKING_PARAM);
        }
		public void PlayHurtAnimation()
        {
            ChangeAnimation(IS_HURT_PARAM);
        }
		public void PlayDyingAnimation()
        {
            ChangeAnimation(IS_DYING_PARAM);
        }
		public void PlaySlashingAnimation()
		{
			Debug.Log($"Changing animation to --------------- {IS_SLASHING_PARAM}");
			ChangeAnimation(IS_SLASHING_PARAM);
		}
		public void PlayIdleAnimation()
		{
			ChangeAnimation(IS_IDLE_PARAM);
		}
		public void ChangeRaceSprite(Sprite newSprite)
		{
			if (faceRenderer != null)
			{
				faceRenderer.sprite = newSprite;
			}
		}
		public void ChangeHeadSprite(Sprite newSprite)
		{
			if (headRenderer != null)
			{
				headRenderer.sprite = newSprite;
			}
		}
		public void ChangeBodySprite(Sprite newSprite)
		{
			if (bodyRenderer != null)
			{
				bodyRenderer.sprite = newSprite;
			}
		}

		public void ChangeArmsItems(Sprite newRArmSprite, Sprite newLArmSprite, Sprite newRHandSprite, Sprite newLHandSprite, Sprite newSwordSprite, Sprite newFXSprite)
		{
			// 1. Bras (Gauche et Droit)
			if (rightArmRenderer != null)
			{
				rightArmRenderer.sprite = newRArmSprite;
			}
			if (leftArmRenderer != null)
			{
				leftArmRenderer.sprite = newLArmSprite;
			}

			// 2. Mains (Gauche et Droite)
			if (rightHandRenderer != null)
			{
				rightHandRenderer.sprite = newRHandSprite;
			}
			if (leftHandRenderer != null)
			{
				leftHandRenderer.sprite = newLHandSprite;
			}

			// 3. Épée
			if (swordRenderer != null)
			{
				swordRenderer.sprite = newSwordSprite;
			}
			if (fxRenderer != null)
			{
				fxRenderer.sprite = newFXSprite;
			}
		}
		public void ChangeLegsSprite(Sprite newLLeg ,Sprite newRLeg)
		{
			if (leftLegRenderer != null)
			{
				leftLegRenderer.sprite = newLLeg;
			}
			if (rightLegRenderer != null)
			{
				rightLegRenderer.sprite = newRLeg;
			}
		}
	}
}
