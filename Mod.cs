﻿using System;
using ICities;
using ColossalFramework.UI;
using UnityEngine;

namespace CS_EmploymentDetailsExtender
{
    public class Mod : IUserMod
    {
        public string Name { get { return "Employement Details Extension"; } }
        public string Description { get { return "Add more informations about employment"; } }
    }

    /**
     * Mod Loading source code from Skylines-ExtendedPublicTransport
     * https://github.com/justacid/Skylines-ExtendedPublicTransport/tree/master/ExtendedPublicTransportUI
     **/
    public class JdeModLoader : LoadingExtensionBase
    {
        private UIEmploymentDetailPanel _employmentDetailsPanel;
        private UIComponent _unemployementPanel;
        private UIButton _unemploymentButton;

        private LoadMode _mode;

        public override void OnLevelLoaded(LoadMode mode)
        {
            _mode = mode;

            // don't load mod in asset and map editor
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            // attach extended panels
            var view = UIView.GetAView();

            if (view == null)
            {
                Debug.Log("Failed to get AView");
                return;
            }

            var goJdp = new GameObject("EmploymentDetailsPanel");
            _employmentDetailsPanel = goJdp.AddComponent<UIEmploymentDetailPanel>();

            if (_employmentDetailsPanel == null)
            {
                Debug.Log("Failed to create Employement Detail Panel");
                return;
            }

            _employmentDetailsPanel.transform.parent = view.transform;

            HookIntoNativeUI();
        }

        public override void OnLevelUnloading()
        {
            if (_mode != LoadMode.LoadGame && _mode != LoadMode.NewGame)
                return;

            // Making absolutely sure to unsubscribe ALL callbacks that where set on
            // loading the mod. It seems the game sometimes(?) caches UI Elements even
            // when going back to the main menu. This obviously leads to issues when
            // the now non-existent GameObjects are being referenced.
            if (_unemployementPanel != null)
            {
                if (_unemployementPanel.parent != null)
                    _unemployementPanel.parent.eventVisibilityChanged -= _employmentDetailsPanel.InfoPanelOnEventVisibilityChanged;

                if (_unemploymentButton != null) 
                    _unemployementPanel.RemoveUIComponent(_unemploymentButton);
            }

            if (_employmentDetailsPanel != null)
                GameObject.Destroy(_employmentDetailsPanel.gameObject);

            if (_unemploymentButton != null)
                GameObject.Destroy(_unemploymentButton.gameObject);
        }

        private void HookIntoNativeUI()
        {
            _unemployementPanel = UIUtil.FindUIComponent("Unemployment");

            if (_unemployementPanel == null)
            {
                Debug.Log("Failed to locate Unemployement Panel - could not hook into native UI.");
                return;
            }

            _unemploymentButton = _unemployementPanel.AddUIComponent<UIButton>();

            if (_unemploymentButton == null)
            {
                Debug.Log("Failed to add Unemployement Button - could not hook into native UI.");
                return;
            }

            _unemploymentButton.relativePosition = new Vector3(300, 0);
            _unemploymentButton.size = new Vector2(17, 16);
            _unemploymentButton.textScale = 1f;
            _unemploymentButton.normalBgSprite = "ButtonMenu";
            _unemploymentButton.hoveredBgSprite = "ButtonMenuHovered";
            _unemploymentButton.focusedBgSprite = "ButtonMenuFocused";
            _unemploymentButton.pressedBgSprite = "ButtonMenuPressed";
            _unemploymentButton.text = "+";

            // extended bus hook
            _unemploymentButton.eventClick += UnemployementButtonOnEventClick;
           
            // hide all extended panels, when the population info view gets closed
            _unemployementPanel.parent.eventVisibilityChanged += _employmentDetailsPanel.InfoPanelOnEventVisibilityChanged;
        }

        private void UnemployementButtonOnEventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            _employmentDetailsPanel.isVisible = !_employmentDetailsPanel.isVisible;
            _employmentDetailsPanel.relativePosition = new Vector3(438, 58);
        }
    }
}
