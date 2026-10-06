/**
 * Target Sistemas - ERP Modern Dashboard Engine
 * Vanilla JavaScript Modular Architecture mirroring .NET 10 Domain Services
 * Strict implementation of Clean Code and Object Calisthenics principles.
 */

// ============================================================================
// CONSTANTES & PAYLOADS OFICIAIS
// ============================================================================
const PAYLOAD_VENDAS_OFICIAL = {
  "vendas": [
    { "vendedor": "João Silva", "valor": 1200.50 }, { "vendedor": "João Silva", "valor": 950.75 }, { "vendedor": "João Silva", "valor": 1800.00 }, { "vendedor": "João Silva", "valor": 1400.30 }, { "vendedor": "João Silva", "valor": 1100.90 }, { "vendedor": "João Silva", "valor": 1550.00 }, { "vendedor": "João Silva", "valor": 1700.80 }, { "vendedor": "João Silva", "valor": 250.30 }, { "vendedor": "João Silva", "valor": 480.75 }, { "vendedor": "João Silva", "valor": 320.40 },
    { "vendedor": "Maria Souza", "valor": 2100.40 }, { "vendedor": "Maria Souza", "valor": 1350.60 }, { "vendedor": "Maria Souza", "valor": 950.20 }, { "vendedor": "Maria Souza", "valor": 1600.75 }, { "vendedor": "Maria Souza", "valor": 1750.00 }, { "vendedor": "Maria Souza", "valor": 1450.90 }, { "vendedor": "Maria Souza", "valor": 400.50 }, { "vendedor": "Maria Souza", "valor": 180.20 }, { "vendedor": "Maria Souza", "valor": 90.75 },
    { "vendedor": "Carlos Oliveira", "valor": 800.50 }, { "vendedor": "Carlos Oliveira", "valor": 1200.00 }, { "vendedor": "Carlos Oliveira", "valor": 1950.30 }, { "vendedor": "Carlos Oliveira", "valor": 1750.80 }, { "vendedor": "Carlos Oliveira", "valor": 1300.60 }, { "vendedor": "Carlos Oliveira", "valor": 300.40 }, { "vendedor": "Carlos Oliveira", "valor": 500.00 }, { "vendedor": "Carlos Oliveira", "valor": 125.75 },
    { "vendedor": "Ana Lima", "valor": 1000.00 }, { "vendedor": "Ana Lima", "valor": 1100.50 }, { "vendedor": "Ana Lima", "valor": 1250.75 }, { "vendedor": "Ana Lima", "valor": 1400.20 }, { "vendedor": "Ana Lima", "valor": 1550.90 }, { "vendedor": "Ana Lima", "valor": 1650.00 }, { "vendedor": "Ana Lima", "valor": 75.30 }, { "vendedor": "Ana Lima", "valor": 420.90 }, { "vendedor": "Ana Lima", "valor": 315.40 }
  ]
};

const PAYLOAD_ESTOQUE_INICIAL = [
  { codigoProduto: 101, descricaoProduto: "Caneta Azul", estoqueInicial: 150, estoqueAtual: 150 },
  { codigoProduto: 102, descricaoProduto: "Caderno Universitário", estoqueInicial: 75, estoqueAtual: 75 },
  { codigoProduto: 103, descricaoProduto: "Borracha Branca", estoqueInicial: 200, estoqueAtual: 200 },
  { codigoProduto: 104, descricaoProduto: "Lápis Preto HB", estoqueInicial: 320, estoqueAtual: 320 },
  { codigoProduto: 105, descricaoProduto: "Marcador de Texto Amarelo", estoqueInicial: 90, estoqueAtual: 90 }
];

// ============================================================================
// FORMATAÇÃO E UTILITÁRIOS
// ============================================================================
const formatadorMoeda = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });

function formatarMoeda(valor) {
  return formatadorMoeda.format(valor || 0);
}

function formatarPercentual(valor) {
  return `${(valor || 0).toFixed(2).replace('.', ',')}%`;
}

// ============================================================================
// 1. MÓDULO COMERCIAL - MOTOR DE CÁLCULO DE COMISSÕES
// ============================================================================
class MotorComercial {
  static obterAliquotaComissao(valor) {
    if (valor < 100.00) return 0.00;
    if (valor < 500.00) return 0.01;
    return 0.05;
  }

