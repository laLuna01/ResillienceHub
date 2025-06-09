# ResilienceHub API - Documentação do Projeto

Este documento detalha a implementação da API .NET para o projeto ResilienceHub, que integra um sistema de alerta antecipado baseado em IA, gestão de abrigos e recursos, e monitoramento em tempo real de áreas de risco.

## 1. Visão Geral do Projeto

A API foi desenvolvida em .NET (Web API) e segue as melhores práticas de design RESTful, incluindo HATEOAS e Rate Limiting. Ela se conecta a um banco de dados Oracle e utiliza ML.NET para previsão de desastres, além de microsserviços com RabbitMQ para comunicação assíncrona.

## 2. Estrutura do Projeto

O projeto está organizado nas seguintes camadas:

- **ResilienceHub.Api**: Contém os controladores da API, configuração de inicialização, Swagger e injeção de dependências.
- **ResilienceHub.Core**: Define os modelos de dados (entidades) e contratos de mensagens.
- **ResilienceHub.Infrastructure**: Implementa a lógica de acesso a dados (OracleDbContext), serviços de RabbitMQ e ML.NET.
- **ResilienceHub.Tests**: Contém os testes de unidade para os controladores da API.

## 3. Entidades Implementadas

As seguintes entidades principais foram implementadas com operações CRUD (Create, Read, Update, Delete):

- **Abrigo**: Gerenciamento de abrigos, incluindo capacidade, localização e ocupação.
- **Recurso**: Gerenciamento de recursos, como alimentos, medicamentos e cobertores.
- **Usuário**: Gerenciamento de usuários, com diferentes tipos (admin, gerente, voluntário, afetado).
- **Alerta**: Gerenciamento de alertas de desastres, com informações sobre tipo, severidade, área geográfica e fonte (incluindo ML.NET).

Além disso, foram implementadas entidades de relacionamento e tabelas temporárias:

- **AbrigoRecurso**: Associação entre abrigos e recursos.
- **AbrigoUsuario**: Registro de check-in/check-out de usuários em abrigos.
- **AbrigoOcupacaoHistorico**: Histórico de ocupação dos abrigos.
- **TmpUsuariosAlerta**: Tabela temporária para usuários em alerta.

## 4. Boas Práticas de Design de API RESTful

### HATEOAS (Hypermedia as an Engine of Application State)

Todos os controladores principais (Abrigo, Recurso, Usuário, Alerta) foram implementados com suporte a HATEOAS. Isso significa que as respostas da API incluem links relevantes para ações relacionadas, permitindo que os clientes descubram as capacidades da API dinamicamente. Por exemplo, ao buscar um abrigo, a resposta incluirá links para atualizar ou excluir esse abrigo.

### Rate Limiting

Foi configurado um Rate Limiting para proteger a API contra abusos e garantir a disponibilidade. As configurações de Rate Limiting podem ser ajustadas no arquivo `appsettings.json`.

## 5. Documentação da API (Swagger)

A API é auto-documentada via Swagger (OpenAPI). Ao executar o projeto, você pode acessar a documentação interativa em `https://localhost:<port>/swagger` (ou a porta configurada). A documentação inclui todos os endpoints, modelos de dados e exemplos de requisições e respostas, facilitando o consumo da API por outros desenvolvedores.

## 6. Microsserviços com RabbitMQ

Para comunicação assíncrona e desacoplamento de serviços, foi implementado um sistema de mensagens utilizando RabbitMQ. Atualmente, quando um novo alerta é criado (incluindo alertas gerados por ML), uma mensagem é publicada em uma fila do RabbitMQ. Isso permite que outros serviços possam consumir esses eventos e reagir a eles de forma independente.

## 7. Integração com ML.NET para Previsão de Desastres

Foi integrado o ML.NET para a funcionalidade de previsão de desastres. Um serviço `MLService` foi criado para treinar e utilizar um modelo de classificação binária (FastTree) para prever a ocorrência de desastres com base em dados de entrada como tipo, severidade, área geográfica, latitude, longitude e raio afetado. Um endpoint específico (`/api/Alerta/predict-ml`) foi adicionado ao `AlertaController` para permitir que os clientes da API enviem dados e recebam previsões do modelo de ML.

## 8. Cobertura de Testes (XUnit)

O projeto inclui um projeto de testes de unidade (`ResilienceHub.Tests`) utilizando a estrutura XUnit e a biblioteca Moq para simulação de dependências. Foram criados testes para os controladores principais (Abrigo, Recurso, Usuário, Alerta) para garantir a correção e a robustez das funcionalidades da API.

## 9. Endpoints da API

A API expõe os seguintes endpoints:

### AbrigoController

- `POST /api/Abrigo`: Cria um novo abrigo.
- `PUT /api/Abrigo/{id}`: Atualiza um abrigo existente.
- `DELETE /api/Abrigo/{id}`: Exclui um abrigo.
- `GET /api/Abrigo/{id}`: Obtém um abrigo pelo ID.
- `GET /api/Abrigo/proximos/{latitude}/{longitude}/{raioKm}`: Obtém abrigos próximos a uma localização.

### RecursoController

