# 🏗️ Arquitectura Refactorizada - Diagrama C4 Detallado

## 📐 Diagrama de Componentes (Nivel 3)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          CAPA DE PRESENTACIÓN                               │
│                    (SGHR.Web/ApiConsumer/Controllers)                       │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                    ClienteApiController                              │  │
│  │  Responsabilidades:                                                  │  │
│  │  • Coordinación de requests HTTP                                     │  │
│  │  • Validación de ModelState                                          │  │
│  │  • Gestión de TempData                                               │  │
│  │  • Retorno de Views/Partials                                         │  │
│  │                                                                       │  │
│  │  Dependencias:                                                        │  │
│  │  • IClienteApiService (inyectado)                                    │  │
│  │  • ILogger<ClienteApiController> (inyectado)                        │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                            │                                                  │
│                            │ usa                                              │
│                            ▼                                                  │
└─────────────────────────────────────────────────────────────────────────────┘
                            │
                            │
┌─────────────────────────────────────────────────────────────────────────────┐
│                    CAPA DE SERVICIOS DE API                                 │
│          (SGHR.Web/Infrastructure/Services/Api)                             │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                    IClienteApiService                                │  │
│  │  (Interfaz - Abstracción)                                            │  │
│  │  • Define contrato para consumo de API de Clientes                   │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                            ▲                                                  │
│                            │ implementa                                       │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                    ClienteApiService                                │  │
│  │  Responsabilidades:                                                  │  │
│  │  • Configuración específica de endpoints                            │  │
│  │  • Delegación a BaseApiService para lógica común                     │  │
│  │                                                                       │  │
│  │  Propiedades:                                                        │  │
│  │  • EntityName = "Cliente"                                            │  │
│  │  • BaseEndpoint = "Cliente"                                         │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                            │                                                  │
│                            │ hereda de                                        │
│                            ▼                                                  │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │         BaseApiService<TDto, TCreateDto, TUpdateDto, TDeleteDto>      │  │
│  │  Responsabilidades:                                                  │  │
│  │  • Implementación común de CRUD                                      │  │
│  │  • Serialización/Deserialización JSON                                │  │
│  │  • Manejo de errores HTTP                                            │  │
│  │  • Logging centralizado                                              │  │
│  │                                                                       │  │
│  │  Métodos:                                                             │  │
│  │  • GetAllAsync()                                                      │  │
│  │  • GetByIdAsync(id)                                                  │  │
│  │  • CreateAsync(dto)                                                  │  │
│  │  • UpdateAsync(dto)                                                  │  │
│  │  • DeleteAsync(id)                                                   │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                            │                                                  │
│                            │ usa                                              │
│                            ▼                                                  │
└─────────────────────────────────────────────────────────────────────────────┘
                            │
                            │
┌─────────────────────────────────────────────────────────────────────────────┐
│                    CAPA DE INFRAESTRUCTURA                                  │
│                    (HttpClient - .NET Framework)                             │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                    HttpClient                                         │  │
│  │  • Gestionado por Dependency Injection                                │  │
│  │  • Configurado en Program.cs                                          │  │
│  │  • BaseAddress: http://localhost:5066/api/                           │  │
│  │  • Timeout: 30 segundos                                              │  │
│  │  • Headers: Accept: application/json                                 │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                            │                                                  │
│                            │ HTTP/REST                                        │
│                            ▼                                                  │
└─────────────────────────────────────────────────────────────────────────────┘
                            │
                            │
┌─────────────────────────────────────────────────────────────────────────────┐
│                          API BACKEND                                        │
│                    (SGHR.Api - ASP.NET Core Web API)                        │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                    ClienteController (API)                           │  │
│  │  • GET    /api/Cliente                                                │  │
│  │  • GET    /api/Cliente/{id}                                           │  │
│  │  • POST   /api/Cliente                                                │  │
│  │  • PUT    /api/Cliente                                                │  │
│  │  • DELETE /api/Cliente/{id}                                           │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────────┘
```

## 🔄 Flujo de Datos (Ejemplo: Crear Cliente)

```
1. Usuario envía formulario
   │
   ▼
2. ClienteApiController.Create(ClienteCreateDTO model)
   │  • Valida ModelState
   │  • Llama a _clienteApiService.CreateAsync(model)
   │
   ▼
3. ClienteApiService.CreateAsync(ClienteCreateDTO dto)
   │  • Delega a BaseApiService.CreateAsync()
   │
   ▼
