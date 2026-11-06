# Gateway (YARP) — capítulo 06

**Estilo:** microsserviço de borda · **Status:** planejado

- **Papel:** ponto único de entrada; valida JWT na borda; rate limit por conta; roteia por rota.
- **Por que existe:** é o ponto de desvio do Strangler Fig. Extrair um serviço = trocar uma rota; rollback = desfazer a rota.
- **Dados que possui:** nenhum.
- **Critério de pronto:** 100% do tráfego passando pelo gateway sem mudar contrato; teste de rollback de rota.
