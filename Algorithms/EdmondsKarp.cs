using Models;
using System;
using System.Collections.Generic;

namespace TP_GRAFOS.Algorithms
{
    public class CaminhoFluxo
    {
        private List<int> vertices;
        private double fluxoEnviado;

        public List<int> Vertices { get { return vertices; } }
        public double FluxoEnviado { get { return fluxoEnviado; } }

        public CaminhoFluxo(List<int> vertices, double fluxoEnviado)
        {
            this.vertices = vertices;
            this.fluxoEnviado = fluxoEnviado;
        }

        public string FormatarCaminho()
        {
            string texto = "";

            for (int i = 0; i < vertices.Count; i++)
            {
                if (i > 0)
                {
                    texto = texto + " -> ";
                }

                texto = texto + vertices[i];
            }

            return texto;
        }
    }

    public class ResultadoFluxo
    {
        private int origem;
        private int destino;
        private double fluxoMaximo;
        private List<CaminhoFluxo> caminhosAumentantes;
        private List<Aresta> corteMinimo;

        public int Origem { get { return origem; } }
        public int Destino { get { return destino; } }
        public double FluxoMaximo { get { return fluxoMaximo; } }
        public List<CaminhoFluxo> CaminhosAumentantes { get { return caminhosAumentantes; } }
        public List<Aresta> CorteMinimo { get { return corteMinimo; } }

        public ResultadoFluxo(int origem, int destino, double fluxoMaximo, List<CaminhoFluxo> caminhosAumentantes, List<Aresta> corteMinimo)
        {
            this.origem = origem;
            this.destino = destino;
            this.fluxoMaximo = fluxoMaximo;
            this.caminhosAumentantes = caminhosAumentantes;
            this.corteMinimo = corteMinimo;
        }
    }

    public class EdmondsKarp
    {
        private const double Epsilon = 0.000001;

        public ResultadoFluxo Calcular(Grafo grafo, int idOrigem, int idDestino)
        {
            if (grafo.ObterVerticePorId(idOrigem) == null)
            {
                throw new ArgumentException("Vertice de origem nao existe no grafo.");
            }

            if (grafo.ObterVerticePorId(idDestino) == null)
            {
                throw new ArgumentException("Vertice de destino nao existe no grafo.");
            }

            if (idOrigem == idDestino)
            {
                throw new ArgumentException("Origem e destino devem ser vertices diferentes.");
            }

            LimparFluxos(grafo);

            int quantidadeVertices = grafo.Vertices.Count;
            double[,] capacidades = MontarMatrizCapacidades(grafo);
            double[,] fluxos = new double[quantidadeVertices, quantidadeVertices];
            int indiceOrigem = ObterIndiceVertice(grafo, idOrigem);
            int indiceDestino = ObterIndiceVertice(grafo, idDestino);
            int[] pais = new int[quantidadeVertices];
            double fluxoMaximo = 0.0;
            List<CaminhoFluxo> caminhosAumentantes = new List<CaminhoFluxo>();

            while (BuscarCaminhoAumentante(capacidades, fluxos, indiceOrigem, indiceDestino, pais))
            {
                double gargalo = CalcularGargalo(capacidades, fluxos, pais, indiceOrigem, indiceDestino);
                List<int> caminho = MontarCaminho(grafo, pais, indiceOrigem, indiceDestino);

                int atual = indiceDestino;

                while (atual != indiceOrigem)
                {
                    int anterior = pais[atual];
                    fluxos[anterior, atual] = fluxos[anterior, atual] + gargalo;
                    fluxos[atual, anterior] = fluxos[atual, anterior] - gargalo;
                    atual = anterior;
                }

                fluxoMaximo = fluxoMaximo + gargalo;
                caminhosAumentantes.Add(new CaminhoFluxo(caminho, gargalo));
            }

            AtualizarFluxoDasArestasOriginais(grafo, fluxos);

            bool[] verticesAlcancaveis = EncontrarVerticesAlcancaveisNoResidual(capacidades, fluxos, indiceOrigem);
            List<Aresta> corteMinimo = IdentificarCorteMinimo(grafo, verticesAlcancaveis);

            return new ResultadoFluxo(idOrigem, idDestino, fluxoMaximo, caminhosAumentantes, corteMinimo);
        }

        private double[,] MontarMatrizCapacidades(Grafo grafo)
        {
            int quantidadeVertices = grafo.Vertices.Count;
            double[,] capacidades = new double[quantidadeVertices, quantidadeVertices];

            for (int i = 0; i < grafo.Arestas.Count; i++)
            {
                Aresta aresta = grafo.Arestas[i];
                int origem = ObterIndiceVertice(grafo, aresta.Origem.Id);
                int destino = ObterIndiceVertice(grafo, aresta.Destino.Id);

                if (origem != -1 && destino != -1)
                {
                    capacidades[origem, destino] = capacidades[origem, destino] + aresta.Capacidade;
                }
            }

            return capacidades;
        }

