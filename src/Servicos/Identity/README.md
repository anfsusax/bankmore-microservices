# Identity — capítulo 07

**Estilo:** microsserviço · **Status:** planejado (hoje vive dentro do monólito)

- **Papel:** cadastro de usuários e emissão de JWT.
- **Por que migrar:** fronteira clara, ninguém faz join com `usuarios`, ritmo de mudança diferente (segurança).
- **Dados que possui:** `usuarios` (banco próprio).
- **Eventos publicados:** `UsuarioCadastrado`, `UsuarioEncerrado`.
- **Armadilha:** o token emitido precisa ser idêntico ao do monólito (issuer, audience, chave, claims), senão quebra quem já está logado.
- **Critério de pronto:** login roteado pelo gateway para o Identity; monólito não lê mais `usuarios`.
