using System.Text.RegularExpressions;
using Xunit;

namespace TargetSistemas.UnitTests.Architecture;

public class ObjectCalisthenicsArchTests
{
    [Fact]
    public void CodigoFonteDeProducao_NaoDeveConterPalavraChaveElse()
    {
        // Encontra o diretório raiz 'src' subindo a partir do diretório de execução dos testes
        var baseDir = AppContext.BaseDirectory;
        var directory = new DirectoryInfo(baseDir);

        while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "src")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        var srcPath = Path.Combine(directory.FullName, "src");

        var arquivosCs = Directory.GetFiles(srcPath, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains("obj") && !f.Contains("bin"))
            .ToList();

        Assert.NotEmpty(arquivosCs);

        var elseRegex = new Regex(@"\belse\b", RegexOptions.Multiline);
        var stringLiteralRegex = new Regex(@"""(\\""|[^""])*""");
        var violacoes = new List<string>();

        foreach (var arquivo in arquivosCs)
        {
            var linhas = File.ReadAllLines(arquivo);
            for (var i = 0; i < linhas.Length; i++)
            {
                var linha = linhas[i].Trim();
                // Ignora comentários de linha única ou tags XML
                if (linha.StartsWith("//") || linha.StartsWith("/*") || linha.StartsWith("*"))
                    continue;

                // Remove literais de texto entre aspas para inspecionar apenas instruções de código
                var linhaApenasCodigo = stringLiteralRegex.Replace(linha, string.Empty);

                if (elseRegex.IsMatch(linhaApenasCodigo))
                {
                    violacoes.Add($"{Path.GetFileName(arquivo)} (Linha {i + 1}): {linha}");
                }
            }
        }

        Assert.True(
            violacoes.Count == 0,
            $"Violação da diretriz Object Calisthenics: palavra-chave 'else' detectada em:\n{string.Join("\n", violacoes)}"
        );
    }
}
