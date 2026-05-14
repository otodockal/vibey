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

export interface User {
  id: number;
  email: string;
  name: string;
  createdAt: string;
}

export interface CreateUserRequest {
  email: string;
  name: string;
}

export interface StockItem {
  productId: number;
  available: number;
  reserved: number;
}

export interface ReserveRequest {
  productId: number;
  quantity: number;
}

export interface ReserveResponse {
  productId: number;
  available: number;
  reserved: number;
  ok: boolean;
}

export type PaymentStatus = 'Pending' | 'Captured' | 'Failed';

export interface Payment {
  id: number;
  orderId: number;
  amount: number;
  status: PaymentStatus;
  createdAt: string;
}

export interface CreatePaymentRequest {
  orderId: number;
  amount: number;
}
