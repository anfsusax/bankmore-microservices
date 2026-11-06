// ==========================================================================
// Escola BankMore — interações do livro (sem dependências além do Mermaid)
// ==========================================================================
'use strict';

document.addEventListener('DOMContentLoaded', () => {
  initTheme();
  initMermaid();
  initMenuMobile();
  initScrollSpyAndProgress();
  initSearch();
  initCopyButtons();
  initDoubleSpendSimulator();
  initIdempotencySimulator();
  initSagaSimulator();
  initCircuitBreakerSimulator();
  initQuiz();
  initChecklist();
});

// ---------- Armazenamento local seguro (pode falhar em modo privado) ----------
function lerLocal(chave) {
  try { return window.localStorage.getItem(chave); } catch { return null; }
}
function gravarLocal(chave, valor) {
  try { window.localStorage.setItem(chave, valor); } catch { /* sem persistência, sem problema */ }
}

// ---------- Tema claro/escuro ----------
function initTheme() {
  const botao = document.getElementById('theme-toggle');
  const icone = document.getElementById('theme-icon');
  const preferido = lerLocal('bankmore-tema')
    || (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');
  aplicar(preferido);

  botao.addEventListener('click', () => {
    const novo = document.body.classList.contains('dark-theme') ? 'light' : 'dark';
    aplicar(novo);
    gravarLocal('bankmore-tema', novo);
    window.dispatchEvent(new Event('bankmore-tema-mudou'));
  });

  function aplicar(tema) {
    document.body.classList.toggle('dark-theme', tema === 'dark');
    document.body.classList.toggle('light-theme', tema !== 'dark');
    icone.textContent = tema === 'dark' ? '☀️' : '🌙';
    botao.setAttribute('aria-pressed', String(tema === 'dark'));
    botao.title = tema === 'dark' ? 'Alternar para modo claro' : 'Alternar para modo escuro';
  }
}

// ---------- Mermaid (re-renderiza ao trocar o tema) ----------
function initMermaid() {
  const blocos = Array.from(document.querySelectorAll('.mermaid'));
  blocos.forEach(b => { b.dataset.fonte = b.textContent.trim(); });

  async function renderizar() {
    if (!window.mermaid) return; // offline: o texto-fonte continua visível
    const escuro = document.body.classList.contains('dark-theme');
    window.mermaid.initialize({ startOnLoad: false, theme: escuro ? 'dark' : 'default', securityLevel: 'strict' });
    for (const [i, bloco] of blocos.entries()) {
      try {
        const { svg } = await window.mermaid.render(`diagrama-${i}-${Date.now()}`, bloco.dataset.fonte);
        bloco.innerHTML = svg;
        bloco.setAttribute('data-processed', 'true');
      } catch (erro) {
        console.warn('Diagrama não renderizado', erro);
      }
    }
  }

  renderizar();
  window.addEventListener('bankmore-tema-mudou', renderizar);
}

// ---------- Menu lateral no celular ----------
function initMenuMobile() {
  const botao = document.getElementById('menu-toggle');
  const sidebar = document.querySelector('.sidebar');
  botao.addEventListener('click', () => {
    const aberta = sidebar.classList.toggle('aberta');
    botao.setAttribute('aria-expanded', String(aberta));
  });
  sidebar.querySelectorAll('.nav-item').forEach(link =>
    link.addEventListener('click', () => {
      sidebar.classList.remove('aberta');
      botao.setAttribute('aria-expanded', 'false');
    }));
}

// ---------- Progresso de leitura + item ativo no menu ----------
function initScrollSpyAndProgress() {
  const barra = document.getElementById('progress-bar-fill');
  const percentual = document.getElementById('progress-percent');
  const links = Array.from(document.querySelectorAll('.sidebar-nav .nav-item'));
  const secoes = links.map(l => document.getElementById(l.dataset.target)).filter(Boolean);

  function atualizar() {
    const total = document.documentElement.scrollHeight - window.innerHeight;
    const pct = total > 0 ? Math.min(100, Math.round((window.scrollY / total) * 100)) : 0;
    barra.style.width = `${pct}%`;
    barra.setAttribute('aria-valuenow', String(pct));
    percentual.textContent = `${pct}%`;

    let atual = secoes[0];
    for (const s of secoes) {
      if (s.getBoundingClientRect().top <= 140) atual = s;
    }
    links.forEach(l => {
      const ativo = atual && l.dataset.target === atual.id;
      l.classList.toggle('active', ativo);
      if (ativo) l.setAttribute('aria-current', 'location'); else l.removeAttribute('aria-current');
    });
  }

  window.addEventListener('scroll', atualizar, { passive: true });
  window.addEventListener('resize', atualizar);
  atualizar();
}

// ---------- Busca de conceitos ----------
function initSearch() {
  const campo = document.getElementById('module-search');
  const resultado = document.getElementById('search-results');
  const secoes = Array.from(document.querySelectorAll('.lesson-module'));

  campo.addEventListener('input', () => {
    const termo = campo.value.trim().toLowerCase();
    if (!termo) {
      secoes.forEach(s => s.classList.remove('oculto-busca'));
      resultado.textContent = '';
      return;
    }
    let encontrados = 0;
    secoes.forEach(s => {
      const bate = s.textContent.toLowerCase().includes(termo);
      s.classList.toggle('oculto-busca', !bate);
      if (bate) encontrados++;
    });
    resultado.textContent = encontrados === 1 ? '1 módulo' : `${encontrados} módulos`;
  });
}

// ---------- Copiar código ----------
function initCopyButtons() {
  document.addEventListener('click', async (e) => {
    const botao = e.target.closest('.btn-copy');
    if (!botao) return;
    const codigo = botao.closest('.code-spotlight').querySelector('pre code').innerText;
    try {
      await navigator.clipboard.writeText(codigo);
      botao.textContent = '✔ Copiado';
    } catch {
      botao.textContent = 'Selecione e copie';
    }
    setTimeout(() => { botao.textContent = '📋 Copiar'; }, 1600);
  });
}

// ---------- Utilitários dos simuladores ----------
const esperar = (ms) => new Promise(r => setTimeout(r, ms));
const brl = (v) => v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });

