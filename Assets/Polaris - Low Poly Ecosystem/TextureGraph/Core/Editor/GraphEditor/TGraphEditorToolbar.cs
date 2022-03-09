using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using System;

namespace Pinwheel.TextureGraph
{
    public class TGraphEditorToolbar : Toolbar
    {
        public enum TToolbarMenu
        {
            File, Edit, Create, Module, View, Help
        }

        public TGraphEditor GraphEditor { get; set; }

        public TGraphEditorToolbar() : base()
        {
            StyleSheet styles = Resources.Load<StyleSheet>("TextureGraph/USS/ToolbarStyles");
            this.styleSheets.Add(styles);

            AddMenus();
        }

        private void AddMenus()
        {
            foreach (object m in System.Enum.GetValues(typeof(TToolbarMenu)))
            {
                AddMenu((TToolbarMenu)m);
            }
        }

        private void AddMenu(TToolbarMenu menuType)
        {
            ToolbarButton button = new ToolbarButton();
            button.text = menuType.ToString();

            button.clicked += () =>
            {
                Rect r = button.layout;
                r.position = new Vector2(r.min.x, r.max.y);

                GenericMenu menu = GetMenu(menuType);
                if (menu != null)
                {
                    menu.DropDown(r);
                }
            };

            this.Add(button);
        }

        private GenericMenu GetMenu(TToolbarMenu menuType)
        {
            if (menuType == TToolbarMenu.File)
                return GetFileMenu();
            else if (menuType == TToolbarMenu.Edit)
                return GetEditMenu();
            else if (menuType == TToolbarMenu.Create)
                return GetCreateMenu();
            else if (menuType == TToolbarMenu.Module)
                return GetModuleMenu();
            else if (menuType == TToolbarMenu.View)
                return GetViewMenu();
            else if (menuType == TToolbarMenu.Help)
                return GetHelpMenu();
            return null;
        }

        private GenericMenu GetFileMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddMenuItem(
                true,
                "Save %S",
                () => { GraphEditor.HandleSave(); });
            menu.AddMenuItem(
                true,
                "Export",
                () => { GraphEditor.OpenExportWindow(); });

            return menu;
        }

        private GenericMenu GetEditMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddMenuItem(
                true,
                "Undo %Z",
                () => { GraphEditor.HandleUndo(); });
            menu.AddMenuItem(
                true,
                "Redo %Y",
                () => { GraphEditor.HandleRedo(); });
            menu.AddSeparator(null);
            menu.AddMenuItem(
                GraphEditor.HasSelection(),
                "Cut %X",
                () => { GraphEditor.HandleCut(); });
            menu.AddMenuItem(
                GraphEditor.HasSelection(),
                "Copy %C",
                () => { GraphEditor.HandleCopy(); });
            menu.AddMenuItem(
                GraphEditor.CanPaste(),
                "Paste %V",
                () => { GraphEditor.HandlePaste(); });
            menu.AddSeparator(null);
            menu.AddMenuItem(
                GraphEditor.HasSelection(),
                "Duplicate %D",
                () => { GraphEditor.HandleCopy(); GraphEditor.HandlePaste(); });
            menu.AddSeparator(null);
            menu.AddMenuItem(
                GraphEditor.HasSelection(),
                "Delete",
                () => { GraphEditor.GraphView.DeleteSelection(); });

            menu.AddSeparator(null);
            menu.AddMenuItem(
                true,
                "Graph Settings",
                () => { GraphEditor.OpenGraphSettings(); });

            return menu;
        }

        private GenericMenu GetCreateMenu()
        {
            GenericMenu menu = new GenericMenu();
            List<TMenuEntryGroup<Type>> flattenMenu = TNodeCreationMenu.GetFlattenMenu();
            for (int i = 0; i < flattenMenu.Count; ++i)
            {
                TMenuEntryGroup<Type> entry = flattenMenu[i];
                if (entry.Data == null)
                    continue;

                menu.AddItem(
                    new GUIContent("Node/" + entry.Path),
                    false,
                    () =>
                    {
                        Vector2 screenPos = GraphEditor.position.position + GraphEditor.GraphView.LocalToWorld(GraphEditor.GraphView.layout.center);
                        GraphEditor.CreateNodeOfType(entry.Data, screenPos);
                    });
            }
            return menu;
        }

        private GenericMenu GetModuleMenu()
        {
            GenericMenu menu = new GenericMenu();
//            bool isHighResModuleInstalled = false;
//#if TEXTURE_GRAPH_HIGH_RESOLUTION
//            isHighResModuleInstalled = true;
//#endif
            //menu.AddItem(
            //    new GUIContent("High Resolution"),
            //    isHighResModuleInstalled,
            //    () => { Application.OpenURL(TLink.HIGH_RESOLUTION_MODULE); });

            return menu;
        }

        private GenericMenu GetViewMenu()
        {
            GenericMenu menu = new GenericMenu();

            string view2Dkey = GraphEditor.ClonedGraph.name + TGraphEditor.SUB_WINDOW_2D_VIEW_DATA_KEY;
            menu.AddItem(
                new GUIContent("2D View"),
                TViewManager.IsViewVisible(view2Dkey),
                () =>
                {
                    TViewManager.ToggleViewVisibility(GraphEditor.view2DWindow, view2Dkey);
                });

            string view3Dkey = GraphEditor.ClonedGraph.name + TGraphEditor.SUB_WINDOW_3D_VIEW_DATA_KEY;
            menu.AddItem(
                new GUIContent("3D View"),
                TViewManager.IsViewVisible(view3Dkey),
                () =>
                {
                    TViewManager.ToggleViewVisibility(GraphEditor.view3DWindow, view3Dkey);
                });


            return menu;
        }

        private GenericMenu GetHelpMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(
                new GUIContent("User Guide"),
                false,
                () => { Application.OpenURL(TLink.USER_GUIDE); });
            menu.AddItem(
                new GUIContent("Scripting API"),
                false,
                () => { Application.OpenURL(TLink.SCRIPTING_API); });
            menu.AddSeparator(null);
            menu.AddItem(
                new GUIContent("Facebook"),
                false,
                () => { Application.OpenURL(TLink.FACEBOOK); });
            menu.AddItem(
                new GUIContent("Youtube"),
                false,
                () => { Application.OpenURL(TLink.YOUTUBE); });
            menu.AddItem(
                new GUIContent("Have A Chat"),
                false,
                () => { Application.OpenURL(TLink.DISCORD); });
            menu.AddItem(
                new GUIContent("Send Support Email"),
                false,
                () => { TEditorCommon.OpenEmailEditor(TLink.SUPPORT_EMAIL, "[Texture Graph] SUBJECT_HERE", "MESSAGE_HERE"); });
            menu.AddSeparator(null);
            menu.AddItem(
                new GUIContent("Version Info"),
                false,
                () => { EditorUtility.DisplayDialog("Version Info", TVersionInfo.ProductNameAndVersion, "OK"); });
            menu.AddItem(
                new GUIContent("Release Logs"),
                false,
                () => { Application.OpenURL(TLink.RELEASE_LOG); });
            menu.AddItem(
                new GUIContent("Leave A Review"),
                false,
                () => { Application.OpenURL(TLink.STORE_PAGE); });

            return menu;
        }


    }
}
