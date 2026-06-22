using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace TP_GRAFOS.Algorithms
{
    // Resolve o Cenário A do problema V: "É possível que o inspetor percorra
    // todas as rotas (arestas) exatamente uma vez e retorne ao ponto de partida?"
    //
    // Isso é o problema do CIRCUITO EULERIANO em um grafo DIRECIONADO.
    //
    // Condição (grafo direcionado):
    //   1) Para todo vértice v: GrauDeEntrada(v) == GrauDeSaida(v)
    //   2) Todos os vértices que possuem pelo menos uma aresta pertencem
    //      a uma única componente conexa (considerando o grafo "como se
    //      fosse não-direcionado", para fins de conectividade).
    //
    // OBS: a condição rigorosa é "fortemente conexo restrito ao subgrafo
    // de vértices com grau > 0". A verificação de conectividade abaixo usa
    // uma versão simplificada (conectividade fraca) — é a forma normalmente
    // aceita em disciplinas de graduação, mas vale confirmar com o professor
    // se ele exige a verificação de forte conectividade.
    internal static class Euleriano
    {
        // ----------------------------------------------------------------
        // 1) VERIFICAÇÃO DE EXISTÊNCIA
        // ----------------------------------------------------------------
        public static bool VerificarEuleriano(Grafo grafo)
        {
            if (grafo.Vertices.Count == 0 || grafo.Arestas.Count == 0)
            {
                return false;
            }

            // Condição 1: grau de entrada == grau de saída para todo vértice
            for (int i = 0; i < grafo.Vertices.Count; i++)
            {
                Vertice v = grafo.Vertices[i];

                if (v.GrauDeEntrada != v.GrauDeSaida)
                {
                    return false;
                }
            }

            // Condição 2: conectividade (considerando arestas como não-direcionadas)
            if (!TodosConectados(grafo))
            {
                return false;
            }

            return true;
        }

        private static bool TodosConectados(Grafo grafo)
        {
            // Pega o primeiro vértice que tem pelo menos uma aresta (entrada ou saída)
            Vertice? inicio = grafo.Vertices.FirstOrDefault(v => v.GrauDeSaida > 0 || v.GrauDeEntrada > 0);

            if (inicio == null)
            {
                return true; // nenhum vértice com aresta -> trivialmente conexo
            }

            // Monta um "mapa de adjacência não-direcionada" só para o teste de conectividade
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

            // BFS a partir de "inicio"
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

            // Todo vértice que tem grau > 0 precisa ter sido visitado
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

        // ----------------------------------------------------------------
        // 2) CONSTRUÇÃO DO CIRCUITO (Algoritmo de Hierholzer)
        // ----------------------------------------------------------------
        // Retorna a sequência de vértices do circuito euleriano, já
        // começando e terminando no mesmo vértice. Lança exceção se o
        // grafo não atender à condição de Euleriano (chame
        // VerificarEuleriano antes).
        public static List<int> ConstruirCircuito(Grafo grafo)
        {
            if (!VerificarEuleriano(grafo))
            {
                throw new Exception("O grafo nao possui circuito euleriano.");
            }

            // Copia as arestas disponíveis por vértice de origem, para irmos "consumindo"
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

        // Texto amigável para log/console, ex: "1 -> 2 -> 4 -> 1"
        public static string FormatarCircuito(List<int> circuito)
        {
            return string.Join(" -> ", circuito);
        }
    }
}
