using System;
using System.Collections.Generic;
using System.Linq;
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

            if (!TodosConectados(grafo))
            {
                return false;
            }

            return true;
        }

        private static bool TodosConectados(Grafo grafo)
        {
            Vertice? inicio = grafo.Vertices.FirstOrDefault(v => v.GrauDeSaida > 0 || v.GrauDeEntrada > 0);

            if (inicio == null)
            {
                return true;
            }
            
            Dictionary<int, HashSet<int>> adjacenciaNaoDirecionada = new Dictionary<int, HashSet<int>>();

            foreach (Vertice v in grafo.Vertices)
            {
                adjacenciaNaoDirecionada[v.Id] = new HashSet<int>();
            }

            foreach (Aresta a in grafo.Arestas)
            {
                adjacenciaNaoDirecionada[a.Origem.Id].Add(a.Destino.Id);
                adjacenciaNaoDirecionada[a.Destino.Id].Add(a.Origem.Id);
            }

            HashSet<int> visitados = new HashSet<int>();
            Queue<int> fila = new Queue<int>();
            fila.Enqueue(inicio.Id);
            visitados.Add(inicio.Id);

            while (fila.Count > 0)
            {
                int atual = fila.Dequeue();

                foreach (int vizinho in adjacenciaNaoDirecionada[atual])
                {
                    if (!visitados.Contains(vizinho))
                    {
                        visitados.Add(vizinho);
                        fila.Enqueue(vizinho);
                    }
                }
            }

            foreach (Vertice v in grafo.Vertices)
            {
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

            Dictionary<int, List<int>> arestasDisponiveis = new Dictionary<int, List<int>>();

            foreach (Vertice v in grafo.Vertices)
            {
                arestasDisponiveis[v.Id] = new List<int>();
            }

            foreach (Aresta a in grafo.Arestas)
            {
                arestasDisponiveis[a.Origem.Id].Add(a.Destino.Id);
            }

            int verticeInicial = grafo.Arestas[0].Origem.Id;

            Stack<int> pilha = new Stack<int>();
            List<int> circuito = new List<int>();
            pilha.Push(verticeInicial);

            while (pilha.Count > 0)
            {
                int atual = pilha.Peek();

                if (arestasDisponiveis[atual].Count > 0)
                {
                    int proximo = arestasDisponiveis[atual][0];
                    arestasDisponiveis[atual].RemoveAt(0);
                    pilha.Push(proximo);
                }
                else
                {
                    circuito.Add(pilha.Pop());
                }
            }

            circuito.Reverse();
            return circuito;
        }
        public static string FormatarCircuito(List<int> circuito)
        {
            return string.Join(" -> ", circuito);
        }
    }
}
