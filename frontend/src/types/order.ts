export interface OrderItem {
  id?: string;
  productId: string;
  productDescription?: string;
  quantity: number;
  unitPrice?: number;
}

export interface Order {
  id: string;
  statusName: string;
  status: number;
  createdAt: string;
  updatedAt: string;
  expiresAt: string;
  items: OrderItem[];
}

export interface CreateOrderPayload {
  items: OrderItem[];
}
