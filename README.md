# Clean Architecture Ürün Yönetim Sistemi

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-9.0-green.svg)](https://docs.microsoft.com/en-us/ef/)

> Clean Architecture, CQRS, Repository Pattern, Specification Pattern ve modern .NET 9.0 geliştirme pratiklerinin kapsamlı bir demonstrasyonu.

## İçindekiler

- [Genel Bakış](#genel-bakış)
- [Mimari](#mimari)
- [Özellikler](#özellikler)
- [Teknoloji Yığını](#teknoloji-yığını)
- [Başlarken](#başlarken)
- [Proje Yapısı](#proje-yapısı)
- [Tasarım Desenleri](#tasarım-desenleri)
- [API Dokümantasyonu](#api-dokümantasyonu)
- [Test](#test)
- [Katkıda Bulunma](#katkıda-bulunma)

## Genel Bakış

Bu proje, modern .NET 9.0 ile **Clean Architecture** implementasyonunu göstererek, ölçeklenebilir ve sürdürülebilir uygulamalar geliştirmek için kurumsal düzeyde desenler ve uygulamaları sergilemektedir. CQRS, Repository Pattern, Specification Pattern ve kapsamlı testleri içeren basit bir Ürün Yönetim sistemini uygular.

### Ana Özellikler

- **Clean Architecture** katı bağımlılık kuralları ile
- **SOLID Prensiplerinin** uygulanması
- **Domain-Driven Design** (DDD) konseptleri
- **CQRS** (Command Query Responsibility Segregation)
- **Repository ve Unit of Work** desenleri
- **Specification Pattern** karmaşık sorgular için
- **Kapsamlı Unit ve Integration Testleri**

## Mimari

Bu proje aşağıdaki katmanlarla **Clean Architecture** prensiplerine uyar:

```
┌─────────────────────────────────────────┐
│              WebAPI (Sunum)             │
├─────────────────────────────────────────┤
│      Infrastructure (Veri Erişimi)      │
├─────────────────────────────────────────┤
│       Application (İş Mantığı)          │
├─────────────────────────────────────────┤
│            Domain (Varlıklar)           │
└─────────────────────────────────────────┘
```

### Bağımlılık Akışı
- **Domain**: Hiçbir bağımlılığı yoktur (Temel iş varlıkları)
- **Application**: Sadece Domain'e bağımlıdır
- **Infrastructure**: Application ve Domain'e bağımlıdır
- **WebAPI**: Tüm katmanlara bağımlı olabilir

## Özellikler

### Teknik Özellikler
- Açık sorumluluk ayrımı ile **Clean Architecture**
- Komut/sorgu ayrımı için MediatR kullanan **CQRS Pattern**
- Generic implementasyon ile **Repository Pattern**
- Transaction yönetimi için **Unit of Work Pattern**
- Yeniden kullanılabilir sorgu mantığı için **Specification Pattern**
- Nesne-nesne eşleme için **AutoMapper**
- Code-First yaklaşımı ile **Entity Framework Core 9.0**
- Audit trail ile **Soft Delete** işlevselliği
- **Swagger/OpenAPI** dokümantasyonu

### Kalite Güvencesi
- %100 başarı oranı ile **22 Unit Test**
- **xUnit** test framework'ü
- Okunabilir test assertionları için **FluentAssertions**
- Bağımlılıkları mock'lamak için **Moq**
- Test için **InMemory Database**
- **Test Kategorileri**: Unit, Integration, Repository, Specification

### İş Özellikleri
- Ürün yönetimi (Oluştur, Oku)
- Audit trail (Oluşturan/Güncelleyen, zaman damgaları)
- Soft delete işlevselliği
- Veri doğrulama ve hata yönetimi

## Teknoloji Yığını

| Kategori | Teknoloji |
|----------|-----------|
| **Framework** | .NET 9.0 |
| **Veritabanı** | SQL Server 2022 |
| **ORM** | Entity Framework Core 9.0 |
| **Test** | xUnit, FluentAssertions, Moq |
| **Dokümantasyon** | Swagger/OpenAPI |
| **Mimari** | Clean Architecture |
| **Desenler** | CQRS, Repository, Unit of Work, Specification |

### NuGet Paketleri

#### Temel Paketler
```xml
<PackageReference Include="MediatR" Version="13.0.0" />
<PackageReference Include="AutoMapper" Version="12.0.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
```

#### Test Paketleri
```xml
<PackageReference Include="xunit" Version="2.9.2" />
<PackageReference Include="FluentAssertions" Version="8.7.1" />
<PackageReference Include="Moq" Version="4.20.72" />
```

## Başlarken

### Gereksinimler

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- SQL Server (veya Docker Compose kurulumunu kullanın)

### Kurulum

1. **Repository'yi klonlayın**
   ```bash
   git clone https://github.com/EbruTezel/CleanArchSpecPatternDemo.git
   cd CleanArchSpecPatternDemo/ProductManagement
   ```

2. **NuGet paketlerini yükleyin**
   ```bash
   dotnet restore
   ```

3. **Veritabanını güncelleyin**
   ```bash
   dotnet ef database update --project ProductManagement.Infrastructure --startup-project WebAPI
   ```

4. **Uygulamayı çalıştırın**
   ```bash
   dotnet run --project WebAPI
   ```

5. **Swagger UI'ya erişin**
   ```
   https://localhost:7043/swagger/index.html
   ```

### Testleri Çalıştırma

```bash
# Tüm testleri çalıştır
dotnet test

# Detaylı çıktı ile
dotnet test --verbosity normal

# Belirli test projesini çalıştır
dotnet test ProductManagement.Tests/
```

## Proje Yapısı

```
ProductManagement.sln
├── ProductManagement.Domain/
│   ├── Abstractions/
│   │   ├── BaseEntity.cs
│   │   └── AuditEntity.cs
│   └── Product.cs
├── ProductManagement.Application/
│   ├── Common/Responses/
│   ├── Features/Product/
│   │   ├── Commands/Create/
│   │   ├── Queries/GetById/
│   │   ├── Dtos/
│   │   └── Spesifications/
│   ├── Persistence/
│   │   ├── Repositories/
│   │   └── Specifications/
│   └── DependencyInjection.cs
├── ProductManagement.Infrastructure/
│   └── Persistence/
│       ├── DbContexts/
│       ├── Repositories/
│       ├── Configurations/
│       └── Specifications/
├── WebAPI/
│   ├── Controllers/
│   └── Program.cs
├── ProductManagement.Tests/
│   ├── Features/
│   ├── Repositories/
│   ├── Specifications/
│   └── Helpers/
```

## Tasarım Desenleri

### Clean Architecture
- **Sorumluluk Ayrımı**: Her katmanın belirli bir sorumluluğu vardır
- **Bağımlılık Tersine Çevirme**: Bağımlılıklar domain'e doğru işaret eder
- **Test Edilebilirlik**: İş mantığı izole edilmiş ve kolayca test edilebilir

### CQRS (Command Query Responsibility Segregation)
```csharp
// Command Örneği
public record CreateProductCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
}

// Query Örneği
public record ProductGetByIdQuery : IRequest<ProductDto?>
{
    public Guid Id { get; init; }
}
```

### Specification Pattern
```csharp
public class ProductByIdSpec : BaseSpecification<Product>
{
    public ProductByIdSpec(Guid id)
    {
        Criteria = c => c.Id == id && !c.IsDeleted;
    }
}
```

### Repository Pattern
```csharp
public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    // ... diğer metodlar
}
```

## API Dokümantasyonu

### Endpoints

| Method | Endpoint | Açıklama | Request Body | Response |
|--------|----------|----------|--------------|----------|
| `GET` | `/api/Product/{id}` | ID ile ürün getir | - | `ApiResponse<ProductDto>` |
| `POST` | `/api/Product` | Yeni ürün oluştur | `CreateProductCommand` | `ApiResponse<Guid>` |

### Örnek İstekler

#### Ürün Oluştur
```bash
curl -X POST "https://localhost:7043/api/Product" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop",
    "price": 999.99,
    "stock": 10
  }'
```

#### Ürün Getir
```bash
curl -X GET "https://localhost:7043/api/Product/{product-id}"
```

### Response Formatı
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Laptop",
    "price": 999.99,
    "stock": 10,
    "createDate": "2024-01-01T00:00:00Z",
    "isDeleted": false
  },
  "success": true,
  "message": "Success"
}
```

## Test

Proje, **%100 başarı oranı** elde eden **22 unit test** ile kapsamlı test içerir.

### Test Kategorileri

#### Unit Tests
- **Command Handler Tests**: İş mantığını doğrular
- **Query Handler Tests**: Veri alma işlemlerini test eder
- **Specification Tests**: Filtreleme mantığını doğrular

#### Integration Tests  
- **Repository Tests**: Veri erişim katmanını test eder
- **Database Integration**: InMemory database testi

### Test Yapısı
```csharp
[Fact]
public async Task Handle_ValidCommand_ShouldCreateProductAndReturnId()
{
    // Arrange
    var command = new CreateProductCommand { /* ... */ };
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.Should().NotBeEmpty();
}
```

### Belirli Testleri Çalıştırma
```bash
# Belirli test sınıfını çalıştır
dotnet test --filter "ClassName~ProductByIdSpecTests"

# Belirli test metodunu çalıştır
dotnet test --filter "TestName~Handle_ValidCommand_ShouldSetAuditProperties"
```

## Bu Mimarinin Faydaları

### Sürdürülebilirlik
- Açık sorumluluk ayrımı
- Diğer katmanları etkilemeden kolay değiştirme
- SOLID prensiplerine uygun implementasyon

### Test Edilebilirlik
- İş mantığının infrastructure'dan izole edilmesi
- Kolay mock'lama ve unit test
- Kapsamlı test coverage

### Ölçeklenebilirlik
- Modüler tasarım kolay genişletmeye olanak sağlar
- Tutarlılık için pattern-based yaklaşım
- Generic implementasyonlar kod tekrarını azaltır

### Güvenilirlik
- Uygun katmanlarda hata yönetimi
- Unit of Work ile transaction yönetimi
- Veri değişiklikleri için audit trail

## Katkıda Bulunma

1. Repository'yi fork edin
2. Feature branch'inizi oluşturun (`git checkout -b feature/HarikaOzellik`)
3. Değişikliklerinizi commit edin (`git commit -m 'Harika özellik ekle'`)
4. Branch'e push edin (`git push origin feature/HarikaOzellik`)
5. Pull Request açın

### Geliştirme Rehberi
- SOLID prensiplerine uyun
- Yeni özellikler için unit test yazın
- Dokümantasyonu gerektiği gibi güncelleyin
- %80'in üzerinde kod coverage'ını koruyun

## Teşekkürler

- Robert C. Martin tarafından Clean Architecture
- CQRS Pattern dokümantasyonu
- Entity Framework Core ekibi
- En iyi uygulamalar için .NET Topluluğu

## İletişim

- **GitHub**: https://github.com/EbruTezel
- **LinkedIn**: https://www.linkedin.com/in/ebru-tezel/
- **Email**: ebru.tezel@outlook.com

---

Bu projeyi yararlı bulduysanız, lütfen yıldız verin!
