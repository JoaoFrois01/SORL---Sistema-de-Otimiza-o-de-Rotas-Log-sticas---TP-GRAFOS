using System;
using System.Collections.Generic;
using Models;

namespace TP_GRAFOS.Algorithms
{
    public static class Kruskal
    {
        private static int[] grupo;

        private static int Encontrar(int i)
        {
            
            if (grupo[i] == i)
            {
                return i;
            }

            
            return Encontrar(grupo[i]);
        }

        private static void Unir(int i, int j)
        {
            
            int representanteI = Encontrar(i);
            int representanteJ = Encontrar(j);

            
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

            
            List<Aresta> arestasOrdenadas = OrdenarArestasPorPeso(grafo.Arestas);

            List<Aresta> arvoreGeradora = new List<Aresta>();
            double custoTotal = 0;

            
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

        private static List<Aresta> OrdenarArestasPorPeso(List<Aresta> arestas)
        {
            List<Aresta> ordenadas = new List<Aresta>();

            for (int i = 0; i < arestas.Count; i++)
            {
                ordenadas.Add(arestas[i]);
            }

            for (int i = 0; i < ordenadas.Count - 1; i++)
            {
                int indiceMenor = i;

                for (int j = i + 1; j < ordenadas.Count; j++)
                {
                    if (ordenadas[j].Peso < ordenadas[indiceMenor].Peso)
                    {
                        indiceMenor = j;
                    }
                }

                if (indiceMenor != i)
                {
                    Aresta auxiliar = ordenadas[i];
                    ordenadas[i] = ordenadas[indiceMenor];
                    ordenadas[indiceMenor] = auxiliar;
                }
            }

            return ordenadas;
        }
    }
}
