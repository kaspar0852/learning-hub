import { useQuery } from '@tanstack/react-query'
import { LocationData, locationService } from '../api/services/locationService'
import { appConfig } from '../config/app'

export const useLocation = () => {
  const { data: location, isLoading, isError } = useQuery({
    queryKey: ['location'],
    queryFn: () => locationService.getUserLocation(),
    staleTime: 1000 * 60 * 60, // 1 hour
    retry: 2,
  })

  return {
    location: location || ({ currencyCode: appConfig.currency.default } as LocationData),
    isLoading,
    isError,
  }
};
