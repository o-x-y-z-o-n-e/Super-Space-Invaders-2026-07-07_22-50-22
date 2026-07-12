using UnityEngine;

public class GuiView : MonoBehaviour {

	public virtual void OnEscapePressed() {
		Core.Gui.Back();
	}

	protected virtual void OnEnable() {
		
	}

	protected virtual void OnDisable() {
		
	}

}
