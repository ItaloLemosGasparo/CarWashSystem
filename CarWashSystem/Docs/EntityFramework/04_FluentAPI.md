# Introdução ao Fluent API — Entity Framework 6 (.NET Framework 4.8)

## Objetivo

Este arquivo apresenta o que é a **Fluent API** no Entity Framework 6, quando e por que usá-la, vantagens e desvantagens, e inclui exemplos práticos de `EntityTypeConfiguration<T>` com comentários inline explicativos no estilo que você mencionou (comentários ao lado das linhas de configuração).

---

## Índice desta página

* [O que é Fluent API](#o-que-e-fluent-api)
* [Quando usar Fluent API](#quando-usar-fluent-api)
* [Vantagens e desvantagens](#vantagens-e-desvantagens)
* [Estrutura básica de uma classe de configuração](#estrutura-basica-de-uma-classe-de-configuracao)
* [Exemplos práticos (com comentários inline)](#exemplos-praticos-com-comentarios-inline)
* [Boas práticas rápidas](#boas-praticas-rapidas)
* [Links para aprofundamento](#links-para-aprofundamento)

---

## O que é Fluent API

A **Fluent API** é uma API em estilo encadeado (métodos "fluentes") usada para declarar o mapeamento entre as classes de domínio (entidades) e o esquema do banco de dados. No EF6 isso é feito criando classes que herdam de `EntityTypeConfiguration<T>` e implementando regras como nome de tabela, chaves, propriedades e relacionamentos.

A ideia central é separar regras de persistência do modelo de domínio (sem poluir as classes com muitos atributos) e permitir configurações avançadas que não são possíveis apenas com anotações (DataAnnotations).

---

## Quando usar Fluent API

Use Fluent API quando:

* Você precisa de mapeamentos mais complexos (índices compostos via migration, mapas 1:1 com principal/dependente explícito, herança complexa).
* Prefere manter a classe de domínio limpa (sem muitos atributos).
* Quer centralizar todas as configurações de persistência em uma pasta/assembly específica (facilita manutenção).

Não é obrigatório para casos simples — atributos (`[Required]`, `[MaxLength]`, `[ForeignKey]`) podem ser suficientes — mas Fluent API oferece controle e previsibilidade.

---

## Vantagens e desvantagens

**Vantagens**

* Configuração centralizada e mais poderosa que DataAnnotations.
* Melhor para grandes projetos onde regras de persistência podem mudar.
* Facilita separar responsabilidades (modelo vs persistência).

**Desvantagens**

* Verbosidade: mais código de configuração.
* Curva de aprendizado inicial (métodos `WithRequiredPrincipal`, `WithRequiredDependent`, etc.).

---

## Estrutura básica de uma classe de configuração

Uma classe típica de configuração no EF6 tem a forma:

```csharp
public class EntidadeConfiguration : EntityTypeConfiguration<Entidade>
{
    public EntidadeConfiguration()
    {
        ToTable("Entidades");               // Mapeia para a tabela
        HasKey(e => e.Id);                    // Define chave primária

        Property(e => e.Nome)
            .IsRequired()                     // Obrigatório
            .HasMaxLength(200);               // Tamanho máximo

        // Relações (exemplos a seguir)
    }
}
```

---

## Exemplos práticos (com comentários inline)

A seguir exemplos que ilustram configurações comuns. Cada linha crítica tem um comentário explicando sua finalidade.

### ClientConfiguration (1:N com Vehicle e ClientPhone)

```csharp
public class ClientConfiguration : EntityTypeConfiguration<Client>
{
    public ClientConfiguration()
    {
        ToTable("Clients");                              // Mapeia para a tabela 'Clients'
        HasKey(c => c.Id);                                // PK

        Property(c => c.Name)
            .IsRequired()                                 // Name é obrigatório
            .HasMaxLength(200);                           // Tamanho máximo 200

        Property(c => c.Email)
            .HasMaxLength(200);                           // Email com limite

        // Client -> Vehicles (1:N)
        HasMany(c => c.Vehicles)                          // Cliente tem muitos Vehicles
            .WithRequired(v => v.Client)                  // cada Vehicle precisa de um Client
            .HasForeignKey(v => v.ClientId)               // FK na tabela Vehicle.ClientId
            .WillCascadeOnDelete(false);                  // não deletar vehicles ao deletar client

        // Client -> ClientPhone (1:N)
        HasMany(c => c.PhoneNumbers)                      // Cliente tem muitos telefones
            .WithRequired(p => p.Client)                  // cada telefone precisa de um cliente
            .HasForeignKey(p => p.ClientId)               // FK na tabela ClientPhone.ClientId
            .WillCascadeOnDelete(true);                   // deletar client apaga telefones
    }
}
```

### ServiceOrderConfiguration (Vehicle, ServiceItems, Payment — relação 0..1:1)

```csharp
public class ServiceOrderConfiguration : EntityTypeConfiguration<ServiceOrder>
{
    public ServiceOrderConfiguration()
    {
        ToTable("ServiceOrders");                       // Mapeia para tabela
        HasKey(so => so.Id);                              // PK

        // ServiceOrder -> Vehicle (N:1)
        HasRequired(so => so.Vehicle)                     // cada ServiceOrder precisa de um Vehicle
            .WithMany()                                   // Vehicle pode ter histórico (opcional)
            .HasForeignKey(so => so.VehicleId)            // FK em ServiceOrder.VehicleId
            .WillCascadeOnDelete(false);                  // não deletar ordens ao deletar vehicle

        // ServiceOrder -> ServiceItems (1:N)
        HasMany(so => so.ServiceItems)                    // ServiceOrder tem muitos ServiceItems
            .WithRequired(si => si.ServiceOrder)          // cada ServiceItem pertence a uma ServiceOrder
            .HasForeignKey(si => si.ServiceOrderId)       // FK em ServiceItem
            .WillCascadeOnDelete(true);                   // deletar ServiceOrder apaga ServiceItems

        // ServiceOrder -> Payment (0..1 : 1)
        HasOptional(so => so.Payment)                     // ServiceOrder pode não ter Payment (0..1)
            .WithRequired(payment => payment.ServiceOrder) // Payment precisa obrigatoriamente de um ServiceOrder (1)
            .WillCascadeOnDelete(false);                  // não deletar Payment se deletar ServiceOrder
    }
}
```

> Observação: usar `HasOptional(...).WithRequired(...)` é a forma direta e clara no EF6 para 0..1 → 1 quando a FK opcional está na tabela do lado oposto (ServiceOrder.PaymentId). Em cenários ambíguos use `WithRequiredPrincipal` ou `WithRequiredDependent` explicitamente.

---

## Boas práticas rápidas

* Configure relações em **apenas uma** classe de configuração para evitar duplicidade.
* Prefira `WillCascadeOnDelete(true)` apenas em relações fortemente dependentes (ex.: itens de pedidos).
* Mantenha nomes de tabelas e colunas explícitos quando necessário (`ToTable`, `HasColumnName`).
* Use DataAnnotations quando a regra for simples; use Fluent API para regras de persistência mais complexas.

---

## Links para aprofundamento

* [01_Configuracoes-Entidades.md](./04_FluentAPI/01_Configuracoes-Entidades.md) — exemplos detalhados de `EntityTypeConfiguration<T>`
* [02_Relacionamentos.md](./04_FluentAPI/02_Relacionamentos.md) — relações 1:N, N:1, 0..1:1, N:N com exemplos e diagramas
* [03_CascadeDelete.md](./04_FluentAPI/03_CascadeDelete.md) — regras e boas práticas sobre exclusão em cascata
* [04_Resumo-FluentAPI.md](./04_FluentAPI/04_Resumo-FluentAPI.md) — resumo visual e referências rápidas

---

Documento gerado com auxílio de IA.
