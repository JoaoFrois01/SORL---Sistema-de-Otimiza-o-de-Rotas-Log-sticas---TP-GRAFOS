# SORL — Sistema de Otimização de Rotas Logísticas

Trabalho Prático da disciplina **Algoritmos em Grafos** — PUC Minas (01/2026).

## Contexto

A **Entrega Máxima Logística S.A.** é uma transportadora nacional que enfrenta atrasos, custos elevados e gargalos operacionais em sua malha de transporte. Este projeto implementa o **SORL**, um sistema que modela a rede logística da empresa como um **grafo direcionado e ponderado** e responde a perguntas estratégicas do departamento de planejamento por meio de algoritmos clássicos de grafos.

## Modelagem do Grafo

| Elemento | Representa |
|---|---|
| Vértice | Centro de Distribuição (Hub) ou Ponto de Entrega |
| Aresta | Rota Rodoviária / Ligação Viária entre dois hubs |
| Peso da aresta | Custo financeiro (R$) por unidade de carga transportada |
| Capacidade da aresta | Limite máximo diário (toneladas) que a rota suporta |

O grafo é construído a partir de arquivos no **formato DIMACS**, e a estrutura de dados (lista ou matriz de adjacência) é escolhida automaticamente em função da densidade do grafo.

### Formato DIMACS utilizado

```
<num_vertices> <num_arestas>
<origem> <destino> <peso> <capacidade>
...
```

A primeira linha define o tamanho do grafo. Cada linha seguinte define uma aresta direcionada, com peso (custo) e capacidade.

## Problemas resolvidos

| # | Pergunta de negócio | Problema de grafos | Algoritmo |
|---|---|---|---|
| 1 | Trajeto mais econômico entre dois hubs | Menor caminho | Dijkstra |
| 2 | Limite diário de escoamento entre origem (S) e destino (T) | Fluxo máximo / Corte mínimo | Edmonds-Karp |
| 3 | Conectar todos os hubs ao menor custo | Árvore Geradora Mínima | Kruskal |
| 4 | Cronograma de manutenção sem conflito de recursos | Coloração de grafo (grafo de conflitos) | Coloração |
| 5a | Percurso único pelas rotas (arestas) | Ciclo Euleriano | Verificação + construção de circuito euleriano |
| 5b | Percurso único pelos hubs (vértices) | Ciclo Hamiltoniano | Verificação + construção de circuito hamiltoniano |

## Estrutura do projeto

```
SORL/
├── Models/
│   ├── Grafo.cs
│   ├── Vertice.cs
│   └── Aresta.cs
├── Algorithms/
│   ├── Dijkstra.cs
│   ├── Kruskal.cs
│   ├── EdmondsKarp.cs
│   ├── Coloracao.cs
│   ├── Euleriano.cs
│   └── Hamiltoniano.cs
├── Utils/
│   ├── LeitorDimacs.cs
│   └── LogService.cs
├── Logs/
├── Data/
│   └── grafos_dimacs/
└── Program.cs
```

## Como executar

```bash
git clone https://github.com/SEU_USUARIO/SORL.git
cd SORL
dotnet run
```

O menu principal permite escolher o grafo de entrada (entre os disponibilizados em `Data/grafos_dimacs/`) e a análise desejada:

```
1 - Menor custo entre dois hubs
2 - Fluxo máximo e corte mínimo
3 - Árvore Geradora Mínima (rede de comunicação)
4 - Agendamento de manutenções
5 - Percurso de inspeção (Euleriano / Hamiltoniano)
0 - Sair
```

Os resultados de cada execução (caminhos, fluxos, custos, turnos e percursos) são registrados automaticamente em `Logs/`.

## Equipe e divisão de responsabilidades

| Integrante | Frente de trabalho |
|---|---|
| **Membro 1** | Arquitetura do projeto, classes base (`Grafo`, `Vertice`, `Aresta`), leitura DIMACS, escolha automática de estrutura de dados, menu principal, Dijkstra (menor caminho), Kruskal (AGM) |
| **Membro 2** | Capacidades e grafo residual, fluxo máximo e corte mínimo (Edmonds-Karp), serviço de logs, padronização da saída no console |
| **Membro 3** | Modelagem do grafo de conflitos, coloração (turnos de manutenção), verificação de percursos euleriano e hamiltoniano, testes finais e relatório técnico |

## Relatório

O relatório técnico documenta:
- Estrutura de dados utilizada e critério de escolha (lista vs. matriz);
- Leitura e modelagem do grafo;
- Algoritmo de menor caminho e sua justificativa;
- Árvore geradora mínima;
- Fluxo máximo e corte mínimo, com interpretação logística dos resultados;
- Coloração e geração dos turnos de manutenção;
- Verificação de percursos euleriano e hamiltoniano;
- Resultados gerais obtidos nos 7 grafos de teste.

## Restrições do trabalho

- Implementado em **C#**.
- Apenas algoritmos estudados em sala de aula.
- Testado com os 7 grafos DIMACS fornecidos na disciplina.

---
Disciplina: Algoritmos em Grafos — PUC Minas, 01/2026.
