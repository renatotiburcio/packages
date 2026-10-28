using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.TreeView
{
    public partial class XFTreeView : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        private readonly HashSet<string> expandedIds = new(StringComparer.Ordinal);
        private readonly HashSet<string> selectedIds = new(StringComparer.Ordinal);
        private readonly HashSet<string> checkedIds = new(StringComparer.Ordinal);

        private XFTreeNode? _dragSource;
        private string? _dropTargetId;
        private XFTreeDropPosition _dropPosition = XFTreeDropPosition.None;

        protected override string BaseClass => "xf-treeview";

        [Parameter] public IReadOnlyList<XFTreeNode>? Items { get; set; }
        [Parameter] public IReadOnlyList<string> ExpandedIds { get; set; } = Array.Empty<string>();
        [Parameter] public EventCallback<IReadOnlyList<string>> ExpandedIdsChanged { get; set; }
        [Parameter] public IReadOnlyList<string> SelectedIds { get; set; } = Array.Empty<string>();
        [Parameter] public EventCallback<IReadOnlyList<string>> SelectedIdsChanged { get; set; }
        [Parameter] public EventCallback<XFTreeNodeEventArgs> NodeSelected { get; set; }
        [Parameter] public EventCallback<XFTreeNodeEventArgs> NodeExpanded { get; set; }
        [Parameter] public EventCallback<XFTreeNodeEventArgs> NodeCollapsed { get; set; }
        [Parameter] public RenderFragment<XFTreeNode>? NodeTemplate { get; set; }
        [Parameter] public string? FilterText { get; set; }
        [Parameter] public Func<XFTreeNode, string, bool>? FilterPredicate { get; set; }
        [Parameter] public bool ExpandFilteredMatches { get; set; } = true;
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public string EmptyText { get; set; } = default!;
        [Parameter] public XFTreeViewSelectionMode SelectionMode { get; set; } = XFTreeViewSelectionMode.Single;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFContainerVariant Variant { get; set; } = XFContainerVariant.Surface;
        [Parameter] public bool ExpandOnNodeClick { get; set; } = true;
        [Parameter] public bool SelectOnNodeClick { get; set; } = true;
        [Parameter] public string ExpandItemLabel { get; set; } = default!;
        [Parameter] public string CollapseItemLabel { get; set; } = default!;

        // Checkbox parameters
        [Parameter] public bool AllowCheckBoxes { get; set; }
        [Parameter] public IReadOnlyList<string> CheckedIds { get; set; } = Array.Empty<string>();
        [Parameter] public EventCallback<IReadOnlyList<string>> CheckedIdsChanged { get; set; }
        [Parameter] public EventCallback<XFTreeNodeEventArgs> NodeChecked { get; set; }
        [Parameter] public EventCallback<XFTreeNodeEventArgs> NodeUnchecked { get; set; }
        [Parameter] public bool AllowCheckChildren { get; set; } = true;
        [Parameter] public bool AllowCheckParents { get; set; } = true;
        [Parameter] public string CheckLabel { get; set; } = default!;
        [Parameter] public string UncheckLabel { get; set; } = default!;
        [Parameter] public string IndeterminateLabel { get; set; } = default!;

        // Drag/drop parameters
        [Parameter] public bool AllowDrag { get; set; }
        [Parameter] public bool AllowDrop { get; set; }
        [Parameter] public EventCallback<XFTreeDragEventArgs> OnDragStart { get; set; }
        [Parameter] public EventCallback<XFTreeDropEventArgs> OnDrop { get; set; }
        [Parameter] public EventCallback<XFTreeLoadChildrenEventArgs> OnLoadChildren { get; set; }


        [Parameter] public string DragLabel { get; set; } = default!;
        [Parameter] public string DropBeforeLabel { get; set; } = default!;
        [Parameter] public string DropInsideLabel { get; set; } = default!;
        [Parameter] public string DropAfterLabel { get; set; } = default!;

        private IReadOnlyList<XFTreeNode> ItemsList => Items ?? Array.Empty<XFTreeNode>();

        private bool HasFilter => !string.IsNullOrWhiteSpace(FilterText);

        private IReadOnlyList<XFTreeNode> DisplayItems => HasFilter
            ? FilterNodes(ItemsList, FilterText!.Trim()).ToArray()
            : ItemsList;

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(EmptyText)) EmptyText = L("treeview.empty", EmptyText);
            if (string.IsNullOrWhiteSpace(ExpandItemLabel)) ExpandItemLabel = L("treeview.expand", ExpandItemLabel);
            if (string.IsNullOrWhiteSpace(CollapseItemLabel)) CollapseItemLabel = L("treeview.collapse", CollapseItemLabel);
            if (string.IsNullOrWhiteSpace(CheckLabel)) CheckLabel = L("treeview.check", CheckLabel);
            if (string.IsNullOrWhiteSpace(UncheckLabel)) UncheckLabel = L("treeview.uncheck", UncheckLabel);
            if (string.IsNullOrWhiteSpace(IndeterminateLabel)) IndeterminateLabel = L("treeview.indeterminate", IndeterminateLabel);
            if (string.IsNullOrWhiteSpace(DragLabel)) DragLabel = L("treeview.drag", DragLabel);
            if (string.IsNullOrWhiteSpace(DropBeforeLabel)) DropBeforeLabel = L("treeview.drop_before", DropBeforeLabel);
            if (string.IsNullOrWhiteSpace(DropInsideLabel)) DropInsideLabel = L("treeview.drop_inside", DropInsideLabel);
            if (string.IsNullOrWhiteSpace(DropAfterLabel)) DropAfterLabel = L("treeview.drop_after", DropAfterLabel);
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            expandedIds.Clear();
            foreach (var id in ExpandedIds.Where(id => !string.IsNullOrWhiteSpace(id)))
            {
                expandedIds.Add(id);
            }

            selectedIds.Clear();
            foreach (var id in SelectedIds.Where(id => !string.IsNullOrWhiteSpace(id)))
            {
                selectedIds.Add(id);
            }

            checkedIds.Clear();
            foreach (var id in CheckedIds.Where(id => !string.IsNullOrWhiteSpace(id)))
            {
                checkedIds.Add(id);
            }
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {VariantClass}");
        }

        private RenderFragment RenderNode(XFTreeNode node, int level) => builder =>
        {
            var hasChildren = HasChildren(node);
            var isExpanded = IsExpanded(node);
            var isSelected = IsSelected(node);
            var checkState = GetCheckState(node);
            var isDragging = _dragSource?.Id == node.Id;
            var isDropTarget = _dropTargetId == node.Id;
            var seq = 0;

            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", Join(
                "select-none",
                isDragging ? "opacity-50" : null));

            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "id", GetNodeDomId(node));
            builder.AddAttribute(seq++, "role", "treeitem");
            builder.AddAttribute(seq++, "aria-level", level.ToString());
            builder.AddAttribute(seq++, "aria-selected", isSelected.ToString().ToLowerInvariant());
            builder.AddAttribute(seq++, "aria-disabled", node.Disabled.ToString().ToLowerInvariant());
            builder.AddAttribute(seq++, "aria-busy", node.Loading.ToString().ToLowerInvariant());
            if (hasChildren)
            {
                builder.AddAttribute(seq++, "aria-expanded", isExpanded.ToString().ToLowerInvariant());
            }
            if (AllowCheckBoxes)
            {
                builder.AddAttribute(seq++, "aria-checked", checkState switch
                {
                    true => "true",
                    false => "false",
                    _ => "mixed"
                });
            }
            if (AllowDrag && node.Draggable && !node.Disabled)
            {
                builder.AddAttribute(seq++, "draggable", "true");
                builder.AddAttribute(seq++, "aria-roledescription", DragLabel);
            }
            builder.AddAttribute(seq++, "class", Join(
                ItemClass(node, isSelected),
                isDropTarget ? DropTargetClass(_dropPosition) : null));
            builder.AddAttribute(seq++, "tabindex", node.Disabled ? "-1" : "0");
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => HandleNodeClickAsync(node)));

            // Drag events
            if (AllowDrag && node.Draggable && !node.Disabled)
            {
                builder.AddAttribute(seq++, "ondragstart", EventCallback.Factory.Create<DragEventArgs>(this, (e) => HandleDragStartAsync(e, node)));
                builder.AddAttribute(seq++, "ondragend", EventCallback.Factory.Create<DragEventArgs>(this, (e) => HandleDragEndAsync(e)));
            }

            // Drop events
            if (AllowDrop && node.Droppable && !node.Disabled)
            {
                builder.AddAttribute(seq++, "ondragover", EventCallback.Factory.Create<DragEventArgs>(this, (e) => HandleDragOverAsync(e, node)));
                builder.AddEventPreventDefaultAttribute(seq++, "ondragover", true);
                builder.AddAttribute(seq++, "ondragleave", EventCallback.Factory.Create<DragEventArgs>(this, (e) => HandleDragLeaveAsync(e)));
                builder.AddAttribute(seq++, "ondrop", EventCallback.Factory.Create<DragEventArgs>(this, (e) => HandleDropAsync(e, node)));
                builder.AddEventPreventDefaultAttribute(seq++, "ondrop", true);
            }

            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "class", IndentClass(level));
            builder.CloseElement();

            builder.OpenElement(seq++, "button");
            builder.AddAttribute(seq++, "type", "button");
            builder.AddAttribute(seq++, "class", ToggleClass(hasChildren, isExpanded));
            builder.AddAttribute(seq++, "disabled", !hasChildren || node.Disabled);
            builder.AddAttribute(seq++, "aria-label", isExpanded ? CollapseItemLabel : ExpandItemLabel);
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => ToggleNodeAsync(node)));
            builder.AddEventStopPropagationAttribute(seq++, "onclick", true);
            if (node.Loading)
            {
                builder.AddContent(seq++, "...");
            }
            else
            {
                builder.AddMarkupContent(seq++, IconMarkup("chevron-right", 12));
            }
            builder.CloseElement();

            // Checkbox
            if (AllowCheckBoxes)
            {
                builder.OpenElement(seq++, "label");
                builder.AddAttribute(seq++, "class", "inline-flex items-center");
                builder.AddEventStopPropagationAttribute(seq++, "onclick", true);

                builder.OpenElement(seq++, "span");
                builder.AddAttribute(seq++, "class", CheckboxWrapperClass(node.Disabled));
                builder.AddAttribute(seq++, "role", "checkbox");
                builder.AddAttribute(seq++, "aria-checked", checkState switch
                {
                    true => "true",
                    false => "false",
                    _ => "mixed"
                });
                builder.AddAttribute(seq++, "aria-label", checkState switch
                {
                    true => UncheckLabel,
                    false => CheckLabel,
                    _ => IndeterminateLabel
                });
                builder.AddAttribute(seq++, "tabindex", "-1");
                if (!node.Disabled && node.Checkable)
                {
                    builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => ToggleCheckAsync(node)));
                }
                builder.OpenElement(seq++, "svg");
                builder.AddAttribute(seq++, "class", CheckboxIconClass(checkState));
                builder.AddAttribute(seq++, "viewBox", "0 0 14 14");
                builder.AddAttribute(seq++, "fill", "none");
                builder.AddAttribute(seq++, "xmlns", "http://www.w3.org/2000/svg");
                builder.AddAttribute(seq++, "aria-hidden", "true");
                if (checkState == true)
                {
                    builder.AddMarkupContent(seq++, "<path d=\"M11.5 4L5.5 10L2.5 7\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/>");
                }
                else if (checkState == null)
                {
                    builder.AddMarkupContent(seq++, "<path d=\"M3 7H11\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\"/>");
                }
                builder.CloseElement();

                builder.CloseElement();
                builder.CloseElement();
            }

            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "class", "min-w-0 flex-1");
            if (NodeTemplate is not null)
            {
                builder.AddContent(seq++, NodeTemplate(node));
            }
            else
            {
                builder.OpenElement(seq++, "span");
                builder.AddAttribute(seq++, "class", "block truncate font-medium");
                builder.AddContent(seq++, node.Text);
                builder.CloseElement();
                if (!string.IsNullOrWhiteSpace(node.Description))
                {
                    builder.OpenElement(seq++, "span");
                    builder.AddAttribute(seq++, "class", "block truncate text-xs text-text-muted");
                    builder.AddContent(seq++, node.Description);
                    builder.CloseElement();
                }
            }
            builder.CloseElement();

            builder.CloseElement();

            // Drop indicator (before)
            if (isDropTarget && _dropPosition == XFTreeDropPosition.Before)
            {
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "class", "h-0.5 rounded bg-primary transition-all duration-150");
                builder.CloseElement();
            }

            if (hasChildren && isExpanded)
            {
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "role", "group");
                builder.AddAttribute(seq++, "class", "grid gap-1");
                foreach (var child in node.Children)
                {
                    builder.AddContent(seq++, RenderNode(child, level + 1));
                }
                builder.CloseElement();
            }

            // Drop indicator (after)
            if (isDropTarget && _dropPosition == XFTreeDropPosition.After)
            {
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "class", "h-0.5 rounded bg-primary transition-all duration-150");
                builder.CloseElement();
            }

            builder.CloseElement();
        };

        private async Task HandleNodeClickAsync(XFTreeNode node)
        {
            if (node.Disabled)
            {
                return;
            }

            if (SelectOnNodeClick)
            {
                await SelectNodeAsync(node);
            }

            if (ExpandOnNodeClick && HasChildren(node))
            {
                await ToggleNodeAsync(node);
            }
        }

        private async Task ToggleNodeAsync(XFTreeNode node)
        {
            if (node.Disabled || !HasChildren(node))
            {
                return;
            }

            if (expandedIds.Contains(node.Id))
            {
                expandedIds.Remove(node.Id);
                await ExpandedIdsChanged.InvokeAsync(expandedIds.ToArray());
                await NodeCollapsed.InvokeAsync(new XFTreeNodeEventArgs(node));
                return;
            }

            if (OnLoadChildren.HasDelegate && node.Children.Count == 0)
            {
                node.Loading = true;
                var args = new XFTreeLoadChildrenEventArgs(node);
                await OnLoadChildren.InvokeAsync(args);
                if (args.Children is not null)
                {
                    node.Children = args.Children;
                }
                node.Loading = false;
            }

            expandedIds.Add(node.Id);
            await ExpandedIdsChanged.InvokeAsync(expandedIds.ToArray());
            await NodeExpanded.InvokeAsync(new XFTreeNodeEventArgs(node));
        }

        private async Task SelectNodeAsync(XFTreeNode node)
        {
            if (SelectionMode == XFTreeViewSelectionMode.None || node.Disabled)
            {
                return;
            }

            if (SelectionMode == XFTreeViewSelectionMode.Single)
            {
                selectedIds.Clear();
                selectedIds.Add(node.Id);
            }
            else if (!selectedIds.Add(node.Id))
            {
                selectedIds.Remove(node.Id);
            }

            await SelectedIdsChanged.InvokeAsync(selectedIds.ToArray());
            await NodeSelected.InvokeAsync(new XFTreeNodeEventArgs(node));
        }

        // Checkbox logic
        private bool? GetCheckState(XFTreeNode node)
        {
            if (checkedIds.Contains(node.Id))
            {
                return true;
            }

            if (AllowCheckParents && HasChildren(node))
            {
                var allDescendants = GetAllDescendants(node).Where(n => n.Checkable && !n.Disabled).ToArray();
                if (allDescendants.Length == 0)
                {
                    return false;
                }

                var checkedCount = allDescendants.Count(n => checkedIds.Contains(n.Id));
                if (checkedCount == allDescendants.Length)
                {
                    return true;
                }
                if (checkedCount > 0)
                {
                    return null; // indeterminate
                }
            }

            return false;
        }

        private IEnumerable<XFTreeNode> GetAllDescendants(XFTreeNode node)
        {
            foreach (var child in node.Children)
            {
                yield return child;
                foreach (var descendant in GetAllDescendants(child))
                {
                    yield return descendant;
                }
            }
        }

        private async Task ToggleCheckAsync(XFTreeNode node)
        {
            if (node.Disabled || !node.Checkable)
            {
                return;
            }

            var wasChecked = checkedIds.Contains(node.Id);

            if (wasChecked)
            {
                checkedIds.Remove(node.Id);
                // Uncheck all descendants
                if (AllowCheckChildren)
                {
                    foreach (var descendant in GetAllDescendants(node))
                    {
                        checkedIds.Remove(descendant.Id);
                    }
                }
            }
            else
            {
                checkedIds.Add(node.Id);
                // Check all descendants
                if (AllowCheckChildren)
                {
                    foreach (var descendant in GetAllDescendants(node).Where(n => n.Checkable && !n.Disabled))
                    {
                        checkedIds.Add(descendant.Id);
                    }
                }
            }

            // Update parent chain
            if (AllowCheckParents)
            {
                UpdateParentCheckStates(node);
            }

            await CheckedIdsChanged.InvokeAsync(checkedIds.ToArray());
            if (wasChecked)
            {
                await NodeUnchecked.InvokeAsync(new XFTreeNodeEventArgs(node));
            }
            else
            {
                await NodeChecked.InvokeAsync(new XFTreeNodeEventArgs(node));
            }
        }

        private void UpdateParentCheckStates(XFTreeNode node)
        {
            var parent = FindParent(ItemsList, node);
            if (parent is null || !parent.Checkable || parent.Disabled)
            {
                return;
            }

            var checkableChildren = parent.Children.Where(c => c.Checkable && !c.Disabled).ToArray();
            if (checkableChildren.Length == 0)
            {
                return;
            }

            var allChecked = checkableChildren.All(c => checkedIds.Contains(c.Id));
            var anyChecked = checkableChildren.Any(c => checkedIds.Contains(c.Id));

            if (allChecked)
            {
                checkedIds.Add(parent.Id);
            }
            else
            {
                checkedIds.Remove(parent.Id);
            }

            UpdateParentCheckStates(parent);
        }

        private XFTreeNode? FindParent(IReadOnlyList<XFTreeNode> nodes, XFTreeNode target)
        {
            foreach (var node in nodes)
            {
                if (node.Children.Any(c => c.Id == target.Id))
                {
                    return node;
                }
                var found = FindParent(node.Children, target);
                if (found is not null)
                {
                    return found;
                }
            }
            return null;
        }

        // Drag/drop logic
        private async Task HandleDragStartAsync(DragEventArgs e, XFTreeNode node)
        {
            if (!AllowDrag || node.Disabled || !node.Draggable)
            {
                return;
            }

            _dragSource = node;
            e.DataTransfer!.EffectAllowed = "move";

            var args = new XFTreeDragEventArgs(node);
            await OnDragStart.InvokeAsync(args);
            if (args.Cancel)
            {
                _dragSource = null;
            }
        }

        private Task HandleDragEndAsync(DragEventArgs e)
        {
            _dragSource = null;
            _dropTargetId = null;
            _dropPosition = XFTreeDropPosition.None;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private Task HandleDragOverAsync(DragEventArgs e, XFTreeNode node)
        {
            if (!AllowDrop || _dragSource is null || node.Disabled || !node.Droppable || _dragSource.Id == node.Id)
            {
                return Task.CompletedTask;
            }

            // Note: preventDefault is handled via AddEventPreventDefaultAttribute in RenderNode.
            // DataTransfer effect is set by the browser based on effectAllowed from dragstart.
            _dropTargetId = node.Id;

            // Determine drop position based on cursor Y within the element
            // The actual clientY-based positioning would need JS interop;
            // default to "Inside" for simplicity
            _dropPosition = XFTreeDropPosition.Inside;

            StateHasChanged();
            return Task.CompletedTask;
        }

        private Task HandleDragLeaveAsync(DragEventArgs e)
        {
            _dropTargetId = null;
            _dropPosition = XFTreeDropPosition.None;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task HandleDropAsync(DragEventArgs e, XFTreeNode target)
        {
            if (!AllowDrop || _dragSource is null || target.Disabled || !target.Droppable || _dragSource.Id == target.Id)
            {
                return;
            }

            // Note: preventDefault is handled via AddEventPreventDefaultAttribute in RenderNode.
            var args = new XFTreeDropEventArgs(_dragSource, target, _dropPosition);
            await OnDrop.InvokeAsync(args);

            if (!args.Cancel)
            {
                // Actual tree mutation is handled by the consumer via the Drop event
            }

            _dragSource = null;
            _dropTargetId = null;
            _dropPosition = XFTreeDropPosition.None;
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            var flat = FlattenVisible(DisplayItems).Where(node => !node.Disabled).ToArray();
            if (flat.Length == 0)
            {
                return;
            }

            var current = flat.FirstOrDefault(node => selectedIds.Contains(node.Id)) ?? flat[0];
            var index = Array.IndexOf(flat, current);
            if (args.Key == "ArrowDown")
            {
                await SelectNodeAsync(flat[Math.Min(flat.Length - 1, index + 1)]);
            }
            else if (args.Key == "ArrowUp")
            {
                await SelectNodeAsync(flat[Math.Max(0, index - 1)]);
            }
            else if (args.Key == "ArrowRight" && HasChildren(current) && !IsExpanded(current))
            {
                await ToggleNodeAsync(current);
            }
            else if (args.Key == "ArrowLeft" && HasChildren(current) && IsExpanded(current))
            {
                await ToggleNodeAsync(current);
            }
            else if (args.Key == "Home")
            {
                await SelectNodeAsync(flat[0]);
            }
            else if (args.Key == "End")
            {
                await SelectNodeAsync(flat[^1]);
            }
            else if (args.Key == " " && AllowCheckBoxes && current.Checkable)
            {
                await ToggleCheckAsync(current);
            }
        }

        private IEnumerable<XFTreeNode> FlattenVisible(IEnumerable<XFTreeNode> nodes)
        {
            foreach (var node in nodes)
            {
                yield return node;
                if (IsExpanded(node))
                {
                    foreach (var child in FlattenVisible(node.Children))
                    {
                        yield return child;
                    }
                }
            }
        }

        private bool HasChildren(XFTreeNode node) => node.HasChildren || node.Children.Count > 0;

        private bool IsExpanded(XFTreeNode node) => expandedIds.Contains(node.Id) || (HasFilter && ExpandFilteredMatches && node.Children.Count > 0);

        private bool IsSelected(XFTreeNode node) => selectedIds.Contains(node.Id);

        private string GetNodeDomId(XFTreeNode node) => $"{Id}-{node.Id}";

        private IEnumerable<XFTreeNode> FilterNodes(IEnumerable<XFTreeNode> nodes, string filter)
        {
            foreach (var node in nodes)
            {
                var children = FilterNodes(node.Children, filter).ToArray();
                if (NodeMatchesFilter(node, filter) || children.Length > 0)
                {
                    yield return new XFTreeNode
                    {
                        Id = node.Id,
                        Text = node.Text,
                        Description = node.Description,
                        Disabled = node.Disabled,
                        HasChildren = node.HasChildren,
                        Loading = node.Loading,
                        Checkable = node.Checkable,
                        Draggable = node.Draggable,
                        Droppable = node.Droppable,
                        Children = children
                    };
                }
            }
        }

        private bool NodeMatchesFilter(XFTreeNode node, string filter)
        {
            if (FilterPredicate is not null)
            {
                return FilterPredicate(node, filter);
            }

            return node.Text.Contains(filter, StringComparison.CurrentCultureIgnoreCase)
                || node.Description?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true;
        }

        private string IsMultiSelectString => (SelectionMode == XFTreeViewSelectionMode.Multiple).ToString().ToLowerInvariant();

        private string VariantClass => Variant switch
        {
            XFContainerVariant.Plain => "grid gap-1 text-text-primary",
            XFContainerVariant.Soft => "grid gap-1 rounded-lg bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)] p-2 text-text-primary",
            _ => "grid gap-1 rounded-lg border border-border-light bg-bg-surface p-2 text-text-primary shadow-[0_1px_2px_rgb(15_23_42/.04)]"
        };

        private string ItemClass(XFTreeNode node, bool selected)
        {
            return Join(
                "group flex min-w-0 items-center gap-1 rounded-md outline-none transition-all duration-150 focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]",
                SizeClass,
                selected ? "bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]" : "hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]",
                node.Disabled ? "cursor-not-allowed text-text-muted opacity-60" : "cursor-pointer");
        }

        private string SizeClass => Size switch
        {
            XFSize.Sm => "min-h-8 px-2 py-1 text-xs",
            XFSize.Lg => "min-h-11 px-3 py-2 text-base",
            _ => "min-h-9 px-2.5 py-1.5 text-sm"
        };

        private static string IndentClass(int level) => level switch
        {
            <= 1 => "w-0 shrink-0",
            2 => "w-4 shrink-0",
            3 => "w-8 shrink-0",
            4 => "w-12 shrink-0",
            _ => "w-16 shrink-0"
        };

        private static string ToggleClass(bool visible, bool expanded)
        {
            return Join(
                "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md text-text-muted transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] hover:text-primary disabled:pointer-events-none",
                visible ? "opacity-100" : "opacity-0",
                expanded ? "rotate-90" : null);
        }

        private string EmptyClass => "rounded-md border border-dashed border-border-light bg-surface-50 px-3 py-6 text-center text-sm text-text-muted";

        private static string CheckboxWrapperClass(bool disabled)
        {
            return Join(
                "inline-flex h-4 w-4 shrink-0 items-center justify-center rounded border transition-all duration-150",
                disabled
                    ? "cursor-not-allowed border-border-light bg-surface-100 text-text-muted"
                    : "cursor-pointer border-border-light bg-bg-surface hover:border-primary");
        }

        private static string CheckboxIconClass(bool? checkState) => checkState switch
        {
            true => "h-3.5 w-3.5 text-primary",
            null => "h-3.5 w-3.5 text-primary",
            _ => "h-3.5 w-3.5 text-transparent"
        };

        private static string DropTargetClass(XFTreeDropPosition position) => position switch
        {
            XFTreeDropPosition.Before => "ring-2 ring-primary ring-inset",
            XFTreeDropPosition.Inside => "bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] ring-2 ring-primary",
            XFTreeDropPosition.After => "ring-2 ring-primary ring-inset",
            _ => ""
        };

        private static string IconMarkup(string name, int size)
        {
            return XFSvgIcons.GetWithSize(name, size);
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
