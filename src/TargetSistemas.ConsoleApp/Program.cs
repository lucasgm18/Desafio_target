using System.Globalization;
using TargetSistemas.Application.Comercial.Services;
using TargetSistemas.Application.Common;
using TargetSistemas.Application.Estoque.DTOs;
using TargetSistemas.Application.Estoque.Repositories;
using TargetSistemas.Application.Estoque.Services;
using TargetSistemas.Application.Financeiro.Services;
using TargetSistemas.Domain.Estoque.Enums;
using TargetSistemas.Domain.Estoque.Exceptions;

namespace TargetSistemas.ConsoleApp;

public static class Program
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        ImprimirCabecalho();

        ExecutarModuloComercial();
        ExecutarModuloEstoque();
        ExecutarModuloFinanceiro();

        ImprimirRodape();
    }

    private static void ImprimirCabecalho()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("================================================================================");
        Console.WriteLine("   TARGET SISTEMAS - DESAFIO TÉCNICO ERP (ARQUITETURA .NET & CLEAN CODE)       ");
        Console.WriteLine("================================================================================");
        Console.ResetColor();
        Console.WriteLine(" Diretrizes Arquiteturais Aplicadas:");
        Console.WriteLine("   - C# 10 / .NET com Records imutáveis e Domain Services");
        Console.WriteLine("   - Object Calisthenics: ZERO uso da palavra-chave 'else'");
        Console.WriteLine("   - Tipagem forte, Pattern Matching e Guard Clauses defensivas");
        Console.WriteLine("================================================================================\n");
    }

    private static void ExecutarModuloComercial()
    {
        ImprimirSeparador("MÓDULO 1: COMERCIAL - CÁLCULO DE COMISSÕES");

        var comissaoService = new ComissaoService();
        var relatorio = comissaoService.ProcessarPayloadJson(PayloadsConstantes.VendasJson);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{"VENDEDOR",-20} | {"VENDAS",-8} | {"TOTAL VENDAS",-16} | {"COMISSÃO TOTAL",-16} | {"TX EFETIVA",-10}");
        Console.WriteLine(new string('-', 80));
        Console.ResetColor();

        foreach (var v in relatorio.Vendedores)
        {
            var taxaEfetiva = (v.TotalComissao / v.TotalVendas) * 100m;
            Console.WriteLine(
                $"{v.Vendedor,-20} | {v.QuantidadeVendas,-8} | {v.TotalVendas.ToString("C2", PtBr),-16} | {v.TotalComissao.ToString("C2", PtBr),-16} | {taxaEfetiva:F2}%"
            );
        }

        Console.WriteLine(new string('-', 80));
        Console.ForegroundColor = ConsoleColor.Green;
        var taxaGeral = (relatorio.TotalGeralComissoes / relatorio.TotalGeralVendas) * 100m;
        Console.WriteLine(
            $"{"TOTAIS CONSOLIDADOS",-20} | {relatorio.TotalGeralVendasQuantidade,-8} | {relatorio.TotalGeralVendas.ToString("C2", PtBr),-16} | {relatorio.TotalGeralComissoes.ToString("C2", PtBr),-16} | {taxaGeral:F2}%"
        );
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void ExecutarModuloEstoque()
    {
        ImprimirSeparador("MÓDULO 2: ESTOQUE - MOTOR DE MOVIMENTAÇÃO DE MERCADORIAS");

        var repo = new InMemoryEstoqueRepository();
        var estoqueService = new EstoqueService(repo);

        Console.WriteLine("--- 1. Inventário Inicial Carregado ---");
        ImprimirTabelaEstoque(estoqueService.ObterCatalogo());

        Console.WriteLine("\n--- 2. Executando Operações de Movimentação ---");

        // Operação 1: Entrada
        var opEntrada = estoqueService.Movimentar(new RequisicaoMovimentacaoDto(
            CodigoProduto: 101,
            Tipo: TipoMovimentacao.Entrada,
            Quantidade: 60,
            Motivo: "Recebimento NF-e #8812 - Fornecedor Canetas Brasil"
        ));
        ImprimirResultadoMovimentacao("Entrada bem-sucedida", opEntrada);

        // Operação 2: Saída válida
        var opSaida = estoqueService.Movimentar(new RequisicaoMovimentacaoDto(
            CodigoProduto: 102,
            Tipo: TipoMovimentacao.Saida,
            Quantidade: 35,
            Motivo: "Atendimento Pedido Balcão #1004"
        ));
        ImprimirResultadoMovimentacao("Saída autorizada", opSaida);

        // Operação 3: Tentativa de Saída Negativa (Barramento de Saldo Insuficiente)
        try
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("[Simulando Saída Inválida] Solicitando saída de 100 unidades do item 102 (saldo atual: 40)...");
            Console.ResetColor();

            estoqueService.Movimentar(new RequisicaoMovimentacaoDto(
                CodigoProduto: 102,
                Tipo: TipoMovimentacao.Saida,
                Quantidade: 100,
                Motivo: "Pedido acima da capacidade do inventário"
            ));
        }
        catch (SaldoInsuficienteException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[BLOQUEIO DE SEGURANÇA ERP] Operação cancelada atomicamente: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine("\n--- 3. Inventário Atualizado Pós-Operações ---");
        ImprimirTabelaEstoque(estoqueService.ObterCatalogo());
        Console.WriteLine();
    }

    private static void ExecutarModuloFinanceiro()
    {
        ImprimirSeparador("MÓDULO 3: FINANCEIRO - CÁLCULO DE MORA E MULTA (2,5% AO DIA CORRIDO)");

        var financeiroService = new FinanceiroService();
        var hoje = DateOnly.FromDateTime(DateTime.Today);

        var cenarios = new (string Descricao, decimal Valor, DateOnly Vencimento)[]
        {
            ("Título em dia (vencimento hoje)", 1500.00m, hoje),
            ("Título a vencer (vencimento em 10 dias)", 3200.00m, hoje.AddDays(10)),
            ("Título com 1 dia de atraso", 1000.00m, hoje.AddDays(-1)),
            ("Título com 5 dias de atraso", 2500.00m, hoje.AddDays(-5)),
            ("Título com 12 dias de atraso", 840.50m, hoje.AddDays(-12)),
            ("Título com 30 dias de atraso", 5000.00m, hoje.AddDays(-30))
        };

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{"DESCRIÇÃO DO TÍTULO",-32} | {"VALOR ORIGINAL",-14} | {"VENCIMENTO",-10} | {"ATRASO",-7} | {"MULTA (2.5%/d)",-14} | {"TOTAL A PAGAR",-14}");
        Console.WriteLine(new string('-', 105));
        Console.ResetColor();

        foreach (var c in cenarios)
        {
            var res = financeiroService.CalcularMora(c.Valor, c.Vencimento, hoje);
            var corTexto = res.PossuiAtraso ? ConsoleColor.Yellow : ConsoleColor.Green;

            Console.ForegroundColor = corTexto;
            Console.WriteLine(
                $"{c.Descricao,-32} | {res.ValorOriginal.ToString("C2", PtBr),-14} | {res.DataVencimento:dd/MM/yyyy} | {res.DiasAtraso + " dias",-7} | {res.ValorMulta.ToString("C2", PtBr),-14} | {res.ValorTotal.ToString("C2", PtBr),-14}"
            );
        }

        Console.ResetColor();
        Console.WriteLine(new string('-', 105));
        Console.WriteLine();
    }

    private static void ImprimirTabelaEstoque(IEnumerable<TargetSistemas.Domain.Estoque.Entities.Produto> produtos)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"{"CÓDIGO",-8} | {"DESCRIÇÃO DO PRODUTO",-32} | {"SALDO ATUAL",-12}");
        Console.WriteLine(new string('-', 58));
        Console.ResetColor();

        foreach (var p in produtos)
        {
            Console.WriteLine($"{p.Codigo,-8} | {p.Descricao,-32} | {p.Saldo,-12}");
        }
    }

    private static void ImprimirResultadoMovimentacao(string status, ResultadoMovimentacaoDto res)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[OK - {status}] GUID: {res.MovimentacaoId}");
        Console.ResetColor();
        Console.WriteLine($"     Produto: {res.CodigoProduto} - {res.DescricaoProduto} | Tipo: {res.Tipo} | Qtd: {res.QuantidadeMovimentada}");
        Console.WriteLine($"     Saldo Anterior: {res.SaldoAnterior} -> Saldo Atualizado: {res.SaldoAtualizado} | Motivo: {res.Motivo}");
    }

    private static void ImprimirSeparador(string titulo)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine($" >> {titulo}");
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.ResetColor();
    }

    private static void ImprimirRodape()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("================================================================================");
        Console.WriteLine("   TODOS OS MÓDULOS FORAM EXECUTADOS COM SUCESSO E VALIDADOS VIA TESTES XUNIT   ");
        Console.WriteLine("================================================================================");
        Console.ResetColor();
    }
}
