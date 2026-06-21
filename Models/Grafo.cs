using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Security.Cryptography.X509Certificates;
using System.Linq;


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

        public Grafo(int numVertices, int numArestas)
        {
            this.numVertices = numVertices;
            this.numArestas = numArestas;
            vertices = new List<Vertice>();
            arestas = new List<Aresta>();
            Escolher_Representacao();
        }

        public int NumVertices { get { return numVertices; }}
        public string RepresentacaoAtual { get { return representacaoAtual; } }

        public string DefinirDensidade()
        {
            //A / V (V-1) - Formato de Cálculo - Slides Disciplina
            double calculoDensidade = (double)numArestas / (numVertices * (numVertices- 1));
            return calculoDensidade <= 0.5 ? "esparso" : "denso";
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
                representacaoAtual = "Matriz de Adjacências";
            }
            else
            {
                Construir_ListaAjdacencias();
                representacaoAtual = "Lista de Adjacências";
            }
            return "Grafo Criado : Modelo Escolhido: " + representacaoAtual;

        }
        public List<Vertice> ObterAdjacentes(int idVertice)
        {
            List<Vertice> verticesAdjacentes = new List<Vertice>();

            if (representacaoAtual == "Lista de Adjacências")
            {
                lista_adjacencias.TryGetValue(idVertice, out verticesAdjacentes);
                return verticesAdjacentes;

            }
            else
            {
                int posVertice = 0;
                for (int i = 0; i < numVertices; i++)
                {
                    if(idVertice == vertices[i].Id)
                        posVertice = i;
                }
                //Utilizo o For para passar por todas as linhas da matriz, somente na coluna referente ao vértice. Utilizo o número de vértices pois é o mesmo
                //número de colunas da matriz
                for (int i = 0; i < vertices.Count; i++)
                {
                    if (matriz_adjacencias[posVertice, i] != 0)
                        verticesAdjacentes.Add(vertices[i]);
                }
                return verticesAdjacentes;
            }
        }

        public Vertice AdicionarVertice(Vertice v)
        {
            Vertice existente = vertices.FirstOrDefault(vert => vert.Id == v.Id);
            if (existente != null)
                return existente;   // já existia: retorna o que JÁ ESTÁ na lista

            vertices.Add(v);
            if (representacaoAtual == "Lista de Adjacências")
            {
                lista_adjacencias.Add(v.Id, new List<Vertice>());
            }
            return v;
        }
        public bool AdicionarAresta (Aresta a)
        {
            //Verifica dois pontos, 1º se algum veértice não existe e depois se a aresta já existe.Se algum desses casos é real, a aresta não pode ser adicionada.
            if (!vertices.Any(v => v.Id == a.Origem.Id) || !vertices.Any(v => v.Id == a.Destino.Id) || arestas.Any(v=> a.Origem.Id == v.Origem.Id && a.Destino.Id == v.Destino.Id))
            {
                return false;                                       
            }
            arestas.Add(a);
            if(representacaoAtual == "Lista de Adjacências")
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

        //Método que irá retornar qual a posição do vértice, independente de qual representação esteja sendo utilizada.
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

        public List<Aresta> Arestas{ get {return arestas; } }




    }



}