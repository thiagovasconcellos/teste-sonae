"use client";

import { useEffect, useState } from "react";
import { Product } from "../types/product";
import { ProductService } from "../services/productService";
import { useCart } from "../context/CartContext";

export default function ProductList() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [isClient, setIsClient] = useState(false);
  const [quantities, setQuantities] = useState<Record<string, number>>({});
  const { addToCart } = useCart();

  useEffect(() => {
    setIsClient(true);

    const fetchProducts = async () => {
      try {
        setLoading(true);
        const data = await ProductService.getProducts();
        setProducts(data);

        const initialQuantities: Record<string, number> = {};
        data.forEach((product) => {
          initialQuantities[product.id] = 1;
        });
        setQuantities(initialQuantities);

        setError(null);
      } catch (err) {
        setError("Failed to fetch products");
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);

  const handleQuantityChange = (productId: string, value: number) => {
    setQuantities((prev) => ({
      ...prev,
      [productId]: Math.max(
        1,
        Math.min(value, products.find((p) => p.id === productId)?.inStock || 1)
      ),
    }));
  };

  const handleAddToCart = (product: Product) => {
    const quantity = quantities[product.id] || 1;
    addToCart(product, quantity);

    setQuantities((prev) => ({
      ...prev,
      [product.id]: 1,
    }));
  };

  if (!isClient) {
    return <div className="text-center py-10">Carregando...</div>;
  }

  if (loading)
    return <div className="text-center py-10">Carregando produtos...</div>;
  if (error)
    return <div className="text-center py-10 text-red-500">{error}</div>;
  if (products.length === 0)
    return <div className="text-center py-10">Não há produtos 🫤</div>;

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      {products.map((product) => (
        <div key={product.id} className="border rounded-lg p-4 shadow-sm">
          <h2 className="text-xl font-semibold mb-2">
            {product.productDescription}
          </h2>
          <p className="text-lg font-bold text-blue-600">
            Valor unitário: ${product.value.toFixed(2)}
          </p>
          <p className="text-sm text-gray-600">
            Disponíveis: {product.inStock}
          </p>
          <p className="text-xs text-gray-500 mt-2">
            Última atualização:{" "}
            {new Date(product.updatedAt).toISOString().split("T")[0]}
          </p>

          <div className="mt-4 flex items-center">
            <label htmlFor={`quantity-${product.id}`} className="mr-2">
              Quantidade:
            </label>
            <input
              id={`quantity-${product.id}`}
              type="number"
              min="1"
              max={product.inStock}
              value={quantities[product.id] || 1}
              onChange={(e) =>
                handleQuantityChange(product.id, parseInt(e.target.value))
              }
              className="border rounded px-2 py-1 w-16 text-center"
            />
          </div>

          <button
            onClick={() => handleAddToCart(product)}
            className="mt-3 w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition"
          >
            Adicionar ao carrinho
          </button>
        </div>
      ))}
    </div>
  );
}