  static calcularComissaoItem(valor) {
    const aliquota = this.obterAliquotaComissao(valor);
    const comissao = valor * aliquota;
    return Math.round(comissao * 100) / 100;
  }

  static processarVendas(listaVendas) {
    if (!Array.isArray(listaVendas)) {
      throw new Error("Coleção de vendas inválida.");
    }

    const mapaVendedores = new Map();

    listaVendas.forEach(venda => {
      const nome = (venda.vendedor || '').trim();
      if (!nome) return;

      const valor = Number(venda.valor) || 0;
      const aliquota = this.obterAliquotaComissao(valor);
      const comissao = this.calcularComissaoItem(valor);

      const vendaProcessada = {
        vendedor: nome,
        valor,
        aliquota,
        comissao
      };

      if (!mapaVendedores.has(nome)) {
        mapaVendedores.set(nome, []);
      }
      mapaVendedores.get(nome).push(vendaProcessada);
    });

    const consolidados = Array.from(mapaVendedores.entries()).map(([vendedor, vendas]) => {
      const totalVendas = Math.round(vendas.reduce((acc, v) => acc + v.valor, 0) * 100) / 100;
      const totalComissao = Math.round(vendas.reduce((acc, v) => acc + v.comissao, 0) * 100) / 100;
      const taxaEfetiva = totalVendas > 0 ? (totalComissao / totalVendas) * 100 : 0;

      return {
        vendedor,
        quantidadeVendas: vendas.length,
        totalVendas,
        totalComissao,
        taxaEfetiva,
        vendas
      };
    });

    // Ordena pelo maior volume de vendas
    consolidados.sort((a, b) => b.totalVendas - a.totalVendas);

    const totalGeralVendas = Math.round(consolidados.reduce((acc, v) => acc + v.totalVendas, 0) * 100) / 100;
    const totalGeralComissoes = Math.round(consolidados.reduce((acc, v) => acc + v.totalComissao, 0) * 100) / 100;
    const totalGeralQuantidade = consolidados.reduce((acc, v) => acc + v.quantidadeVendas, 0);

    return {
      vendedores: consolidados,
      totalGeralVendas,
      totalGeralComissoes,
      totalGeralQuantidade,
      taxaGeralMedia: totalGeralVendas > 0 ? (totalGeralComissoes / totalGeralVendas) * 100 : 0
    };
  }
}

// ============================================================================
// 2. MÓDULO DE ESTOQUE - MOTOR DE MOVIMENTAÇÃO DE MERCADORIAS
// ============================================================================
class MotorEstoque {
  constructor() {
    this.produtos = [];
    this.historico = [];
    this.reinicializar();
  }

  reinicializar() {
    this.produtos = PAYLOAD_ESTOQUE_INICIAL.map(p => ({ ...p }));
    this.historico = [];
  }

  obterCatalogo() {
    return this.produtos;
  }

  obterHistorico() {
    return this.historico;
  }

  movimentar(codigoProduto, tipo, quantidade, motivo) {
    const cod = Number(codigoProduto);
    const qtd = parseInt(quantidade, 10);
    const motivoLimpo = (motivo || '').trim();

    if (isNaN(qtd) || qtd <= 0) {
      throw new Error("A quantidade movimentada deve ser um número inteiro estritamente positivo (> 0).");
    }

    if (!motivoLimpo) {
      throw new Error("O motivo da movimentação é obrigatório para fins de auditoria e conformidade.");
    }

    const produto = this.produtos.find(p => p.codigoProduto === cod);
    if (!produto) {
      throw new Error(`Produto com código ${cod} não foi localizado no inventário.`);
    }

    const saldoAnterior = produto.estoqueAtual;
    let saldoAtualizado = saldoAnterior;

    if (tipo === 'Entrada') {
      saldoAtualizado = saldoAnterior + qtd;
      produto.estoqueAtual = saldoAtualizado;
    } else if (tipo === 'Saida') {
      if (qtd > saldoAnterior) {
        throw new Error(`[BLOQUEIO DE SEGURANÇA ERP] Saldo insuficiente para saída do produto ${produto.codigoProduto} (${produto.descricaoProduto}). Saldo disponível: ${saldoAnterior}, Solicitado: ${qtd}.`);
      }
      saldoAtualizado = saldoAnterior - qtd;
      produto.estoqueAtual = saldoAtualizado;
    } else {
      throw new Error(`Tipo de movimentação inválido: ${tipo}`);
    }

    const guid = typeof crypto !== 'undefined' && crypto.randomUUID ? crypto.randomUUID() : this.gerarGuidFallback();

    const registro = {
      id: guid,
      codigoProduto: produto.codigoProduto,
      descricaoProduto: produto.descricaoProduto,
      tipo,
      quantidade: qtd,
      saldoAnterior,
      saldoAtualizado,
      motivo: motivoLimpo,
      dataHora: new Date()
    };

    this.historico.unshift(registro);
    return registro;
  }

