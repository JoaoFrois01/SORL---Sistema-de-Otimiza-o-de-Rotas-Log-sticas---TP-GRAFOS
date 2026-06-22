using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TP_GRAFOS.Algorithms;

namespace Utils
{
    public class LogService
    {
        private string pastaLogs;

        public LogService(string pastaLogs)
        {
            this.pastaLogs = pastaLogs;

            if (!Directory.Exists(this.pastaLogs))
            {
                Directory.CreateDirectory(this.pastaLogs);
            }
        }

        public string RegistrarFluxoMaximo(string nomeGrafo, Grafo grafo, ResultadoFluxo resultado)
        {
            string nomeArquivo = "fluxo_maximo_" + LimparNomeArquivo(nomeGrafo) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            string caminhoArquivo = Path.Combine(pastaLogs, nomeArquivo);
            string conteudo = MontarTextoFluxoMaximo(nomeGrafo, grafo, resultado);

            StreamWriter escritor = new StreamWriter(caminhoArquivo, false, Encoding.UTF8);
            escritor.Write(conteudo);
            escritor.Close();

            return caminhoArquivo;
        }

        public string MontarTextoFluxoMaximo(string nomeGrafo, Grafo grafo, ResultadoFluxo resultado)
        {
            StringBuilder texto = new StringBuilder();

            texto.AppendLine("=== Fluxo Maximo / Corte Minimo ===");
            texto.AppendLine("Grafo: " + nomeGrafo);
            texto.AppendLine("Algoritmo executado: Edmonds-Karp");
            texto.AppendLine("Origem: " + resultado.Origem);
            texto.AppendLine("Destino: " + resultado.Destino);
            texto.AppendLine("Resultado: fluxo maximo = " + resultado.FluxoMaximo);
            texto.AppendLine();

            texto.AppendLine("Caminhos aumentantes encontrados:");

            if (resultado.CaminhosAumentantes.Count == 0)
            {
                texto.AppendLine("Nenhum caminho aumentante foi encontrado.");
            }
            else
            {
                for (int i = 0; i < resultado.CaminhosAumentantes.Count; i++)
                {
                    CaminhoFluxo caminho = resultado.CaminhosAumentantes[i];
                    texto.AppendLine((i + 1) + ". " + caminho.FormatarCaminho() + " | fluxo enviado: " + caminho.FluxoEnviado);
                }
            }

            texto.AppendLine();
            texto.AppendLine("Arestas usadas com fluxo positivo:");
            List<Aresta> arestasComFluxo = ObterArestasComFluxo(grafo);

            if (arestasComFluxo.Count == 0)
            {
                texto.AppendLine("Nenhuma aresta recebeu fluxo.");
            }
            else
            {
                for (int i = 0; i < arestasComFluxo.Count; i++)
                {
                    Aresta aresta = arestasComFluxo[i];
                    texto.AppendLine(aresta.Origem.Id + " -> " + aresta.Destino.Id + " | fluxo: " + aresta.FluxoAtual + " / capacidade: " + aresta.Capacidade);
                }
            }

            texto.AppendLine();
            texto.AppendLine("Corte minimo:");

            if (resultado.CorteMinimo.Count == 0)
            {
                texto.AppendLine("Nenhuma aresta encontrada no corte minimo.");
            }
            else
            {
                for (int i = 0; i < resultado.CorteMinimo.Count; i++)
                {
                    Aresta aresta = resultado.CorteMinimo[i];
                    texto.AppendLine(aresta.Origem.Id + " -> " + aresta.Destino.Id + " | capacidade: " + aresta.Capacidade + " | fluxo atual: " + aresta.FluxoAtual);
                }
            }

            texto.AppendLine();
            texto.AppendLine("Interpretacao logistica:");
            texto.AppendLine("O fluxo maximo representa a maior quantidade de carga que pode sair da origem e chegar ao destino respeitando a capacidade diaria de cada rota.");
            texto.AppendLine("O corte minimo mostra as rotas gargalo: se elas forem ampliadas, a rede pode ganhar mais capacidade entre os dois pontos analisados.");

            return texto.ToString();
        }

        private List<Aresta> ObterArestasComFluxo(Grafo grafo)
        {
            List<Aresta> arestasComFluxo = new List<Aresta>();

            for (int i = 0; i < grafo.Arestas.Count; i++)
            {
                if (grafo.Arestas[i].FluxoAtual > 0)
                {
                    arestasComFluxo.Add(grafo.Arestas[i]);
                }
            }

            return arestasComFluxo;
        }

        private string LimparNomeArquivo(string nome)
        {
            string limpo = "";

            for (int i = 0; i < nome.Length; i++)
            {
                char caractere = nome[i];

                if ((caractere >= 'a' && caractere <= 'z') || (caractere >= 'A' && caractere <= 'Z') || (caractere >= '0' && caractere <= '9') || caractere == '_' || caractere == '-')
                {
                    limpo = limpo + caractere;
                }
            }

            if (limpo.Length == 0)
            {
                limpo = "grafo";
            }

            return limpo;
        }
        public string RegistrarColoracao(string nomeGrafo, Grafo grafo, Dictionary<int, List<string>> turnos, int totalTurnos)
        {
            string nomeArquivo = "coloracao_" + LimparNomeArquivo(nomeGrafo) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            string caminhoArquivo = Path.Combine(pastaLogs, nomeArquivo);
            string conteudo = MontarTextoColoracao(nomeGrafo, grafo, turnos, totalTurnos);

            StreamWriter escritor = new StreamWriter(caminhoArquivo, false, Encoding.UTF8);
            escritor.Write(conteudo);
            escritor.Close();

            return caminhoArquivo;
        }

