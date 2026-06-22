using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace TP_GRAFOS.Algorithms
{
    // Resolve o problema IV: "Determinar o número mínimo de turnos (ou dias)
    // necessários para realizar as manutenções de todas as rotas sem
    // conflitos de recurso, e sugerir a alocação de rotas por turno."
    //
    // Estratégia (a clássica para esse tipo de problema):
    //   1) Construir um GRAFO DE CONFLITOS auxiliar, onde:
    //        - cada VÉRTICE representa uma ROTA (aresta) do grafo original
    //        - existe uma ARESTA entre duas rotas se elas "conflitam"
    //          (compartilham recurso)
    //   2) Aplicar COLORAÇÃO DE GRAFOS (algoritmo guloso / greedy) nesse
    //      grafo de conflitos. Cada COR = um TURNO de manutenção.
    //      Rotas com a mesma cor podem ser feitas no mesmo turno, pois
    //      nunca conflitam entre si.
    //
    // IMPORTANTE — decisão que você precisa validar com o grupo / professor:
    // o enunciado não diz explicitamente QUAL é o critério de conflito entre
    // rotas. O critério mais comum (e mais simples de justificar no
    // relatório) é: "duas rotas conflitam se compartilham um hub
    // (vértice) em comum" — afinal, hubs concentram pátios/oficinas/
    // equipamentos de inspeção, conforme descrito no enunciado.
    // Esse é o critério já implementado abaixo em CompartilhamRecurso().
    // Se o grupo decidir por outro critério, basta alterar esse método.
    internal class Coloracao
    {
        // Representa o grafo de conflitos: chave = índice da aresta original,
        // valor = lista de índices de arestas que conflitam com ela.
        private Dictionary<int, List<int>> grafoConflitos;
        private List<Aresta> rotas;

        public Coloracao()
        {
            grafoConflitos = new Dictionary<int, List<int>>();
            rotas = new List<Aresta>();
        }

        // ----------------------------------------------------------------
        // 1) CONSTRUÇÃO DO GRAFO DE CONFLITOS
        // ----------------------------------------------------------------
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

        // Critério de conflito: duas rotas compartilham recurso se
        // possuem algum hub (origem ou destino) em comum.
        private bool CompartilhamRecurso(Aresta a, Aresta b)
        {
            return a.Origem.Id == b.Origem.Id
                || a.Origem.Id == b.Destino.Id
                || a.Destino.Id == b.Origem.Id
                || a.Destino.Id == b.Destino.Id;
        }

        // ----------------------------------------------------------------
        // 2) COLORAÇÃO GULOSA (greedy coloring)
        // ----------------------------------------------------------------
        // Retorna um dicionário: índice da rota -> número do turno (cor),
        // começando em 1.
        public Dictionary<int, int> Colorir(Grafo grafo)
        {
            ConstruirGrafoDeConflitos(grafo);

            Dictionary<int, int> corPorRota = new Dictionary<int, int>();

            // Ordena as rotas da maior para a menor quantidade de conflitos
            // (heurística clássica: "welsh-powell" — tende a usar menos cores
            // que percorrer na ordem original, mas a ordem original também
            // é aceitável caso o professor exija o greedy "puro").
            List<int> ordemDeProcessamento = Enumerable.Range(0, rotas.Count)
                .OrderByDescending(indice => grafoConflitos[indice].Count)
                .ToList();

            foreach (int indiceRota in ordemDeProcessamento)
            {
                HashSet<int> coresUsadasPelosVizinhos = new HashSet<int>();

                foreach (int vizinho in grafoConflitos[indiceRota])
                {
                    if (corPorRota.ContainsKey(vizinho))
                    {
                        coresUsadasPelosVizinhos.Add(corPorRota[vizinho]);
                    }
                }

                int cor = 1;

                while (coresUsadasPelosVizinhos.Contains(cor))
                {
                    cor++;
                }

                corPorRota[indiceRota] = cor;
            }

            return corPorRota;
        }

        // ----------------------------------------------------------------
        // 3) MONTAGEM DOS TURNOS PARA EXIBIÇÃO/LOG
        // ----------------------------------------------------------------
        // Agrupa as rotas por turno (cor) -> lista de strings "origem -> destino"
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

            return corPorRota.Values.Max();
        }
    }
}