  gerarGuidFallback() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}

// ============================================================================
// 3. MÓDULO FINANCEIRO - MOTOR DE CÁLCULO DE MORA E MULTA
// ============================================================================
class MotorFinanceiro {
  static TAXA_DIARIA_PADRAO = 2.5; // 2,5% ao dia corrido

  static calcularMora(valorOriginal, dataVencimentoStr, dataReferenciaStr) {
    const valor = Number(valorOriginal);
    if (isNaN(valor) || valor <= 0) {
      throw new Error("O valor original do título deve ser maior que zero.");
    }

    const [vAno, vMes, vDia] = dataVencimentoStr.split('-').map(Number);
    const [rAno, rMes, rDia] = dataReferenciaStr.split('-').map(Number);

    const dataVenc = new Date(Date.UTC(vAno, vMes - 1, vDia));
    const dataRef = new Date(Date.UTC(rAno, rMes - 1, rDia));

    const diffMs = dataRef.getTime() - dataVenc.getTime();
    const diffDias = Math.floor(diffMs / (1000 * 60 * 60 * 24));

    if (diffDias <= 0) {
      return {
        valorOriginal: valor,
        dataVencimento: dataVencimentoStr,
        dataReferencia: dataReferenciaStr,
        diasAtraso: 0,
        taxaDiariaPercentual: this.TAXA_DIARIA_PADRAO,
        percentualTotalMora: 0,
        valorMulta: 0,
        valorTotal: valor,
        possuiAtraso: false
      };
    }

    const percentualTotalMora = diffDias * this.TAXA_DIARIA_PADRAO;
    const taxaDecimal = (diffDias * 0.025);
    const valorMulta = Math.round((valor * taxaDecimal) * 100) / 100;
    const valorTotal = Math.round((valor + valorMulta) * 100) / 100;

    return {
      valorOriginal: valor,
      dataVencimento: dataVencimentoStr,
      dataReferencia: dataReferenciaStr,
      diasAtraso: diffDias,
      taxaDiariaPercentual: this.TAXA_DIARIA_PADRAO,
      percentualTotalMora,
      valorMulta,
      valorTotal,
      possuiAtraso: true
    };
  }
}

// ============================================================================
// ESTADO DA APLICAÇÃO & CONTROLE DE INTERFACE
// ============================================================================
const motorEstoqueInstance = new MotorEstoque();
let relatorioComercialAtual = null;

// Inicialização da Página
document.addEventListener('DOMContentLoaded', () => {
  configurarNavegacaoAbas();
  inicializarModuloComercial();
  inicializarModuloEstoque();
  inicializarModuloFinanceiro();
  atualizarDashboardGeral();
});

// Navegação de Abas
function configurarNavegacaoAbas() {
  const tabs = document.querySelectorAll('.nav-tab');
  const sections = document.querySelectorAll('.tab-section');

  tabs.forEach(tab => {
    tab.addEventListener('click', (e) => {
      e.preventDefault();
      const targetId = tab.getAttribute('data-target');

      tabs.forEach(t => {
        t.classList.remove('tab-active');
        t.classList.add('text-slate-400');
      });

      tab.classList.add('tab-active');
      tab.classList.remove('text-slate-400');

      sections.forEach(sec => {
        sec.classList.toggle('hidden', sec.id !== targetId);
      });
    });
  });
}

// ============================================================================
// RENDERIZAÇÃO: DASHBOARD GERAL
// ============================================================================
function atualizarDashboardGeral() {
  const relatorio = MotorComercial.processarVendas(PAYLOAD_VENDAS_OFICIAL.vendas);
  document.getElementById('dash-total-vendas').textContent = formatarMoeda(relatorio.totalGeralVendas);
  document.getElementById('dash-total-comissoes').textContent = formatarMoeda(relatorio.totalGeralComissoes);
  
  const catalogo = motorEstoqueInstance.obterCatalogo();
  const totalItens = catalogo.reduce((acc, p) => acc + p.estoqueAtual, 0);
  document.getElementById('dash-total-estoque').textContent = `${totalItens} un`;
  document.getElementById('dash-top-vendedor').textContent = relatorio.vendedores[0]?.vendedor || '-';
}

