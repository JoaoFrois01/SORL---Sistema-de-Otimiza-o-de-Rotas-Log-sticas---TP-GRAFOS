using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace TP_GRAFOS.Algorithms
{
    public static class Kruskal
    {
        private static int[] grupo;

        private static int Encontrar(int i)
        {
            // TODO A
            if (grupo[i] == i)
            {
                return i;
            }

            // TODO B
            return Encontrar(grupo[i]);
        }

        private static void Unir(int i, int j)
        {
            // TODO C
            int representanteI = Encontrar(i);
            int representanteJ = Encontrar(j);

            // TODO D
            if (representanteI != representanteJ)
            {
                grupo[representanteI] = representanteJ;
            }
        }

        public static string AGM(Grafo grafo)
        {
            int numVertices = grafo.NumVertices;

            grupo = new int[numVertices];

            for (int i = 0; i < numVertices; i++)
            {
                grupo[i] = i;
            }

            // TODO 1
            List<Aresta> arestasOrdenadas = grafo.Arestas
                .OrderBy(a => a.Peso)
                .ToList();

            List<Aresta> arvoreGeradora = new List<Aresta>();
            double custoTotal = 0;

            // TODO 2
            foreach (Aresta aresta in arestasOrdenadas)
            {
                int posOrigem =
                    grafo.PosicaoVertice(aresta.Origem);

                int posDestino =
                    grafo.PosicaoVertice(aresta.Destino);

                int representanteOrigem =
                    Encontrar(posOrigem);

                int representanteDestino =
                    Encontrar(posDestino);

                if (representanteOrigem != representanteDestino)
                {
                    arvoreGeradora.Add(aresta);

                    custoTotal += aresta.Peso;

                    Unir(posOrigem, posDestino);
                }
            }

            // TODO 3
            string resultado = "";

            if (arvoreGeradora.Count != numVertices - 1)
            {
                resultado +=
                    "O grafo é desconexo. Não foi possível gerar uma AGM completa.\n\n";
            }

            resultado +=
                "Árvore Geradora Mínima:\n";

            foreach (Aresta aresta in arvoreGeradora)
            {
                resultado +=
                    aresta.Origem.Id +
                    " -> " +
                    aresta.Destino.Id +
                    " | Peso: " +
                    aresta.Peso +
                    "\n";
            }

            resultado +=
                "\nCusto Total: " +
                custoTotal;

            return resultado;
        }
    }
}