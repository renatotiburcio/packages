using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Enterprise;

// ─── Enums ─────────────────────────────────────────────────────────────────────

/// <summary>Authentication state.</summary>
public enum XFAuthState
{
    Anonymous = 0,
    Authenticating = 1,
    Authenticated = 2,
    Expired = 3,
    Locked = 4,
    Error = 5
}

/// <summary>Authentication provider type.</summary>
public enum XFAuthProviderType
{
    OIDC = 0,
    OAuth2 = 1,
    SAML = 2,
    LDAP = 3,
    APIKey = 4,
    Custom = 5
}

/// <summary>RBAC decision result.</summary>
public enum XFRbacDecision
{
    Allow = 0,
    Deny = 1,
    Inherit = 2
}

/// <summary>Audit log severity level.</summary>
public enum XFAuditSeverity
{
    Info = 0,
    Warning = 1,
    Error = 2,
    Critical = 3
}

/// <summary>Audit log action type.</summary>
public enum XFAuditAction
{
    Create = 0,
    Read = 1,
    Update = 2,
    Delete = 3,
    Login = 4,
    Logout = 5,
    Export = 6,
    Import = 7,
    Approve = 8,
    Reject = 9,
    Custom = 10
}

/// <summary>Compliance regulation framework.</summary>
public enum XFComplianceRegulation
{
    LGPD = 0,
    GDPR = 1,
    CCPA = 2,
    HIPAA = 3,
    SOX = 4,
    PCI = 5,
    Custom = 6
}

/// <summary>User consent status.</summary>
public enum XFConsentStatus
{
    Pending = 0,
    Granted = 1,
    Revoked = 2,
    Expired = 3
}

/// <summary>Data deletion request status.</summary>
public enum XFDeletionRequestStatus
{
    Pending = 0,
    Approved = 1,
    Processing = 2,
    Completed = 3,
    Rejected = 4
}

/// <summary>Micro-frontend module load state.</summary>
public enum XFMicroFrontendState
{
    Idle = 0,
    Loading = 1,
    Loaded = 2,
    Error = 3,
    Timeout = 4
}

/// <summary>Data category for compliance.</summary>
public enum XFDataCategory
{
    Personal = 0,
    Financial = 1,
    Health = 2,
    Behavioral = 3,
    Biometric = 4,
    Location = 5,
    Custom = 6
}

/// <summary>RBAC logical operator for combining rules.</summary>
public enum XFRbacOperator
{
    And = 0,
    Or = 1
}

/// <summary>Consent legal basis (LGPD Art. 7 / GDPR Art. 6).</summary>
public enum XFLegalBasis
{
    Consent = 0,
    Contract = 1,
    LegalObligation = 2,
    VitalInterest = 3,
    PublicTask = 4,
    LegitimateInterest = 5
}

// ─── Records ───────────────────────────────────────────────────────────────────

/// <summary>Authenticated user information.</summary>
public sealed record XFAuthUser
{
    /// <summary>User unique identifier.</summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>Display name.</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>Email address.</summary>
    public string? Email { get; init; }

    /// <summary>Avatar URL.</summary>
    public string? AvatarUrl { get; init; }

    /// <summary>Assigned roles.</summary>
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();

    /// <summary>Assigned permissions.</summary>
    public IReadOnlyList<string> Permissions { get; init; } = Array.Empty<string>();

    /// <summary>Additional metadata.</summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>Authentication session information.</summary>
public sealed record XFAuthSession
{
    /// <summary>Session unique identifier.</summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>Associated user ID.</summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>Authentication provider used.</summary>
    public XFAuthProviderType Provider { get; init; }

    /// <summary>Session start time.</summary>
    public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Session expiration time.</summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>Whether session is currently active.</summary>
    public bool IsActive { get; init; } = true;

    /// <summary>Client IP address.</summary>
    public string? IpAddress { get; init; }

    /// <summary>Client user agent.</summary>
    public string? UserAgent { get; init; }
}

/// <summary>RBAC access rule definition.</summary>
public sealed record XFRbacRule
{
    /// <summary>Rule unique identifier.</summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>Human-readable rule name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Required roles (any match grants access when Operator=Or).</summary>
    public IReadOnlyList<string> RequiredRoles { get; init; } = Array.Empty<string>();

    /// <summary>Required permissions.</summary>
    public IReadOnlyList<string> RequiredPermissions { get; init; } = Array.Empty<string>();

    /// <summary>Logical operator for combining role/permission checks.</summary>
    public XFRbacOperator Operator { get; init; } = XFRbacOperator.And;

    /// <summary>Decision when rule matches.</summary>
    public XFRbacDecision Effect { get; init; } = XFRbacDecision.Allow;
}

/// <summary>Audit log entry.</summary>
[Obsolete("Use AiAuditEntry from XForge.DesignSystem.Services.AI instead. This record will be removed in v2.0.")]
public sealed record XFAuditEntry
{
    /// <summary>Entry unique identifier.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>Timestamp of the action.</summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>User ID who performed the action.</summary>
    public string? UserId { get; init; }

    /// <summary>User display name.</summary>
    public string? UserName { get; init; }

    /// <summary>Action performed.</summary>
    public XFAuditAction Action { get; init; }

    /// <summary>Custom action label (when Action = Custom).</summary>
    public string? CustomAction { get; init; }

    /// <summary>Target resource identifier.</summary>
    public string? Resource { get; init; }

    /// <summary>Severity level.</summary>
    public XFAuditSeverity Severity { get; init; } = XFAuditSeverity.Info;