- `POST /api/Recurso`: Cria um novo recurso.
- `PUT /api/Recurso/{id}`: Atualiza um recurso existente.
- `DELETE /api/Recurso/{id}`: Exclui um recurso.
- `GET /api/Recurso/{id}`: Obtém um recurso pelo ID.
- `GET /api/Recurso/abrigo/{idAbrigo}`: Obtém recursos associados a um abrigo.

### UsuarioController

- `POST /api/Usuario`: Cria um novo usuário.
- `PUT /api/Usuario/{id}`: Atualiza um usuário existente.
- `DELETE /api/Usuario/{id}`: Exclui um usuário.
- `GET /api/Usuario/{id}`: Obtém um usuário pelo ID.
- `GET /api/Usuario/alerta/{idAlerta}`: Obtém usuários em alerta.
- `POST /api/Usuario/login`: Realiza o login de um usuário.

### AlertaController

- `POST /api/Alerta`: Cria um novo alerta.
- `PUT /api/Alerta/{id}`: Atualiza um alerta existente.
- `DELETE /api/Alerta/{id}`: Exclui um alerta.
- `GET /api/Alerta/{id}`: Obtém um alerta pelo ID.
- `GET /api/Alerta/ativos`: Obtém todos os alertas ativos.
- `GET /api/Alerta/tipo/{tipo}`: Obtém alertas por tipo.
- `GET /api/Alerta/severidade/{severidade}`: Obtém alertas por severidade.
- `POST /api/Alerta/ml-generated`: Cria um alerta gerado por ML.NET.
- `POST /api/Alerta/predict-ml`: Realiza uma previsão de alerta usando o modelo ML.NET.

### AbrigoRecursoController

- `POST /api/AbrigoRecurso`: Associa um recurso a um abrigo.
- `DELETE /api/AbrigoRecurso/{idAbrigo}/{idRecurso}`: Desassocia um recurso de um abrigo.
- `GET /api/AbrigoRecurso/abrigo/{idAbrigo}`: Obtém recursos associados a um abrigo.

### AbrigoUsuarioController

- `POST /api/AbrigoUsuario/checkin`: Registra o check-in de um usuário em um abrigo.
- `POST /api/AbrigoUsuario/checkout`: Registra o check-out de um usuário de um abrigo.
- `GET /api/AbrigoUsuario/abrigo/{idAbrigo}`: Obtém usuários atualmente em um abrigo.

## 10. Como Executar o Projeto

### Pré-requisitos

- .NET SDK 6.0 ou superior
- Docker (para RabbitMQ) ou uma instância de RabbitMQ em execução
- Acesso a um banco de dados Oracle com o schema `resilience_pkg` e as tabelas criadas conforme o `pasted_content.txt`.

### Configuração do Banco de Dados

Certifique-se de que a `ConnectionString` no arquivo `ResilienceHub.Api/appsettings.json` esteja configurada corretamente para o seu banco de dados Oracle:

```json
"ConnectionStrings": {
  "OracleConnection": "User Id=rm552621Password=200804;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=ORCL)));"
}
```

### Configuração do RabbitMQ

Se você estiver usando Docker, pode iniciar uma instância do RabbitMQ com o seguinte comando:

```bash
docker run -d --hostname my-rabbit --name some-rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

Certifique-se de que as configurações do RabbitMQ no `appsettings.json` estejam corretas:

```json
"RabbitMQ": {
  "HostName": "localhost",
  "UserName": "guest",
  "Password": "guest"
}
```

### Execução da API

1. Navegue até o diretório raiz do projeto (`ResilienceHub`):
   ```bash
   cd ResilienceHub
   ```

2. Restaure as dependências:
   ```bash
   dotnet restore
   ```

3. Compile o projeto:
   ```bash
   dotnet build
   ```

4. Execute a API:
   ```bash
   dotnet run --project ResilienceHub.Api
   ```

A API estará disponível em `https://localhost:<porta_aleatoria>` (a porta será exibida no console). Você pode acessar a documentação Swagger em `https://localhost:<porta_aleatoria>/swagger`.

## 11. Instruções de Testes

O projeto inclui um projeto de testes de unidade (`ResilienceHub.Tests`) utilizando a estrutura XUnit e a biblioteca Moq para simulação de dependências. Para executar os testes:

1. Navegue até o diretório raiz do projeto (`ResilienceHub`):
   ```bash
   cd ResilienceHub
   ```

2. Execute os testes:
   ```bash
   dotnet test ResilienceHub.Tests
   ```

## 12. Considerações Finais

Esta API fornece uma base robusta para o projeto ResilienceHub, com funcionalidades essenciais de gerenciamento de dados, integração com sistemas externos (RabbitMQ) e capacidades de inteligência artificial (ML.NET). A arquitetura modular e a adesão às boas práticas de desenvolvimento garantem escalabilidade, manutenibilidade e extensibilidade futuras.

Para futuras melhorias, pode-se considerar:

- Implementação de autenticação e autorização mais robustas (ex: JWT).
- Adição de logging e monitoramento mais detalhados.
- Otimização das consultas ao banco de dados Oracle.
- Expansão do modelo ML.NET com mais dados e algoritmos avançados.
- Implementação de consumidores de mensagens RabbitMQ para processamento assíncrono de alertas.


