using System;
using System.Collections.Generic;
using Models;

namespace TP_GRAFOS.Algorithms
{
    internal static class Euleriano
    {
        public static bool VerificarEuleriano(Grafo grafo)
        {
            if (grafo.Vertices.Count == 0 || grafo.Arestas.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < grafo.Vertices.Count; i++)
            {
                Vertice v = grafo.Vertices[i];

                if (v.GrauDeEntrada != v.GrauDeSaida)
                {
                    return false;
                }
            }

            if (!GrafoEConexo(grafo))
            {
                return false;
            }

            return true;
        }

        private static bool GrafoEConexo(Grafo grafo)
        {
            Dictionary<int, List<int>> adjacencia = new Dictionary<int, List<int>>();

            for (int i = 0; i < grafo.Vertices.Count; i++)
            {
                adjacencia[grafo.Vertices[i].Id] = new List<int>();
            }

            for (int i = 0; i < grafo.Arestas.Count; i++)
            {
                Aresta a = grafo.Arestas[i];
                adjacencia[a.Origem.Id].Add(a.Destino.Id);
                adjacencia[a.Destino.Id].Add(a.Origem.Id);
            }

            int verticeInicial = -1;

            for (int i = 0; i < grafo.Vertices.Count; i++)
            {
                Vertice v = grafo.Vertices[i];

                if (v.GrauDeSaida > 0 || v.GrauDeEntrada > 0)
                {
                    verticeInicial = v.Id;
                    break;
                }
            }

            if (verticeInicial == -1)
            {
                return true;
            }

            HashSet<int> visitados = new HashSet<int>();
            Queue<int> fila = new Queue<int>();

            fila.Enqueue(verticeInicial);
            visitados.Add(verticeInicial);

            while (fila.Count > 0)
            {
                int atual = fila.Dequeue();

                for (int i = 0; i < adjacencia[atual].Count; i++)
                {
                    int vizinho = adjacencia[atual][i];

                    if (!visitados.Contains(vizinho))
                    {
                        visitados.Add(vizinho);
                        fila.Enqueue(vizinho);
                    }
                }
            }

            for (int i = 0; i < grafo.Vertices.Count; i++)
            {
                Vertice v = grafo.Vertices[i];
                bool temGrau = v.GrauDeEntrada > 0 || v.GrauDeSaida > 0;

                if (temGrau && !visitados.Contains(v.Id))
                {
                    return false;
                }
            }

            return true;
        }

        public static List<int> ConstruirCircuito(Grafo grafo)
        {
            if (!VerificarEuleriano(grafo))
            {
                throw new Exception("O grafo nao possui circuito euleriano.");
            }

            Dictionary<int, List<int>> gAux = new Dictionary<int, List<int>>();

            for (int i = 0; i < grafo.Vertices.Count; i++)
            {
                gAux[grafo.Vertices[i].Id] = new List<int>();
            }

            for (int i = 0; i < grafo.Arestas.Count; i++)
            {
                gAux[grafo.Arestas[i].Origem.Id].Add(grafo.Arestas[i].Destino.Id);
            }

            int verticeAtual = -1;

            for (int i = 0; i < grafo.Vertices.Count; i++)
            {
                if (gAux[grafo.Vertices[i].Id].Count > 0)
                {
                    verticeAtual = grafo.Vertices[i].Id;
                    break;
                }
            }

            List<int> circuito = new List<int>();
            circuito.Add(verticeAtual);

            while (ContarArestas(gAux) > 0)
            {
                List<int> vizinhos = gAux[verticeAtual];

                int proximo;

                if (vizinhos.Count == 1)
                {
                    proximo = vizinhos[0];
                }
                else
                {
                    proximo = EscolherArestaNaoPonte(verticeAtual, gAux);
                }

                gAux[verticeAtual].Remove(proximo);
                verticeAtual = proximo;
                circuito.Add(verticeAtual);
            }

            return circuito;
        }

        private static int EscolherArestaNaoPonte(int vertice, Dictionary<int, List<int>> gAux)
        {
            List<int> candidatos = new List<int>(gAux[vertice]);

            for (int i = 0; i < candidatos.Count; i++)
            {
                int candidato = candidatos[i];

                gAux[vertice].Remove(candidato);

                bool ehPonte = ContarArestas(gAux) > 0
                               && !TodasArestasAlcancaveis(candidato, gAux);

                gAux[vertice].Add(candidato);

                if (!ehPonte)
                {
                    return candidato;
                }
            }

            return gAux[vertice][0];
        }

        private static bool TodasArestasAlcancaveis(int inicio, Dictionary<int, List<int>> gAux)
        {
            HashSet<int> visitados = new HashSet<int>();
            Queue<int> fila = new Queue<int>();

            fila.Enqueue(inicio);
            visitados.Add(inicio);

            while (fila.Count > 0)
            {
                int atual = fila.Dequeue();

                for (int i = 0; i < gAux[atual].Count; i++)
                {
                    int proximo = gAux[atual][i];

                    if (!visitados.Contains(proximo))
                    {
                        visitados.Add(proximo);
                        fila.Enqueue(proximo);
                    }
                }
            }

            foreach (KeyValuePair<int, List<int>> par in gAux)
            {
                if (par.Value.Count > 0 && !visitados.Contains(par.Key))
                {
                    return false;
                }
            }

            return true;
        }

        private static int ContarArestas(Dictionary<int, List<int>> gAux)
        {
            int total = 0;

            foreach (List<int> lista in gAux.Values)
            {
                total += lista.Count;
            }

            return total;
        }

        public static string FormatarCircuito(List<int> circuito)
        {
            string texto = "";

            for (int i = 0; i < circuito.Count; i++)
            {
                if (i > 0)
                {
                    texto += " -> ";
                }

                texto += circuito[i];
            }

            return texto;
        }
    }
}
