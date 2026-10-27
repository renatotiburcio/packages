# Multi-Tenant Configuration

## Overview

XForge supports multiple tenants in a single template installation. Each tenant has its own:
- Provider/model configuration
- Knowledge base paths
- RAG index (shared or isolated)

## Setup

### 1. Create a tenant config
Copy `default.json` to `<tenant-id>.json` in `.xforge/tenants/`:

```json
{
  "id": "my-tenant",
  "name": "My Tenant Name",
  "provider": {
    "active": "openrouter",
    "model": "anthropic/claude-sonnet-4-20250514"
  },
  "knowledge": {
    "paths": [".xforge/tenants/my-tenant/knowledge"],
    "excludePatterns": []
  },
  "rag": {
    "indexDir": ".xforge/tenants/my-tenant/rag/indexes",
    "shared": false
  }
}
```

### 2. Select active tenant
```powershell
$env:XFORGE_TENANT = "my-tenant"
```

### 3. Verify
```powershell
powershell -File .xforge/scripts/tenant-manager.ps1
```

## Tenant Manager

```powershell
# List all tenants
powershell -File .xforge/scripts/tenant-manager.ps1

# Set active tenant
$env:XFORGE_TENANT = "tenant-acme"
powershell -File .xforge/scripts/tenant-manager.ps1
```

## Directory Structure

```
.xforge/tenants/
  default.json          # Default tenant config
  tenant-acme.json      # ACME Corp tenant config
  tenant-acme/
    knowledge/          # ACME-specific knowledge
    rag/
      indexes/          # ACME-specific RAG index
```
