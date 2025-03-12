"use client";

import { useState, useEffect } from "react";
import { useCart } from "../context/CartContext";
import { Order } from "../types/order";
import { OrderService } from "@/services/orderService";

export default function Cart() {
  const { cartItems, removeFromCart, updateQuantity, clearCart, getTotal } =
    useCart();
  const [order, setOrder] = useState<Order | null>(null);
  const [orderStatus, setOrderStatus] = useState<
    "idle" | "created" | "confirmed" | "canceled"
  >("idle");
  const [isLoading, setIsLoading] = useState(false);
  const [timeRemaining, setTimeRemaining] = useState<number | null>(null);

  useEffect(() => {
    if (!order || orderStatus !== "created") return;

    const expiresAt = new Date(order.expiresAt).getTime();
    const now = new Date().getTime();
    const initialTimeRemaining = Math.max(
      0,
      Math.floor((expiresAt - now) / 1000)
    );

    setTimeRemaining(initialTimeRemaining);

    const timer = setInterval(() => {
      setTimeRemaining((prev) => {
        if (prev === null || prev <= 0) {
          clearInterval(timer);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [order, orderStatus]);

  useEffect(() => {
    if (timeRemaining === 0 && orderStatus === "created") {
      setOrderStatus("canceled");
    }
  }, [timeRemaining, orderStatus]);

  const handleCreateOrder = async () => {
    if (cartItems.length === 0) return;

    setIsLoading(true);
    try {
      const items = cartItems.map((item) => ({
        productId: item.productId,
        quantity: item.quantity,
      }));

      const result = await OrderService.createOrder({ items });
      if (result) {
        setOrder(result);
        setOrderStatus("created");
        clearCart();
      }
    } catch (error) {
      console.error("Error creating order:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleConfirmOrder = async () => {
    if (!order) return;

    setIsLoading(true);
    try {
      const success = await OrderService.confirmOrder(order.id);
      if (success) {
        setOrderStatus("confirmed");
      }
    } catch (error) {
      console.error("Error confirming order:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancelOrder = async () => {
    if (!order) return;

    setIsLoading(true);
    try {
      const success = await OrderService.cancelOrder(order.id);
      if (success) {
        setOrderStatus("canceled");
      }
    } catch (error) {
      console.error("Error canceling order:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const formatDateTime = (dateString: string) => {
    return new Date(dateString).toLocaleString();
  };

  if (orderStatus === "created" && order) {
    return (
      <div className="border rounded-lg p-6 shadow-md">
        <h2 className="text-xl font-bold mb-4">
          Encomenda criada com sucesso!
        </h2>
        <p className="mb-2">
          Order ID: <span className="font-mono">{order.id}</span>
        </p>
        <p className="mb-2">Status: {order.statusName}</p>
        <p className="mb-2">Criado em: {formatDateTime(order.createdAt)}</p>

        <div className="mb-4">
          {/* <p className="font-semibold text-amber-600">
            Expira em: {formatDateTime(order.expiresAt)}
          </p> */}
          {timeRemaining !== null && (
            <div
              className={`mt-2 font-bold text-lg ${
                timeRemaining < 30 ? "text-red-600" : "text-amber-600"
              }`}
            >
              Expira em: {Math.floor(timeRemaining / 60)}:
              {(timeRemaining % 60).toString().padStart(2, "0")}
            </div>
          )}
        </div>

        <h3 className="text-lg font-semibold mb-2">Produtos:</h3>
        <ul className="mb-4">
          {order.items.map((item) => (
            <li key={item.id} className="mb-2">
              {item.productDescription} - {item.quantity} x $
              {item.unitPrice?.toFixed(2)} = $
              {(item.quantity * (item.unitPrice || 0)).toFixed(2)}
            </li>
          ))}
        </ul>

        <div className="flex space-x-4">
          <button
            onClick={handleConfirmOrder}
            disabled={isLoading || timeRemaining === 0}
            className="bg-green-600 text-white py-2 px-4 rounded hover:bg-green-700 transition disabled:opacity-50"
          >
            {isLoading ? "Carregando..." : "Confirmar"}
          </button>
          <button
            onClick={handleCancelOrder}
            disabled={isLoading}
            className="bg-red-600 text-white py-2 px-4 rounded hover:bg-red-700 transition disabled:opacity-50"
          >
            {isLoading ? "Carregando..." : "Cancelar"}
          </button>
        </div>
      </div>
    );
  }

  if (orderStatus === "confirmed") {
    return (
      <div className="border rounded-lg p-6 shadow-md bg-green-50">
        <h2 className="text-xl font-bold mb-4 text-green-700">
          Encomenda confirmada
        </h2>
        <p>Seu pedido foi confirmado.</p>
      </div>
    );
  }

  if (orderStatus === "canceled") {
    return (
      <div className="border rounded-lg p-6 shadow-md bg-red-50">
        <h2 className="text-xl font-bold mb-4 text-red-700">
          Pediod cancelado
        </h2>
        <p>Seu pedido foi cancelado.</p>
      </div>
    );
  }

  return (
    <div className="border rounded-lg p-6 shadow-md">
      <h2 className="text-xl font-bold mb-4">Carrinho</h2>

      {cartItems.length === 0 ? (
        <p>Não há nada aqui 😭.</p>
      ) : (
        <>
          <ul className="divide-y">
            {cartItems.map((item) => (
              <li
                key={item.productId}
                className="py-3 flex justify-between items-center"
              >
                <div>
                  <h3 className="font-medium">{item.productDescription}</h3>
                  <p className="text-gray-600">
                    ${item.unitPrice.toFixed(2)} por unidade
                  </p>
                </div>

                <div className="flex items-center">
                  <button
                    onClick={() =>
                      updateQuantity(item.productId, item.quantity - 1)
                    }
                    className="px-2 py-1 bg-gray-200 rounded-l"
                  >
                    -
                  </button>
                  <span className="px-3 py-1 bg-gray-100">{item.quantity}</span>
                  <button
                    onClick={() =>
                      updateQuantity(item.productId, item.quantity + 1)
                    }
                    className="px-2 py-1 bg-gray-200 rounded-r"
                  >
                    +
                  </button>
                  <button
                    onClick={() => removeFromCart(item.productId)}
                    className="ml-3 text-red-500 hover:text-red-700"
                  >
                    Remover
                  </button>
                </div>
              </li>
            ))}
          </ul>

          <div className="mt-4 pt-4 border-t">
            <div className="flex justify-between font-bold text-lg mb-4">
              <span>Total:</span>
              <span>${getTotal().toFixed(2)}</span>
            </div>

            <button
              onClick={handleCreateOrder}
              disabled={isLoading}
              className="w-full bg-green-600 text-white py-2 rounded hover:bg-green-700 transition disabled:opacity-50"
            >
              {isLoading ? "Carregando..." : "Salvar pedido"}
            </button>
          </div>
        </>
      )}
    </div>
  );
}
