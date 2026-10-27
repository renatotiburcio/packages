---
name: release
description: Prepare and execute release process
---

# /release

## Objective
Prepare a new release with changelog, version bump, and deployment.

## Steps
1. Run quality gates
2. Run security audit
3. Generate changelog
4. Create release branch
5. Tag release
6. Deploy

## Usage
/release prepare v2.1.0 with changes from last 2 weeks
/release execute release v2.1.0
