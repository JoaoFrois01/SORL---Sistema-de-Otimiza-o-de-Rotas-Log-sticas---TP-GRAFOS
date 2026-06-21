using System;

namespace Models
{

    public class Vertice
    {
        private int id;
        private int grauDeEntrada;
        private int grauDeSaida;
        private int cor;

        public int Id { get { return id; } } 
        public int GrauDeEntrada { get { return grauDeEntrada; } }
        public int GrauDeSaida { get { return grauDeSaida; } }
        public int Cor { get { return cor; } }

        public Vertice(int id, int grauDeEntrada, int grauDeSaida)
        {
            this.id = id;
            this.grauDeEntrada = grauDeEntrada;
            this.grauDeSaida = grauDeSaida;
            this.cor = -1;
        }

        public Vertice(int id, int grauDeEntrada, int grauDeSaida, int cor)
        {
            this.id = id;
            this.grauDeEntrada = grauDeEntrada;
            this.grauDeSaida = grauDeSaida;
            this.cor = cor;
        }
        

        public int Atualizar_GrauDeEntrda(int novoGrau)
        {
            if (grauDeEntrada >= 0)
            {
                grauDeEntrada = novoGrau;
            }
            return grauDeEntrada;
        }

        public int Atualizar_GrauDeSaida(int novoGrau)
        {
            if (grauDeSaida >= 0)
            {
                grauDeSaida= novoGrau;
            }
            return grauDeSaida;
        }

        public int Atualizar_Cor (int novaCor)
        {
            return cor = novaCor;
        }

   





    }



}