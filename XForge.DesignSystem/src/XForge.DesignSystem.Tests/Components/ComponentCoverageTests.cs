// T200: Sprint 5 - Component coverage smoke tests.
// Verifies that each cataloged XForge component is reachable at runtime.
// Generated to address the test coverage gap (159 untested XF components).
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class ComponentCoverageTests
{
    private static readonly Dictionary<string, Type> TypesByName = BuildTypeIndex();

    private static Dictionary<string, Type> BuildTypeIndex()
    {
        var result = new Dictionary<string, Type>(StringComparer.Ordinal);
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch { continue; }
            foreach (var t in types)
            {
                if (!t.Name.StartsWith("XF", StringComparison.Ordinal)) continue;
                if (!result.ContainsKey(t.Name)) result[t.Name] = t;
                // Also index generic types by their name without backtick (e.g. XFDataGridColumn`1 -> XFDataGridColumn)
                var tickIdx = t.Name.IndexOf("`", StringComparison.Ordinal);
                if (tickIdx > 0)
                {
                    var cleanName = t.Name.Substring(0, tickIdx);
                    if (!result.ContainsKey(cleanName)) result[cleanName] = t;
                }
            }
        }
        return result;
    }

    [Theory]
    [InlineData("XFAccordionItem")]
    [InlineData("XFAiAccountingAssistant")]
    [InlineData("XFAiAgentConsole")]
    [InlineData("XFAiAgentOrchestrator")]
    [InlineData("XFAiAgentPersonality")]
    [InlineData("XFAiAuditTimeline")]
    [InlineData("XFAiAutomationRule")]
    [InlineData("XFAiAvatar")]
    [InlineData("XFAiBatchProcessor")]
    [InlineData("XFAiBundleAnalyzer")]
    [InlineData("XFAiChat")]
    [InlineData("XFAiChatWithEmoji")]
    [InlineData("XFAiCommandPalette")]
    [InlineData("XFAiConversationContext")]
    [InlineData("XFAiCostEstimator")]
    [InlineData("XFAiDashboardInsight")]
    [InlineData("XFAiDataGridAssistant")]
    [InlineData("XFAiDataQualityScanner")]
    [InlineData("XFAiDocumentReader")]
    [InlineData("XFAiExplainButton")]
    [InlineData("XFAiFinancialConciliator")]
    [InlineData("XFAiFormAssistant")]
    [InlineData("XFAiHumanApprovalCard")]
    [InlineData("XFAiKnowledgeBase")]
    [InlineData("XFAiLatencyMonitor")]
    [InlineData("XFAiLazyComponent")]
    [InlineData("XFAiMemoryProfiler")]
    [InlineData("XFAiMemoryViewer")]
    [InlineData("XFAiModelComparator")]
    [InlineData("XFAiMultiModalChat")]
    [InlineData("XFAiNFeAnalyzer")]
    [InlineData("XFAiOnboardingWizard")]
    [InlineData("XFAiPerformanceMonitor")]
    [InlineData("XFAiProactiveInsight")]
    [InlineData("XFAiPromptBox")]
    [InlineData("XFAiPromptLibrary")]
    [InlineData("XFAiProviderConfig")]
    [InlineData("XFAiReportBuilder")]
    [InlineData("XFAiRewriteButton")]
    [InlineData("XFAiRiskBadge")]
    [InlineData("XFAiSalesGoalAdvisor")]
    [InlineData("XFAiScheduler")]
    [InlineData("XFAiSentimentGauge")]
    [InlineData("XFAiSourceCitationList")]
    [InlineData("XFAiSqlQueryBuilder")]
    [InlineData("XFAiStockAdvisor")]
    [InlineData("XFAiSuggestionPanel")]
    [InlineData("XFAiValidationSummary")]
    [InlineData("XFAiVideoAnalyzer")]
    [InlineData("XFAiBatchProcessor")]
    [InlineData("XFAiVisionPanel")]
    [InlineData("XFAiVoiceAssistant")]
    [InlineData("XFAiVoiceBiometric")]
    [InlineData("XFAiWorkflowDesigner")]
    [InlineData("XFAiWorkflowExecutor")]
    [InlineData("XFAnonymizationHelper")]
    [InlineData("XFAuditLogger")]
    [InlineData("XFAuthProvider")]
    [InlineData("XFAvatarGroup")]
    [InlineData("XFBadgeIcon")]
    [InlineData("XFBIDashboard")]
    [InlineData("XFBlockEditor")]
    [InlineData("XFBoletoGenerator")]
    [InlineData("XFBrazilianCitiesSelect")]
    [InlineData("XFBrazilianStatesSelect")]
    [InlineData("XFBreadcrumbItem")]
    [InlineData("XFCandlestickChart")]
    [InlineData("XFCarouselSlide")]
    [InlineData("XFCepInput")]
    [InlineData("XFCfopSelect")]
    [InlineData("XFChipList")]
    [InlineData("XFCnaeSelect")]
    [InlineData("XFCnpjCpfInput")]
    [InlineData("XFCnpjInput")]
    [InlineData("XFCompareValidator")]
    [InlineData("XFCompliancePanel")]
    [InlineData("XFComponentGenerator")]
    [InlineData("XFComponentGrid")]
    [InlineData("XFConsentBanner")]
    [InlineData("XFCookieConsent")]
    [InlineData("XFCpfInput")]
    [InlineData("XFCustomValidator")]
    [InlineData("XFDataDeleteRequest")]
    [InlineData("XFDataExportRequest")]
    [InlineData("XFDataGridAdvanced")]
    [InlineData("XFDataGridColumn")]
    [InlineData("XFDesignTokenExplorer")]
    [InlineData("XFDeviceBridge")]
    [InlineData("XFDiagramAdvanced")]
    [InlineData("XFDialogProvider")]
    [InlineData("XFDocumentEditor")]
    [InlineData("XFDragDropZone")]
    [InlineData("XFEmailValidator")]
    [InlineData("XFFieldset")]
    [InlineData("XFFigmaSync")]
    [InlineData("XFFigure")]
    [InlineData("XFFluentEmoji")]
    [InlineData("XFFluentIcon")]
    [InlineData("XFFunnelChart")]
    [InlineData("XFGanttAdvanced")]
    [InlineData("XFGestureHandler")]
    [InlineData("XFHeatmapChart")]
    [InlineData("XFHighlighter")]
    [InlineData("XFIconColorPicker")]
    [InlineData("XFIconPicker")]
    [InlineData("XFImageEditor")]
    [InlineData("XFLengthValidator")]
    [InlineData("XFLinearGauge")]
    [InlineData("XFListItem")]
    [InlineData("XFMapCircle")]
    [InlineData("XFMapMarker")]
    [InlineData("XFMapPolygon")]
    [InlineData("XFMapPolyline")]
    [InlineData("XFMapsAdvanced")]
    [InlineData("XFMapTileLayer")]
    [InlineData("XFMessageList")]
    [InlineData("XFMicroFrontend")]
    [InlineData("XFMultiSplitter")]
    [InlineData("XFMultiSplitterPane")]
    [InlineData("XFNcmSelect")]
    [InlineData("XFNFeViewer")]
    [InlineData("XFOfflineStorage")]
    [InlineData("XFOrgChart")]
    [InlineData("XFPageProgressProvider")]
    [InlineData("XFPdfViewerAdvanced")]
    [InlineData("XFPivotFieldList")]
    [InlineData("XFPivotGrid")]
    [InlineData("XFPixKeyInput")]
    [InlineData("XFPrivacyPolicyModal")]
    [InlineData("XFRadarChart")]
    [InlineData("XFRangeValidator")]
    [InlineData("XFRbacComponent")]
    [InlineData("XFRegexValidator")]
    [InlineData("XFRequiredValidator")]
    [InlineData("XFResponsiveContainer")]
    [InlineData("XFSchedulerAdvanced")]
    [InlineData("XFSelectGroup")]
    [InlineData("XFSelectOption")]
    [InlineData("XFSkeletonGroup")]
    [InlineData("XFSkeletonTable")]
    [InlineData("XFSkintonePicker")]
    [InlineData("XFSpedViewer")]
    [InlineData("XFSpreadsheet")]
    [InlineData("XFStorybook")]
    [InlineData("XFTableBody")]
    [InlineData("XFTableCell")]
    [InlineData("XFTableHeader")]
    [InlineData("XFTableRow")]
    [InlineData("XFTabPanel")]
    [InlineData("XFThemeEditor")]
    [InlineData("XFTimelineItem")]
    [InlineData("XFToastProvider")]
    [InlineData("XFTreeGrid")]
    [InlineData("XFValidationMessage")]
    [InlineData("XFValidationStatus")]
    [InlineData("XFValidationSummary")]
    [InlineData("XFVehiclePlateInput")]
    [InlineData("XFWizardStep")]
    [InlineData("XFWordCloudChart")]

    public void Component_IsReachable(string componentName)
    {
        Assert.True(TypesByName.ContainsKey(componentName));
    }
}