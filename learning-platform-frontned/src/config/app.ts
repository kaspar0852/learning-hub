export const appConfig = {
  api: {
    baseUrl: import.meta.env.VITE_API_BASE_URL || 'https://api.example.com',
  },
  features: {
    locationDetection: import.meta.env.VITE_ENABLE_LOCATION_DETECTION !== 'false',
    priceConversion: import.meta.env.VITE_ENABLE_PRICE_CONVERSION !== 'false',
    favorites: import.meta.env.VITE_FEATURE_FAVORITES !== 'false',
    search: import.meta.env.VITE_FEATURE_SEARCH !== 'false',
  },
  currency: {
    default: import.meta.env.VITE_DEFAULT_CURRENCY || 'USD',
  },
  queryClient: {
    defaultOptions: {
      queries: {
        refetchOnWindowFocus: false,
        refetchOnReconnect: true,
        retry: 3,
      },
    },
  },
};