// ============================================================================
// RENDERIZAÇÃO: MÓDULO COMERCIAL
// ============================================================================
function inicializarModuloComercial() {
  const textarea = document.getElementById('comercial-json-input');
  if (textarea) {
    textarea.value = JSON.stringify(PAYLOAD_VENDAS_OFICIAL, null, 2);
  }

  document.getElementById('btn-processar-vendas')?.addEventListener('click', () => {
    processarPayloadComercial();
  });

  document.getElementById('btn-restaurar-json-vendas')?.addEventListener('click', () => {
    textarea.value = JSON.stringify(PAYLOAD_VENDAS_OFICIAL, null, 2);
    processarPayloadComercial();
  });

  document.getElementById('form-add-venda')?.addEventListener('submit', (e) => {
    e.preventDefault();
    adicionarVendaAvulsa();
  });

  processarPayloadComercial();
}

function processarPayloadComercial() {
  const textarea = document.getElementById('comercial-json-input');
  const alertEl = document.getElementById('comercial-alert');
  alertEl.classList.add('hidden');

  try {
    const parsed = JSON.parse(textarea.value);
    const vendas = parsed.vendas || [];
    relatorioComercialAtual = MotorComercial.processarVendas(vendas);
    renderizarTabelaComercial(relatorioComercialAtual);
    atualizarDashboardGeral();
  } catch (err) {
    alertEl.textContent = `Erro ao processar JSON: ${err.message}`;
    alertEl.classList.remove('hidden');
  }
}

function renderizarTabelaComercial(relatorio) {
  const tbody = document.getElementById('tabela-vendedores-body');
  tbody.innerHTML = '';

  document.getElementById('kpi-total-vendas').textContent = formatarMoeda(relatorio.totalGeralVendas);
  document.getElementById('kpi-total-comissoes').textContent = formatarMoeda(relatorio.totalGeralComissoes);
  document.getElementById('kpi-taxa-media').textContent = formatarPercentual(relatorio.taxaGeralMedia);
  document.getElementById('kpi-qtd-vendas').textContent = `${relatorio.totalGeralQuantidade} transações`;

  relatorio.vendedores.forEach((v, idx) => {
    const tr = document.createElement('tr');
    tr.className = 'border-b border-slate-700/50 hover:bg-slate-800/40 transition-colors';

    const rankingMedal = idx === 0 ? '🥇 ' : idx === 1 ? '🥈 ' : idx === 2 ? '🥉 ' : `#${idx + 1} `;

    tr.innerHTML = `
      <td class="py-3 px-4 font-semibold text-slate-100">
        <span class="text-amber-400 font-mono text-xs mr-2">${rankingMedal}</span>${v.vendedor}
      </td>
      <td class="py-3 px-4 text-center">
        <span class="px-2.5 py-0.5 rounded-full text-xs font-mono bg-blue-500/10 text-blue-400 border border-blue-500/20">
          ${v.quantidadeVendas} vendas
        </span>
      </td>
      <td class="py-3 px-4 text-right font-mono font-medium text-slate-200">${formatarMoeda(v.totalVendas)}</td>
      <td class="py-3 px-4 text-right font-mono font-bold text-emerald-400">${formatarMoeda(v.totalComissao)}</td>
      <td class="py-3 px-4 text-right font-mono text-slate-400">${formatarPercentual(v.taxaEfetiva)}</td>
      <td class="py-3 px-4 text-center">
        <button onclick="abrirModalVendedor('${v.vendedor.replace(/'/g, "\\'")}')" class="px-2.5 py-1 text-xs rounded bg-slate-700 hover:bg-slate-600 text-slate-200 transition-colors">
          Ver Extrato
        </button>
      </td>
    `;
    tbody.appendChild(tr);
  });
}

