using Models;
using System;
using System.IO;
using TP_GRAFOS.Algorithms;
using Utils;

string pastaDados = Path.Combine("Data", "GrafosDimacs");
string pastaLogs = "Logs";
LogService logService = new LogService(pastaLogs);

if (args.Length > 0 && args[0] == "--teste-fluxo")
{
    ExecutarTesteFluxoNosGrafos(pastaDados, logService);
    return;
}

ExecutarMenuPrincipal(pastaDados, logService);

static void ExecutarMenuPrincipal(string pastaDados, LogService logService)
{
    bool continuar = true;

    while (continuar)
    {
        Console.WriteLine("=== SORL - Sistema de Otimizacao de Rotas Logisticas ===");
        Console.WriteLine("1 - Menor custo entre dois hubs");
        Console.WriteLine("2 - Fluxo maximo e corte minimo");
        Console.WriteLine("3 - Arvore Geradora Minima");
        Console.WriteLine("4 - Agendamento de manutencoes");
        Console.WriteLine("5 - Percurso de inspecao");
        Console.WriteLine("6 - Testar fluxo maximo nos 7 grafos");
        Console.WriteLine("0 - Sair");
        Console.Write("Escolha uma opcao: ");

        int opcao = LerInteiro();
        Console.WriteLine();

        if (opcao == 0)
        {
            continuar = false;
        }
        else if (opcao == 2)
        {
            ExecutarFluxoMaximo(pastaDados, logService);
        }
        else if (opcao == 6)
        {
            ExecutarTesteFluxoNosGrafos(pastaDados, logService);
        }
        else
        {
            Console.WriteLine("Opcao ainda nao implementada nesta etapa do projeto.");
            Console.WriteLine();
        }
    }
}

static void ExecutarFluxoMaximo(string pastaDados, LogService logService)
{
    string[] arquivos = ObterArquivosDimacs(pastaDados);

    if (arquivos.Length == 0)
    {
        Console.WriteLine("Nenhum arquivo DIMACS foi encontrado em " + pastaDados + ".");
        Console.WriteLine();
        return;
    }

    string caminhoGrafo = SelecionarArquivoGrafo(arquivos);
    Grafo grafo = new Grafo(1, 0);

    if (!LeitorDimacs.Ler(caminhoGrafo, ref grafo))
    {
        Console.WriteLine("Nao foi possivel carregar o grafo.");
        Console.WriteLine();
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Grafo carregado: " + Path.GetFileName(caminhoGrafo));
    Console.WriteLine("Vertices: " + grafo.Vertices.Count);
    Console.WriteLine("Arestas: " + grafo.Arestas.Count);
    Console.WriteLine("Representacao escolhida: " + grafo.RepresentacaoAtual);
    Console.Write("Informe o vertice de origem: ");
    int origem = LerInteiro();
    Console.Write("Informe o vertice de destino: ");
    int destino = LerInteiro();
    Console.WriteLine();

    try
    {
        EdmondsKarp algoritmo = new EdmondsKarp();
        ResultadoFluxo resultado = algoritmo.Calcular(grafo, origem, destino);
        string nomeGrafo = Path.GetFileName(caminhoGrafo);
        string textoResultado = logService.MontarTextoFluxoMaximo(nomeGrafo, grafo, resultado);
        string caminhoLog = logService.RegistrarFluxoMaximo(nomeGrafo, grafo, resultado);

        Console.WriteLine(textoResultado);
        Console.WriteLine("Log salvo em: " + caminhoLog);
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro ao executar fluxo maximo: " + ex.Message);
        Console.WriteLine();
    }
}

static void ExecutarTesteFluxoNosGrafos(string pastaDados, LogService logService)
{
    string[] arquivos = ObterArquivosDimacs(pastaDados);

    if (arquivos.Length == 0)
    {
        Console.WriteLine("Nenhum arquivo DIMACS foi encontrado em " + pastaDados + ".");
        return;
    }

    Console.WriteLine("=== Teste automatico de fluxo maximo ===");
    Console.WriteLine("Origem usada: 1");
    Console.WriteLine("Destino usado: maior id de vertice do grafo");
    Console.WriteLine();

    for (int i = 0; i < arquivos.Length; i++)
    {
        Grafo grafo = new Grafo(1, 0);
        string nomeGrafo = Path.GetFileName(arquivos[i]);

        if (LeitorDimacs.Ler(arquivos[i], ref grafo))
        {
            int origem = 1;
            int destino = ObterMaiorIdVertice(grafo);
            EdmondsKarp algoritmo = new EdmondsKarp();
            ResultadoFluxo resultado = algoritmo.Calcular(grafo, origem, destino);
            string caminhoLog = logService.RegistrarFluxoMaximo(nomeGrafo, grafo, resultado);

            Console.WriteLine(nomeGrafo + " | origem: " + origem + " | destino: " + destino + " | fluxo maximo: " + resultado.FluxoMaximo + " | corte minimo: " + resultado.CorteMinimo.Count + " aresta(s)");
            Console.WriteLine("Log: " + caminhoLog);
        }
        else
        {
            Console.WriteLine(nomeGrafo + " | erro ao carregar grafo.");
        }
    }

    Console.WriteLine();
}

static string[] ObterArquivosDimacs(string pastaDados)
{
    if (!Directory.Exists(pastaDados))
    {
        return new string[0];
    }

    string[] arquivos = Directory.GetFiles(pastaDados, "*.dimacs");
    Array.Sort(arquivos);
    return arquivos;
}

static string SelecionarArquivoGrafo(string[] arquivos)
{
    Console.WriteLine("Grafos disponiveis:");

    for (int i = 0; i < arquivos.Length; i++)
    {
        Console.WriteLine((i + 1) + " - " + Path.GetFileName(arquivos[i]));
    }

    Console.Write("Escolha o grafo: ");
    int escolha = LerInteiro();

    while (escolha < 1 || escolha > arquivos.Length)
    {
        Console.Write("Opcao invalida. Escolha novamente: ");
        escolha = LerInteiro();
    }

    return arquivos[escolha - 1];
}

static int LerInteiro()
{
    string? entrada = Console.ReadLine();
    int valor = 0;
    bool numeroValido = false;

    while (!numeroValido)
    {
        try
        {
            if (entrada == null)
            {
                entrada = "";
            }

            valor = int.Parse(entrada);
            numeroValido = true;
        }
        catch
        {
            Console.Write("Valor invalido. Digite um numero inteiro: ");
            entrada = Console.ReadLine();
        }
    }

    return valor;
}

static int ObterMaiorIdVertice(Grafo grafo)
{
    int maiorId = grafo.Vertices[0].Id;

    for (int i = 1; i < grafo.Vertices.Count; i++)
    {
        if (grafo.Vertices[i].Id > maiorId)
        {
            maiorId = grafo.Vertices[i].Id;
        }
    }

    return maiorId;
}