    /// <summary>Human-readable details.</summary>
    public string? Details { get; init; }

    /// <summary>Client IP address.</summary>
    public string? IpAddress { get; init; }

    /// <summary>Action duration.</summary>
    public TimeSpan? Duration { get; init; }

    /// <summary>Whether action succeeded.</summary>
    public bool Success { get; init; } = true;

    /// <summary>Error message if failed.</summary>
    public string? Error { get; init; }

    /// <summary>Additional metadata.</summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>Audit log query filter.</summary>
public sealed record XFAuditFilter
{
    /// <summary>Filter by user ID.</summary>
    public string? UserId { get; init; }

    /// <summary>Filter by action type.</summary>
    public XFAuditAction? Action { get; init; }

    /// <summary>Filter by severity.</summary>
    public XFAuditSeverity? Severity { get; init; }

    /// <summary>Filter from date.</summary>
    public DateTimeOffset? From { get; init; }

    /// <summary>Filter to date.</summary>
    public DateTimeOffset? To { get; init; }

    /// <summary>Full-text search in details/resource.</summary>
    public string? SearchText { get; init; }

    /// <summary>Maximum results.</summary>
    public int Limit { get; init; } = 100;

    /// <summary>Pagination offset.</summary>
    public int Offset { get; init; }
}

/// <summary>User consent record.</summary>
public sealed record XFConsentRecord
{
    /// <summary>Consent unique identifier.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>User who granted/revoked consent.</summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>Data category this consent covers.</summary>
    public XFDataCategory Category { get; init; }

    /// <summary>Current consent status.</summary>
    public XFConsentStatus Status { get; init; } = XFConsentStatus.Pending;

    /// <summary>When consent was granted.</summary>
    public DateTimeOffset? GrantedAt { get; init; }

    /// <summary>When consent expires (null = indefinite).</summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>Purpose description.</summary>
    public string Purpose { get; init; } = string.Empty;

    /// <summary>Legal basis for processing.</summary>
    public XFLegalBasis LegalBasis { get; init; }
}

/// <summary>Data deletion request (LGPD Art. 18 / GDPR Art. 17).</summary>
public sealed record XFDeletionRequest
{
    /// <summary>Request unique identifier.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>User requesting deletion.</summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>When request was submitted.</summary>
    public DateTimeOffset RequestedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Current request status.</summary>
    public XFDeletionRequestStatus Status { get; init; } = XFDeletionRequestStatus.Pending;

    /// <summary>Who approved the request.</summary>
    public string? ApprovedBy { get; init; }

    /// <summary>When deletion was completed.</summary>
    public DateTimeOffset? CompletedAt { get; init; }

    /// <summary>Reason for deletion request.</summary>
    public string? Reason { get; init; }

    /// <summary>Data categories to delete.</summary>
    public IReadOnlyList<XFDataCategory> DataCategories { get; init; } = Array.Empty<XFDataCategory>();
}

/// <summary>Data retention policy.</summary>
public sealed record XFDataRetentionPolicy
{
    /// <summary>Data category this policy applies to.</summary>
    public XFDataCategory Category { get; init; }

    /// <summary>Retention period in days.</summary>
    public int RetentionDays { get; init; } = 365;

    /// <summary>Whether to auto-delete after retention period.</summary>
    public bool AutoDelete { get; init; }

    /// <summary>Legal basis for retention.</summary>
    public XFLegalBasis LegalBasis { get; init; }

    /// <summary>Policy description.</summary>
    public string? Description { get; init; }
}

/// <summary>Compliance dashboard summary.</summary>
public sealed record XFComplianceReport
{
    /// <summary>When report was generated.</summary>
    public DateTimeOffset GeneratedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Total registered users.</summary>
    public int TotalUsers { get; init; }

    /// <summary>Active (granted) consents.</summary>
    public int ActiveConsents { get; init; }

    /// <summary>Revoked consents.</summary>
    public int RevokedConsents { get; init; }

    /// <summary>Pending deletion requests.</summary>
    public int PendingDeletions { get; init; }

    /// <summary>Completed deletions.</summary>
    public int CompletedDeletions { get; init; }

    /// <summary>Active retention policies.</summary>
    public IReadOnlyList<XFDataRetentionPolicy> RetentionPolicies { get; init; } = Array.Empty<XFDataRetentionPolicy>();

    /// <summary>Applicable regulations.</summary>
    public IReadOnlyList<XFComplianceRegulation> Regulations { get; init; } = Array.Empty<XFComplianceRegulation>();

    /// <summary>Consent breakdown by category.</summary>
    public IReadOnlyDictionary<XFDataCategory, int> ConsentsByCategory { get; init; } = new Dictionary<XFDataCategory, int>();

    /// <summary>Deletion requests by status.</summary>
    public IReadOnlyDictionary<XFDeletionRequestStatus, int> DeletionsByStatus { get; init; } = new Dictionary<XFDeletionRequestStatus, int>();
}

/// <summary>Micro-frontend module definition.</summary>
public sealed record XFMicroFrontendModule
{
    /// <summary>Module name/identifier.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Remote URL to load the module from.</summary>
    public string Url { get; init; } = string.Empty;

    /// <summary>Module version.</summary>
    public string? Version { get; init; }

    /// <summary>Module scope for isolation.</summary>
    public string? Scope { get; init; }

    /// <summary>Module dependencies.</summary>
    public IReadOnlyList<string> Dependencies { get; init; } = Array.Empty<string>();

    /// <summary>Whether module is currently enabled.</summary>
    public bool Enabled { get; init; } = true;
}
