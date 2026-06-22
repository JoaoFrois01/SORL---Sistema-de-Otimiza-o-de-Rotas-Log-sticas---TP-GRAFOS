using System;
using System.Collections.Generic;
using Models;

namespace TP_GRAFOS.Algorithms
{
    // Resolve o Cenário B do problema V: "É possível que o inspetor visite
    // todos os hubs (vértices) exatamente uma vez, retornando ao hub inicial?"
    //
    // Isso é o problema do CICLO HAMILTONIANO em um grafo DIRECIONADO.
    //
    // Não existe condição simples (P) para decidir isso em geral — o problema
    // é NP-completo. O algoritmo estudado em sala para isso é BACKTRACKING
    // com poda: tenta construir o caminho vértice a vértice e desiste assim
    // que percebe que não há mais como completar.
    //
    // Como os grafos de teste fornecidos são pequenos (de 103 bytes a 4,6 kB),
    // o backtracking é perfeitamente viável em tempo de execução.
    internal static class Hamiltoniano
    {
        // ----------------------------------------------------------------
        // 1) VERIFICAÇÃO + CONSTRUÇÃO (em uma única busca)
        // ----------------------------------------------------------------
        // Retorna o ciclo encontrado (lista de Ids de vértices, começando
        // e terminando no mesmo vértice) ou null se não existir.
        public static List<int>? EncontrarCicloHamiltoniano(Grafo grafo)
        {
            if (grafo.Vertices.Count == 0)
            {
                return null;
            }

            int verticeInicial = grafo.Vertices[0].Id;

            List<int> caminho = new List<int>();
            HashSet<int> visitados = new HashSet<int>();

            caminho.Add(verticeInicial);
            visitados.Add(verticeInicial);

            bool encontrou = Backtrack(grafo, verticeInicial, verticeInicial, caminho, visitados);

            if (!encontrou)
            {
                return null;
            }

            caminho.Add(verticeInicial); // fecha o ciclo
            return caminho;
        }

        // Atalho que só responde sim/não — útil pro menu, quando o usuário
        // só quer saber se "é possível", sem necessariamente exibir o percurso.
        public static bool VerificarHamiltoniano(Grafo grafo)
        {
            return EncontrarCicloHamiltoniano(grafo) != null;
        }

        // ----------------------------------------------------------------
        // 2) BACKTRACKING RECURSIVO
        // ----------------------------------------------------------------
        // verticeAtual: último vértice incluído no caminho parcial
        // verticeInicial: para onde precisamos voltar ao final
        // caminho: caminho parcial construído até agora (mutado por referência)
        // visitados: conjunto de vértices já usados no caminho parcial
        private static bool Backtrack(Grafo grafo, int verticeAtual, int verticeInicial, List<int> caminho, HashSet<int> visitados)
        {
            // Caso base: já visitamos todos os vértices.
            // Só fecha o ciclo se existir aresta de volta para o início.
            if (caminho.Count == grafo.Vertices.Count)
            {
                return ExisteAresta(grafo, verticeAtual, verticeInicial);
            }

            List<Vertice> adjacentes = grafo.ObterAdjacentes(verticeAtual);

            foreach (Vertice candidato in adjacentes)
            {
                if (!visitados.Contains(candidato.Id))
                {
                    // Escolhe
                    caminho.Add(candidato.Id);
                    visitados.Add(candidato.Id);

                    // Tenta completar o resto do caminho a partir daqui
                    if (Backtrack(grafo, candidato.Id, verticeInicial, caminho, visitados))
                    {
                        return true;
                    }

                    // Desfaz (poda) e tenta o próximo candidato
                    caminho.RemoveAt(caminho.Count - 1);
                    visitados.Remove(candidato.Id);
                }
            }

            return false;
        }

        private static bool ExisteAresta(Grafo grafo, int idOrigem, int idDestino)
        {
            List<Vertice> adjacentes = grafo.ObterAdjacentes(idOrigem);

            foreach (Vertice v in adjacentes)
            {
                if (v.Id == idDestino)
                {
                    return true;
                }
            }

            return false;
        }

        // Texto amigável para log/console, ex: "1 -> 3 -> 2 -> 1"
        public static string FormatarCiclo(List<int> ciclo)
        {
            return string.Join(" -> ", ciclo);
        }
    }
}