function adicionarVendaAvulsa() {
  const vendedorInput = document.getElementById('input-novo-vendedor');
  const valorInput = document.getElementById('input-novo-valor');
  const textarea = document.getElementById('comercial-json-input');

  const nome = vendedorInput.value.trim();
  const valor = parseFloat(valorInput.value);

  if (!nome || isNaN(valor) || valor < 0) {
    alert("Informe um nome de vendedor e um valor positivo.");
    return;
  }

  try {
    const parsed = JSON.parse(textarea.value);
    if (!parsed.vendas) parsed.vendas = [];
    parsed.vendas.push({ vendedor: nome, valor });
    textarea.value = JSON.stringify(parsed, null, 2);
    processarPayloadComercial();
    vendedorInput.value = '';
    valorInput.value = '';
  } catch (err) {
    alert("Erro ao atualizar payload: " + err.message);
  }
}

window.abrirModalVendedor = function(nomeVendedor) {
  if (!relatorioComercialAtual) return;
  const vend = relatorioComercialAtual.vendedores.find(v => v.vendedor === nomeVendedor);
  if (!vend) return;

  const modal = document.getElementById('modal-extrato-vendedor');
  document.getElementById('modal-titulo-vendedor').textContent = `Extrato de Vendas - ${vend.vendedor}`;
  document.getElementById('modal-resumo-vendedor').textContent = `Total Vendido: ${formatarMoeda(vend.totalVendas)} | Comissão Acumulada: ${formatarMoeda(vend.totalComissao)} (${vend.quantidadeVendas} vendas)`;

  const tbody = document.getElementById('modal-tabela-vendas-body');
  tbody.innerHTML = '';

  vend.vendas.forEach((item, i) => {
    const badgeColor = item.aliquota === 0.05 
      ? 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20'
      : item.aliquota === 0.01 
        ? 'bg-amber-500/10 text-amber-400 border-amber-500/20' 
        : 'bg-slate-600/20 text-slate-400 border-slate-600/30';

    const tr = document.createElement('tr');
    tr.className = 'border-b border-slate-700/40';
    tr.innerHTML = `
      <td class="py-2 px-3 text-slate-400 font-mono text-xs">#${i + 1}</td>
      <td class="py-2 px-3 font-mono font-medium text-slate-200">${formatarMoeda(item.valor)}</td>
      <td class="py-2 px-3">
        <span class="px-2 py-0.5 rounded text-xs border font-mono ${badgeColor}">
          ${(item.aliquota * 100).toFixed(0)}%
        </span>
      </td>
      <td class="py-2 px-3 text-right font-mono font-bold text-emerald-400">${formatarMoeda(item.comissao)}</td>
    `;
    tbody.appendChild(tr);
  });

  modal.classList.remove('hidden');
};

window.fecharModalVendedor = function() {
  document.getElementById('modal-extrato-vendedor').classList.add('hidden');
};

// ============================================================================
// RENDERIZAÇÃO: MÓDULO DE ESTOQUE
// ============================================================================
function inicializarModuloEstoque() {
  renderizarCatalogoEstoque();
  renderizarHistoricoEstoque();

  document.getElementById('form-movimentacao-estoque')?.addEventListener('submit', (e) => {
    e.preventDefault();
    executarMovimentacaoEstoque();
  });

  document.getElementById('btn-resetar-estoque')?.addEventListener('click', () => {
    motorEstoqueInstance.reinicializar();
    renderizarCatalogoEstoque();
    renderizarHistoricoEstoque();
    atualizarDashboardGeral();
    mostrarFeedbackEstoque("Inventário restaurado com sucesso para os dados iniciais do desafio.", "info");
  });
}

function renderizarCatalogoEstoque() {
  const tbody = document.getElementById('tabela-estoque-body');
  const select = document.getElementById('estoque-select-produto');
  if (!tbody) return;

  tbody.innerHTML = '';
  if (select) select.innerHTML = '';

  const produtos = motorEstoqueInstance.obterCatalogo();

  produtos.forEach(p => {
    // Select option
    if (select) {
      const opt = document.createElement('option');
      opt.value = p.codigoProduto;
      opt.textContent = `${p.codigoProduto} - ${p.descricaoProduto} (Saldo: ${p.estoqueAtual})`;
      select.appendChild(opt);
    }

    // Tabela linha
    const statusBadge = p.estoqueAtual <= 0
      ? '<span class="px-2 py-0.5 rounded text-xs bg-rose-500/10 text-rose-400 border border-rose-500/20">Esgotado</span>'
      : p.estoqueAtual < 100
        ? '<span class="px-2 py-0.5 rounded text-xs bg-amber-500/10 text-amber-400 border border-amber-500/20">Atenção</span>'
        : '<span class="px-2 py-0.5 rounded text-xs bg-emerald-500/10 text-emerald-400 border border-emerald-500/20">Normal</span>';

    const tr = document.createElement('tr');
    tr.className = 'border-b border-slate-700/50 hover:bg-slate-800/40 transition-colors';
    tr.innerHTML = `
      <td class="py-3 px-4 font-mono font-bold text-blue-400">${p.codigoProduto}</td>
      <td class="py-3 px-4 text-slate-100 font-medium">${p.descricaoProduto}</td>
      <td class="py-3 px-4 text-center font-mono text-slate-400">${p.estoqueInicial}</td>
      <td class="py-3 px-4 text-center font-mono font-bold text-slate-100 text-base">${p.estoqueAtual}</td>
      <td class="py-3 px-4 text-center">${statusBadge}</td>
    `;
    tbody.appendChild(tr);
  });
}

