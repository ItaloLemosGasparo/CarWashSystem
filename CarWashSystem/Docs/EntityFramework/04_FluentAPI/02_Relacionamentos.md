# Relações em EF6 (Fluent API) — 1:N, N:1, 1:1, 0..1:1 e N:N

Este arquivo explica, de forma prática e didática, como modelar e configurar os principais tipos de relacionamento no **Entity Framework 6** usando **Fluent API** (`EntityTypeConfiguration<T>`). Cada exemplo contém orientações sobre **em qual lado configurar**, onde a **FK** fica e como tratar `WillCascadeOnDelete`.

---

## Sumário

* [Conceitos rápidos](#conceitos-rapidos)
* [1:N (um-para-muitos)](#rel-1n-um-para-muitos)
* [N:1 (muitos-para-um)](#n1-muitos-para-um)
* [1:1 (um-para-um)](#rel-11-um-para-um)
* [0..1:1 (opcional-para-um)](#rel-011-opcional-para-um)
* [N:N (muitos-para-muitos) no EF6](#nn-muitos-para-muitos-no-ef6)
* [Decidir onde configurar (regra prática)](#decidir-onde-configurar-regra-pratica)
* [Comportamento de Cascade Delete — recomendações](#comportamento-de-cascade-delete-recomendacoes)
* [Tabela de referência rápida (sintaxe EF6)](#tabela-de-referencia-rapida-sintaxe-ef6)
* [Exemplos aplicados ao projeto (resumo)](#exemplos-aplicados-ao-projeto-resumo)

---

## Conceitos rápidos

* **Entidade principal**: a entidade que não contém a FK; é a “dona” conceitual da relação.
* **Entidade dependente**: a entidade que contém a FK (por exemplo `VehicleId` em `ServiceOrder`).
* **Navegação**: propriedade que aponta para a entidade relacionada (`virtual Client Client { get; set; }` ou `ICollection<ServiceOrder> ServiceOrders`).
* **Lazy Loading**: habilitado por `virtual` em propriedades de navegação.

---

## 1:N um-para-muitos {#rel-1n-um-para-muitos}

**Descrição**: Um registro em A corresponde a muitos registros em B. Ex.: `Client` (1) → `Vehicle` (N).

**Onde está a FK**: na entidade dependente (ex.: `Vehicle.ClientId`).

**Configurar no lado dependente (recomendado)**: configure no `VehicleConfiguration` ou no `ClientConfiguration` — escolha o lado que faz mais sentido no seu layout de código, mas configure apenas em **um** lugar.

**Exemplo (configurar no ClientConfiguration)**:

```csharp
HasMany(c => c.Vehicles)                     // Cliente tem muitos Vehicles
    .WithRequired(v => v.Client)              // cada Vehicle precisa de um Client
    .HasForeignKey(v => v.ClientId)           // FK em Vehicle.ClientId
    .WillCascadeOnDelete(false);              // não deletar vehicles ao deletar client (decisão de negócio)
```

**Observações**:

* Não duplique a configuração no `VehicleConfiguration`.
* Use `WillCascadeOnDelete(true)` somente se for seguro apagar dependentes automaticamente (ex.: itens de pedido).

---

## N:1 muitos-para-um

**Descrição**: É a mesma relação de 1:N, mas vista do outro lado — cada `ServiceItem` (N) refere-se a um `Service` (1).

**Exemplo (configurar no lado dependente `ServiceItemConfiguration`)**:

```csharp
HasRequired(si => si.Service)                 // ServiceItem precisa de Service
    .WithMany()                               // Service pode ter muitos ServiceItems
    .HasForeignKey(si => si.ServiceId)        // FK em ServiceItem.ServiceId
    .WillCascadeOnDelete(false);
```

---

## 1:1 um-para-um {#rel-11-um-para-um}

**Descrição**: Cada registro em A corresponde a exatamente 1 registro em B e vice-versa.

**Onde colocar a FK**: em apenas uma das tabelas; escolha quem é dependente.

**Sintaxe (exemplo)**:

* Se **A é principal** e **B é dependente** (B tem FK):

```csharp
// Configurar a partir do principal (A)
HasOptional(a => a.B)                         // A pode ter B (ou usar HasRequired se for obrigatório)
    .WithRequired(b => b.A);                  // B requer A (1:1)
```

* Quando precisar ser explícito sobre principal/dependent:

```csharp
HasOptional(a => a.B)
    .WithRequiredDependent(b => b.A);        // b é dependente

// ou explicitando o principal
HasRequired(b => b.A)
    .WithRequiredPrincipal(a => a.B);        // a é principal
```

**Observação**: 1:1 é raro; avalie se não é redundância (dados duplicados).

---

## 0..1:1 (opcional-para-um) {#rel-011-opcional-para-um}

**Descrição**: Um lado pode ter zero ou um e o outro tem exatamente um. Ex.: `ServiceOrder` (0..1) → `Payment` (1).

**Sintaxe recomendada no EF6**:

```csharp
HasOptional(so => so.Payment)                 // ServiceOrder pode não ter Payment (0..1)
    .WithRequired(p => p.ServiceOrder)        // Payment precisa de ServiceOrder (1)
    .WillCascadeOnDelete(false);              // escolha conforme necessidade de histórico
```

**Notas sobre `WithRequiredPrincipal` e `WithRequiredDependent`**:

* Use `WithRequiredDependent` quando quiser deixar claro que o lado passado é o dependente.
* Use `WithRequiredPrincipal` quando quiser declarar explicitamente qual é o principal.
* Na prática `HasOptional(...).WithRequired(...)` costuma ser suficiente.

---

## N:N (muitos-para-muitos) no EF6

EF6 simplifica N:N sem entidade de junção explícita (somente até o EF6 e sem payload). Se precisar de campos adicionais na associação (payload), modelar uma entidade de junção (ex.: `TagAssignment`) é a melhor prática.

**Exemplo simples (sem payload)**:

```csharp
// Supondo que Category e Product possuem coleções um do outro
modelBuilder.Entity<Product>()
    .HasMany(p => p.Categories)
    .WithMany(c => c.Products)
    .Map(m => {
        m.ToTable("ProductCategories");
        m.MapLeftKey("ProductId");
        m.MapRightKey("CategoryId");
    });
```

**Quando usar entidade de junção**: sempre que precisar guardar dados sobre a relação (ex.: data de associação, quantidade) ou controlar a FK diretamente.

---

## Decidir onde configurar (regra prática)

* **Configure do lado dependente** (a entidade que tem a FK) para 1:N / N:1 — é direto e evita confusão.
* **Configure no principal** quando a navegação principal existe somente no principal e você prefere centralizar lá.
* **Nunca duplique** a configuração nos dois lados — evita conflito.

Regra curta: **configure onde está a FK**.

---

## Comportamento de Cascade Delete — recomendações

* `WillCascadeOnDelete(true)`: use para dependentes que não fazem sentido sem o principal (ex.: `ServiceItems` de `ServiceOrder`).
* `WillCascadeOnDelete(false)`: use quando precisar preservar histórico ou evitar remoções acidentais (ex.: `Payment`, `Vehicle`).
* Teste as deleções em um ambiente de desenvolvimento (LocalDB) para verificar o comportamento real.

---

## Tabela de referência rápida (sintaxe EF6)

| Relação | Sintaxe típica (lado dependente)                        | Significado rápido                              |
| ------: | ------------------------------------------------------- | ----------------------------------------------- |
|     1:N | `HasMany(a => a.Bs).WithRequired(b => b.A)...`          | A tem muitos B; B tem FK para A                 |
|     N:1 | `HasRequired(b => b.A).WithMany(a => a.Bs)...`          | B depende de A                                  |
|     1:1 | `HasRequired(b => b.A).WithRequiredPrincipal(a => a.B)` | 1:1 com principal/dependente explícitos         |
|  0..1:1 | `HasOptional(a => a.B).WithRequired(b => b.A)`          | A pode não ter B; B precisa de A                |
|     N:N | `HasMany().WithMany().Map(...)`                         | Tabela intermediária gerada (ou manual via Map) |

---

## Exemplos aplicados ao projeto (resumo)

* **Client ↔ Vehicle (1:N)**

```csharp
// Configure em ClientConfiguration (lado 1)
HasMany(c => c.Vehicles)
    .WithRequired(v => v.Client)
    .HasForeignKey(v => v.ClientId)
    .WillCascadeOnDelete(false); // mantém vehicles mesmo se client for apagado
```

* **ServiceOrder ↔ ServiceItem (1:N)**

```csharp
// Configure em ServiceOrderConfiguration (lado 1)
HasMany(so => so.ServiceItems)
    .WithRequired(si => si.ServiceOrder)
    .HasForeignKey(si => si.ServiceOrderId)
    .WillCascadeOnDelete(true); // itens removidos com a ordem
```

* **ServiceOrder ↔ Payment (0..1:1)**

```csharp
// Configure em ServiceOrderConfiguration (lado opcional)
HasOptional(so => so.Payment)
    .WithRequired(p => p.ServiceOrder)
    .WillCascadeOnDelete(false); // preserve Payments por histórico
```

* **User ↔ UserPhone (1:N)**

```csharp
// Configure em UserPhoneConfiguration (lado dependente)
HasRequired(up => up.User)
    .WithMany(u => u.PhoneNumbers)
    .HasForeignKey(up => up.UserId)
    .WillCascadeOnDelete(true); // telefones removidos com o usuário
```

---

## Nota final

Documento gerado com auxílio de IA.

---

Fim do documento.
