using System;
using System.Collections.Generic;
using Models;

namespace TP_GRAFOS.Algorithms
{
    internal class Coloracao
    {
        private Dictionary<int, List<int>> grafoConflitos;
        private List<Aresta> rotas;

        public Coloracao()
        {
            grafoConflitos = new Dictionary<int, List<int>>();
            rotas = new List<Aresta>();
        }

        public void ConstruirGrafoDeConflitos(Grafo grafo)
        {
            rotas = grafo.Arestas;
            grafoConflitos = new Dictionary<int, List<int>>();

            for (int i = 0; i < rotas.Count; i++)
            {
                grafoConflitos[i] = new List<int>();
            }

            for (int i = 0; i < rotas.Count; i++)
            {
                for (int j = i + 1; j < rotas.Count; j++)
                {
                    if (CompartilhamRecurso(rotas[i], rotas[j]))
                    {
                        grafoConflitos[i].Add(j);
                        grafoConflitos[j].Add(i);
                    }
                }
            }
        }

        private bool CompartilhamRecurso(Aresta a, Aresta b)
        {
            return a.Origem.Id == b.Origem.Id
                || a.Origem.Id == b.Destino.Id
                || a.Destino.Id == b.Origem.Id
                || a.Destino.Id == b.Destino.Id;
        }

        public Dictionary<int, int> Colorir(Grafo grafo)
        {
            ConstruirGrafoDeConflitos(grafo);

            List<int> ordemDeProcessamento = MontarOrdemDeProcessamento();

            Dictionary<int, int> corPorRota = new Dictionary<int, int>();
            int corAtual = 1;

            while (corPorRota.Count < rotas.Count)
            {
                foreach (int indiceRota in ordemDeProcessamento)
                {
                    if (corPorRota.ContainsKey(indiceRota))
                        continue;

                    bool conflito = false;

                    foreach (int vizinho in grafoConflitos[indiceRota])
                    {
                        if (corPorRota.ContainsKey(vizinho) && corPorRota[vizinho] == corAtual)
                        {
                            conflito = true;
                            break;
                        }
                    }

                    if (!conflito)
                    {
                        corPorRota[indiceRota] = corAtual;
                    }
                }

                corAtual++;
            }

            return corPorRota;
        }

        public Dictionary<int, List<string>> MontarTurnos(Dictionary<int, int> corPorRota)
        {
            Dictionary<int, List<string>> turnos = new Dictionary<int, List<string>>();

            foreach (KeyValuePair<int, int> par in corPorRota)
            {
                int indiceRota = par.Key;
                int turno = par.Value;

                if (!turnos.ContainsKey(turno))
                {
                    turnos[turno] = new List<string>();
                }

                Aresta rota = rotas[indiceRota];
                turnos[turno].Add(rota.Origem.Id + " -> " + rota.Destino.Id);
            }

            return turnos;
        }

        public int NumeroMinimoDeTurnos(Dictionary<int, int> corPorRota)
        {
            if (corPorRota.Count == 0)
            {
                return 0;
            }

            int maiorCor = 0;

            foreach (KeyValuePair<int, int> par in corPorRota)
            {
                if (par.Value > maiorCor)
                {
                    maiorCor = par.Value;
                }
            }

            return maiorCor;
        }

        private List<int> MontarOrdemDeProcessamento()
        {
            List<int> ordem = new List<int>();

            for (int i = 0; i < rotas.Count; i++)
            {
                ordem.Add(i);
            }

            for (int i = 0; i < ordem.Count - 1; i++)
            {
                int indiceMaior = i;

                for (int j = i + 1; j < ordem.Count; j++)
                {
                    if (grafoConflitos[ordem[j]].Count > grafoConflitos[ordem[indiceMaior]].Count)
                    {
                        indiceMaior = j;
                    }
                }

                if (indiceMaior != i)
                {
                    int auxiliar = ordem[i];
                    ordem[i] = ordem[indiceMaior];
                    ordem[indiceMaior] = auxiliar;
                }
            }

            return ordem;
        }
    }
}
