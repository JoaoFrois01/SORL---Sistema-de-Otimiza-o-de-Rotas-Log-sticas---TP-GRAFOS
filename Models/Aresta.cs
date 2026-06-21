using System;

namespace Models
{
    public class Aresta
    {
        private Vertice origem;
        private Vertice destino;
        private double peso;
        private double capacidade;
        private double fluxoAtual;

        public Vertice Origem {get { return origem; }}
        public Vertice Destino {get { return destino; }}
        public double Peso {get { return peso; }}
        public double Capacidade { get { return capacidade; } }
        public double FluxoAtual { get { return fluxoAtual; } }

        public Aresta(Vertice origem, Vertice destino, double peso, double capacidade, double fluxoAtual)
        {
            this.origem = origem;
            this.destino = destino;
            this.peso = peso;
            this.capacidade = capacidade;
            this.fluxoAtual = fluxoAtual;
        }

        public double Atualizar_FluxoAtual(double novoFluxo)
        {
            return fluxoAtual = novoFluxo;
        }

        public double ObterCapacidadeResidual()
        {
            return capacidade - fluxoAtual;
        }

        public void LimparFluxo()
        {
            fluxoAtual = 0.0;
        }
    }
}
