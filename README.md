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
├── Docs/
│   └── Relatorio_TP_Grafos.pdf
├── UML/
│   ├── 01_modelos_base.svg
│   ├── 02_algoritmos_e_servicos.svg
│   ├── 03_fluxos_de_uso.svg
│   ├── 04_sequencia_fluxo_maximo.svg
│   └── 05_sequencia_manutencao_inspecao.svg
├── Data/
│   └── GrafosDimacs/
└── Program.cs
```

## Como executar

```bash
git clone https://github.com/SEU_USUARIO/SORL.git
cd SORL
dotnet run
```

O projeto está configurado para **.NET 10.0**. O menu principal permite escolher o grafo de entrada (entre os disponibilizados em `Data/GrafosDimacs/`) e a análise desejada:

```
1 - Menor custo entre dois hubs
2 - Fluxo máximo e corte mínimo
3 - Árvore Geradora Mínima (rede de comunicação)
4 - Agendamento de manutenções
5 - Percurso de inspeção (Euleriano / Hamiltoniano)
6 - Testar fluxo máximo nos 7 grafos
7 - Testar inspeção nos 7 grafos
0 - Sair
```

Os resultados de cada execução (caminhos, fluxos, custos, turnos e percursos) são registrados automaticamente em `Logs/`.

## Equipe e divisão de responsabilidades

| Membro | Integrante | Frente de trabalho | Funções principais no sistema |
|---|---|---|---|
| **Membro 1** | **Joao Victor Frois** | Arquitetura, base do projeto e algoritmos de custo | Modelagem base (`Grafo`, `Vertice`, `Aresta`), leitura DIMACS, escolha entre lista/matriz, menu, Dijkstra e Kruskal |
| **Membro 2** | **Joao Victor Soares** | Fluxo, capacidade e logs | Capacidades das arestas, grafo residual, fluxo máximo, corte mínimo, logs e saída padronizada |
| **Membro 3** | **Lucas Gabriel** | Manutenção, percursos e documentação final | Grafo de conflitos, coloração, turnos de manutenção, Euleriano, Hamiltoniano, testes finais e relatório |

## Relatório

O relatório técnico de conclusão está em:

```text
Docs/Relatorio_TP_Grafos.pdf
```

Ele responde diretamente às perguntas do trabalho e documenta:
- Estrutura de dados utilizada e critério de escolha (lista vs. matriz);
- Leitura e modelagem do grafo;
- Algoritmo de menor caminho e sua justificativa;
- Árvore geradora mínima;
- Fluxo máximo e corte mínimo, com interpretação logística dos resultados;
- Coloração e geração dos turnos de manutenção;
- Verificação de percursos euleriano e hamiltoniano;
- Resultados gerais obtidos nos 7 grafos de teste.

Os diagramas UML finais estão disponíveis em `UML/` no formato SVG.

## Restrições do trabalho

- Implementado em **C#**.
- Apenas algoritmos estudados em sala de aula.
- Testado com os 7 grafos DIMACS fornecidos na disciplina.

---
Disciplina: Algoritmos em Grafos — PUC Minas, 01/2026.
