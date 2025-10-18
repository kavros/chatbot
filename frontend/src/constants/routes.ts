export enum PublicRoutes {
  LOGIN = "/",
  ERROR = "/error",
  UNKNOWN = "*",
}

export const PUBLIC_ROUTES_ARRAY: string[] = Object.values(PublicRoutes);
