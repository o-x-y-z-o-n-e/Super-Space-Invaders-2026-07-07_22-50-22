using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour {
	
	private Canvas canvas;
	private Dictionary<string, GuiView> viewLookup;
	private List<GuiView> viewList;
	private Stack<GuiView> viewStack;
	
	[SerializeField] private GuiView defaultView;
	[SerializeField] private Image screenTransition;
	
	private bool screenTransitionFade;
	private float screenTransitionTime;
	private Action screenTransitionOnClose;
	private Action screenTransitionOnOpen;

	private void Awake() {
		canvas = GetComponent<Canvas>();
		viewLookup = new Dictionary<string, GuiView>();
		viewList = new List<GuiView>();
		viewStack = new Stack<GuiView>();

		foreach(var view in GetComponentsInChildren<GuiView>(true)) {
			string name = view.GetType().Name;
			if(!viewLookup.ContainsKey(name)) {
				viewLookup.Add(name, view);
				viewList.Add(view);
			} else {
				Debug.LogError($"View of type name {name} is already registered.");
			}
		}
	}

	private void Update() {
		if(screenTransition.gameObject.activeSelf) {
			if(screenTransitionFade) {
				screenTransitionTime = Mathf.Clamp01(screenTransitionTime + Time.deltaTime);
				if(screenTransitionTime == 1.0F) {
					screenTransitionFade = false;
					screenTransitionOnClose?.Invoke();
				}
			} else {
				screenTransitionTime = Mathf.Clamp01(screenTransitionTime - Time.deltaTime);
				if(screenTransitionTime == 0.0F) {
					screenTransition.gameObject.SetActive(false);
					screenTransitionOnOpen?.Invoke();
				}
			}
			screenTransition.color = new Color(0, 0, 0, Mathf.SmoothStep(0, 1, screenTransitionTime));
		} else {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				viewStack.Peek()?.OnEscapePressed();
			}
		}
	}

	private void Start() {
		if(viewStack.Count == 0 && defaultView != null) {
			Open(defaultView, true);
		}
	}

	public T Find<T>() where T : GuiView {
		foreach(var view in viewList) {
			if(view is T) {
				return (T)view;
			}
		}
		return null;
	}

	public void Open(GuiView view, bool clearStack = false) {
		if(clearStack) {
			CloseAll();
		}
		viewStack.Push(view);
		view.gameObject.SetActive(true);
	}

	public T Open<T>(bool clearStack = false) where T : GuiView {
		T view = Find<T>();
		if(view == null) {
			Debug.LogError($"View {nameof(T)} does not exist");
			return null;
		}
		Open(view, clearStack);
		return view;
	}

	public void CloseAll() {
		while(viewStack.Count > 0) {
			Back();
		}
	}

	public void Back() {
		viewStack.Pop().gameObject.SetActive(false);
		if(viewStack.Count > 0) {
			viewStack.Peek().gameObject.SetActive(true);
		}
	}

	public void Transition(Action onClose = null, Action onOpen = null) {
		screenTransitionFade = true;
		screenTransitionTime = 0.0F;
		screenTransitionOnClose = onClose;
		screenTransitionOnOpen = onOpen;
		screenTransition.color = new Color(0, 0, 0, 0);
		screenTransition.gameObject.SetActive(true);
	}
    
}