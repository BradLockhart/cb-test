import type { PagedResult, Product } from "../types/product";

export async function getProducts(
  search: string,
  page: number,
  pageSize: number,
  signal: AbortSignal,
): Promise<PagedResult<Product>> {
  const parameters = new URLSearchParams({
    page: page.toString(),
    pageSize: pageSize.toString(),
  });

  if (search) {
    parameters.set("search", search);
  }

  const response = await fetch(`/products?${parameters}`, { signal });

  if (!response.ok) {
    throw new Error(`Products could not be loaded (${response.status}).`);
  }

  return response.json() as Promise<PagedResult<Product>>;
}