        private bool BuscarCaminhoAumentante(double[,] capacidades, double[,] fluxos, int origem, int destino, int[] pais)
        {
            int quantidadeVertices = capacidades.GetLength(0);
            bool[] visitados = new bool[quantidadeVertices];
            Queue<int> fila = new Queue<int>();

            for (int i = 0; i < quantidadeVertices; i++)
            {
                pais[i] = -1;
                visitados[i] = false;
            }

            visitados[origem] = true;
            fila.Enqueue(origem);

            while (fila.Count > 0)
            {
                int atual = fila.Dequeue();

                for (int proximo = 0; proximo < quantidadeVertices; proximo++)
                {
                    double capacidadeResidual = capacidades[atual, proximo] - fluxos[atual, proximo];

                    if (!visitados[proximo] && capacidadeResidual > Epsilon)
                    {
                        pais[proximo] = atual;
                        visitados[proximo] = true;

                        if (proximo == destino)
                        {
                            return true;
                        }

                        fila.Enqueue(proximo);
                    }
                }
            }

            return false;
        }

        private double CalcularGargalo(double[,] capacidades, double[,] fluxos, int[] pais, int origem, int destino)
        {
            double gargalo = double.MaxValue;
            int atual = destino;

            while (atual != origem)
            {
                int anterior = pais[atual];
                double capacidadeResidual = capacidades[anterior, atual] - fluxos[anterior, atual];

                if (capacidadeResidual < gargalo)
                {
                    gargalo = capacidadeResidual;
                }

                atual = anterior;
            }

            return gargalo;
        }

        private List<int> MontarCaminho(Grafo grafo, int[] pais, int origem, int destino)
        {
            List<int> caminho = new List<int>();
            int atual = destino;

            while (atual != origem)
            {
                caminho.Insert(0, grafo.Vertices[atual].Id);
                atual = pais[atual];
            }

            caminho.Insert(0, grafo.Vertices[origem].Id);
            return caminho;
        }

        private bool[] EncontrarVerticesAlcancaveisNoResidual(double[,] capacidades, double[,] fluxos, int origem)
        {
            int quantidadeVertices = capacidades.GetLength(0);
            bool[] visitados = new bool[quantidadeVertices];
            Queue<int> fila = new Queue<int>();

            visitados[origem] = true;
            fila.Enqueue(origem);

            while (fila.Count > 0)
            {
                int atual = fila.Dequeue();

                for (int proximo = 0; proximo < quantidadeVertices; proximo++)
                {
                    double capacidadeResidual = capacidades[atual, proximo] - fluxos[atual, proximo];

                    if (!visitados[proximo] && capacidadeResidual > Epsilon)
                    {
                        visitados[proximo] = true;
                        fila.Enqueue(proximo);
                    }
                }
            }

            return visitados;
        }

        private List<Aresta> IdentificarCorteMinimo(Grafo grafo, bool[] verticesAlcancaveis)
        {
            List<Aresta> corteMinimo = new List<Aresta>();

            for (int i = 0; i < grafo.Arestas.Count; i++)
            {
                Aresta aresta = grafo.Arestas[i];
                int origem = ObterIndiceVertice(grafo, aresta.Origem.Id);
                int destino = ObterIndiceVertice(grafo, aresta.Destino.Id);

                if (origem != -1 && destino != -1 && verticesAlcancaveis[origem] && !verticesAlcancaveis[destino])
                {
                    corteMinimo.Add(aresta);
                }
            }

            return corteMinimo;
        }

        private void AtualizarFluxoDasArestasOriginais(Grafo grafo, double[,] fluxos)
        {
            for (int i = 0; i < grafo.Arestas.Count; i++)
            {
                Aresta aresta = grafo.Arestas[i];
                int origem = ObterIndiceVertice(grafo, aresta.Origem.Id);
                int destino = ObterIndiceVertice(grafo, aresta.Destino.Id);

                if (origem != -1 && destino != -1)
                {
                    aresta.Atualizar_FluxoAtual(fluxos[origem, destino]);
                }
            }
        }

        private void LimparFluxos(Grafo grafo)
        {
            for (int i = 0; i < grafo.Arestas.Count; i++)
            {
                grafo.Arestas[i].LimparFluxo();
            }
        }

        private int ObterIndiceVertice(Grafo grafo, int idVertice)
        {
            for (int i = 0; i < grafo.Vertices.Count; i++)
            {
                if (grafo.Vertices[i].Id == idVertice)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
