using System;
using System.Collections.Generic;
using Models;

namespace TP_GRAFOS.Algorithms
{
    internal static class Hamiltoniano
    {
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

            caminho.Add(verticeInicial);
            return caminho;
        }
        public static bool VerificarHamiltoniano(Grafo grafo)
        {
            return EncontrarCicloHamiltoniano(grafo) != null;
        }

        private static bool Backtrack(Grafo grafo, int verticeAtual, int verticeInicial, List<int> caminho, HashSet<int> visitados)
        {
            if (caminho.Count == grafo.Vertices.Count)
            {
                return ExisteAresta(grafo, verticeAtual, verticeInicial);
            }

            List<Vertice> adjacentes = grafo.ObterAdjacentes(verticeAtual);

            foreach (Vertice candidato in adjacentes)
            {
                if (!visitados.Contains(candidato.Id))
                {
                    caminho.Add(candidato.Id);
                    visitados.Add(candidato.Id);

                    if (Backtrack(grafo, candidato.Id, verticeInicial, caminho, visitados))
                    {
                        return true;
                    }

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
        
        public static string FormatarCiclo(List<int> ciclo)
        {
            return string.Join(" -> ", ciclo);
        }
    }
}
