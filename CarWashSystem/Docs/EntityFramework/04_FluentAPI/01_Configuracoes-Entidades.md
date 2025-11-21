# Configurações de Entidades (Fluent API) — EF6 / .NET Framework 4.8

Este arquivo mostra exemplos práticos de classes `EntityTypeConfiguration<T>` para suas entidades do projeto (Client, Vehicle, Phone, User, Service, ServiceItem, ServiceOrder, Payment). Cada exemplo contém comentários inline ao lado das chamadas Fluent API, e observações importantes sobre porque usar cada configuração.

> Nota: estas configurações são escritas para **Entity Framework 6** (EF6). Para registrar as configurações no `DbContext` use `modelBuilder.Configurations.Add(new XxxConfiguration());` dentro de `OnModelCreating(DbModelBuilder modelBuilder)`.

---

## Registro das Configurations no DbContext (exemplo)

```csharp
protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
    modelBuilder.Configurations.Add(new ClientConfiguration());         // Registra configuração do Client
    modelBuilder.Configurations.Add(new VehicleConfiguration());        // Registra configuração do Vehicle
    modelBuilder.Configurations.Add(new ClientPhoneConfiguration());    // Registra configuração do ClientPhone
    modelBuilder.Configurations.Add(new UserConfiguration());           // Registra User
    modelBuilder.Configurations.Add(new UserPhoneConfiguration());      // Registra UserPhone
    modelBuilder.Configurations.Add(new ServiceConfiguration());        // Registra Service
    modelBuilder.Configurations.Add(new ServiceItemConfiguration());    // Registra ServiceItem
    modelBuilder.Configurations.Add(new ServiceOrderConfiguration());   // Registra ServiceOrder (inclui Payment relation)
    modelBuilder.Configurations.Add(new PaymentConfiguration());        // Registra Payment (sem repetir relação)
}
```

---

## Exemplos de Configuration

### ClientConfiguration

```csharp
public class ClientConfiguration : EntityTypeConfiguration<Client>
{
    public ClientConfiguration()
    {
        ToTable("Clients");                                 // Mapeia para a tabela 'Clients'
        HasKey(c => c.Id);                                   // Define PK

        Property(c => c.Name)
            .IsRequired()                                    // Name é obrigatório
            .HasMaxLength(200);                              // Tamanho máximo 200

        Property(c => c.Email)
            .HasMaxLength(200);                              // Email com limite

        HasMany(c => c.Vehicles)                             // Cliente tem muitos Vehicles
            .WithRequired(v => v.Client)                     // cada Vehicle precisa de um Client
            .HasForeignKey(v => v.ClientId)                  // FK em Vehicle.ClientId
            .WillCascadeOnDelete(false);                     // não deletar vehicles ao deletar client

        HasMany(c => c.PhoneNumbers)                         // Cliente tem muitos telefones
            .WithRequired(p => p.Client)                     // cada telefone precisa de um cliente
            .HasForeignKey(p => p.ClientId)                  // FK em ClientPhone.ClientId
            .WillCascadeOnDelete(true);                      // deletar client apaga telefones
    }
}
```

---

### VehicleConfiguration

```csharp
public class VehicleConfiguration : EntityTypeConfiguration<Vehicle>
{
    public VehicleConfiguration()
    {
        ToTable("Vehicles");
        HasKey(v => v.Id);                                   // PK

        Property(v => v.Plate)
            .IsRequired()                                    // Placa obrigatória
            .HasMaxLength(7)                                 // 7 caracteres
            .IsUnicode(false);                               // ASCII (evita Unicode desnecessário)

        Property(v => v.Model)
            .HasMaxLength(100);                              // Modelo com limite

        Property(v => v.Brand)
            .HasMaxLength(100);                              // Marca com limite

        Property(v => v.YearFab)
            .IsRequired();                                   // Ano de fabricação obrigatório

        Property(v => v.YearModel)
            .IsRequired();                                   // Ano do modelo obrigatório

        // Observação: criar índice único em Plate via migration ou SQL (EF6 não tem HasIndex nativo)
    }
}
```

---

### ClientPhoneConfiguration

```csharp
public class ClientPhoneConfiguration : EntityTypeConfiguration<ClientPhone>
{
    public ClientPhoneConfiguration()
    {
        ToTable("ClientPhones");
        HasKey(p => p.Id);                                   // PK

        Property(p => p.DDD)
            .IsRequired();                                   // DDD obrigatório

        Property(p => p.Number)
            .IsRequired()                                    // Número obrigatório
            .HasMaxLength(9)                                 // 8 ou 9 dígitos
            .IsUnicode(false);                               // Apenas dígitos
    }
}
```

---

### UserConfiguration

```csharp
public class UserConfiguration : EntityTypeConfiguration<User>
{
    public UserConfiguration()
    {
        ToTable("Users");
        HasKey(u => u.Id);                                  // PK

        Property(u => u.Name)
            .IsRequired()                                   // Nome obrigatório
            .HasMaxLength(200);

        Property(u => u.Email)
            .HasMaxLength(200);

        HasMany(u => u.PhoneNumbers)                        // User tem muitos telefones
            .WithRequired(p => p.User)                     // cada telefone precisa de um User
            .HasForeignKey(p => p.UserId)                  // FK em UserPhone.UserId
            .WillCascadeOnDelete(true);

        HasMany(u => u.ServiceItems)                        // User (employee) pode ter vários ServiceItems
            .WithRequired(si => si.Employee)               // cada ServiceItem tem o Employee
            .HasForeignKey(si => si.EmployeeId)            // FK em ServiceItem.EmployeeId
            .WillCascadeOnDelete(false);
    }
}
```