function renderizarHistoricoEstoque() {
  const tbody = document.getElementById('tabela-historico-estoque-body');
  if (!tbody) return;

  const historico = motorEstoqueInstance.obterHistorico();
  tbody.innerHTML = '';

  if (historico.length === 0) {
    tbody.innerHTML = `
      <tr>
        <td colspan="7" class="py-6 text-center text-slate-500 italic">
          Nenhuma movimentação realizada ainda nesta sessão. Utilize o formulário acima para efetuar entradas e saídas.
        </td>
      </tr>
    `;
    return;
  }

  historico.forEach(h => {
    const badgeTipo = h.tipo === 'Entrada'
      ? '<span class="px-2 py-0.5 rounded text-xs bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 font-semibold">▲ Entrada</span>'
      : '<span class="px-2 py-0.5 rounded text-xs bg-rose-500/10 text-rose-400 border border-rose-500/20 font-semibold">▼ Saída</span>';

    const horaStr = new Intl.DateTimeFormat('pt-BR', { hour: '2-digit', minute: '2-digit', second: '2-digit' }).format(h.dataHora);

    const tr = document.createElement('tr');
    tr.className = 'border-b border-slate-700/40 hover:bg-slate-800/40 transition-colors text-sm';
    tr.innerHTML = `
      <td class="py-2.5 px-3 font-mono text-xs text-slate-400" title="${h.id}">
        <span class="bg-slate-800 px-1.5 py-0.5 rounded text-cyan-400 border border-slate-700">${h.id.substring(0, 8)}...</span>
      </td>
      <td class="py-2.5 px-3 text-slate-400 font-mono text-xs">${horaStr}</td>
      <td class="py-2.5 px-3 font-medium text-slate-200">${h.codigoProduto} - ${h.descricaoProduto}</td>
      <td class="py-2.5 px-3 text-center">${badgeTipo}</td>
      <td class="py-2.5 px-3 text-center font-mono font-bold text-slate-100">${h.quantidade}</td>
      <td class="py-2.5 px-3 text-center font-mono text-slate-400">${h.saldoAnterior} ➔ <strong class="text-white">${h.saldoAtualizado}</strong></td>
      <td class="py-2.5 px-3 text-slate-300 italic text-xs">${h.motivo}</td>
    `;
    tbody.appendChild(tr);
  });
}

function executarMovimentacaoEstoque() {
  const selectProduto = document.getElementById('estoque-select-produto');
  const selectTipo = document.getElementById('estoque-select-tipo');
  const inputQtd = document.getElementById('estoque-input-quantidade');
  const inputMotivo = document.getElementById('estoque-input-motivo');

  const codigo = selectProduto.value;
  const tipo = selectTipo.value;
  const quantidade = inputQtd.value;
  const motivo = inputMotivo.value;

  try {
    const recibo = motorEstoqueInstance.movimentar(codigo, tipo, quantidade, motivo);
    renderizarCatalogoEstoque();
    renderizarHistoricoEstoque();
    atualizarDashboardGeral();

    mostrarFeedbackEstoque(`Movimentação autorizada! GUID: ${recibo.id}. Saldo atualizado para: ${recibo.saldoAtualizado}.`, "success");
    inputQtd.value = '';
    inputMotivo.value = '';
  } catch (err) {
    mostrarFeedbackEstoque(err.message, "error");
  }
}

