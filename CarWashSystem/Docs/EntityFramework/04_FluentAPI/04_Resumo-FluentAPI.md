# Resumo Rápido — Fluent API (EF6) — Cheatsheet e Boas Práticas

Este documento é um resumo prático e objetivo das configurações mais comuns feitas com **Fluent API** no **Entity Framework 6** (EF6). Use-o como referência rápida enquanto implementa `EntityTypeConfiguration<T>` no seu projeto (.NET Framework 4.8).

---

## Guia rápido: quando usar cada método

* `HasKey(x => x.Id)` — define a chave primária.
* `ToTable("Name")` — mapeia a entidade para a tabela.
* `Property(x => x.Prop).IsRequired().HasMaxLength(n)` — configurações de propriedade.
* `HasPrecision(precision, scale)` — define precisão para `decimal`.
* `HasMany(a => a.Bs)` / `WithMany()` — inicia mapeamento 1:N ou N:N.
* `HasRequired(a => a.B)` / `HasOptional(a => a.B)` — inicia mapeamento onde o lado que chama tem nav obrigatória/opcional.
* `WithRequired(...)`, `WithRequiredDependent(...)`, `WithRequiredPrincipal(...)` — use para 1:1 e 0..1:1 quando precisar explicitar principal/dependente.
* `HasForeignKey(x => x.Fk)` — aponta qual propriedade é a FK.
* `WillCascadeOnDelete(true|false)` — ativa/desativa cascade delete no nível do banco.

---

## Padrões práticos (cheatsheet de código)

### 1. 1:N — ex.: Client (1) → Vehicle (N)

```csharp
// Configure no lado dependente (recomendado) ou no lado 1
HasMany(c => c.Vehicles)                   // Cliente tem muitos Vehicles
    .WithRequired(v => v.Client)           // cada Vehicle precisa de um Client
    .HasForeignKey(v => v.ClientId)        // FK em Vehicle.ClientId
    .WillCascadeOnDelete(false);           // escolher conforme negócio
```

### 2. N:1 — ex.: ServiceItem (N) → Service (1)

```csharp
HasRequired(si => si.Service)              // ServiceItem precisa de Service
    .WithMany()                            // Service pode ter muitos ServiceItems
    .HasForeignKey(si => si.ServiceId)
    .WillCascadeOnDelete(false);
```

### 3. 0..1:1 — ex.: ServiceOrder (0..1) → Payment (1)

```csharp
HasOptional(so => so.Payment)              // ServiceOrder pode não ter Payment
    .WithRequired(p => p.ServiceOrder)     // Payment precisa do ServiceOrder
    .WillCascadeOnDelete(false);            // preserve Payment se necessário
```

### 4. 1:1 (principal/dependente explícito)

```csharp
// A é principal, B é dependente (B tem a FK)
HasRequired(b => b.A)                      // B depende de A
    .WithRequiredPrincipal(a => a.B);      // A é principal, B é dependente
```

### 5. N:N (EF6) — map manual da tabela de junção

```csharp
modelBuilder.Entity<Product>()
    .HasMany(p => p.Categories)
    .WithMany(c => c.Products)
    .Map(m => {
        m.ToTable("ProductCategories");
        m.MapLeftKey("ProductId");
        m.MapRightKey("CategoryId");
    });
```

---

## Onde configurar a relação (regra prática)

* **Configure no lado dependente** (a entidade que tem a FK). Ex.: `ServiceItem` tem `ServiceId` → configure em `ServiceItemConfiguration` ou no `ServiceOrderConfiguration` se você já tem a coleção do outro lado.
* **Não duplique** a configuração em ambas as `Configuration` (evita conflito).
* Configure em quem faz sentido para a leitura/manutenção do código (normalmente a entity com FK).

---

## Cascade delete — recomendações resumidas

* `true` para relações onde o filho **não faz sentido sem o pai** (ex.: itens de pedido).
* `false` para preservar históricos (ex.: pagamentos, logs) e evitar múltiplos caminhos de cascade.
* Se houver erro de "multiple cascade paths" no SQL Server, prefira `WillCascadeOnDelete(false)` e implemente deleção manual ou soft delete.

---

## Problemas comuns (e soluções rápidas)

* **`HasIndex` não existe no EF6**: crie índices via migrations `Sql()` ou scripts manuais.
* **Multiple cascade paths**: remover cascade em uma das relações ou usar deleção manual.
* **Ambiguidade 1:1**: use `WithRequiredPrincipal` / `WithRequiredDependent` para clarificar onde a FK estará.
* **Enums**: por padrão são persistidos como `int` no EF6 — se quiser string, use campo auxiliar.

---

## Boas práticas de implementação

* Separe as `Configuration` em arquivos por entidade (uma classe por arquivo).
* Agrupe todas as configurations em uma pasta `Configurations` e registre-as no `DbContext.OnModelCreating` com `modelBuilder.Configurations.Add(...)`.
* Prefira Fluent API para relacionamentos e regras de persistência; mantenha DataAnnotations para validações simples quando conveniente.
* Documente decisões de cascade/índices em comentários próximos ao código e na documentação do projeto.

---

## Exemplo rápido final (ServiceOrder — resumo)

```csharp
// ServiceOrderConfiguration (lado dependente)
HasRequired(so => so.Vehicle)                // cada ServiceOrder precisa de um Vehicle
    .WithMany(v => v.ServiceOrders)          // Vehicle pode ter muitas ServiceOrders
    .HasForeignKey(so => so.VehicleId)
    .WillCascadeOnDelete(false);

HasMany(so => so.ServiceItems)               // ServiceOrder tem muitos ServiceItems
    .WithRequired(si => si.ServiceOrder)
    .HasForeignKey(si => si.ServiceOrderId)
    .WillCascadeOnDelete(true);

HasOptional(so => so.Payment)                // ServiceOrder pode não ter Payment (0..1)
    .WithRequired(p => p.ServiceOrder)       // Payment precisa do ServiceOrder
    .WillCascadeOnDelete(false);
```

---

Documento gerado com auxílio de IA.