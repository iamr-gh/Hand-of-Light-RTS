using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelButton : Button {
    public int level { get; set; }
    public new class UxmlFactory : UxmlFactory<LevelButton, UxmlTraits> { }
    public new class UxmlTraits : Button.UxmlTraits {
        UxmlIntAttributeDescription m_Status = new UxmlIntAttributeDescription { name = "level" };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
            base.Init(ve, bag, cc);
            ((LevelButton)ve).level = m_Status.GetValueFromBag(bag, cc);
        }
    }

    public LevelButton() {
        RegisterCallback<ClickEvent>(LoadScene);
    }

    private void LoadScene(ClickEvent evt) {
        MainMenuUI.instance.LoadScene(level);
    }
}
