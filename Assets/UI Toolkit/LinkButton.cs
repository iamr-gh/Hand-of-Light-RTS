using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LinkButton : Button {
    public string link { get; set; }
    public new class UxmlFactory : UxmlFactory<LinkButton, UxmlTraits> { }
    public new class UxmlTraits : Button.UxmlTraits {
        UxmlStringAttributeDescription m_Status = new UxmlStringAttributeDescription { name = "link" };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
            base.Init(ve, bag, cc);
            ((LinkButton)ve).link = m_Status.GetValueFromBag(bag, cc);
        }
    }

    public LinkButton() {
        RegisterCallback<ClickEvent>(OpenURL);
    }

    private void OpenURL(ClickEvent evt) {
        Application.OpenURL(link);
    }
}
