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
    }
}
