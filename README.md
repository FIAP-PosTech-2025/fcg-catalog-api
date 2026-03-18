# TcCatalog API

API em **.NET 8** para catálogo de jogos e gerenciamento de biblioteca de usuários, com autenticação JWT, autorização por papéis e persistência via Entity Framework Core.

## 📦 Tecnologias

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQL Server
- RabbitMQ (mensageria de eventos)
- JWT Bearer Authentication
- Swagger / OpenAPI
- Serilog (logs estruturados)

## 🧱 Estrutura da solução

- `TcCatalog.Api`: camada de apresentação (controllers, middlewares, autenticação, Swagger)
- `TcCatalog.Application`: regras de aplicação, DTOs e serviços
- `TcCatalog.Domain`: entidades e enums de domínio
- `TcCatalog.Infra`: contexto EF Core, mapeamentos, repositórios e migrations

## ✅ Pré-requisitos

- .NET SDK 8.0+
- SQL Server (local ou remoto)
- RabbitMQ (local ou remoto)

## ⚙️ Configuração

1. Ajuste a conexão com banco no arquivo `TcCatalog.Api/appsettings.json` em `ConnectionStrings:DefaultConnection`.
2. Revise as configurações JWT em `Jwt` (`Issuer`, `Audience`, `Secret`, `ExpirationMinutes`).
3. Configure a seção `RabbitMq` em `TcCatalog.Api/appsettings.json` (`HostName`, `Port`, `UserName`, `Password`, `VirtualHost` e nomes das filas).

> Recomenda-se utilizar variáveis de ambiente ou `dotnet user-secrets` para segredos em ambiente local.

## ▶️ Como executar

No diretório raiz do repositório:

```bash
dotnet restore
dotnet build TcCatalog.sln
dotnet run --project TcCatalog.Api
```

Por padrão, a API sobe com Swagger em ambiente de desenvolvimento em `http://localhost:5001/swagger`.

## 🗄️ Banco de dados (migrations)

Para aplicar as migrations existentes:

```bash
dotnet ef database update --project TcCatalog.Infra --startup-project TcCatalog.Api
```

Se necessário criar nova migration:

```bash
dotnet ef migrations add NomeDaMigration --project TcCatalog.Infra --startup-project TcCatalog.Api
```

## 🔐 Autenticação e autorização

A API usa **Bearer Token (JWT)**.

- Endpoints de jogos (`/api/jogos`) exigem autenticação.
- Operações administrativas (criar/editar/remover jogo e administração de biblioteca) exigem papel `Admin`.
- Endpoints de biblioteca do usuário usam o `UserId` do token autenticado.

No Swagger, clique em **Authorize** e informe:

```text
Bearer {seu_token}
```

## 🧭 Principais endpoints

### Jogos

- `GET /api/jogos`
- `GET /api/jogos/{id}`
- `POST /api/jogos` (Admin)
- `PUT /api/jogos/{id}` (Admin)
- `DELETE /api/jogos/{id}` (Admin)
- `POST /api/jogos/{id}/promocao` (Admin)

### Biblioteca do usuário autenticado

- `GET /api/user/biblioteca/me`
- Retorna itens no formato `BibliotecaJogoDto` (dados do jogo + informações da biblioteca):
  - `status` (int): código do status (`0` EmAberto, `1` Pendente, `2` Aprovado, `3` Reprovado)
  - `statusDescricao` (string): descrição amigável do status
  - `payId` (Guid?): identificador do pagamento quando disponível

