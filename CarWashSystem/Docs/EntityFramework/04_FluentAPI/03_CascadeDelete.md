# Cascade Delete — Obrigação, Efeitos e Boas Práticas (EF6 / .NET Framework 4.8)

Este documento explica de forma prática e didática o comportamento de **cascade delete** no **Entity Framework 6**, como configurá-lo com **Fluent API** (`WillCascadeOnDelete`), quais efeitos ele provoca no banco de dados (SQL Server / LocalDB) e as melhores práticas para evitar problemas como múltiplos caminhos em cascade e perda de histórico.

---

## Índice

* [O que é Cascade Delete](#o-que-e-cascade-delete)
* [Como configurar no EF6 (Fluent API)](#como-configurar-no-ef6-fluent-api)
* [Exemplos práticos com comentários inline](#exemplos-praticos-com-comentarios-inline)
* [Comportamento no banco de dados (SQL Server / LocalDB)](#comportamento-no-banco-de-dados-sql-server-localdb)
* [Problemas comuns e como resolver](#problemas-comuns-e-como-resolver)
* [Alternativas ao Cascade Delete (soft delete, manual delete)](#alternativas-ao-cascade-delete-soft-delete-manual-delete)
* [Testes e verificação](#testes-e-verificacao)
* [Boas práticas resumidas](#boas-praticas-resumidas)

---

## O que é Cascade Delete

**Cascade delete** é um comportamento que faz com que, ao excluir um registro na tabela principal, o banco (ou o EF) apague automaticamente os registros dependentes que referenciam aquele registro via chave estrangeira (FK). No EF6 isso é configurado via Fluent API com `WillCascadeOnDelete(true)`.

---

## Como configurar no EF6 (Fluent API)

A configuração é feita na `EntityTypeConfiguration<T>` usando `WillCascadeOnDelete(true/false)` em conjunto com `HasRequired/HasOptional/HasMany/WithRequired/WithMany`.

Exemplo sintaxe:

```csharp
HasMany(a => a.Children)                 // A tem muitos Children
    .WithRequired(c => c.Parent)         // cada Child precisa de Parent
    .HasForeignKey(c => c.ParentId)      // FK no Child.ParentId
    .WillCascadeOnDelete(true);          // deletar Parent apaga Children (cascade)
```

`WillCascadeOnDelete(false)` indica que o banco **não** deve deletar dependentes automaticamente; você precisará remover manualmente os filhos ou evitar a exclusão do pai.

---

## Exemplos práticos com comentários inline

```csharp
// Exemplo 1 — Itens da ordem: deletar ordem apaga itens
HasMany(so => so.ServiceItems)            // ServiceOrder tem muitos ServiceItems
    .WithRequired(si => si.ServiceOrder)  // ServiceItem precisa de ServiceOrder
    .HasForeignKey(si => si.ServiceOrderId)
    .WillCascadeOnDelete(true);           // deletar ServiceOrder -> deleta ServiceItems

// Exemplo 2 — Cliente e veículos: normalmente NÃO deletar veículos junto
HasMany(c => c.Vehicles)                  // Client tem muitos Vehicles
    .WithRequired(v => v.Client)          // Vehicle precisa de Client
    .HasForeignKey(v => v.ClientId)
    .WillCascadeOnDelete(false);          // evitar exclusão de vehicles ao deletar client

// Exemplo 3 — Ordem de serviço e pagamento: preserve pagamento para histórico
HasOptional(so => so.Payment)             // ServiceOrder pode não ter Payment
    .WithRequired(p => p.ServiceOrder)    // Payment precisa de ServiceOrder
    .WillCascadeOnDelete(false);          // preserve Payment (não apagar com a ordem)

// Exemplo 4 — Usuário e telefones: deletar usuário apaga telefones (limpeza de dados)
HasRequired(up => up.User)                // UserPhone precisa de User
    .WithMany(u => u.PhoneNumbers)        // User tem muitos UserPhone
    .HasForeignKey(up => up.UserId)
    .WillCascadeOnDelete(true);           // deletar User -> deleta telefones
```

---

## Comportamento no banco de dados (SQL Server / LocalDB)

* Quando `WillCascadeOnDelete(true)` é aplicado, o **EF gera a constraint FK no banco com `ON DELETE CASCADE`** (quando possível). Assim, a exclusão é executada diretamente pelo mecanismo do banco.
* **Vantagem:** a exclusão em cascata no nível do banco é eficiente (menos roundtrips) e garante integridade referencial mesmo fora do EF.
* **Observação importante:** o SQL Server **restringe** a criação de múltiplos caminhos de cascade entre tabelas ("multiple cascade paths"). Se você tiver várias FKs com `ON DELETE CASCADE` que podem levar a um mesmo registro dependente por caminhos diferentes, o SQL Server pode rejeitar a criação da constraint ou lançar erro em tempo de execução.

---

## Problemas comuns e como resolver

### 1. Multiple cascade paths (SQL Server)

**Sintoma:** ao aplicar migrations ou criar o schema, o SQL Server reclama sobre "multiple cascade paths". Isso ocorre quando existem dois ou mais caminhos de cascade que podem atingir a mesma linha na tabela dependente.

**Soluções**:

* Remova `WillCascadeOnDelete(true)` de uma das relações e trate a deleção manualmente no código (ou via trigger).
* Use `WillCascadeOnDelete(false)` e implemente a exclusão em transação no lado do aplicativo (remover filhos primeiro, depois o pai).
* Modelar a relação de forma diferente (por exemplo, usar soft delete ou entidade de junção) para evitar caminhos múltiplos.

### 2. Perda de histórico (apagando registros importantes)

**Solução:** para entidades financeiras ou de auditoria (ex.: `Payment`) prefira `WillCascadeOnDelete(false)` e registre histórico em tabelas apropriadas ou mantenha o registro mesmo após a exclusão do pai.

### 3. Cascading profundo e performance

Excluir um nó que aciona muitas cascades pode gerar uma operação muito pesada: considere operações em lote, índices adequados e, se necessário, deletar em etapas dentro de uma transação.

---

## Alternativas ao Cascade Delete (quando não usar) {#alternativas-ao-cascade-delete-soft-delete-manual-delete}

1. **Soft delete** — adicione `IsDeleted` e filtre queries (boa para histórico).
2. **Deleção manual em código** — carregar dependentes e remover explicitamente dentro de uma transação.
3. **Triggers no banco** — implementar lógica complexa de remoção no banco (uso geralmente desencorajado por teste/portabilidade).

---

## Testes e verificação

* Execute testes em LocalDB/SQL Server e verifique as FKs no SSMS: a FK deve ter `DELETE CASCADE` no comportamento quando configurado.
* Teste cenários com múltiplos caminhos de cascade para garantir que o banco aceite o esquema.
* Use `Database.Initialize(true)` em ambiente de desenvolvimento para recriar o schema e validar as constraints.

---

## Boas práticas resumidas

* Configure cascade apenas para relações cuja vida do dependente **é estritamente ligada** ao pai (ex.: itens de pedido).
* Para entidades financeiras, registros de auditoria e históricos, **não** habilite cascade — preserve dados.
* Configure a relação **no lado dependente** (onde está a FK) e faça a validação do `WillCascadeOnDelete` ali.
* Quando enfrentar "multiple cascade paths", prefira `WillCascadeOnDelete(false)` e deleção manual controlada.
* Teste em LocalDB antes de aplicar em produção.

---

Documento gerado com auxílio de IA.