function mostrarFeedbackEstoque(mensagem, tipo) {
  const box = document.getElementById('estoque-feedback');
  if (!box) return;

  box.classList.remove('hidden', 'bg-emerald-950/60', 'border-emerald-600', 'text-emerald-300', 'bg-rose-950/60', 'border-rose-600', 'text-rose-300', 'bg-blue-950/60', 'border-blue-600', 'text-blue-300');

  if (tipo === 'success') {
    box.classList.add('bg-emerald-950/60', 'border-emerald-600', 'text-emerald-300');
  } else if (tipo === 'error') {
    box.classList.add('bg-rose-950/60', 'border-rose-600', 'text-rose-300');
  } else {
    box.classList.add('bg-blue-950/60', 'border-blue-600', 'text-blue-300');
  }

  box.textContent = mensagem;
}

// ============================================================================
// RENDERIZAÇÃO: MÓDULO FINANCEIRO
// ============================================================================
function inicializarModuloFinanceiro() {
  const hoje = new Date();
  const hojeStr = hoje.toISOString().split('T')[0];

  const inputValor = document.getElementById('fin-input-valor');
  const inputVenc = document.getElementById('fin-input-vencimento');
  const inputRef = document.getElementById('fin-input-referencia');

  if (inputRef && !inputRef.value) inputRef.value = hojeStr;
  if (inputVenc && !inputVenc.value) {
    // Vencimento de 5 dias atrás por padrão para demonstração instantânea
    const diasAtraso = new Date();
    diasAtraso.setDate(diasAtraso.getDate() - 5);
    inputVenc.value = diasAtraso.toISOString().split('T')[0];
  }

  document.getElementById('form-financeiro')?.addEventListener('input', () => {
    recalcularFinanceiro();
  });

  document.getElementById('form-financeiro')?.addEventListener('submit', (e) => {
    e.preventDefault();
    recalcularFinanceiro();
  });

  configurarBotoesPresetFinanceiro();
  recalcularFinanceiro();
}

function configurarBotoesPresetFinanceiro() {
  const btnPresets = document.querySelectorAll('.btn-fin-preset');
  btnPresets.forEach(btn => {
    btn.addEventListener('click', () => {
      const dias = parseInt(btn.getAttribute('data-dias'), 10);
      const inputRef = document.getElementById('fin-input-referencia');
      const inputVenc = document.getElementById('fin-input-vencimento');

      const dataBase = inputRef.value ? new Date(inputRef.value) : new Date();
      const novaDataVenc = new Date(dataBase);
      novaDataVenc.setDate(novaDataVenc.getDate() - dias);

      inputVenc.value = novaDataVenc.toISOString().split('T')[0];
      recalcularFinanceiro();
    });
  });
}

function recalcularFinanceiro() {
  const valor = document.getElementById('fin-input-valor').value;
  const vencimento = document.getElementById('fin-input-vencimento').value;
  const referencia = document.getElementById('fin-input-referencia').value;
  const erroBox = document.getElementById('fin-erro-box');

  if (!valor || !vencimento || !referencia) return;

  try {
    erroBox.classList.add('hidden');
    const res = MotorFinanceiro.calcularMora(valor, vencimento, referencia);

    document.getElementById('fin-res-valor-original').textContent = formatarMoeda(res.valorOriginal);
    document.getElementById('fin-res-dias-atraso').textContent = `${res.diasAtraso} dias`;
    document.getElementById('fin-res-taxa-diaria').textContent = `${res.taxaDiariaPercentual}% / dia`;
    document.getElementById('fin-res-percentual-total').textContent = `${res.percentualTotalMora.toFixed(1).replace('.', ',')}%`;
    document.getElementById('fin-res-valor-multa').textContent = formatarMoeda(res.valorMulta);
    document.getElementById('fin-res-valor-total').textContent = formatarMoeda(res.valorTotal);

    const badgeStatus = document.getElementById('fin-res-status-badge');
    if (res.possuiAtraso) {
      badgeStatus.className = 'px-3 py-1 rounded-full text-xs font-semibold bg-amber-500/10 text-amber-400 border border-amber-500/20';
      badgeStatus.textContent = `Em Atraso (${res.diasAtraso} dias corridos)`;
    } else {
      badgeStatus.className = 'px-3 py-1 rounded-full text-xs font-semibold bg-emerald-500/10 text-emerald-400 border border-emerald-500/20';
      badgeStatus.textContent = 'Em Dia / Sem Encargos';
    }
  } catch (err) {
    erroBox.textContent = err.message;
    erroBox.classList.remove('hidden');
  }
}
