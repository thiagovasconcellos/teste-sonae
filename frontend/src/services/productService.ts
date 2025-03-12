import api from "./api";

import { ApiResponse } from "@/types/apiResponse";
import { Product } from "@/types/product";

export const ProductService = {
  getProducts: async (): Promise<Product[]> => {
    try {
      const { data: apiResponse } = await api.get<ApiResponse<Product[]>>(
        "/products"
      );
      return apiResponse.data;
    } catch (error) {
      console.error("Error fetching products:", error);
      return [];
    }
  },
};