4. BaseApiService.CreateAsync(TCreateDto dto)
   │  • Serializa dto a JSON
   │  • Llama a _httpClient.PostAsJsonAsync(BaseEndpoint, dto)
   │  • Espera HttpResponseMessage
   │  • Deserializa respuesta a OperationResult<TDto>
   │  • Maneja errores HTTP
   │  • Logging
   │
   ▼
5. HttpClient.PostAsJsonAsync("Cliente", dto)
   │  • Envía HTTP POST a http://localhost:5066/api/Cliente
   │  • Headers: Content-Type: application/json
   │
   ▼
6. API Backend (ClienteController)
   │  • Recibe request
   │  • Valida y procesa
   │  • Retorna OperationResult<ClienteDTO>
   │
   ▼
7. BaseApiService recibe respuesta
   │  • Deserializa JSON
   │  • Retorna OperationResult<ClienteDTO>
   │
   ▼
8. ClienteApiService retorna resultado
   │
   ▼
9. ClienteApiController
   │  • Evalúa result.Success
   │  • Establece TempData["Success"] o TempData["Error"]
   │  • Retorna RedirectToAction("Index") o View(model)
   │
   ▼
10. Vista renderizada con mensaje de éxito/error
```

## 📊 Comparación Antes vs Después

### Antes (Código Original)

```csharp
// ❌ Problemas:
// 1. Instanciación directa
private readonly ClienteHttpClient _clienteClient = new ClienteHttpClient();

// 2. Lógica HTTP mezclada con presentación
public async Task<IActionResult> _List()
{
    try {
        using (_clienteClient.client) {
            var response = await _clienteClient.Index();
            var responseString = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<...>(responseString, _jsonSerializerOptions);
            // ... 30+ líneas más
        }
    } catch { ... }
}

// 3. Código duplicado en cada método
// 4. Imposible testear
// 5. Alto acoplamiento
```

### Después (Código Refactorizado)

```csharp
// ✅ Soluciones:
// 1. Inyección de dependencias
private readonly IClienteApiService _clienteApiService;
public ClienteApiController(IClienteApiService clienteApiService, ...) { ... }

// 2. Separación de responsabilidades
public async Task<IActionResult> _List()
{
    var result = await _clienteApiService.GetAllAsync(); // ✅ Delegación
    if (result.Success)
        return PartialView("_List", result.Data);
    // ... 5 líneas totales
}

// 3. Código reutilizable en BaseApiService
// 4. Fácil de testear con mocks
// 5. Bajo acoplamiento
```

## 🎯 Principios SOLID Aplicados - Resumen

| Principio | Aplicación | Beneficio |
|-----------|------------|-----------|
| **S**ingle Responsibility | Controlador solo presenta, Service solo consume API | Mantenibilidad ⬆️ |
| **O**pen/Closed | BaseApiService extensible sin modificar | Escalabilidad ⬆️ |
| **L**iskov Substitution | Todas las implementaciones intercambiables | Flexibilidad ⬆️ |
| **I**nterface Segregation | Interfaces específicas por entidad | Claridad ⬆️ |
| **D**ependency Inversion | Dependencias de interfaces, no clases | Testabilidad ⬆️ |

## 🏛️ Patrones Implementados

1. **Repository Pattern** (Adaptado): Servicios actúan como repositorios remotos
2. **Dependency Injection**: HttpClient y servicios inyectados
3. **Template Method**: BaseApiService define algoritmo, clases derivadas especifican detalles
4. **Facade Pattern**: Servicios simplifican interacción con API
5. **Strategy Pattern**: Diferentes servicios pueden tener diferentes estrategias (futuro)

---

## ✅ Checklist de Implementación

- [x] Interfaces creadas para todos los servicios
- [x] BaseApiService implementado con lógica común
- [x] Servicios específicos creados (10 servicios)
- [x] Inyección de dependencias configurada en Program.cs
- [x] ClienteApiController refactorizado como ejemplo
- [x] Documentación técnica completa
- [x] Diagramas C4 incluidos
- [ ] Refactorizar controladores restantes (pendiente)
- [ ] Tests unitarios (pendiente)

---

## 📝 Notas para el Equipo

1. **Migración Gradual**: Se puede migrar controlador por controlador sin romper funcionalidad
2. **Compatibilidad**: Los HttpClient wrappers originales se mantienen pero no se usan directamente
3. **Testing**: Ahora es posible crear mocks de `IApiService` para tests unitarios
4. **Extensibilidad**: Agregar nuevas entidades es trivial (3 pasos: interfaz, servicio, registro DI)