function logger(elemento) {
  return (mensagem, tipo = 'info') => {
    const linha = document.createElement('div');
    linha.className = `log-${tipo}`;
    linha.textContent = `[${new Date().toLocaleTimeString('pt-BR')}] ${mensagem}`;
    elemento.appendChild(linha);
    elemento.scrollTop = elemento.scrollHeight;
  };
}

// ---------- Simulador 1: double-spend (antes x depois) ----------
function initDoubleSpendSimulator() {
  const botao = document.getElementById('ds-run');
  if (!botao) return;
  const qtd = document.getElementById('ds-qtd');
  const saldoInicial = 100;
  const valor = 10;

  botao.addEventListener('click', async () => {
    botao.disabled = true;
    const n = Math.max(2, Math.min(50, Number(qtd.value) || 20));
    await Promise.all([rodar('antes', n), rodar('depois', n)]);
    botao.disabled = false;
  });

  async function rodar(modo, n) {
    const saldoEl = document.getElementById(`ds-saldo-${modo}`);
    const logEl = document.getElementById(`ds-log-${modo}`);
    const resumoEl = document.getElementById(`ds-resumo-${modo}`);
    logEl.innerHTML = '';
    const log = logger(logEl);
    let saldo = saldoInicial;
    let aceitos = 0;
    let recusados = 0;
    saldoEl.classList.remove('negativo');
    saldoEl.textContent = brl(saldo);

    const saques = Array.from({ length: n }, (_, i) => async () => {
      await esperar(Math.random() * 40);
      if (modo === 'antes') {
        // 1) lê o saldo  2) "pensa"  3) grava o débito: outro saque cabe no meio
        const lido = saldo;
        await esperar(20 + Math.random() * 30);
        if (lido >= valor) {
          saldo -= valor;
          aceitos++;
          log(`Saque #${i + 1}: leu ${brl(lido)} → debitou ${brl(valor)}`, saldo < 0 ? 'erro' : 'ok');
        } else {
          recusados++;
          log(`Saque #${i + 1}: recusado (leu ${brl(lido)})`, 'aviso');
        }
      } else {
        // UPDATE ... WHERE saldo >= valor: checar e debitar é UMA operação atômica
        if (saldo >= valor) {
          saldo -= valor;
          aceitos++;
          log(`Saque #${i + 1}: UPDATE afetou 1 linha`, 'ok');
        } else {
          recusados++;
          log(`Saque #${i + 1}: UPDATE afetou 0 linhas → 422 INSUFFICIENT_FUNDS`, 'aviso');
        }
      }
      saldoEl.textContent = brl(saldo);
      saldoEl.classList.toggle('negativo', saldo < 0);
    });

    await Promise.all(saques.map(s => s()));
    const ok = saldo >= 0 && aceitos === saldoInicial / valor;
    resumoEl.innerHTML = `${aceitos} aceitos · ${recusados} recusados · saldo final <strong>${brl(saldo)}</strong> ${ok ? '✅' : '❌ dinheiro criado do nada'}`;
  }
}

