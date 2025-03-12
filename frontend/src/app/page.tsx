"use client";

import ProductList from "../components/ProductList";
import Cart from "../components/Cart";
import { CartProvider } from "../context/CartContext";
import { useState } from "react";

export default function Home() {
  const [showCart, setShowCart] = useState(false);

  return (
    <CartProvider>
      <main className="container mx-auto px-4 py-8">
        <header className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold">Produtos</h1>
          <button
            onClick={() => setShowCart(!showCart)}
            className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition"
          >
            {showCart ? "Produtos" : "Carrinho"}
          </button>
        </header>

        {showCart ? <Cart /> : <ProductList />}
      </main>
    </CartProvider>
  );
}
