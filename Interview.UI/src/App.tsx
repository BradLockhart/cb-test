import { useEffect, useState, type FormEvent } from "react";
import { getProducts } from "./api/productsApi";
import { PaginationControls } from "./components/PaginationControls";
import { ProductList } from "./components/ProductList";
import type { PagedResult, Product } from "./types/product";

const DEFAULT_PAGE_SIZE = 10;

export default function App() {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [result, setResult] = useState<PagedResult<Product> | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [reloadKey, setReloadKey] = useState(0);

  useEffect(() => {
    const controller = new AbortController();

    async function loadProducts() {
      setIsLoading(true);
      setError(null);

      try {
        const products = await getProducts(search, page, pageSize, controller.signal);
        setResult(products);
      } catch (requestError) {
        if (requestError instanceof DOMException && requestError.name === "AbortError") {
          return;
        }

        setError(
          requestError instanceof Error
            ? requestError.message
            : "Products could not be loaded.",
        );
      } finally {
        if (!controller.signal.aborted) {
          setIsLoading(false);
        }
      }
    }

    void loadProducts();

    return () => controller.abort();
  }, [page, pageSize, search, reloadKey]);

  function changePageSize(nextPageSize: number) {
    setPage(1);
    setPageSize(nextPageSize);
  }

  function submitSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setPage(1);
    setSearch(searchInput.trim());
  }

  function clearSearch() {
    setSearchInput("");
    setSearch("");
    setPage(1);
  }

  return (
    <main>
      <header>
        <h1>Products</h1>
        <form className="search" role="search" onSubmit={submitSearch}>
          <label htmlFor="product-search">Search by product name or SKU</label>
          <div>
            <input
              id="product-search"
              type="search"
              value={searchInput}
              maxLength={100}
              onChange={(event) => setSearchInput(event.target.value)}
            />
            <button type="submit" disabled={isLoading}>Search</button>
            {search && (
              <button type="button" disabled={isLoading} onClick={clearSearch}>
                Clear
              </button>
            )}
          </div>
        </form>
      </header>

      <section className="catalogue" aria-labelledby="catalogue-heading">
        <div className="catalogue-header">
          <h2 id="catalogue-heading">Product list</h2>
          {result && <p>{result.totalItems} total products</p>}
        </div>

        {error ? (
          <div className="message error" role="alert">
            <h2>Products could not be loaded</h2>
            <p>{error}</p>
            <button type="button" onClick={() => setReloadKey((key) => key + 1)}>
              Try again
            </button>
          </div>
        ) : isLoading && !result ? (
          <p className="message" role="status">Loading products...</p>
        ) : result ? (
          <>
            <div className={isLoading ? "loading" : undefined}>
              <ProductList products={result.items} />
            </div>
            <PaginationControls
              page={result.page}
              pageSize={result.pageSize}
              totalItems={result.totalItems}
              totalPages={result.totalPages}
              disabled={isLoading}
              onPageChange={setPage}
              onPageSizeChange={changePageSize}
            />
          </>
        ) : null}
      </section>
    </main>
  );
}