// ---------- Simulador 2: idempotência ----------
function initIdempotencySimulator() {
  const botao = document.getElementById('idem-enviar');
  if (!botao) return;
  const chave = document.getElementById('idem-chave');
  const valor = document.getElementById('idem-valor');
  const log = logger(document.getElementById('idem-log'));
  const registros = new Map();
  let saldo = 1000;
  const saldoEl = document.getElementById('idem-saldo');

  botao.addEventListener('click', () => {
    const k = chave.value.trim();
    const v = Number(valor.value);
    if (!k || !(v > 0)) { log('400: informe chave e valor positivo', 'erro'); return; }

    const conteudo = `transferencia:destino-2002:${v.toFixed(2)}`;
    const existente = registros.get(k);
    if (existente) {
      if (existente.conteudo === conteudo) {
        log(`200 OK (replay): mesma chave + mesmo conteúdo → id ${existente.id}. Nenhum dinheiro saiu de novo.`, 'info');
      } else {
        log(`422 IDEMPOTENCY_CONFLICT: a chave "${k}" já foi usada com outro valor.`, 'erro');
      }
      return;
    }
    if (saldo < v) { log('422 INSUFFICIENT_FUNDS', 'aviso'); return; }
    saldo -= v;
    const id = Math.random().toString(16).slice(2, 10);
    registros.set(k, { conteudo, id });
    saldoEl.textContent = brl(saldo);
    log(`201 Created: transferência ${id} de ${brl(v)}`, 'ok');
  });

  document.getElementById('idem-timeout').addEventListener('click', () => {
    log('⏱️ Timeout no app do cliente... ele não sabe se o PIX saiu. Reenviando com a MESMA chave:', 'aviso');
    botao.click();
  });
}

// ---------- Simulador 3: saga do PIX ----------
function initSagaSimulator() {
  const botao = document.getElementById('saga-run');
  if (!botao) return;
  const falha = document.getElementById('saga-falha');
  const estadoEl = document.getElementById('saga-estado');
  const log = logger(document.getElementById('saga-log'));
  const passos = ['bloquear', 'antifraude', 'liquidar', 'efetivar'].map(id => document.getElementById(`saga-${id}`));

  botao.addEventListener('click', async () => {
    botao.disabled = true;
    document.getElementById('saga-log').innerHTML = '';
    passos.forEach(p => p.className = 'saga-step');
    const ponto = falha.value;

    const estado = (e) => { estadoEl.textContent = e; };

    estado('INICIADA');
    log('PIX de R$ 250,00 recebido (EndToEndId E2E-0001)', 'info');

    if (!await passo(0, 'BLOQUEADA', 'Saldo bloqueado (hold) na conta de origem')) return fim();
    if (ponto === 'antifraude') {
      await marcarFalha(1, 'Antifraude recusou: padrão suspeito');
      await compensar('Desbloqueio do saldo (compensação do passo 1)');
      estado('ESTORNADA');
      return fim();
    }
    if (!await passo(1, 'EM_ANALISE', 'Antifraude aprovou')) return fim();

    if (ponto === 'spi' || ponto === 'estorno') {
      await marcarFalha(2, 'SPI indisponível (503) após retries');
      if (ponto === 'estorno') {
        log('Compensação falhou: Contas fora do ar!', 'erro');
        passos[0].className = 'saga-step falha';
        estado('FALHA_COMPENSACAO');
        log('Estado persistido. Job de reconciliação vai tentar de novo e alertar a operação.', 'aviso');
        return fim();
      }
      await compensar('Desbloqueio do saldo (compensação do passo 1)');
      estado('ESTORNADA');
      return fim();
    }
    if (!await passo(2, 'LIQUIDADA', 'SPI confirmou a liquidação')) return fim();
    await passo(3, 'CONCLUIDA', 'Débito efetivado; evento PixConcluido gravado no Outbox');
    fim();

    async function passo(i, novoEstado, mensagem) {
      passos[i].classList.add('ativo');
      await esperar(650);
      passos[i].className = 'saga-step ok';
      estado(novoEstado);
      log(mensagem, 'ok');
      return true;
    }
    async function marcarFalha(i, mensagem) {
      passos[i].classList.add('ativo');
      await esperar(650);
      passos[i].className = 'saga-step falha';
      log(mensagem, 'erro');
    }
    async function compensar(mensagem) {
      await esperar(500);
      passos[0].className = 'saga-step compensado';
      log(mensagem, 'aviso');
    }
    function fim() { botao.disabled = false; }
  });
}

