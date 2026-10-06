# Target Sistemas — Solução Técnica Desafio ERP (.NET / C# & Frontend)

Solução desenvolvida para o Desafio Técnico da **Target Sistemas**, contemplando a lógica de negócio dos três módulos propostos, boas práticas de Clean Code, Object Calisthenics e uma interface interativa para visualização e testes dos dados.

---

## 📑 Sumário

- **Backend:** C# (.NET 10 / .NET 8), organizado em camadas limpas (Domain, Application, ConsoleApp e UnitTests).
- **Frontend:** Interface em HTML5, Tailwind CSS e JavaScript modular, **100% autônoma** (permite testar e simular as regras de negócio diretamente no navegador).
- **Testes Automatizados:** 35 testes unitários e arquiteturais desenvolvidos com **xUnit**, cobrindo todas as regras de negócio e validando as restrições de código.

---

## 🏛️ Diretrizes de Código & Boas Práticas

O projeto foi construído seguindo rigorosamente os princípios solicitados no desafio:

### 1. Ausência da Palavra-Chave `else` (Object Calisthenics)
Nenhuma estrutura condicional `else` foi utilizada no código de produção. As tomadas de decisão foram estruturadas através de:
- **Guard Clauses & Early Returns:** Validação defensiva imediata nas primeiras linhas dos métodos.
- **Pattern Matching & Switch Expressions:** Expressões declarativas do C# para faixas de comissão e tipos de operação:
  ```csharp
  // Exemplo no Domínio Comercial (Venda.cs)
  public decimal TaxaComissao => Valor switch
  {
      < 100.00m => 0.00m,
      < 500.00m => 0.01m,
      _ => 0.05m
  };
  ```
- **Teste Arquitetural Automatizado:** O arquivo `ObjectCalisthenicsArchTests.cs` inspeciona todos os arquivos `.cs` da pasta `src/`, garantindo que nenhuma ocorrência da palavra-chave `else` exista no código.

### 2. Imutabilidade e Tipagem Forte com Records
- Utilização de `readonly record struct` e `sealed record` para DTOs, requisições e eventos (`Venda`, `MovimentacaoEstoque`, `CalculoMoraResultado`).
- Prevenção contra alterações indevidas de estado e código mais conciso.

### 3. Encapsulamento de Regras
- A entidade de domínio `Produto` encapsula os métodos `AdicionarSaldo` e `DeduzirSaldo`, garantindo que o saldo não possa ser alterado externamente de forma inconsistente.

---

## 📦 Detalhamento dos 3 Módulos do Desafio

### 💼 Módulo 1: Comercial — Cálculo de Comissões
Consolidação de vendas por vendedor e aplicação individual das alíquotas:
- **Vendas < R$ 100,00:** 0% de comissão
- **Vendas entre R$ 100,00 e R$ 499,99:** 1% de comissão
- **Vendas >= R$ 500,00:** 5% de comissão

#### Resultados Consolidados do Payload Oficial:
| Vendedor | Quantidade de Vendas | Total Faturado | Comissão Consolidada | Taxa Efetiva |
| :--- | :---: | :---: | :---: | :---: |
| **João Silva** | 10 | R$ 10.754,70 | **R$ 495,69** | 4,61% |
| **Maria Souza** | 9 | R$ 9.874,30 | **R$ 465,96** | 4,72% |
| **Ana Lima** | 9 | R$ 8.763,95 | **R$ 404,99** | 4,62% |
| **Carlos Oliveira** | 8 | R$ 7.928,35 | **R$ 379,38** | 4,79% |
| **TOTAIS GERAIS** | **36** | **R$ 37.321,30** | **R$ 1.746,02** | **4,68%** |

---

### 📦 Módulo 2: Estoque — Motor de Movimentação de Mercadorias
Motor transacional com carga inicial dos 5 produtos informados no enunciado (Caneta Azul, Caderno Universitário, Borracha Branca, Lápis Preto HB e Marcador de Texto Amarelo).
- Geração de identificador único **GUID** para cada movimentação realizada.
- Registro completo: Tipo (`Entrada` / `Saída`), motivo/descrição e quantidade movimentada.
- **Validação de saldo negativo:** Lança `SaldoInsuficienteException` ao tentar retirar quantidade superior ao saldo, mantendo o inventário íntegro.
- Retorno do saldo atualizado pós-operação e histórico de movimentações.

---

### 💰 Módulo 3: Financeiro — Cálculo de Mora e Multa
Cálculo de juros por atraso a partir de um valor e data de vencimento:
- **Multa diária:** 2,5% ao dia corrido de atraso em relação à data de referência.
- **Títulos em dia ou com vencimento futuro:** Não sofrem nenhum acréscimo (0 dias de atraso, R$ 0,00 de multa).
- Fórmula: $\text{Multa} = \text{Valor Original} \times (\text{Dias de Atraso} \times 0,025)$.

---

## 🗂️ Estrutura do Repositório

```text
desafio_target/
├── TargetSistemas.sln                     # Arquivo de solução
├── src/
│   ├── TargetSistemas.Domain/             # Entidades, Enums, Records e Exceções
│   │   ├── Comercial/
│   │   ├── Estoque/
│   │   └── Financeiro/
│   ├── TargetSistemas.Application/        # Serviços, DTOs, Interfaces e Repositório
│   │   ├── Comercial/
│   │   ├── Estoque/
│   │   └── Financeiro/
│   └── TargetSistemas.ConsoleApp/         # Aplicação console demonstrando os 3 módulos
├── tests/
│   └── TargetSistemas.UnitTests/          # 35 Testes unitários com xUnit
│       ├── Comercial/
│       ├── Estoque/
│       ├── Financeiro/
│       └── Architecture/
├── frontend/                              # Interface web para demonstração
│   ├── index.html                         # Dashboard interativo com abas
│   ├── app.js                             # Lógica em JavaScript
│   └── styles.css                         # Estilos visuais
├── index.html                             # Acesso rápido na raiz
├── app.js
└── styles.css
```

---

## 🚀 Como Executar o Backend em C# (.NET)

### Pré-requisitos:
- .NET SDK instalado (.NET 8 ou superior).

### 1. Compilar a Solução:
```bash
dotnet build
```

### 2. Executar os Testes Unitários (xUnit):
```bash
dotnet test
```
> **Resultado:** 35 testes executados com 100% de aprovação.

### 3. Executar o Console Demonstrativo:
```bash
dotnet run --project src/TargetSistemas.ConsoleApp
```

---

## 🌐 Como Executar o Frontend (Demonstração Visual)

O frontend foi desenvolvido para funcionar diretamente no navegador, sem precisar compilar ou rodar nenhum servidor backend local.

Basta abrir o arquivo `index.html` (na raiz do projeto ou dentro da pasta `frontend/`) em qualquer navegador:
```bash
# No Linux
xdg-open index.html

# No macOS
open index.html

# No Windows
start index.html
```
Ou simplesmente dar um duplo clique no arquivo `index.html` pelo gerenciador de arquivos.
