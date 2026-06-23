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

        public static string FormatarResultado(Grafo grafo)
        {
            if (grafo.Vertices.Count < 3)
            {
                return "Grafo com menos de 3 vértices — verificação não aplicável.";
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

            resultado += "Teorema de Dirac (1952):          "
                       + (dirac ? "SATISFEITO — grafo é hamiltoniano." : "Não satisfeito.")
                       + "\n";

            if (!dirac)
            {
                bool ore = SatisfazOre(grafo, grauTotal, n);

                resultado += "Teorema de Ore (1961):            "
                           + (ore ? "SATISFEITO — grafo é hamiltoniano." : "Não satisfeito.")
                           + "\n";

                if (!ore)
                {
                    bool bondy = SatisfazBondyChvatal(grafo, grauTotal, n);

                    resultado += "Teorema de Bondy-Chvátal (1976):  "
                               + (bondy ? "SATISFEITO — grafo é hamiltoniano." : "Não satisfeito.")
                               + "\n";

                    if (!bondy)
                    {
                        resultado += "Nenhum teorema foi conclusivo.\n";
                        resultado += "O grafo pode ou não ser hamiltoniano.\n";
                        resultado += "(Determinação exata requer busca exaustiva — NP-Completo, Karp 1972)";
                    }
                }
            }

            return resultado;
        }
    }
}