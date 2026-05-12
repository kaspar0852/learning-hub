import apiClient from '../client'
import type { CurrencyDetectionDto } from '../../types'

export interface LocationData {
  currencyCode: string
  countryCode?: string | null
  countryName?: string | null
}

export const locationService = {
  getUserLocation: async (): Promise<LocationData> => {
    try {
      const response = await apiClient.get<CurrencyDetectionDto>('/api/localization/currency')
      return {
        currencyCode: response.data.currencyCode,
        countryCode: response.data.countryCode ?? null,
        countryName: response.data.countryName ?? null,
      }
    } catch (error) {
      console.error('[v0] Location fetch error:', error)
      return { currencyCode: 'USD', countryCode: null, countryName: null }
    }
  },
};
