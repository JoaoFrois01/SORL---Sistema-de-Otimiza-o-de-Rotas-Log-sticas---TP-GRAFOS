using System.Collections.Generic;
using System;

namespace Models
{
    public class Grafo
    {
        private int numVertices;
        private int numArestas;
        private List<Vertice> vertices;
        private List<Aresta> arestas;
        private string representacaoAtual;
        private Dictionary<int, List<Vertice>> lista_adjacencias;
        private double[,] matriz_adjacencias;

        public int NumVertices { get { return numVertices; } }
        public int NumArestas { get { return numArestas; } }
        public List<Vertice> Vertices { get { return vertices; } }
        public List<Aresta> Arestas { get { return arestas; } }
        public string RepresentacaoAtual { get { return representacaoAtual; } }

        public Grafo(int numVertices, int numArestas)
        {
            this.numVertices = numVertices;
            this.numArestas = numArestas;
            vertices = new List<Vertice>();
            arestas = new List<Aresta>();
            representacaoAtual = "";
            lista_adjacencias = new Dictionary<int, List<Vertice>>();
            matriz_adjacencias = new double[0, 0];
            Escolher_Representacao();
        }
        public Grafo()
        {
            this.numVertices = 0;
            this.numArestas = 0;
            vertices = new List<Vertice>();
            arestas = new List<Aresta>();
            representacaoAtual = "";
            lista_adjacencias = new Dictionary<int, List<Vertice>>();
            matriz_adjacencias = new double[0, 0];
            Escolher_Representacao();
        }

        public string DefinirDensidade()
        {
            double calculoDensidade = (double)numArestas / (numVertices * (numVertices - 1));

            if (calculoDensidade <= 0.5)
            {
                return "esparso";
            }

            return "denso";
        }

        public void Construir_ListaAjdacencias()
        {
            lista_adjacencias = new Dictionary<int, List<Vertice>>();
        }

        public void Contruir_MatrizAdjacencias()
        {
            matriz_adjacencias = new double[numVertices, numVertices];
        }

        public string Escolher_Representacao()
        {
            string densidade = DefinirDensidade();

            if (densidade == "denso")
            {
                Contruir_MatrizAdjacencias();
                representacaoAtual = "Matriz de Adjacencias";
            }
            else
            {
                Construir_ListaAjdacencias();
                representacaoAtual = "Lista de Adjacencias";
            }

            return "Grafo Criado : Modelo Escolhido: " + representacaoAtual;
        }

        public List<Vertice> ObterAdjacentes(int idVertice)
        {
            List<Vertice> verticesAdjacentes = new List<Vertice>();

            if (representacaoAtual == "Lista de Adjacencias")
            {
                if (lista_adjacencias.ContainsKey(idVertice))
                {
                    verticesAdjacentes = lista_adjacencias[idVertice];
                }

                return verticesAdjacentes;
            }

            int posVertice = ObterIndiceVertice(idVertice);

            if (posVertice == -1)
            {
                return verticesAdjacentes;
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                if (matriz_adjacencias[posVertice, i] != 0)
                {
                    verticesAdjacentes.Add(vertices[i]);
                }
            }

            return verticesAdjacentes;
        }

        public Vertice AdicionarVertice(Vertice v)
        {
            Vertice? existente = ObterVerticePorId(v.Id);

            if (existente != null)
            {
                return existente;
            }

            vertices.Add(v);

            if (representacaoAtual == "Lista de Adjacencias")
            {
                lista_adjacencias.Add(v.Id, new List<Vertice>());
            }

            return v;
        }

        public bool AdicionarAresta(Aresta a)
        {
            if (ObterVerticePorId(a.Origem.Id) == null || ObterVerticePorId(a.Destino.Id) == null || ArestaExiste(a.Origem.Id, a.Destino.Id))
            {
                return false;
            }

            arestas.Add(a);

            if (representacaoAtual == "Lista de Adjacencias")
            {
                lista_adjacencias[a.Origem.Id].Add(a.Destino);
                a.Origem.Atualizar_GrauDeSaida(a.Origem.GrauDeSaida + 1);
                a.Destino.Atualizar_GrauDeEntrda(a.Destino.GrauDeEntrada + 1);
            }
            else
            {
                int posicaoOrigem = PosicaoVertice(a.Origem);
                int posicaoDestino = PosicaoVertice(a.Destino);
                matriz_adjacencias[posicaoOrigem, posicaoDestino] = a.Peso;
                vertices[posicaoOrigem].Atualizar_GrauDeSaida(vertices[posicaoOrigem].GrauDeSaida + 1);
                vertices[posicaoDestino].Atualizar_GrauDeEntrda(vertices[posicaoDestino].GrauDeEntrada + 1);
            }

            return true;
        }

        //M�todo que ir� retornar qual a posi��o do v�rtice, independente de qual representa��o esteja sendo utilizada.
        public int PosicaoVertice(Vertice vertice)
        {
           int posicao = vertices.FindIndex(v =>  v.Id == vertice.Id);
            return posicao;
        }
        public double PesoAresta (Vertice origem, Vertice destino)
        {
            double pesoAresta = -1; 
            for (int i = 0; i < arestas.Count; i++)
            {
                if (arestas[i].Origem.Id == origem.Id && arestas[i].Destino.Id == destino.Id)
                    pesoAresta = arestas[i].Peso;
            }
            if (pesoAresta == -1)
                throw new Exception("Aresta não encontrada.");
            return pesoAresta;

        }
        public Vertice ObterVertice(int posicao)
        {
            return vertices[posicao];
        }

        public Vertice? ObterVerticePorId(int id)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].Id == id)
                {
                    return vertices[i];
                }
            }

            return null;
        }

        public List<Aresta> ObterArestasSaindoDe(int idVertice)
        {
            List<Aresta> arestasSaindo = new List<Aresta>();

            for (int i = 0; i < arestas.Count; i++)
            {
                if (arestas[i].Origem.Id == idVertice)
                {
                    arestasSaindo.Add(arestas[i]);
                }
            }

            return arestasSaindo;
        }

        private int ObterIndiceVertice(int idVertice)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].Id == idVertice)
                {
                    return i;
                }
            }

            return -1;
        }

        private bool ArestaExiste(int idOrigem, int idDestino)
        {
            for (int i = 0; i < arestas.Count; i++)
            {
                if (arestas[i].Origem.Id == idOrigem && arestas[i].Destino.Id == idDestino)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