// ---------- Simulador 4: circuit breaker no SPI ----------
function initCircuitBreakerSimulator() {
  const enviar = document.getElementById('cb-enviar');
  if (!enviar) return;
  const spiFora = document.getElementById('cb-spi-fora');
  const luz = document.getElementById('cb-luz');
  const estadoEl = document.getElementById('cb-estado');
  const falhasEl = document.getElementById('cb-falhas');
  const bloqueadosEl = document.getElementById('cb-bloqueados');
  const pendentesEl = document.getElementById('cb-pendentes');
  const log = logger(document.getElementById('cb-log'));
  const limite = 3;
  const pausaMs = 8000;
  let estado = 'Fechado';
  let falhas = 0;
  let bloqueados = 0;
  let pendentes = 0;
  let abertoEm = 0;

  function atualizar() {
    if (estado === 'Aberto' && Date.now() - abertoEm >= pausaMs) estado = 'Meio-aberto';
    estadoEl.textContent = estado;
    luz.className = 'cb-light' + (estado === 'Aberto' ? ' aberto' : estado === 'Meio-aberto' ? ' meio' : '');
    falhasEl.textContent = `Falhas seguidas: ${falhas}/${limite}`;
    bloqueadosEl.textContent = `Chamadas evitadas: ${bloqueados}`;
    pendentesEl.textContent = `PIX pendentes: ${pendentes}`;
  }
  setInterval(atualizar, 500);

  enviar.addEventListener('click', () => {
    atualizar();
    if (estado === 'Aberto') {
      bloqueados++;
      pendentes++;
      log('Circuito ABERTO: SPI nem foi chamado. PIX → PENDENTE_LIQUIDACAO (saldo segue bloqueado).', 'aviso');
      return atualizar();
    }
    const falhou = spiFora.checked;
    if (falhou) {
      falhas++;
      log(`SPI respondeu 503 (${falhas}/${limite})`, 'erro');
      if (estado === 'Meio-aberto' || falhas >= limite) {
        estado = 'Aberto';
        abertoEm = Date.now();
        log(`Circuito ABERTO por ${pausaMs / 1000}s: protege o SPI e responde rápido ao cliente.`, 'erro');
      }
    } else {
      if (estado === 'Meio-aberto') log('Chamada de teste OK: circuito FECHADO de novo.', 'ok');
      estado = 'Fechado';
      falhas = 0;
      log('PIX liquidado no SPI ✅', 'ok');
    }
    atualizar();
  });

  document.getElementById('cb-reset').addEventListener('click', () => {
    estado = 'Fechado'; falhas = 0; bloqueados = 0; pendentes = 0;
    document.getElementById('cb-log').innerHTML = '';
    atualizar();
  });
  atualizar();
}

// ---------- Quiz ----------
function initQuiz() {
  const itens = Array.from(document.querySelectorAll('.quiz-item'));
  const placar = document.getElementById('quiz-score');
  let acertos = 0;
  let respondidas = 0;

  itens.forEach(item => {
    const opcoes = Array.from(item.querySelectorAll('.quiz-option'));
    const feedback = item.querySelector('.quiz-feedback');
    opcoes.forEach(op => op.addEventListener('click', () => {
      const certa = op.dataset.correta === 'true';
      opcoes.forEach(o => {
        o.disabled = true;
        if (o.dataset.correta === 'true') o.classList.add('correta');
      });
      if (!certa) op.classList.add('errada');
      feedback.classList.add('visivel');
      respondidas++;
      if (certa) acertos++;
      placar.textContent = `${acertos} de ${respondidas} (total ${itens.length})`;
    }));
  });
}

// ---------- Checklist com progresso salvo no navegador ----------
function initChecklist() {
  const caixas = Array.from(document.querySelectorAll('.checklist input[type=checkbox]'));
  let salvo = {};
  try { salvo = JSON.parse(lerLocal('bankmore-checklist') || '{}'); } catch { salvo = {}; }

  caixas.forEach(c => {
    c.checked = Boolean(salvo[c.id]);
    c.addEventListener('change', () => {
      salvo[c.id] = c.checked;
      gravarLocal('bankmore-checklist', JSON.stringify(salvo));
    });
  });
}
