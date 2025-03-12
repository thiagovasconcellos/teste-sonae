## Teste Técnico

### User-Story:

- Gestão de encomendas, para 1 produto com stock de 100.
- Ao submeter encomenda reserva o produto durante X tempo.
- Após X tempo, caso não concluído com sucesso, retorna produtos ao stock

### Critérios Aceitação Técnica:

- Aplicação Backend .NET 8

  1. Execução de encomendas em determinado momento (tempo)
  2. Consultar estado das encomendas
  3. API para criar novas encomendas
  4. Testes unitários ao CORE das regras exigidas pela userstory.
  5. (Sugestão) Utilização de CQRS ou MVC.

- Aplicação Frontend React
  1. Ecrã para consulta das encomendas
  2. Utilização de Reacthooks, Typescript

### Definition of Done:

    1. Uma API (A) para submissão de encomendas.
    2. Uma API (B) para consultar as encomendas.
    3. Uma API (C) para finalizar a encomenda.
    4. Página (Y) para criar encomenda, indicando quantidade do produto.
    5. Página (X) com a consulta de estado ou quantidade remascente do produto.

### Fora de contexto / ambito do teste:

    1. Não é necessário criar base de dados.

## Submissão do Teste

1. Criar repositório git público com o código da aplicação.
2. Criar pull request com as implementações, com o objetivo de justificar a abordagem do desenvolvimento.
