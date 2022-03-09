using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using Object = UnityEngine.Object;
using Pinwheel.TextureGraph.UIElements;
#if TG_SEARCHER
using UnityEditor.Searcher;
#endif

namespace Pinwheel.TextureGraph
{
    public partial class TGraphEditor : EditorWindow
    {
        public static readonly string UNDO_NAME_GRAPH_EDITING = "Graph Editing";

        private static List<TGraphEditor> openedEditors;
        public static List<TGraphEditor> OpenedEditors
        {
            get
            {
                if (openedEditors == null)
                {
                    openedEditors = new List<TGraphEditor>();
                }
                return openedEditors;
            }
        }

        private static List<TGraph> graphDirtyState;
        public static List<TGraph> GraphDirtyState
        {
            get
            {
                if (graphDirtyState == null)
                {
                    graphDirtyState = new List<TGraph>();
                }
                return graphDirtyState;
            }
            set
            {
                graphDirtyState = value;
            }
        }

        private static TGraph openingGraph;
        public TGraph SourceGraph { get; set; }
        public TGraph ClonedGraph { get; set; }

        public VisualElement MainContainer;
        public TGraphEditorToolbar Toolbar;
        public TGraphView GraphView;
        public ScrollView ParameterScrollView;
#if TG_SEARCHER
        private TNodeSearcherWindow nodeSearcherWindow;
#endif

        public TView2DWindow view2DWindow;
        public const string SUB_WINDOW_2D_VIEW_DATA_KEY = "view2D";

        public TView3DWindow view3DWindow;
        public const string SUB_WINDOW_3D_VIEW_DATA_KEY = "view3D";

        public TGraphSettingsWindow settingsWindow;
        public TExportWindow exportWindow;

        public static void OpenGraph(TGraph graph)
        {
            TGraphEditor window = OpenedEditors.Find(e => e.SourceGraph == graph);
            if (window == null)
            {
                openingGraph = graph;
                window = ScriptableObject.CreateInstance<TGraphEditor>();
                window.titleContent = new GUIContent(graph.name);
                window.SourceGraph = graph;
            }
            window.Show();
            window.Focus();
        }

        public void OnEnable()
        {
            OpenedEditors.Add(this);
            CreateGUI();
            RegisterCallbacks();
        }

        public void OnDisable()
        {
            UnregisterCallbacks();
            RemoveGUI();
            OpenedEditors.Remove(this);
        }

        private void OnDestroy()
        {
            if (view2DWindow != null)
            {
                view2DWindow.OnDestroy();
            }
            if (view3DWindow != null)
            {
                view3DWindow.OnDestroy();
            }

            if (ClonedGraph != null && SourceGraph != null && IsGraphDirty(SourceGraph))
            {
                if (EditorUtility.DisplayDialog(
                    "Texture Graph has been modified",
                    $"Do you want to save change(s) made to your graph?\n{AssetDatabase.GetAssetPath(SourceGraph)}\n\nIf you don't save, all changes will be lost!",
                    "Save", "Don't Save"))
                {
                    TGraphSaver.Save(ClonedGraph, SourceGraph);
                }
                SetGraphDirty(SourceGraph, false);
            }
        }

        private void OnGUI()
        {
            if (IsGraphDirty(SourceGraph))
            {
                titleContent = new GUIContent(SourceGraph.name + "*");
            }
            else
            {
                titleContent = new GUIContent(SourceGraph.name);
            }

            if (Event.current != null && Event.current.isKey)
            {
                HandleShortcutKeys(Event.current.keyCode, Event.current.control, Event.current.shift, Event.current.alt);
            }
        }

