import api from "./api";

import { CreateOrderPayload, Order } from "@/types/order";
import { ApiResponse } from "@/types/apiResponse";

export const OrderService = {
  createOrder: async (payload: CreateOrderPayload): Promise<Order | null> => {
    try {
      const { data: apiResponse } = await api.post<ApiResponse<Order>>(
        "/orders",
        payload
      );
      return apiResponse.data;
    } catch (error) {
      console.error("Error creating order:", error);
      return null;
    }
  },

  confirmOrder: async (orderId: string): Promise<boolean> => {
    try {
      await api.put(`/orders/${orderId}/confirm`);
      return true;
    } catch (error) {
      console.error("Error confirming order:", error);
      return false;
    }
  },

  cancelOrder: async (orderId: string): Promise<boolean> => {
    try {
      await api.put(`/orders/${orderId}/cancel`);
      return true;
    } catch (error) {
      console.error("Error canceling order:", error);
      return false;
    }
  },
};