Exemplo de resposta:

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "descricao": "Jogo Exemplo",
    "genero": "Aventura",
    "preco": 99.90,
    "dataCadastro": "2026-03-07T12:00:00Z",
    "status": 2,
    "statusDescricao": "Pagamento APROVADO",
    "payId": "b3a6fa5b-95f5-4cb2-9f0a-9fd60ab8d2d1"
  }
]
```

### Administração de biblioteca (Admin)

- `POST /api/admin/biblioteca`
- `GET /api/admin/biblioteca?userId={guid}`
- `DELETE /api/admin/biblioteca?userId={guid}&jogoId={guid}`
- `DELETE /api/admin/biblioteca/todos?userId={guid}`
- O endpoint `GET` também retorna `BibliotecaJogoDto` com `status`, `statusDescricao` e `payId`.

### Diagnóstico do usuário autenticado

- `GET /api/UserId/me`

## 📣 Mensageria de eventos (RabbitMQ)

- Ao vincular um jogo para o usuário, a API publica `OrderPlacedEvent` na fila `tccatalog.order.placed`.
- O processamento de pagamento deve publicar `PaymentProcessedEvent` na fila `tccatalog.payment.processed`.
- A API possui um consumidor em background que lê `PaymentProcessedEvent` e atualiza o status da biblioteca.

## 💳 Processamento de pagamento

O evento de pagamento processado usa o contrato:

```csharp
PaymentProcessedEvent(Guid UserId, Guid JogoId, Guid PayId, int Status)
```

Regras para `Status` no processamento:

- Apenas `2` (**Aprovado**) e `3` (**Reprovado**) são aceitos.
- Qualquer outro valor é rejeitado com erro de validação.

## 📝 Logs

Os logs são gravados em:

- `logs/info-*.json` para eventos até nível `Information`
- `logs/error-*.json` para eventos de erro


## 🐳 Docker e ☸️ Kubernetes

### Build e execução com Docker

Suba o RabbitMQ separado (fora do `docker-compose.yml` da API):

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

Painel RabbitMQ (opcional):

```text
http://localhost:15672
```

Credenciais padrão: `guest / guest`.

Com Docker

```bash
docker compose up -d --build
```

Para derrubar os containers:

```bash
docker compose down
```

Swagger disponível em:

```text
http://localhost:5001/swagger
```



### Manifestos Kubernetes

Os manifestos estão na pasta `k8s/` na raiz do projeto e incluem:

- `Deployment` (`k8s/deployment.yaml`) para gerenciamento de pods
- `Service` (`k8s/service.yaml`) para exposição interna
- `ConfigMap` (`k8s/configmap.yaml`) para configurações não sensíveis
- `Secret` (`k8s/secret.yaml`) para dados sensíveis

Aplicação:

```bash
kubectl apply -k k8s/
```

> Antes de aplicar, ajuste os valores de `k8s/secret.yaml` (senhas/chaves).
>
> O `Deployment` usa por padrão `image: tccatalog-api:latest`. Gere essa imagem antes do apply.

Build da imagem local:

```bash
docker build -t tccatalog-api:latest .
```

Se estiver usando **kind**:

```bash
kind load docker-image tccatalog-api:latest
```

Se estiver usando **minikube**:

```bash
minikube image load tccatalog-api:latest
```

### API não sobe após `kubectl apply -k k8s/`?

Checklist rápido para diagnóstico:

1. Verifique se o cluster está acessível:

```bash
kubectl config current-context
kubectl cluster-info
kubectl get nodes
```

2. Verifique status dos recursos da aplicação:

```bash
kubectl get deploy,po,svc
kubectl describe deploy tccatalog-api
kubectl logs deploy/tccatalog-api --all-containers=true --tail=200
```

3. Causas mais comuns neste projeto:

- **Imagem inválida ou não publicada** (`ImagePullBackOff`/`ErrImagePull`): atualize `k8s/deployment.yaml` com uma imagem existente e com acesso pelo cluster.
- **Connection string usando `localhost` no Pod**: em Kubernetes, `localhost` aponta para o próprio container. Configure `ConnectionStrings__DefaultConnection` para o host/Service real do banco.
- **Serviço é `ClusterIP`**: para testar localmente, faça port-forward:

```bash
kubectl port-forward svc/tccatalog-api 5001:5001
```

Depois acesse:

```text
http://localhost:5001/swagger
```

### `http://localhost:5001/swagger` não abre

Esse é o fluxo mínimo para funcionar com `ClusterIP`:

```bash
kubectl apply -k k8s/
kubectl rollout restart deployment/tccatalog-api
kubectl rollout status deployment/tccatalog-api
kubectl port-forward svc/tccatalog-api 5001:5001
```

Se o `kubectl apply -k k8s/` mostrar `unchanged`, isso é normal (nenhuma mudança de manifesto). Para aplicar nova imagem/novo pod, faça `rollout restart`.

Em outro terminal, teste:

```bash
curl -I http://localhost:5001/swagger
```

Atalho:

```bash
./scripts/k8s-open-swagger.sh
```

Se aparecer `exceeded its progress deadline`, normalmente é `ImagePullBackOff`, `CrashLoopBackOff` ou probe falhando. Rode:

```bash
./scripts/k8s-debug.sh
```

Neste projeto, uma causa comum é dependência externa indisponível (ex.: RabbitMQ/SQL Server). O consumidor de RabbitMQ foi preparado para re-tentar conexão sem derrubar a API, mas ainda é necessário corrigir os endpoints/credenciais no `ConfigMap`/`Secret` para o ambiente Kubernetes.

Opcional: script de diagnóstico resumido

```bash
./scripts/k8s-debug.sh
```

## 👨🏻‍🎓 Alunos

- Clovis Ribeiro Ramos - rm369652
- Matheus Machado Pinheiro do Valle - rm369919
- Pedro Delgado Henriques -rm369869
- Jose Mauro Morais Mani -rm
