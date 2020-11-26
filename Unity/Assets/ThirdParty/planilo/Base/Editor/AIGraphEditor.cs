using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace Planilo {

  /// <summary>Base editor class for any AIGraph editor.</summary>
  public abstract class AIGraphEditor : NodeGraphEditor {
    public override void OnGUI() {
      // Keep repainting the GUI of the active NodeEditorWindow
      NodeEditorWindow.current.Repaint();
    }
  }
}