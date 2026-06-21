using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using Models;

namespace Utils
{
    public class LeitorDimacs
    {

        public static bool Ler(string caminhoArquivo, ref Grafo grafo)
        {
            if (!File.Exists(caminhoArquivo))
            {
                Console.WriteLine("Arquivo não encontrado: " + caminhoArquivo);
                return false;
            }

            try
            {
                StreamReader arquivo = new StreamReader(caminhoArquivo, Encoding.UTF8, false);
                int quantidadeVertices;
                int quantidadeArestas;
                string linha = arquivo.ReadLine();
                string[] separadorCabecalho = new string[2];
                separadorCabecalho = linha.Split(" ");
                quantidadeVertices = int.Parse(separadorCabecalho[0]);
                quantidadeArestas = int.Parse(separadorCabecalho[1]);
                grafo = new Grafo(quantidadeVertices, quantidadeArestas);
                string[] separadorLinhas = new string[4];
                linha = arquivo.ReadLine();
                Vertice origem;
                Vertice destino;
                while (linha != null)
                { 
                    separadorLinhas = linha.Split(" ");
                    origem = new Vertice(int.Parse(separadorLinhas[0]), 0, 0);
                    destino = new Vertice(int.Parse(separadorLinhas[1]), 0, 0);
                    Aresta aresta = new Aresta(origem, destino, int.Parse(separadorLinhas[2]), int.Parse(separadorLinhas[3]), 0.0);
                    grafo.AdicionarVertice(origem);
                    grafo.AdicionarVertice(destino);
                    grafo.AdicionarAresta(aresta);
                    linha = arquivo.ReadLine();

                }
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

