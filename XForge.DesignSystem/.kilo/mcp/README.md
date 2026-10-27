# MCP Server Configurations

Pre-configured MCP (Model Context Protocol) servers for XForge projects.

## Available Servers

| Server | Package | Env Var |
|--------|---------|---------|
| PostgreSQL | `@modelcontextprotocol/server-postgres` | `POSTGRES_CONNECTION_STRING` |
| Redis | `@modelcontextprotocol/server-redis` | `REDIS_URL` |
| RabbitMQ | `@modelcontextprotocol/server-rabbitmq` | `RABBITMQ_URL` |

## Setup

1. Set the required environment variables:
```powershell
$env:POSTGRES_CONNECTION_STRING = "postgresql://user:pass@localhost:5432/mydb"
$env:REDIS_URL = "redis://localhost:6379"
$env:RABBITMQ_URL = "amqp://user:pass@localhost:5672"
```

2. Install MCP server packages:
```powershell
npm install -g @modelcontextprotocol/server-postgres
npm install -g @modelcontextprotocol/server-redis
npm install -g @modelcontextprotocol/server-rabbitmq
```

3. Configure in kilo.jsonc:
```jsonc
{
  "mcpServers": {
    "postgresql": ".kilo/mcp/postgresql.json",
    "redis": ".kilo/mcp/redis.json",
    "rabbitmq": ".kilo/mcp/rabbitmq.json"
  }
}
```

## Adding Custom Servers

Create a new JSON file in `.kilo/mcp/` following the schema:
```json
{
  "name": "my-server",
  "version": "1.0.0",
  "description": "Description of the server",
  "type": "stdio",
  "command": "npx",
  "args": ["-y", "my-mcp-server"],
  "env": {},
  "capabilities": { "tools": [] }
}
```
