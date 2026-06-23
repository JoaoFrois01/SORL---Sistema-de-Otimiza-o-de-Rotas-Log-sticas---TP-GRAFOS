using System;
using System.Collections.Generic;
using Models;

namespace TP_GRAFOS.Algorithms
{
    internal static class Hamiltoniano
    {
        public static bool VerificarHamiltoniano(Grafo grafo)
        {
            if (grafo.Vertices.Count < 3)
            {
                return false;
            }

            int n = grafo.Vertices.Count;

            Dictionary<int, int> grauTotal = new Dictionary<int, int>();

            for (int i = 0; i < grafo.Vertices.Count; i++)
            {
                Vertice v = grafo.Vertices[i];
                grauTotal[v.Id] = v.GrauDeEntrada + v.GrauDeSaida;
            }

            if (SatisfazDirac(grauTotal, n))
            {
                return true;
            }

            if (SatisfazOre(grafo, grauTotal, n))
            {
                return true;
            }

            if (SatisfazBondyChvatal(grafo, grauTotal, n))
            {
                return true;
            }

            if (grafo.Vertices.Count <= 15)
            {
                List<int> cicloEncontrado = BuscaForcaBruta(grafo);
                if (cicloEncontrado != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool SatisfazDirac(Dictionary<int, int> grauTotal, int n)
        {
            foreach (KeyValuePair<int, int> par in grauTotal)
            {
                if (par.Value * 2 < n)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SatisfazOre(Grafo grafo, Dictionary<int, int> grauTotal, int n)
        {
            List<Vertice> vertices = grafo.Vertices;

            for (int i = 0; i < vertices.Count; i++)
            {
                for (int j = i + 1; j < vertices.Count; j++)
                {
                    Vertice u = vertices[i];
                    Vertice v = vertices[j];

                    if (!SaoAdjacentes(grafo, u.Id, v.Id))
                    {
                        if (grauTotal[u.Id] + grauTotal[v.Id] < n)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool SatisfazBondyChvatal(Grafo grafo, Dictionary<int, int> grauTotal, int n)
        {
            List<Vertice> vertices = grafo.Vertices;

            Dictionary<int, int> idParaIndice = new Dictionary<int, int>();

            for (int i = 0; i < vertices.Count; i++)
            {
                idParaIndice[vertices[i].Id] = i;
            }

            bool[,] fecho = new bool[n, n];

            for (int i = 0; i < grafo.Arestas.Count; i++)
            {
                int origem = idParaIndice[grafo.Arestas[i].Origem.Id];
                int destino = idParaIndice[grafo.Arestas[i].Destino.Id];

                fecho[origem, destino] = true;
                fecho[destino, origem] = true;
            }

            int[] grauFecho = new int[n];

            for (int i = 0; i < vertices.Count; i++)
            {
                grauFecho[i] = grauTotal[vertices[i].Id];
            }

            bool adicionou = true;

            while (adicionou)
            {
                adicionou = false;

                for (int i = 0; i < n; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        if (!fecho[i, j] && grauFecho[i] + grauFecho[j] >= n)
                        {
                            fecho[i, j] = true;
                            fecho[j, i] = true;

                            grauFecho[i]++;
                            grauFecho[j]++;

                            adicionou = true;
                        }
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i != j && !fecho[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool SaoAdjacentes(Grafo grafo, int idU, int idV)
        {
            for (int i = 0; i < grafo.Arestas.Count; i++)
            {
                Aresta a = grafo.Arestas[i];

                bool uParaV = a.Origem.Id == idU && a.Destino.Id == idV;
                bool vParaU = a.Origem.Id == idV && a.Destino.Id == idU;

                if (uParaV || vParaU)
                {
                    return true;
                }
            }

            return false;
        }

        private static List<int> BuscaForcaBruta(Grafo grafo)
        {
            if (grafo.Vertices.Count == 0) return null;

            List<int> caminhoAtual = new List<int>();
            HashSet<int> visitados = new HashSet<int>();

            int verticeInicial = grafo.Vertices[0].Id;
            
            caminhoAtual.Add(verticeInicial);
            visitados.Add(verticeInicial);

            if (TentarCaminho(grafo, caminhoAtual, visitados, verticeInicial))
            {
                caminhoAtual.Add(verticeInicial);
                return caminhoAtual;
            }

            return null; 
        }

        private static bool TentarCaminho(Grafo grafo, List<int> caminho, HashSet<int> visitados, int verticeInicial)
        {
            if (caminho.Count == grafo.Vertices.Count)
            {
                int ultimoVertice = caminho[caminho.Count - 1];
                return ExisteAresta(grafo, ultimoVertice, verticeInicial);
            }

            int verticeAtual = caminho[caminho.Count - 1];

            for (int i = 0; i < grafo.Arestas.Count; i++)
            {
                Aresta aresta = grafo.Arestas[i];
                
                if (aresta.Origem.Id == verticeAtual)
                {
                    int proximoVertice = aresta.Destino.Id;

                    if (!visitados.Contains(proximoVertice))
                    {
                        visitados.Add(proximoVertice);
                        caminho.Add(proximoVertice);

                        if (TentarCaminho(grafo, caminho, visitados, verticeInicial))
                        {
                            return true;
                        }

                        visitados.Remove(proximoVertice);
                        caminho.RemoveAt(caminho.Count - 1);
                    }
                }
            }

            return false;
        }

        private static bool ExisteAresta(Grafo grafo, int origem, int destino)
        {
            for (int i = 0; i < grafo.Arestas.Count; i++)
            {
                if (grafo.Arestas[i].Origem.Id == origem && grafo.Arestas[i].Destino.Id == destino)
                {
                    return true;
                }
            }
            return false;
        }

        public static string FormatarResultado(Grafo grafo)
        {
            if (grafo.Vertices.Count < 3)
            {
                return "Grafo com menos de 3 vertices - verificacao nao aplicavel.\n";
            }

            int n = grafo.Vertices.Count;

            Dictionary<int, int> grauTotal = new Dictionary<int, int>();

            for (int i = 0; i < grafo.Vertices.Count; i++)
            {
                Vertice v = grafo.Vertices[i];
                grauTotal[v.Id] = v.GrauDeEntrada + v.GrauDeSaida;
            }

            string resultado = "";

            bool dirac = SatisfazDirac(grauTotal, n);
            resultado += "Teorema de Dirac (1952):          " + TextoSatisfacao(dirac) + "\n";

            if (!dirac)
            {
                bool ore = SatisfazOre(grafo, grauTotal, n);
                resultado += "Teorema de Ore (1961):            " + TextoSatisfacao(ore) + "\n";

                if (!ore)
                {
                    bool bondy = SatisfazBondyChvatal(grafo, grauTotal, n);
                    resultado += "Teorema de Bondy-Chvatal (1976):  " + TextoSatisfacao(bondy) + "\n";

                    if (!bondy)
                    {
                        resultado += "Nenhum teorema foi conclusivo matematicamente.\n";
                    }
                }
            }

            resultado += "\n--- Busca Exaustiva (Algoritmo de Forca Bruta) ---\n";

            if (grafo.Vertices.Count > 15)
            {
                resultado += "AVISO: Grafo muito grande (" + grafo.Vertices.Count + " vertices).\n";
                resultado += "A busca exaustiva pelo caminho exato foi abortada devido a complexidade da classe NP-Completo.\n";
                resultado += "Tentativas em grafos dessa magnitude requerem seculos de processamento (ref: Aula Grafos Hamiltonianos, slide 28).\n";
            }
            else
            {
                List<int> ciclo = BuscaForcaBruta(grafo);

                if (ciclo != null)
                {
                    resultado += "Resultado: CICLO ENCONTRADO!\n";
                    resultado += "Percurso de Hubs: " + FormatarCiclo(ciclo) + "\n";
                }
                else
                {
                    resultado += "Resultado: CICLO NAO ENCONTRADO.\n";
                    resultado += "O algoritmo percorreu todas as possibilidades e provou que o grafo nao e Hamiltoniano.\n";
                }
            }

            return resultado;
        }

        private static string TextoSatisfacao(bool satisfeito)
        {
            if (satisfeito)
            {
                return "SATISFEITO";
            }

            return "Nao satisfeito.";
        }

        private static string FormatarCiclo(List<int> ciclo)
        {
            string texto = "";

            for (int i = 0; i < ciclo.Count; i++)
            {
                if (i > 0)
                {
                    texto += " -> ";
                }

                texto += ciclo[i];
            }

            return texto;
        }
    }
}
