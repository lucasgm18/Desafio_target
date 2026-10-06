# Target Sistemas — Solução Técnica ERP Comercial (.NET 10 / C# & Modern Frontend)

Solução arquitetural completa desenvolvida para o Desafio Técnico da **Target Sistemas**, com foco em engenharia de sistemas ERP, boas práticas de Clean Code, Object Calisthenics e interface corporativa pronta para deploy estático.

---

## 📑 Sumário Executivo

- **Backend:** C# (.NET 10 / .NET 8 LTS), Arquitetura em Camadas (Domain-Driven Design simplificado).
- **Frontend:** Dashboard ERP em HTML5, Tailwind CSS e Vanilla JavaScript modular, **100% autônomo** (permite que o recrutador interaja com todos os módulos e simulações sem necessidade de subir um backend local).
- **Testes Automatizados:** 35 testes unitários e arquiteturais desenvolvidos com **xUnit**, cobrindo 100% das regras de negócio e testando conformidade com **Object Calisthenics**.
- **Deploy Pronto:** Configurado para **Vercel**, **GitHub Pages** e **Netlify**.

---

## 🏛️ Diretrizes Arquiteturais & Object Calisthenics

O projeto adota práticas avançadas de desenvolvimento de software corporativo:

### 1. Ausência Total da Palavra-Chave `else` (Zero-Else Policy)
Conforme exigido nas diretrizes de Object Calisthenics, **nenhuma estrutura condicional `else` foi utilizada no código de produção**. 
As tomadas de decisão foram estruturadas através de:
- **Guard Clauses & Early Returns:** Validação defensiva imediata nas primeiras linhas dos métodos.
- **Pattern Matching & Switch Expressions:** Expressões declarativas do C# moderno para faixas de comissão e tipos de operação:
  ```csharp
  // Exemplo no Domínio Comercial (Venda.cs)
  public decimal TaxaComissao => Valor switch
  {
      < 100.00m => 0.00m,
      < 500.00m => 0.01m,
      _ => 0.05m
  };
  ```
- **Teste Arquitetural Automatizado:** O arquivo `ObjectCalisthenicsArchTests.cs` inspeciona via reflexão e análise estática todos os arquivos `.cs` da pasta `src/`, garantindo que nenhuma ocorrência de `else` seja introduzida na solução.

### 2. Imutabilidade e Tipagem Forte com Records
- Utilização de `readonly record struct` e `sealed record` para DTOs, requisições e eventos de movimentação (`Venda`, `MovimentacaoEstoque`, `CalculoMoraResultado`).
- Prevenção contra mutações colaterais de estado e facilidade em cenários com concorrência.

### 3. Encapsulamento & Tell, Don't Ask
- A entidade de domínio `Produto` encapsula as operações `AdicionarSaldo` e `DeduzirSaldo`, impedindo que entidades externas manipulem diretamente o saldo e assegurando atomicidade.

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
Motor transacional com carga inicial de 5 SKUs (Caneta Azul, Caderno Universitário, Borracha Branca, Lápis Preto HB e Marcador de Texto Amarelo).
- Geração obrigatória de identificador único **GUID/UUID** para cada movimentação.
- Registro completo: Tipo (`Entrada` / `Saída`), motivo/descrição e quantidade movimentada.
- **Validação atômica de saldo negativo:** Lança `SaldoInsuficienteException` ao tentar retirar quantidade superior ao saldo, mantendo o inventário íntegro.
- Retorno do saldo atualizado pós-operação e histórico auditável.

---

### 💰 Módulo 3: Financeiro — Cálculo de Mora e Multa
Motor de liquidação financeira com base em data de vencimento e valor do título:
- **Multa diária:** 2,5% ao dia corrido de atraso relativo à data de referência.
- **Títulos em dia ou com vencimento futuro:** Não sofrem nenhum acréscimo (0 dias de atraso, R$ 0,00 de multa).
- Fórmula: $\text{Multa} = \text{Valor Original} \times (\text{Dias de Atraso} \times 0,025)$.

---

## 🗂️ Estrutura do Repositório