---

### UserPhoneConfiguration

```csharp
public class UserPhoneConfiguration : EntityTypeConfiguration<UserPhone>
{
    public UserPhoneConfiguration()
    {
        ToTable("UserPhones");
        HasKey(p => p.Id);

        Property(p => p.DDD).IsRequired();                 // DDD obrigatório

        Property(p => p.Number)
            .IsRequired()
            .HasMaxLength(9)
            .IsUnicode(false);
    }
}
```

---

### ServiceConfiguration

```csharp
public class ServiceConfiguration : EntityTypeConfiguration<Service>
{
    public ServiceConfiguration()
    {
        ToTable("Services");
        HasKey(s => s.Id);

        Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        Property(s => s.Description)
            .HasMaxLength(500);

        Property(s => s.BasePrice)
            .IsRequired()                                   // Preço base obrigatório
            .HasPrecision(18, 2);                           // Precisão do decimal
    }
}
```

---

### ServiceItemConfiguration

```csharp
public class ServiceItemConfiguration : EntityTypeConfiguration<ServiceItem>
{
    public ServiceItemConfiguration()
    {
        ToTable("ServiceItems");
        HasKey(i => i.Id);

        Property(i => i.ChargedAmount)
            .IsRequired()                                   // Valor cobrado obrigatório
            .HasPrecision(18, 2);                           // Precisão decimal

        HasRequired(i => i.Service)                        // ServiceItem precisa de Service
            .WithMany()                                    // Service pode ter muitos ServiceItems
            .HasForeignKey(i => i.ServiceId)
            .WillCascadeOnDelete(false);

        HasRequired(i => i.Employee)                       // ServiceItem precisa de Employee (User)
            .WithMany(u => u.ServiceItems)
            .HasForeignKey(i => i.EmployeeId)
            .WillCascadeOnDelete(false);
    }
}
```

---

### ServiceOrderConfiguration (com Payment)

```csharp
public class ServiceOrderConfiguration : EntityTypeConfiguration<ServiceOrder>
{
    public ServiceOrderConfiguration()
    {
        ToTable("ServiceOrders");
        HasKey(so => so.Id);

        // Vehicle (N:1)
        HasRequired(so => so.Vehicle)                        // cada ServiceOrder precisa de um Vehicle
            .WithMany()                                      // Vehicle pode ter histórico (opcional)
            .HasForeignKey(so => so.VehicleId)               // FK em ServiceOrder.VehicleId
            .WillCascadeOnDelete(false);

        // ServiceItems (1:N)
        HasMany(so => so.ServiceItems)                       // ServiceOrder tem muitos ServiceItems
            .WithRequired(si => si.ServiceOrder)             // cada ServiceItem pertence a uma ServiceOrder
            .HasForeignKey(si => si.ServiceOrderId)          // FK em ServiceItem.ServiceOrderId
            .WillCascadeOnDelete(true);                      // deletar ServiceOrder apaga ServiceItems

        // Payment (0..1 : 1)
        HasOptional(so => so.Payment)                        // ServiceOrder pode não ter Payment (0..1)
            .WithRequired(payment => payment.ServiceOrder)  // Payment precisa obrigatoriamente de um ServiceOrder (1)
            .WillCascadeOnDelete(false);

        Property(so => so.EntryDate).IsRequired();          // EntryDate obrigatório
        Property(so => so.LeaveDate).IsOptional();          // LeaveDate opcional
    }
}
```

---

### PaymentConfiguration

```csharp
public class PaymentConfiguration : EntityTypeConfiguration<Payment>
{
    public PaymentConfiguration()
    {
        ToTable("Payments");
        HasKey(p => p.Id);

        Property(p => p.Total)
            .IsRequired()                                    // Total obrigatório
            .HasPrecision(18, 2);                            // Precisão decimal

        Property(p => p.Status)
            .IsRequired();                                  // Status obrigatório

        // Relação configurada em ServiceOrderConfiguration — evite duplicar
    }
}
```

---

## Observações e dicas finais

* **Não duplique relacionamentos** em duas configurations diferentes — configure a associação apenas em uma das entidades para evitar conflitos.
* **Índices**: EF6 não possui `HasIndex` nativo; crie índices únicos (ex.: Plate) via migration ou SQL script pós-criação do schema.
* **Enums**: por padrão EF6 persiste enums como `int`. Se quiser outro comportamento, implemente conversão manual ou use um campo auxiliar.
* **Precision**: sempre defina `HasPrecision` em campos `decimal` para evitar comportamento inesperado.
* **RowVersion / Concurrency**: se quiser controle de concorrência, adicione `public byte[] RowVersion { get; set; }` e configure `.IsRowVersion()`.

---

Documento gerado com auxílio de IA.
