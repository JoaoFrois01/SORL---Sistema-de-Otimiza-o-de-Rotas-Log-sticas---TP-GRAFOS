using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace TP_GRAFOS.Algorithms
{
    static class Dijkstra
    {

        public static string MenorCaminho(Grafo grafo, Vertice origem, Vertice destino)
        {
            int numVertices = grafo.NumVertices;

            double[] distancias = new double[numVertices];
            Vertice[] predecessores = new Vertice[numVertices];
            bool[] visitado = new bool[numVertices];
            double menorDistancia;

            for (int i = 0; i < numVertices; i++)
            {
                distancias[i] = double.MaxValue;
                predecessores[i] = null;
                visitado[i] = false;
            }

            distancias[grafo.PosicaoVertice(origem)] = 0;
            int u = 0;

            for (int contagem = 0; contagem < numVertices && u != -1 ; contagem++)
            {
                // TODO 1
                 u = -1;
                menorDistancia = double.MaxValue;

                for (int i = 0; i < numVertices; i++)
                {
                    if (!visitado[i] && distancias[i] < menorDistancia)
                    {
                        menorDistancia = distancias[i];
                        u = i;
                    }
                }

                visitado[u] = true;

                // Obtém o vértice correspondente à posição u
                Vertice verticeAtual = grafo.ObterVertice(u);

                // TODO 3
                List<Vertice> adjacentes =
                    grafo.ObterAdjacentes(verticeAtual.Id);

                // TODO 4
                foreach (Vertice v in adjacentes)
                {
                    int posV = grafo.PosicaoVertice(v);

                    double peso =
                        grafo.PesoAresta(verticeAtual, v);

                    double novaDistancia =
                        distancias[u] + peso;

                    if (novaDistancia < distancias[posV])
                    {
                        distancias[posV] = novaDistancia;
                        predecessores[posV] = verticeAtual;
                    }
                }
            }

            // TODO 5
            int posDestino = grafo.PosicaoVertice(destino);

            if (distancias[posDestino] == double.MaxValue)
            {
                return "Não existe caminho entre os vértices.";
            }

            List<Vertice> caminho = new List<Vertice>();

            Vertice atual = destino;

            while (atual != null)
            {
                caminho.Add(atual);
                atual = predecessores[grafo.PosicaoVertice(atual)];
            }

            caminho.Reverse();

            string resultado = "Caminho: ";

            for (int i = 0; i < caminho.Count; i++)
            {
                resultado += caminho[i].Id;

                if (i < caminho.Count - 1)
                    resultado += " -> ";
            }

            resultado +=
                "\nCusto total: " + distancias[posDestino];

            return resultado;
        }


    }
}
