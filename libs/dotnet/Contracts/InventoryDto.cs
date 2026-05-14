namespace Vibey.Contracts;

public record StockItemDto(int ProductId, int Available, int Reserved);

public record ReserveRequest(int ProductId, int Quantity);

public record ReserveResponse(int ProductId, int Available, int Reserved, bool Ok);
