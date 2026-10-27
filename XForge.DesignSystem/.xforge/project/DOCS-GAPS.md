# Documentation Gaps Report
**Source**: DocsQualityTests execution
**Date**: 2026-06-13
**Test Result**: 5 passed / 7 failed / 12 total

## Gaps Found

### Every_Parameter_Should_Be_Documented (FAIL)
Parameters not documented across many components.
Affected: Accordion, Alert, Anchor, AppBar, Audio, AutoComplete, Avatar, Badge, Banner, Breadcrumb, Button, ButtonGroup, Calendar, Card, Carousel, Checkbox, Chip, ColorPicker, DateInput, DatePicker, Dialog, Drawer, Dropdown, FileUpload, Gantt, Grid, Icon, Image, InputMask, List, ListItem, MarkdownEditor, Menu, Modal, NavBar, NumericInput, Pagination, Popover, Progress, ProgressBar, Radio, Rating, RichTextEditor, Scrollspy, Select, Sheet, Sidebar, Skeleton, Slider, Sparkline, SpeechToText, SplashScreen, SplitButton, Splitter, Stack, Steps, Switch, Table, Tabs, Textarea, TextInput, TimeInput, Timeline, TimeSpan, Toast, TOC, Toolbar, Tooltip, TransferList, TreeView, Typography, Upload, Video

Common missing: Id, Class, Style, Disabled, ValueExpression

### Every_EventCallback_Should_Be_Documented (FAIL)
- CalendarDocs.razor: SelectionChange
- DataGridDocs.razor: EditingItemChanged, ColumnFilterChanged
- GanttDocs.razor: OnTaskResize, ZoomLevelChanged
- SliderDocs.razor: ValueChanged

### Every_RenderFragment_Should_Be_Documented (FAIL)
- AutocompleteDocs.razor: EndIcon, HeaderContent, FooterContent
- InputMaskDocs.razor: StartIcon, EndIcon

## Recommended Actions
1. Add Id, Class, Style, Disabled sections to all missing components (prioritize common base params)
2. Document 4 missing EventCallbacks
3. Document 5 missing RenderFragments
4. Re-run tests after fixes
