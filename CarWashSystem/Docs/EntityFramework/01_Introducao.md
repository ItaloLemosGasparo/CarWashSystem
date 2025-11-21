# Introdução ao Entity Framework (EF6) — .NET Framework 4.8

## Objetivo deste documento

Este arquivo apresenta uma introdução clara e prática ao **Entity Framework 6 (EF6)** usado com **.NET Framework 4.8**. Aqui você encontrará o escopo da documentação, pré-requisitos recomendados, um resumo do que o EF faz e um índice com links para os demais tópicos da pasta `EntityFramework` — para que você navegue rapidamente pelo material.

---

## Para quem é este material

* Desenvolvedores que estão usando **.NET Framework 4.8** com **EF6**.
* Pessoas que estão aprendendo a modelar domínio e mapear entidades para banco de dados.
* Quem precisa organizar configurações de persistência (Fluent API, Migrations, Initializers, etc.).

---

## Escopo

Esta documentação cobre:

* Conceitos básicos do EF6 e por que usá-lo.
* Estruturas de entidade e convenções do EF6.
* Uso de **Fluent API** (configurações centralizadas via `EntityTypeConfiguration<T>`).
* Configuração de relacionamentos (1:1, 1:N, N:1, N:N, 0..1:1).
* Migrations e inicializadores de banco (LocalDB para desenvolvimento).
* Boas práticas e recomendações para projetos baseados em EF6.

---

## O que o EF faz, em poucas palavras

O Entity Framework é um **ORM (Object-Relational Mapper)** que simplifica o mapeamento entre classes C# (entidades) e tabelas de banco de dados. Com EF você pode:

* Persistir e recuperar objetos do banco de dados sem escrever SQL direto (exceto quando necessário).
* Configurar o modelo por atributos ou por **Fluent API**.
* Usar Migrations para evoluir o esquema do banco conforme o modelo de domínio muda.

---

## Estrutura desta documentação (índice)

EntityFramework/</br>
├── [01_Introducao.md](./01_Introducao.md) — este documento.</br>
├── [02_Modelos.md](./02_Modelos.md) — entidades, propriedades e convenções básicas.</br>
├── [03_Conventions.md](./03_Conventions.md) — convenções do EF6 (naming, chaves, relacionamentos automáticos).</br>
├── [04_FluentAPI.md](./04_FluentAPI.md) — introdução ao Fluent API.</br>
│   ├── [01_Configuracoes-Entidades.md](./04_FluentAPI/01_Configuracoes-Entidades.md) — exemplos de `EntityTypeConfiguration<T>`.</br>
│   ├── [02_Relacionamentos.md](./04_FluentAPI/02_Relacionamentos.md) — 1:N, N:1, 0..1:1, N:N com exemplos.</br>
│   ├── [03_CascadeDelete.md](./04_FluentAPI/03_CascadeDelete.md) — regras e boas práticas sobre exclusão em cascata.</br>
│   └── [04_Resumo-FluentAPI.md](./04_FluentAPI/04_Resumo-FluentAPI.md) — resumo visual e referências rápidas.</br>
├── [05_Migrations.md](./05_Migrations.md) — uso de migrations no EF6.</br>
├── [06_Initializers.md](./06_Initializers.md) — inicializadores de banco e `Seed`.</br>
├── [07_LocalDB.md](./07_LocalDB.md) — configurar e usar LocalDB para desenvolvimento e testes.</br>
└── [08_Boas-Praticas.md](./08_Boas-Praticas.md) — recomendações gerais e anti-patterns.</br>

---

## Como seguir adiante (sugestão de estudo prático)

1. Leia `02_Modelos.md` para revisar o formato das entidades e convenções.
2. Abra `04_FluentAPI/01_Oque-Sao-Fluent-APIs.md` para entender quando e por que usar Fluent API.
3. Implemente as `Configuration` para uma entidade simples (por exemplo `Client`) e rode LocalDB para ver o schema gerado.
4. Avance para `03_Relacionamentos.md` e experimente configurar relações 1:N e 0..1:1 como no seu projeto.

---

## Nota sobre este material

Documento preparado para uso em projeto com .NET Framework 4.8 e Entity Framework 6.
Documentação gerada com auxílio de IA — revisar e adaptar ao seu contexto de projeto é recomendado.

---

Documento gerado com auxilio de IA