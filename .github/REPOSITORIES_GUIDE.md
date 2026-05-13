# Repositories Guide - Detailed Documentation

This guide provides documentation and conventions for repository implementations in the Chinese Sales System API.

## Repository responsibilities
- Handle all data access logic.
- Use Entity Framework Core through the `ChineseSaleContext`.
- Expose methods for services to query and update data.
- Keep business logic out of repositories.

## General repository rules
- Use interfaces in `RepositoryInterfaces/`.
- Implement concrete classes in `Repositories/`.
- Support filtering, pagination, and eager loading where needed.
- Use `async/await` for database operations.
- Use `AsNoTracking()` for read-only queries when appropriate.

## Common repository patterns

### Get all with pagination
```csharp
public async Task<IEnumerable<Gift>> GetGiftsWithPaginationAsync(int page, int pageSize)
{
	return await _context.Gifts
		.OrderBy(g => g.Id)
		.Skip((page - 1) * pageSize)
		.Take(pageSize)
		.ToListAsync();
}
```

### Get by id with includes
```csharp
public async Task<Gift> GetGiftWithDetailsAsync(int giftId)
{
	return await _context.Gifts
		.Include(g => g.Category)
		.Include(g => g.Lotteries)
		.FirstOrDefaultAsync(g => g.Id == giftId);
}
```

### Existence checks
```csharp
public async Task<bool> CartItemExistsAsync(int userId, int cardId)
{
	return await _context.CardCarts
		.AnyAsync(c => c.UserId == userId && c.CardId == cardId);
}
```

## Repository types in this project
- `IUserRepository` / `UserRepository`
- `IGiftRepository` / `GiftRepository`
- `IPackageRepository` / `PackageRepository`
- `ICategoryRepository` / `CategoryRepository`
- `ILotteryRepository` / `LotteryRepository`
- `ICardRepository` / `CardRepository`
- `ICardCartRepository` / `CardCartRepository`
- `IPackageCartRepository` / `PackageCartRepository`
- `IAddressRepository` / `AddressRepository`
- `IDonorRepository` / `DonorRepository`

## Best practices
- Use `AnyAsync` for boolean checks.
- Use `Include` only when related data is needed.
- Avoid loading entire tables into memory.
- Keep queries translatable to SQL.
- Use transactions for multiple related updates.

## Dependency injection
- Register repositories in `Program.cs`.
- Inject interface types into services.

## Creating a new repository
1. Add model to `Models/`.
2. Add DTOs to `Dto/` if needed.
3. Add interface to `RepositoryInterfaces/`.
4. Add concrete class to `Repositories/`.
5. Add `DbSet` to `ChineseSaleContext`.
6. Register in `Program.cs`.
7. Use the repository from a service.

---
**Last updated**: May 12, 2026
