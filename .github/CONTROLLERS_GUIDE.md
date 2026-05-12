# Controllers Guide - Detailed Documentation

This guide provides documentation and conventions for all Controllers in the Chinese Sales System API.

## Controller structure
- Use `ControllerBase` and `[ApiController]`.
- Set route with `[Route("api/[controller]")]`.
- Use DTOs for input and output.
- Keep controllers thin: delegate business logic to services.
- Use attributes for HTTP method mapping and authorization.

## General rules for controllers
- `GET` endpoints return data with `Ok()`.
- `POST` endpoints return `CreatedAtAction(...)` when creating resources.
- `PUT` and `PATCH` update resources and return `NoContent()` or `Ok()`.
- `DELETE` endpoints return `NoContent()` when successful.
- Validate input DTOs and return `BadRequest()` for invalid models.
- Use authorization attributes such as `[Admin]` or `[Authorize]` when needed.

## Controller responsibilities
- Translate HTTP requests into service calls.
- Handle authorization and routing.
- Return proper HTTP status codes.
- Avoid database access or business logic in controllers.

## Common controller patterns

### Standard CRUD
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<MyDto>> GetById(int id)
{
	var item = await _service.GetByIdAsync(id);
	if (item == null)
		return NotFound();
	return Ok(item);
}
```

### Admin-only creation
```csharp
[Admin]
[HttpPost]
public async Task<ActionResult<MyDto>> Create(CreateDto dto)
{
	var created = await _service.CreateAsync(dto);
	return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}
```

### Pagination and filtering
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<MyDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
	var items = await _service.GetAllAsync(page, pageSize);
	return Ok(items);
}
```

## Existing controllers overview
- `UserController`: authentication, registration, profile management.
- `GiftController`: gift CRUD, category filtering, image handling.
- `PackageController`: package CRUD and packaging logic.
- `CategoryController`: category CRUD and organization.
- `LotteryController`: lottery CRUD, draws, reports.
- `CardController`: gift card CRUD.
- `CardCartController`: card cart management.
- `PackageCartController`: package cart management.
- `AddressController`: user address management.
- `DonorController`: donor and donation tracking.
- `FilesController`: file upload and file serving.

## Best practices
- Keep controllers small and focused.
- Do not include business rules in controllers.
- Use services for operations and validation.
- Return DTOs, not entity models.
- Use async/await for all I/O operations.

## Testing controllers
- Use unit tests with mocked services.
- Assert correct status codes and returned data.
- Test authorization and invalid input cases.

---
**Last updated**: May 12, 2026