        public string MontarTextoColoracao(string nomeGrafo, Grafo grafo, Dictionary<int, List<string>> turnos, int totalTurnos)
        {
            StringBuilder texto = new StringBuilder();

            texto.AppendLine("=== Agendamento de Manutencoes (Coloracao de Grafos) ===");
            texto.AppendLine("Grafo: " + nomeGrafo);
            texto.AppendLine("Algoritmo executado: Coloracao Gulosa (Welsh-Powell)");
            texto.AppendLine("Criterio de conflito: rotas que compartilham um hub em comum");
            texto.AppendLine("Total de rotas: " + grafo.Arestas.Count);
            texto.AppendLine("Numero minimo de turnos necessarios: " + totalTurnos);
            texto.AppendLine();

            List<int> turnosOrdenados = new List<int>(turnos.Keys);
            turnosOrdenados.Sort();

            foreach (int turno in turnosOrdenados)
            {
                texto.AppendLine("Turno " + turno + " (" + turnos[turno].Count + " rota(s)):");

                foreach (string rota in turnos[turno])
                {
                    texto.AppendLine("  Rota: " + rota);
                }

                texto.AppendLine();
            }

            texto.AppendLine("Interpretacao logistica:");
            texto.AppendLine("Rotas no mesmo turno nao compartilham hubs, portanto podem ser inspecionadas/mantidas simultaneamente sem conflito de recursos (patios, oficinas, equipamentos).");

            return texto.ToString();
        }

        public string RegistrarInspecao(string nomeGrafo, Grafo grafo, bool euleriano, List<int>? circuitoEuleriano, bool hamiltoniano, List<int>? cicloHamiltoniano)
        {
            string nomeArquivo = "inspecao_" + LimparNomeArquivo(nomeGrafo) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            string caminhoArquivo = Path.Combine(pastaLogs, nomeArquivo);
            string conteudo = MontarTextoInspecao(nomeGrafo, grafo, euleriano, circuitoEuleriano, hamiltoniano, cicloHamiltoniano);

            StreamWriter escritor = new StreamWriter(caminhoArquivo, false, Encoding.UTF8);
            escritor.Write(conteudo);
            escritor.Close();

            return caminhoArquivo;
        }

        public string MontarTextoInspecao(string nomeGrafo, Grafo grafo, bool euleriano, List<int>? circuitoEuleriano, bool hamiltoniano, List<int>? cicloHamiltoniano)
        {
            StringBuilder texto = new StringBuilder();

            texto.AppendLine("=== Rota Unica de Inspecao ===");
            texto.AppendLine("Grafo: " + nomeGrafo);
            texto.AppendLine("Vertices: " + grafo.Vertices.Count);
            texto.AppendLine("Arestas: " + grafo.Arestas.Count);
            texto.AppendLine();

            texto.AppendLine("-- Cenario A: Percurso de Rotas (Circuito Euleriano) --");
            texto.AppendLine("Algoritmo executado: verificacao de graus + Hierholzer");
            texto.AppendLine("Condicao: grau de entrada == grau de saida para todo vertice e grafo fracamente conexo");

            if (euleriano && circuitoEuleriano != null)
            {
                texto.AppendLine("Resultado: EXISTE circuito euleriano");
                texto.AppendLine("Percurso: " + Euleriano.FormatarCircuito(circuitoEuleriano));
            }
            else
            {
                texto.AppendLine("Resultado: NAO existe circuito euleriano");
                texto.AppendLine("Motivo: algum vertice tem grau de entrada diferente do grau de saida, ou o grafo nao e conexo.");
            }

            texto.AppendLine();
            texto.AppendLine("-- Cenario B: Percurso de Hubs (Ciclo Hamiltoniano) --");
            texto.AppendLine("Algoritmo executado: Backtracking com poda");
            texto.AppendLine("Vertice inicial: " + (grafo.Vertices.Count > 0 ? grafo.Vertices[0].Id.ToString() : "N/A"));

            if (hamiltoniano && cicloHamiltoniano != null)
            {
                texto.AppendLine("Resultado: EXISTE ciclo hamiltoniano");
                texto.AppendLine("Percurso: " + Hamiltoniano.FormatarCiclo(cicloHamiltoniano));
            }
            else
            {
                texto.AppendLine("Resultado: NAO existe ciclo hamiltoniano");
                texto.AppendLine("Motivo: nenhum caminho que visita todos os hubs exatamente uma vez e retorna ao inicio foi encontrado.");
            }

            texto.AppendLine();
            texto.AppendLine("Interpretacao logistica:");
            texto.AppendLine("Circuito Euleriano: o inspetor consegue percorrer TODAS as estradas exatamente uma vez e voltar ao ponto de partida — rota ideal para inspecao de vias.");
            texto.AppendLine("Ciclo Hamiltoniano: o inspetor consegue visitar TODOS os hubs exatamente uma vez e voltar ao inicio — rota ideal para inspecao de centros de distribuicao.");

            return texto.ToString();
        }
    }

}
