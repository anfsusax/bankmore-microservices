// Renderização de diagramas Mermaid do Livro do Laboratório.
window.livro = {
    _contador: 0,
    renderizarMermaid: async function (elemento, definicao) {
        if (!window.mermaid || !elemento) return;
        if (!window.livro._iniciado) {
            window.mermaid.initialize({ startOnLoad: false, theme: 'default', securityLevel: 'strict' });
            window.livro._iniciado = true;
        }
        const id = 'livro-mermaid-' + (++window.livro._contador);
        const { svg } = await window.mermaid.render(id, definicao);
        elemento.innerHTML = svg;
    }
};
