import { useEffect, useState, type FormEvent } from "react";
import { getProducts } from "./api/productsApi";
import { PaginationControls } from "./components/PaginationControls";
import { ProductList } from "./components/ProductList";
import type { PagedResult, Product } from "./types/product";

const DEFAULT_PAGE_SIZE = 10;

export default function App() {
  // Inital state of 1, changed by pagination controls when the user clicks "next" or "previous".
  // Gets passed to the API request to get the page I want.
  const [page, setPage] = useState(1);

  // Initial state of the page size, again, changed by the pagination controls to set the number of products per page.
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);

  // SearchInput is the state which holds the search term as it is being input.
  const [searchInput, setSearchInput] = useState("");

  //Search is the state which holds final value of the term to be quried.
  const [search, setSearch] = useState("");

  // API response to display the products and pagination info.
  const [result, setResult] = useState<PagedResult<Product> | null>(null);

  // Changed whilst the API request is in progress.
  // Used to disable buttons and whatnot whilst the request is in progress.
  const [isLoading, setIsLoading] = useState(true);

  // Changed in the catch block of the API request and used to display an error message if the request fails.
  const [error, setError] = useState<string | null>(null);

  // Used in the dependency array of useEffect, when the key is incremented (by clicking "try again"), it causes the useEffect to re-run and make a new API request.
  const [reloadKey, setReloadKey] = useState(0);

  // The react hook which runs when the component gets mount and when something in the dependency array changes.
  // Makes the API request to get the products and handles the response + changes to the above states.
  useEffect(() => {
    const controller = new AbortController();

    async function loadProducts() {
      setIsLoading(true);
      setError(null);

      try {
        // API request to get the products.
        // I don't recall ever personally using controller signals in the real world, this came from AI suggestions - 
        // but it's a nifty way to cancel the request and tell the API to stop its execution if something causes the component to unmount - 
        // or if the useEffect is re-run because something in the dependency array changed.
        // .NET sees that the HTTP request is aborted 
        // - and changes its cancellation token state to cancelled and EF will use that to cancellation the execution.
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
