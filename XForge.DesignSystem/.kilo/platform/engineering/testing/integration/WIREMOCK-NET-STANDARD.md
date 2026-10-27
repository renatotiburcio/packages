# WireMock.Net Standard

## Objetivo

Mockar APIs HTTP externas de forma confiável e reproduzível.

## Quando usar

- gateways de pagamento;
- Open Finance;
- APIs de terceiros;
- SEFAZ-like mocks em ambiente de teste;
- webhooks;
- serviços instáveis;
- simulação de erro/timeouts.

## Regras

- não depender de API externa em teste unitário/integration;
- simular sucesso, erro, timeout, rate limit e payload inválido;
- versionar contratos de mocks;
- testar resiliência e retry.