        private void RegisterCallbacks()
        {
            TGraphView.NodeToInspectChanged += OnNodeToInspectChanged;
            TGraphView.NodeDoubleClicked += OnNodeDoubleClicked;
            TAbstractTextureNode.OnAfterExecuting += OnAfterExecutingNode;

            if (GraphView != null)
            {
                //GraphView.graphViewChanged += OnGraphViewChanged;
                GraphView.RegisterCallback<KeyDownEvent>(HandleShortcutKeysOnGraphView);
            }

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void UnregisterCallbacks()
        {
            TGraphView.NodeToInspectChanged -= OnNodeToInspectChanged;
            TGraphView.NodeDoubleClicked -= OnNodeDoubleClicked;
            TAbstractTextureNode.OnAfterExecuting -= OnAfterExecutingNode;

            if (GraphView != null)
            {
                //GraphView.graphViewChanged -= OnGraphViewChanged;
                GraphView.UnregisterCallback<KeyDownEvent>(HandleShortcutKeysOnGraphView);
            }

            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void CreateGUI()
        {
            if (ClonedGraph == null)
            {
                if (SourceGraph == null && openingGraph != null)
                {
                    SourceGraph = openingGraph;
                    openingGraph = null;
                }
                if (SourceGraph != null)
                {
                    ClonedGraph = ScriptableObject.Instantiate<TGraph>(SourceGraph);
                    //ClonedGraph = SourceGraph;
                    ClonedGraph.name = SourceGraph.name;
                }
                else
                {
                    Close();
                    return;
                }
            }
            StyleSheet styles = Resources.Load<StyleSheet>("TextureGraph/USS/TextureGraphStyles");

            MainContainer = new VisualElement() { name = "MainContainer" };
            rootVisualElement.Add(MainContainer);
            MainContainer.StretchToParentSize();
            MainContainer.styleSheets.Add(styles);

            Toolbar = new TGraphEditorToolbar();
            MainContainer.Add(Toolbar);

            VisualElement bodyContainer = new VisualElement() { name = "BodyContainer" };
            bodyContainer.AddToClassList(TConst.USS_STRETCH);
            bodyContainer.AddToClassList(TConst.USS_ROW);
            MainContainer.Add(bodyContainer);

            VisualElement leftContainer = new VisualElement() { name = "LeftContainer" };
            leftContainer.AddToClassList(TConst.USS_STRETCH);
            leftContainer.AddToClassList(TConst.USS_LEFT_CONTAINER);
            bodyContainer.Add(leftContainer);

            GraphView = new TGraphView();
            GraphView.GraphEditor = this;
            leftContainer.Add(GraphView);
            GraphView.AddToClassList(TConst.USS_STRETCH);
            GraphView.graphViewChanged = OnGraphViewChanged;

            VisualElement rightContainer = new VisualElement() { name = "RightContainer" };
            rightContainer.AddToClassList(TConst.USS_RIGHT_CONTAINER);
            TResizer resizer = new TResizer() { Target = rightContainer };
            rightContainer.Add(resizer);
            bodyContainer.Add(rightContainer);

            ParameterScrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            ParameterScrollView.AddToClassList(TConst.USS_STRETCH);
            rightContainer.Add(ParameterScrollView);

#if TG_SEARCHER
            nodeSearcherWindow = ScriptableObject.CreateInstance<TNodeSearcherWindow>();
            nodeSearcherWindow.Host = this;
            GraphView.nodeCreationRequest = OnNodeCreationRequest;
#endif

            GraphView.CreateViews(ClonedGraph);
            GraphView.viewDataKey = "texture-graph-view" + SourceGraph.name;

            string view2DKey = ClonedGraph.name + SUB_WINDOW_2D_VIEW_DATA_KEY;
            view2DWindow = new TView2DWindow(view2DKey);
            view2DWindow.GraphEditor = this;
            view2DWindow.OnEnable();
            bodyContainer.Add(view2DWindow);
            view2DWindow.SetImage(ClonedGraph.GetMainRT(view2DWindow.viewData.nodeGuid));
            if (TViewManager.IsViewVisible(view2DKey))
            {
                TViewManager.ShowView(view2DWindow, view2DKey);
            }
            else
            {
                TViewManager.HideView(view2DWindow, view2DKey);
            }

            string view3DKey = ClonedGraph.name + SUB_WINDOW_3D_VIEW_DATA_KEY;
            view3DWindow = new TView3DWindow(view3DKey);
            view3DWindow.GraphEditor = this;
            view3DWindow.OnEnable();
            bodyContainer.Add(view3DWindow);
            if (TViewManager.IsViewVisible(view3DKey))
            {
                TViewManager.ShowView(view3DWindow, view3DKey);
            }
            else
            {
                TViewManager.HideView(view3DWindow, view3DKey);
            }
            Toolbar.GraphEditor = this;

            if (ClonedGraph != null)
            {
                ClonedGraph.OnAfterExecuting = OnAfterExecutingGraph;
                ClonedGraph.Execute();
            }
        }

        private void RemoveGUI()
        {
            if (MainContainer != null)
            {
                rootVisualElement.Remove(MainContainer);
            }
#if TG_SEARCHER
            if (nodeSearcherWindow != null)
            {
                Object.DestroyImmediate(nodeSearcherWindow);
            }
#endif
            if (SourceGraph != null)
            {
                CleanUp(SourceGraph);
            }
            if (ClonedGraph != null)
            {
                CleanUp(ClonedGraph);
            }
            if (view2DWindow != null)
            {
                view2DWindow.OnDisable();
            }
            if (view3DWindow != null)
            {
                view2DWindow.OnDisable();
            }
        }

        private void CleanUp(TGraph graph)
        {
            List<TAbstractTextureNode> nodes = graph.GraphData.Nodes;
            foreach (TAbstractTextureNode n in nodes)
            {
                n.Dispose();
            }
        }

#if TG_SEARCHER
        private void OnNodeCreationRequest(NodeCreationContext context)
        {
            SearcherWindow.Show(
                this,
                nodeSearcherWindow.GetItems(),
                "Create Node",
                item =>
                {
                    if (item == null)
                        return false;
                    TNodeSearcherWindow.TNodeSearcherItem i = item as TNodeSearcherWindow.TNodeSearcherItem;
                    if (i.Type == null)
                    {
                        return false;
                    }
                    else
                    {
                        CreateNodeOfType(i.Type, context.screenMousePosition);
                        return true;
                    }
                },
                context.screenMousePosition - this.position.position,
                new SearcherWindow.Alignment(SearcherWindow.Alignment.Vertical.Top, SearcherWindow.Alignment.Horizontal.Left));
        }
#endif

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            HandleMoveElements(change);
            HandleRemoveEdges(change);
            HandleAddEdges(change);
            HandleRemoveNodes(ref change);

            SetGraphDirty(SourceGraph, true);
            return change;
        }

        private void HandleMoveElements(GraphViewChange change)
        {
            if (change.movedElements == null)
                return;
            List<GraphElement> movedElements = change.movedElements;
            List<Guid> movedNodeGuids = new List<Guid>();
            foreach (GraphElement e in movedElements)
            {
                if (e is Node nodeView)
                {
                    movedNodeGuids.Add((Guid)nodeView.userData);
                }
            }

            Undo.RegisterCompleteObjectUndo(ClonedGraph, UNDO_NAME_GRAPH_EDITING);
            ClonedGraph.GraphData.MoveNodes(movedNodeGuids, change.moveDelta);
        }

        private void HandleRemoveEdges(GraphViewChange change)
        {
            if (change.elementsToRemove == null)
                return;
            List<GraphElement> removedElements = change.elementsToRemove;
            List<Guid> removedEdgeGuids = new List<Guid>();
            List<TSlotReference> inputRefs = new List<TSlotReference>();
            foreach (GraphElement e in removedElements)
            {
                if (e is Edge edge)
                {
                    if (edge != null && edge.userData != null)
                    {
                        removedEdgeGuids.Add((Guid)edge.userData);

                        Port inputPort = edge.input;
                        if (inputPort != null && inputPort.userData != null)
                        {
                            TSlotReference inputRef = (TSlotReference)inputPort.userData;
                            inputRefs.Add(inputRef);
                        }
                    }
                }
            }

            if (removedEdgeGuids.Count > 0)
            {
                Undo.RegisterCompleteObjectUndo(ClonedGraph, UNDO_NAME_GRAPH_EDITING);
                ClonedGraph.GraphData.RemoveEdges(removedEdgeGuids);
                foreach (TSlotReference r in inputRefs)
                {
                    ClonedGraph.ExecuteAt(r.NodeGuid);
                }
            }
        }

        private void HandleAddEdges(GraphViewChange change)
        {
            if (change.edgesToCreate == null)
                return;
            List<Edge> edgesToCreate = change.edgesToCreate;
            foreach (Edge edgeView in edgesToCreate)
            {
                if (edgeView.isGhostEdge)
                {
                    Debug.Log("Not adding a ghost edge");
                    continue;
                }

                Port outputPort = edgeView.output;
                Port inputPort = edgeView.input;
                TSlotReference outputRef = (TSlotReference)outputPort.userData;
                TSlotReference inputRef = (TSlotReference)inputPort.userData;

                Undo.RegisterCompleteObjectUndo(ClonedGraph, UNDO_NAME_GRAPH_EDITING);
                TEdge edge = new TEdge(outputRef, inputRef);
                ClonedGraph.GraphData.AddEdge(edge);
                edgeView.userData = edge.GUID;
                ClonedGraph.ExecuteAt(inputRef.NodeGuid);
            }
        }

        private void HandleRemoveNodes(ref GraphViewChange change)
        {
            if (change.elementsToRemove == null)
                return;

            List<GraphElement> elementsToRemove = change.elementsToRemove;
            List<GraphElement> connectedEdgeViewToRemove = new List<GraphElement>();
            List<Guid> removedNodeGuids = new List<Guid>();
            List<Guid> removedEdgeGuids = new List<Guid>();

            foreach (GraphElement element in elementsToRemove)
            {
                if (element is Node node)
                {
                    removedNodeGuids.Add((Guid)node.userData);
                    GraphView.edges.ForEach(e =>
                    {
                        if (e.input.node == node || e.output.node == node)
                        {
                            removedEdgeGuids.Add((Guid)e.userData);
                            connectedEdgeViewToRemove.Add(e);
                        }
                    });
                }
            }

            if (removedNodeGuids.Count > 0)
            {
                Undo.RegisterCompleteObjectUndo(ClonedGraph, UNDO_NAME_GRAPH_EDITING);
                ClonedGraph.GraphData.RemoveNodes(removedNodeGuids);
                ClonedGraph.GraphData.RemoveEdges(removedEdgeGuids);

                elementsToRemove.AddRange(connectedEdgeViewToRemove);
                OnNodeToInspectChanged(GraphView, null);
            }
        }

        public TNodeView CreateNodeOfType(Type type, Vector2 screenMousePosition)
        {
            Undo.RegisterCompleteObjectUndo(ClonedGraph, UNDO_NAME_GRAPH_EDITING);
            TAbstractTextureNode newNode = Activator.CreateInstance(type) as TAbstractTextureNode;
            ClonedGraph.GraphData.AddNode(newNode);

            TNodeView newNodeView = TNodeViewCreator.Create(newNode, ClonedGraph);
            GraphView.AddElement(newNodeView);

            Vector2 mouseWorldPos = screenMousePosition - this.position.position; ;
            Vector2 mouseLocalPos = GraphView.contentViewContainer.WorldToLocal(mouseWorldPos);
            Vector2 nodePos = mouseLocalPos - TConst.NODE_CREATION_POSITION_OFFSET;
            Rect pos = new Rect(nodePos.x, nodePos.y, 0, 0);
            newNodeView.SetPosition(pos);
            TNodeDrawState newState = newNode.DrawState;
            newState.position = pos;
            newNode.DrawState = newState;

            ClonedGraph.ExecuteAt(newNode.GUID);
            SetGraphDirty(SourceGraph, true);

            GraphView.ClearSelection();
            GraphView.AddToSelection(newNodeView);
            GraphView.HandleNodeToInspect();

            return newNodeView;
        }

        private void OnNodeToInspectChanged(TGraphView gv, Node n)
        {
            if (gv != GraphView)
                return;
            ParameterScrollView.contentContainer.Clear();
            if (n == null)
                return;

            Foldout paramFoldout = new Foldout() { text = "PARAMETERS" };
            paramFoldout.AddToClassList("unity-toolbar");
            paramFoldout.viewDataKey = "params-foldout";
            ParameterScrollView.Add(paramFoldout);

            TAbstractTextureNode node = ClonedGraph.GraphData.GetNodeByGUID((Guid)n.userData);
            TParametersDrawer paramsDrawer = TParameterDrawerInitializer.GetDrawer(node.GetType());

            if (paramsDrawer != null)
            {
                paramsDrawer.Graph = ClonedGraph;
                VisualElement paramGui = new IMGUIContainer(() =>
                {
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 110;
                    EditorGUI.BeginChangeCheck();
                    paramsDrawer.DrawGUI(node);
                    if (EditorGUI.EndChangeCheck())
                    {
                        //Undo.RegisterCompleteObjectUndo(ClonedGraph, UNDO_NAME_GRAPH_EDITING);
                        ClonedGraph.ExecuteAt(node.GUID);
                        SetGraphDirty(SourceGraph, true);
                    }
                    EditorGUIUtility.labelWidth = labelWidth;
                });

                paramFoldout.contentContainer.Add(paramGui);
            }

            //VisualElement internalFunctionVE = new VisualElement();
            //internalFunctionVE.style.marginTop = new StyleLength(10);
            //internalFunctionVE.style.borderTopColor = new StyleColor(Color.black);
            //internalFunctionVE.style.borderTopWidth = new StyleFloat(1);

            //IMGUIContainer internalIMGUI = new IMGUIContainer(() =>
            //{
            //    float labelWidth = EditorGUIUtility.labelWidth;
            //    EditorGUIUtility.labelWidth = 110;

            //    EditorGUILayout.LabelField("Node position", node.DrawState.position.position.ToString());
            //    EditorGUILayout.LabelField("Node view position", n.GetPosition().position.ToString());

            //    EditorGUIUtility.labelWidth = labelWidth;
            //});
            //internalFunctionVE.Add(internalIMGUI);
            //parameterScrollView.Add(internalFunctionVE);
        }

        private void OnNodeDoubleClicked(TGraphView gv, Node n)
        {
            if (gv != GraphView)
                return;
            TAbstractTextureNode node = ClonedGraph.GraphData.GetNodeByGUID((Guid)n.userData);
            if (node != null)
            {
                view2DWindow.viewData.nodeGuid = node.GUID;
                ClonedGraph.ExecuteAt(node.GUID);
            }
        }

        private void OnAfterExecutingNode(TAbstractTextureNode n, TAbstractTextureNode.TExecutionMetadata meta)
        {
            if (GraphView != null)
            {
                TNodeView nodeView = GraphView.FindNode(n.GUID);
                if (nodeView != null)
                {
                    string metaText = $"{meta.resolution.x}x{meta.resolution.y}\n{meta.format}";
                    nodeView.SetMetadata(metaText);

                    if (n.GUID == view2DWindow.viewData.nodeGuid)
                    {
                        TSlot previewSlot = n.GetMainOutputSlot();
                        if (previewSlot != null)
                        {
                            RenderTexture tex = ClonedGraph.GetRT(TSlotReference.Create(n.GUID, previewSlot.Id));
                            view2DWindow.SetImage(tex);
                        }
                    }
                }
            }
        }

        private void OnAfterExecutingGraph(TGraph graph) //Cloned Graph execution callback
        {
            if (view3DWindow != null)
            {
                if (TViewManager.IsViewVisible(view3DWindow.viewDataKey))
                {
                    view3DWindow.RenderPreviewScene();
                }
            }
        }

        public static void SetGraphDirty(TGraph graph, bool isDirty)
        {
            if (isDirty)
            {
                if (!GraphDirtyState.Contains(graph))
                {
                    GraphDirtyState.Add(graph);
                }
            }
            else
            {
                GraphDirtyState.RemoveAll(g => g == graph);
            }
        }

        public static bool IsGraphDirty(TGraph graph)
        {
            bool isDirty = GraphDirtyState.Contains(graph);
            return isDirty;
        }

        private void OnUndoRedo()
        {
            GraphView.UpdateViewOnUndoRedo(ClonedGraph);
            view3DWindow.RenderPreviewScene();
        }
    }
}