```text
desafio_target/
├── TargetSistemas.sln
├── src/
│   ├── TargetSistemas.Domain/             # Regras de negócio, Entidades, Enums, Records e Exceções
│   │   ├── Comercial/
│   │   ├── Estoque/
│   │   └── Financeiro/
│   ├── TargetSistemas.Application/        # Serviços aplicacionais, DTOs, Interfaces e Repositório
│   │   ├── Comercial/
│   │   ├── Estoque/
│   │   └── Financeiro/
│   └── TargetSistemas.ConsoleApp/         # Executável CLI demonstrando os 3 módulos formatados
├── tests/
│   └── TargetSistemas.UnitTests/          # 35 Testes xUnit (Comercial, Estoque, Financeiro, Arquitetura)
│       ├── Comercial/
│       ├── Estoque/
│       ├── Financeiro/
│       └── Architecture/
├── frontend/                              # Aplicação Web ERP pronta para deploy estático
│   ├── index.html                         # Interface SPA com Tailwind CSS
│   ├── app.js                             # Motores de cálculo e gerenciamento de estado
│   └── styles.css                         # Estilização corporativa e efeitos visuais
├── .github/workflows/ci-pages.yml         # CI automatizado (build, xUnit e deploy no GitHub Pages)
├── vercel.json                            # Roteamento automático para a Vercel
├── index.html                             # Cópia raiz para preview direto local ou deploy imediato
├── app.js
└── styles.css
```

---

## 🚀 Como Executar o Backend em C# (.NET)

### Pré-requisitos:
- .NET SDK 8.0 ou superior (testado e compilado em .NET 10.0).

### 1. Compilar a Solução:
```bash
dotnet build
```

### 2. Executar a Suíte de Testes (xUnit):
```bash
dotnet test
```
> **Resultado:** 35 testes executados com êxito em ~100ms.

### 3. Executar o Console Demonstrativo:
```bash
dotnet run --project src/TargetSistemas.ConsoleApp
```

---

## 🌐 Como Executar e Publicar o Frontend

O frontend foi desenvolvido de modo auto-suficiente: não requer que o backend esteja em execução para demonstração das regras de negócio.

### Demonstração Local Imediata:
Basta abrir o arquivo `index.html` (na raiz ou dentro da pasta `frontend/`) diretamente no navegador:
```bash
# Exemplo no Linux
xdg-open index.html

# Exemplo no macOS
open index.html

# Exemplo no Windows
start index.html
```

---

## ☁️ Guia de Publicação / Deploy Online

### Opção 1: Vercel (Recomendada - Deploy em 1 minuto)
1. Crie uma conta ou faça login em [vercel.com](https://vercel.com).
2. Clique em **"Add New Project"** e conecte seu repositório do GitHub.
3. Como o arquivo `vercel.json` e os arquivos de entrada já estão configurados, **nenhuma configuração adicional é necessária**.
4. Clique em **"Deploy"**. A URL gerada estará pronta para o recrutador.

*(Alternativa via Vercel CLI no terminal)*:
```bash
npm i -g vercel
vercel
```

---

### Opção 2: GitHub Pages (Automatizado via GitHub Actions)
O repositório já inclui o arquivo de workflow `.github/workflows/ci-pages.yml`. Para ativar:
1. No seu repositório no GitHub, acesse **Settings** ➔ **Pages**.
2. Em **Build and deployment** ➔ **Source**, selecione **GitHub Actions**.
3. Faça um `git push` para a branch `main`. O GitHub rodará os testes xUnit e publicará a página automaticamente em:
   `https://<seu-usuario>.github.io/<nome-do-repositorio>/`

---

### Opção 3: Netlify (Deploy com Drag & Drop)
1. Acesse [app.netlify.com/drop](https://app.netlify.com/drop).
2. Arraste a pasta `frontend` para a janela do navegador.
3. Em menos de 10 segundos o link público será disponibilizado.

---

## 👨‍💻 Autor

Solução desenvolvida por **Lucas**, Engenheiro de Software Sênior.
Contato e referências disponíveis no perfil do GitHub.
