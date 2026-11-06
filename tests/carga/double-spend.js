// Livro do Laboratório, cap. 00 — prova de que não existe double-spend.
// Uso: k6 run tests/carga/double-spend.js   (API em http://localhost:5000, ou BASE_URL=...)
//
// Roteiro: cria um usuário + conta, deposita R$ 100 e dispara 50 saques SIMULTÂNEOS de R$ 10.
// Aprovado se: exatamente 10 saques aceitos, 40 recusados por saldo, saldo final R$ 0,00.
import http from 'k6/http';
import { check } from 'k6';
import { Counter } from 'k6/metrics';

const BASE = __ENV.BASE_URL || 'http://localhost:5000';
const SAQUES = 50;
const VALOR = 10;
const DEPOSITO = 100;

const aceitos = new Counter('saques_aceitos');
const recusadosPorSaldo = new Counter('saques_recusados_saldo');
const erros = new Counter('saques_erro_inesperado');

export const options = {
  scenarios: {
    saques: { executor: 'shared-iterations', vus: SAQUES, iterations: SAQUES, maxDuration: '30s' },
  },
  thresholds: {
    saques_aceitos: [`count==${DEPOSITO / VALOR}`],
    saques_erro_inesperado: ['count==0'],
  },
};

const json = { headers: { 'Content-Type': 'application/json' } };
const auth = (token) => ({ headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` } });

function cpfValido() {
  const n = Array.from({ length: 9 }, () => Math.floor(Math.random() * 10));
  const dv = (base) => {
    const soma = base.reduce((s, d, i) => s + d * (base.length + 1 - i), 0);
    const r = (soma * 10) % 11;
    return r === 10 ? 0 : r;
  };
  n.push(dv(n));
  n.push(dv(n));
  return n.join('');
}

export function setup() {
  const cpf = cpfValido();
  const senha = 'Senha123';
  const numero = 100000 + Math.floor(Math.random() * 800000);

  const reg = http.post(`${BASE}/api/auth/registrar`,
    JSON.stringify({ nome: 'Teste Concorrencia', cpf, email: `k6.${cpf}@bankmore.lab`, senha }), json);
  check(reg, { 'usuário criado': (r) => r.status === 201 });

  const login = http.post(`${BASE}/api/login`, JSON.stringify({ documentoOuConta: cpf, senha }), json);
  const token = login.json('token');

  const conta = http.post(`${BASE}/api/contacorrente`, JSON.stringify({ nome: 'Teste Concorrencia', numero, senha }), auth(token));
  check(conta, { 'conta criada': (r) => r.status === 201 });

  const dep = http.post(`${BASE}/api/movimentacoes`,
    JSON.stringify({ tipo: 'C', valor: DEPOSITO, chaveIdempotencia: `deposito-${numero}` }), auth(token));
  check(dep, { 'depósito ok': (r) => r.status === 201 });

  return { token };
}

export default function (data) {
  const r = http.post(`${BASE}/api/movimentacoes`,
    JSON.stringify({ tipo: 'D', valor: VALOR, chaveIdempotencia: `saque-${__VU}-${__ITER}` }), auth(data.token));

  if (r.status === 201) aceitos.add(1);
  else if (r.status === 422 && r.json('code') === 'INSUFFICIENT_FUNDS') recusadosPorSaldo.add(1);
  else erros.add(1);
}

export function teardown(data) {
  const saldo = http.get(`${BASE}/api/contacorrente/minha`, auth(data.token)).json('saldo');
  console.log(`Saldo final: R$ ${saldo}`);
  check(saldo, { 'saldo final é zero (nenhum centavo criado ou perdido)': (s) => Number(s) === 0 });
}
