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
		
		private const string IS_WALKING_PARAM = "IsWalking";
		private const string IS_HURT_PARAM = "HurtTrigger";
		private const string IS_IDLE_PARAM = "IsIdle";
		private const string IS_DYING_PARAM = "DyingTrigger";
		private const string IS_SLASHING_PARAM = "SlashTrigger";

		private void Awake () {
			RefreshRenders ();
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
			Animator animator = GetComponent<Animator>();
			if (animator != null)
			{
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
			Animator animator = GetComponent<Animator>();
			if (animator != null && animator.enabled)
			{
				animator.ResetTrigger(IS_DYING_PARAM);
				animator.ResetTrigger(IS_HURT_PARAM);
				animator.ResetTrigger(IS_SLASHING_PARAM);
				animator.SetBool(IS_IDLE_PARAM, false);
				animator.SetBool(IS_WALKING_PARAM, false);
			}
		}
		public void StartAnimation()
		{
			Animator animator = GetComponent<Animator>();
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
			ChangeAnimation(IS_SLASHING_PARAM);
		}
		public void PlayIdleAnimation()
		{
			ChangeAnimation(IS_IDLE_PARAM);
		}
	}
}
