using Models;
using System;
using System.IO;
using System.Text;

namespace Utils
{
    public class LeitorDimacs
    {
        public static bool Ler(string caminhoArquivo, ref Grafo grafo)
        {
            if (!File.Exists(caminhoArquivo))
            {
                Console.WriteLine("Arquivo nao encontrado: " + caminhoArquivo);
                return false;
            }

            try
            {
                StreamReader arquivo = new StreamReader(caminhoArquivo, Encoding.UTF8, false);
                string linhaCabecalho = arquivo.ReadLine();

                if (linhaCabecalho == null)
                {
                    Console.WriteLine("Arquivo DIMACS vazio.");
                    arquivo.Close();
                    return false;
                }

                string[] separadorCabecalho = linhaCabecalho.Split(' ');
                int quantidadeVertices = int.Parse(separadorCabecalho[0]);
                int quantidadeArestas = int.Parse(separadorCabecalho[1]);

                grafo = new Grafo(quantidadeVertices, quantidadeArestas);

                for (int i = 1; i <= quantidadeVertices; i++)
                {
                    Vertice vertice = new Vertice(i, 0, 0);
                    grafo.AdicionarVertice(vertice);
                }

                string linha = arquivo.ReadLine();

                while (linha != null)
                {
                    if (linha.Trim().Length > 0)
                    {
                        string[] separadorLinhas = linha.Split(' ');

                        if (separadorLinhas.Length >= 4)
                        {
                            int idOrigem = int.Parse(separadorLinhas[0]);
                            int idDestino = int.Parse(separadorLinhas[1]);
                            int peso = int.Parse(separadorLinhas[2]);
                            int capacidade = int.Parse(separadorLinhas[3]);

                            Vertice origem = grafo.ObterVerticePorId(idOrigem);
                            Vertice destino = grafo.ObterVerticePorId(idDestino);

                            if (origem != null && destino != null)
                            {
                                Aresta aresta = new Aresta(origem, destino, peso, capacidade, 0.0);
                                grafo.AdicionarAresta(aresta);
                            }
                        }
                    }

                    linha = arquivo.ReadLine();
                }

                arquivo.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao ler o arquivo: " + ex.Message);
                return false;
            }
        }
    }
}
