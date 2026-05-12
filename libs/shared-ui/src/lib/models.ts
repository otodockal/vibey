// Mirror of MyApp.Contracts.ProductDto / OrderDto records.
// In a real workspace these would be generated from each service's
// OpenAPI document — see README "Type generation" section.

export interface Product {
  id: number;
  name: string;
  price: number;
  stock: number;
}

export interface OrderLine {
  productId: number;
  quantity: number;
}

export interface Order {
  id: number;
  customerEmail: string;
  createdAt: string;
  lines: OrderLine[];
  total: number;
}

export interface CreateOrderRequest {
  customerEmail: string;
  lines: OrderLine[];
}
